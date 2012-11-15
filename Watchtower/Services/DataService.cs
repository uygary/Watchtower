using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using Community.CsharpSqlite.SQLiteClient;
using Entropik.Web.Gravatar;
using GalaSoft.MvvmLight.Ioc;

using Watchtower.Core;
using Watchtower.Models;

namespace Watchtower.Services
{
    public class DataService : IDataService
    {
        private PluginService _pluginService;
        private Dictionary<string, BitmapImage> _gravatars;

        public DataService()
        {
            _pluginService = SimpleIoc.Default.GetInstance<PluginService>();
            _gravatars = new Dictionary<string, BitmapImage>();

            InitializeDatabase();
        }

        public void BeginGetRepositories(Action<IList<ExtendedRepository>, Exception> callback)
        {
            IList<ExtendedRepository> reposOrderedByName = null;
            Exception ex = null;

            try
            {
                IList<ExtendedRepository> repos = ReadRepositories();
                reposOrderedByName = new List<ExtendedRepository>(from ExtendedRepository r in repos orderby r.Name select r);
            }
            catch(Exception e)
            {
                ex = e;
            }
            finally
            {
                callback(reposOrderedByName, ex);
            }
        }
        public void BeginGetIncomingChanges(ExtendedRepository repository, Action<ExtendedRepository, Exception> callback)
        {
            //Get incoming changesets
            IPlugin rcsPlugin = _pluginService.Plugins[repository.Type];
            ExtendedRepository result = RepositoryExtensionHelper.GetExtendedRepository(rcsPlugin.GetIncomingChanges(RepositoryExtensionHelper.GetRepository(repository)));

            //Get Gravatars
            foreach (ExtendedChangeset changeset in result.IncomingChangesets)
            {
                string author = changeset.AuthorEmail;
                if (_gravatars.ContainsKey(author))
                {
                    changeset.Gravatar = _gravatars[author];
                }
                else
                {
                    BitmapImage gravatar = GravatarHelper.GetBitmapImage(author, rating : GravatarRating.X);
                    if (null != gravatar)
                    {
                        _gravatars.Add(author, gravatar);
                        changeset.Gravatar = gravatar;
                    }
                }
            }
            //Return data
            callback(result, null);
        }

        #region DB related methods
        private IList<ExtendedRepository> ReadRepositories()
        {
            List<ExtendedRepository> result = new List<ExtendedRepository>();

            //if (!File.Exists(Constants.Configuration.DbFileName))
            //    InitializeDatabase();

            //Create connection
            SqliteConnection connection = new SqliteConnection();
            connection.ConnectionString = Constants.Configuration.ConnectionString;

            //Open database
            connection.Open();

            //create command
            IDbCommand cmd = connection.CreateCommand();

            //Select repositories
            cmd.CommandText = "SELECT * FROM Repositories";
            IDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string path = reader.GetString(reader.GetOrdinal("Path"));
                string name = reader.GetString(reader.GetOrdinal("Name"));
                string type = reader.GetString(reader.GetOrdinal("RepoType"));
                ExtendedRepository repo = new ExtendedRepository(type, path);
                repo.Name = name;
                result.Add(repo);
            }

            //Close and cleanup
            connection.Close();
            connection = null;

