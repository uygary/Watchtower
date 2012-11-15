using System;
using System.Collections.Generic;

using Watchtower.Models;

namespace Watchtower.Services
{
    public interface IDataService
    {
        void BeginGetRepositories(Action<IList<ExtendedRepository>, Exception> callback);
        void BeginGetIncomingChanges(ExtendedRepository repository, Action<ExtendedRepository, Exception> callback);

        #region DB related methods
        void SaveRepositories(IEnumerable<ExtendedRepository> repositories);
        void InitializeDatabase();
        ConfigData GetConfiguration();
        void SaveConfiguration(ConfigData configData);
        #endregion
    }
}
