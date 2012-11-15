using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Watchtower.Models;
using System.Collections.ObjectModel;

namespace Watchtower
{
    internal class RepositoryExtensionHelper
    {
        internal static ExtendedRepository GetExtendedRepository(Repository repository)
        {
            ExtendedRepository result = new ExtendedRepository(repository.Type, repository.Path);

            result.Name = repository.Name;
            result.IncomingChangesets = new ObservableCollection<ExtendedChangeset>();
            foreach (Changeset c in repository.IncomingChangesets)
            {
                result.IncomingChangesets.Add(GetExtendedChangeset(c));
            }
            result.OutgoingChangesets = new ObservableCollection<ExtendedChangeset>();
            foreach (Changeset c in repository.OutgoingChangesets)
            {
                result.OutgoingChangesets.Add(GetExtendedChangeset(c));
            }

            return result;
        }
        internal static Repository GetRepository(ExtendedRepository repository)
        {
            Repository result = new Repository(repository.Type, repository.Path);

            result.Name = repository.Name;
            result.IncomingChangesets = new ObservableCollection<Changeset>();
            foreach (ExtendedChangeset c in repository.IncomingChangesets)
            {
                result.IncomingChangesets.Add(GetChangeset(c));
            }
            result.OutgoingChangesets = new ObservableCollection<Changeset>();
            foreach (ExtendedChangeset c in repository.OutgoingChangesets)
            {
                result.OutgoingChangesets.Add(GetChangeset(c));
            }

            return result;
        }

        internal static ExtendedChangeset GetExtendedChangeset(Changeset changeset)
        {
            ExtendedChangeset result = new ExtendedChangeset(changeset.Branch, changeset.Revision, changeset.AuthorEmail, changeset.Timestamp, changeset.AuthorName, changeset.CommitMessage);
            return result;
        }
        internal static Changeset GetChangeset(ExtendedChangeset changeset)
        {
            Changeset result = new Changeset(changeset.Branch, changeset.Revision, changeset.AuthorEmail, changeset.Timestamp, changeset.AuthorName, changeset.CommitMessage);
            return result;
        }
    }
}
