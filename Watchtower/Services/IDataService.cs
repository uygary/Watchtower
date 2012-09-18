using System;
using System.Collections.Generic;

using Watchtower.Model;

namespace Watchtower.Services
{
    public interface IDataService
    {
        void GetRepositories(Action<IList<ExtendedRepository>, Exception> callback);
        void GetIncomingChanges(ExtendedRepository repository, Action<ExtendedRepository, Exception> callback);

        #region DB related methods
        IList<ExtendedRepository> ReadRepositories();
        void UpdateRepositories(IEnumerable<ExtendedRepository> repositories);
        void InitializeDatabase();
        ConfigData ReadConfiguration();
        void UpdateConfiguration(ConfigData configData);
        #endregion
    }
}
