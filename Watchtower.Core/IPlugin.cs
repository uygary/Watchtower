using System.Windows.Media.Imaging;
using Watchtower.Models;

namespace Watchtower.Core
{
    public interface IPlugin
    {
        /// <summary>
        /// Used for initializing plugins and registering them with repository types.
        /// Cleartext name or preferably initials of the source control system should be returned.
        /// </summary>
        string RepositoryType { get; }

        /// <summary>
        /// Icon used for the repository type handled by plugin.
        /// </summary>
        BitmapImage PluginIcon { get; }

        /// <summary>
        /// Checks the repository to see if it belongs to the revision control system in question.
        /// </summary>
        /// <param name="path">Full path of the local repository.</param>
        /// <returns>True if the repository is managed by the RCS, false if not.</returns>
        bool VerifyRepository(string path);

        /// <summary>
        /// Must be implemented for checking incoming changes.
        /// </summary>
        /// <param name="repository"></param>
        /// <returns></returns>
        Repository GetIncomingChanges(Repository repository);
        /// <summary>
        /// Must be implemented for checking outgoing changes.
        /// </summary>
        /// <param name="repository"></param>
        /// <returns></returns>
        Repository GetOutgoingChanges(Repository repository);

        bool PullIncomingChangesets(Repository repository);
        bool PushOutgoingChangesets(Repository repository);
        bool Merge(Repository repository);
        bool StartMerge(Repository repository);
    }
}
