using System;
using System.Collections.Generic;

using Mercurial;

using Watchtower.Core;
using Watchtower.Mercurial;

namespace Watchtower
{
    public class Plugin : IPlugin
    {
        public string RepositoryType { get { return Constants.RepositoryType; } }

        public bool VerifyRepository(string path)
        {
            bool result = false;

            try
            {
                Repository mercurialRepo = new Repository(path);
                IEnumerable<Changeset> incoming = mercurialRepo.Log();
                if (null != incoming)
                    result = true;
                mercurialRepo.Dispose();
            }
            catch
            {

            }

            return result;
        }

        public Model.Repository GetIncomingChanges(Watchtower.Model.Repository repository)
        {
            List<Watchtower.Model.Changeset> changesets = new List<Watchtower.Model.Changeset>();
            Repository mercurialRepo = new Repository(repository.Path);
            IEnumerable<Changeset> incoming = mercurialRepo.Incoming();

            foreach (Changeset cs in incoming)
            {
                Watchtower.Model.Changeset changeset = new Watchtower.Model.Changeset(cs.Branch, cs.Revision, cs.AuthorEmailAddress, cs.Timestamp, cs.AuthorName, cs.CommitMessage);
                changesets.Add(changeset);
            }

            repository.IncomingChangesets = changesets;
            mercurialRepo.Dispose();
            return repository;
        }
        public Model.Repository GetOutgoingChanges(Watchtower.Model.Repository repository)
        {
            List<Watchtower.Model.Changeset> changesets = new List<Watchtower.Model.Changeset>();
            Repository mercurialRepo = new Repository(repository.Path);
            IEnumerable<Changeset> outgoing = mercurialRepo.Outgoing();

            foreach (Changeset cs in outgoing)
            {
                Watchtower.Model.Changeset changeset = new Watchtower.Model.Changeset(cs.Branch, cs.Revision, cs.AuthorEmailAddress, cs.Timestamp, cs.AuthorName, cs.CommitMessage);
                changesets.Add(changeset);
            }

            repository.OutgoingChangesets = changesets;
            mercurialRepo.Dispose();
            return repository;
        }

        public bool PullIncomingChangesets(Watchtower.Model.Repository repository)
        {
            throw new NotImplementedException();
        }
        public bool PushOutgoingChangesets(Watchtower.Model.Repository repository)
        {
            throw new NotImplementedException();
        }

        public bool Merge(Watchtower.Model.Repository repository)
        {
            throw new NotImplementedException();
        }
        public bool StartMerge(Watchtower.Model.Repository repository)
        {
            throw new NotImplementedException();
        }
    }
}
