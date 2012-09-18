using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using Community.CsharpSqlite.SQLiteClient;
using Entropik.Web.Gravatar;
using GalaSoft.MvvmLight.Ioc;

using Watchtower.Core;
using Watchtower.Model;

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

        public void GetRepositories(Action<IList<ExtendedRepository>, Exception> callback)
        {
            IList<ExtendedRepository> repos = ReadRepositories();
            callback(repos, null);
        }
        public void GetIncomingChanges(ExtendedRepository repository, Action<ExtendedRepository, Exception> callback)
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
        public IList<ExtendedRepository> ReadRepositories()
        {
            List<ExtendedRepository> result = new List<ExtendedRepository>();

            string dbFileName = @"Configuration.db";
            //if (!File.Exists(dbFileName))
            //    InitializeDatabase();

            //Create connection
            SqliteConnection connection = new SqliteConnection();
            string connectionString = string.Format("Version=3,uri=file:{0}", dbFileName);
            connection.ConnectionString = connectionString;

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
        public void UpdateRepositories(IEnumerable<ExtendedRepository> repositories)
        {
            string dbFileName = @"Configuration.db";
            //if (!File.Exists(dbFileName))
            //    InitializeDatabase();

            //Create connection
            SqliteConnection connection = new SqliteConnection();
            string connectionString = string.Format("Version=3,uri=file:{0}", dbFileName);
            connection.ConnectionString = connectionString;

            //Open database
            connection.Open();

            //create command
            IDbCommand cmd = connection.CreateCommand();

            //clear REPOSITORIES table
            cmd.CommandText = "DELETE FROM Repositories";
            cmd.ExecuteNonQuery();

            //insert new records
            foreach (ExtendedRepository repo in repositories)
            {
                cmd.CommandText = string.Format("INSERT INTO Repositories ( Path, Name, RepoType ) VALUES ( '{0}', '{1}', '{2}' )", repo.Path, repo.Name, repo.Type);
                cmd.ExecuteNonQuery();
            }

            //Close and cleanup
            connection.Close();
            connection = null;
        }
        public void InitializeDatabase()
        {
            string dbFileName = @"Configuration.db";

            if (File.Exists(dbFileName))
                //File.Delete(dbFileName);
                return;

            string connectionString = string.Format("Version=3,uri=file:{0}", dbFileName);

            //Create connection
            SqliteConnection connection = new SqliteConnection();
            connection.ConnectionString = connectionString;

            //Open database
            connection.Open();

            //create command
            IDbCommand cmd = connection.CreateCommand();

            //create Repositories table
            cmd.CommandText = "CREATE TABLE Repositories ( Path TEXT PRIMARY KEY, Name TEXT, RepoType TEXT )";
            cmd.ExecuteNonQuery();

            //create Configuration table and set initial values
            cmd.CommandText = "CREATE TABLE Configuration ( Key TEXT, Value TEXT )";
            cmd.ExecuteNonQuery();

            //Set initial configuration values
            cmd.CommandText = string.Format("INSERT INTO Configuration ( Key, Value ) VALUES ( '{0}', {1} )", Constants.PeriodKey, Constants.PeriodValue);
            cmd.ExecuteNonQuery();

            //Close and cleanup
            connection.Close();
            connection = null;
        }
        public ConfigData ReadConfiguration()
        {
            ConfigData result = new ConfigData(Constants.PeriodValue);

            string dbFileName = @"Configuration.db";
            //if (!File.Exists(dbFileName))
            //    InitializeDatabase();

            //Create connection
            SqliteConnection connection = new SqliteConnection();
            string connectionString = string.Format("Version=3,uri=file:{0}", dbFileName);
            connection.ConnectionString = connectionString;

            //Open database
            connection.Open();

            //create command
            IDbCommand cmd = connection.CreateCommand();

            //Select repositories
            cmd.CommandText = string.Format("SELECT * FROM Configuration WHERE KEY='{0}'", Constants.PeriodKey);
            IDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                int period = reader.GetInt16(reader.GetOrdinal("Value"));
                result.UpdatePeriod = period;
            }

            //Close and cleanup
            connection.Close();
            connection = null;

            return result;
        }
        public void UpdateConfiguration(ConfigData configData)
        {
            string dbFileName = @"Configuration.db";
            //if (!File.Exists(dbFileName))
            //    InitializeDatabase();

            //Create connection
            SqliteConnection connection = new SqliteConnection();
            string connectionString = string.Format("Version=3,uri=file:{0}", dbFileName);
            connection.ConnectionString = connectionString;

            //Open database
            connection.Open();

            //create command
            IDbCommand cmd = connection.CreateCommand();

            //clear REPOSITORIES table
            cmd.CommandText = string.Format("UPDATE Configuration SET Value='{0}' WHERE Key='{1}'", Constants.PeriodKey, configData.UpdatePeriod);
            cmd.ExecuteNonQuery();

            //Close and cleanup
            connection.Close();
            connection = null;
        }
        #endregion

    }
}