            return result;
        }
        public void SaveRepositories(IEnumerable<ExtendedRepository> repositories)
        {
            //if (!File.Exists(Constants.Configuration.DbFileName))
            //    InitializeDatabase();

            //Create connection
            SqliteConnection connection = new SqliteConnection();
            connection.ConnectionString = Constants.Configuration.ConnectionString;

            //Open database
            connection.Open();

            //create command
            IDbCommand deleteCmd = connection.CreateCommand();

            //clear REPOSITORIES table
            deleteCmd.CommandText = "DELETE FROM Repositories";
            deleteCmd.ExecuteNonQuery();

            //insert new records
            foreach (ExtendedRepository repo in repositories)
            {
                //create command
                IDbCommand insertCmd = connection.CreateCommand();

                //cmd.CommandText = string.Format("INSERT INTO Repositories ( Path, Name, RepoType ) VALUES ( '{0}', '{1}', '{2}' )", repo.Path, repo.Name, repo.Type);
                insertCmd.CommandText = "INSERT INTO Repositories ( Path, Name, RepoType ) VALUES ( @Path, @Name, @RepoType )";

                //cmd.Parameters.Add("@Path", SqlDbType.VarChar).Value = repo.Path;

                IDbDataParameter prm;

                prm = insertCmd.CreateParameter();
                prm.ParameterName = "@Path";
                prm.Value = repo.Path;
                insertCmd.Parameters.Add(prm);

                prm = insertCmd.CreateParameter();
                prm.ParameterName = "@Name";
                prm.Value = repo.Name;
                insertCmd.Parameters.Add(prm);

                prm = insertCmd.CreateParameter();
                prm.ParameterName = "@RepoType";
                prm.Value = repo.Type;
                insertCmd.Parameters.Add(prm);

                insertCmd.ExecuteNonQuery();
            }

            //Close and cleanup
            connection.Close();
            connection = null;
        }
        public void InitializeDatabase()
        {
            if (File.Exists(Constants.Configuration.DbFileName))
                //File.Delete(Constants.Configuration.DbFileName);
                return;

            //Create connection
            SqliteConnection connection = new SqliteConnection();
            connection.ConnectionString = Constants.Configuration.ConnectionString;

            //Open database
            connection.Open();

            IDbCommand cmd;
            IDbDataParameter prm;

            #region Create Tables
            //create command
            cmd = connection.CreateCommand();

            //create Repositories table
            cmd.CommandText = "CREATE TABLE Repositories ( Path TEXT PRIMARY KEY, Name TEXT, RepoType TEXT )";
            cmd.ExecuteNonQuery();

            cmd = connection.CreateCommand();

            //create Configuration table and set initial values
            cmd.CommandText = "CREATE TABLE Configuration ( Key TEXT, Value TEXT )";
            cmd.ExecuteNonQuery();
            #endregion


            //Set initial configuration values

            #region Insert update period
            cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Configuration ( Key, Value ) VALUES ( @Key, @Value )";

            prm = cmd.CreateParameter();
            prm.ParameterName = "@Key";
            prm.Value = Constants.Configuration.PeriodKey;
            cmd.Parameters.Add(prm);

            prm = cmd.CreateParameter();
            prm.ParameterName = "@Value";
            prm.Value = Constants.Configuration.PeriodValue;
            cmd.Parameters.Add(prm);

            cmd.ExecuteNonQuery();
            #endregion

            #region Insert sequential update option
            cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO Configuration ( Key, Value ) VALUES ( @Key, @Value )";

            prm = cmd.CreateParameter();
            prm.ParameterName = "@Key";
            prm.Value = Constants.Configuration.SequentialUpdateKey;
            cmd.Parameters.Add(prm);

            prm = cmd.CreateParameter();
            prm.ParameterName = "@Value";
            prm.Value = Constants.Configuration.SequentialUpdateValue;
            cmd.Parameters.Add(prm);

            cmd.ExecuteNonQuery();
            #endregion

            //Close and cleanup
            connection.Close();
            connection = null;
        }
        public ConfigData GetConfiguration()
        {
            ConfigData result = new ConfigData(Constants.Configuration.PeriodValue, true);

            //if (!File.Exists(Constants.Configuration.DbFileName))
            //    InitializeDatabase();

            //Create connection
            SqliteConnection connection = new SqliteConnection();
            connection.ConnectionString = Constants.Configuration.ConnectionString;

            //Open database
            connection.Open();

            IDbCommand cmd;
            IDataReader reader;

            #region Read update period
            //create command
            cmd = connection.CreateCommand();

            //Select repositories
            cmd.CommandText = string.Format("SELECT * FROM Configuration WHERE KEY='{0}'", Constants.Configuration.PeriodKey);
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                int period = reader.GetInt16(reader.GetOrdinal("Value"));
                result.UpdatePeriod = period;
            }
            #endregion

            #region Read sequential update option
            //create command
            cmd = connection.CreateCommand();

            //Select repositories
            cmd.CommandText = string.Format("SELECT * FROM Configuration WHERE KEY='{0}'", Constants.Configuration.SequentialUpdateKey);
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                bool sequential = reader.GetBoolean(reader.GetOrdinal("Value"));
                result.SequentialUpdate = sequential;
            }
            #endregion

            //Close and cleanup
            connection.Close();
            connection = null;

            return result;
        }
        public void SaveConfiguration(ConfigData configData)
        {
            //if (!File.Exists(Constants.Configuration.DbFileName))
            //    InitializeDatabase();

            //Create connection
            SqliteConnection connection = new SqliteConnection();
            connection.ConnectionString = Constants.Configuration.ConnectionString;

            //Open database
            connection.Open();

            IDbCommand cmd;
            IDbDataParameter prm;

            #region Save update period
            //create command
            cmd = connection.CreateCommand();

            cmd.CommandText = "UPDATE Configuration SET Value=@Value WHERE Key=@Key";

            prm = cmd.CreateParameter();
            prm.ParameterName = "@Value";
            prm.Value = configData.UpdatePeriod;

            prm = cmd.CreateParameter();
            prm.ParameterName = "@Key";
            prm.Value = Constants.Configuration.PeriodKey;

            cmd.ExecuteNonQuery();
            #endregion

            #region Save sequential update option
            //create command
            cmd = connection.CreateCommand();

            cmd.CommandText = "UPDATE Configuration SET Value=@Value WHERE Key=@Key";

            prm = cmd.CreateParameter();
            prm.ParameterName = "@Value";
            prm.Value = configData.SequentialUpdate;

            prm = cmd.CreateParameter();
            prm.ParameterName = "@Key";
            prm.Value = Constants.Configuration.SequentialUpdateKey;

            cmd.ExecuteNonQuery();
            #endregion

            //Close and cleanup
            connection.Close();
            connection = null;
        }
        #endregion

    }
}