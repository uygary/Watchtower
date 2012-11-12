using System;
using System.Collections.Generic;

using Watchtower.Model;
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
            callback(repositories, null);
        }
        public void BeginGetIncomingChanges(ExtendedRepository repository, Action<ExtendedRepository, Exception> callback)
        {
            callback(repository, null);
        }

        #region DB related methods
        public IList<ExtendedRepository> ReadRepositories() { return new List<ExtendedRepository>(); }
        public void UpdateRepositories(IEnumerable<ExtendedRepository> repositories) { }
        public void InitializeDatabase() { }
        public ConfigData ReadConfiguration() { return new ConfigData(Constants.PeriodValue); }
        public void UpdateConfiguration(ConfigData configData) { }
        #endregion
    }
}