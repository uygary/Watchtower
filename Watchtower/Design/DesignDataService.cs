using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Watchtower.Models;
using Watchtower.Services;

namespace Watchtower.Design
{
    /// <summary>
    /// Dummy service implemented for design time support.
    /// </summary>
    public class DesignDataService : IDataService
    {
        public void BeginGetRepositories(Action<IList<ExtendedRepository>, Exception> callback)
        {
            List<ExtendedRepository> repositories = new List<ExtendedRepository>();

            ExtendedRepository repo = new ExtendedRepository("DesignTimeRepoType", "DesignTimeRepoPath");
            repo.Name = "DesignTimeRepo";
            repositories.Add(repo);

            callback(repositories, null);
        }
        public void BeginGetIncomingChanges(ExtendedRepository repository, Action<ExtendedRepository, Exception> callback)
        {
            repository.IncomingChangesets = new ObservableCollection<ExtendedChangeset>();

            ExtendedChangeset changeset = new ExtendedChangeset("DesignTimeChangesetBranch", "DesignTimeChangesetRevision", "DesignTimeChangesetAuthorEmail", DateTime.Now, "DesignTimeChangesetAuthorName", "DesignTimeChangesetCommitMessage");
            //changeset.Gravatar
            repository.IncomingChangesets.Add(changeset);

            callback(repository, null);
        }

        #region DB related methods
        public void SaveRepositories(IEnumerable<ExtendedRepository> repositories) { }
        public void InitializeDatabase() { }
        public ConfigData GetConfiguration() { return new ConfigData(Constants.Configuration.PeriodValue, false); }
        public void SaveConfiguration(ConfigData configData) { }
        #endregion
    }
}