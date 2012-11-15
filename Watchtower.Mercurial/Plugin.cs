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
                IEnumerable<Changeset> changesets = mercurialRepo.Log();

                if (null != changesets)
                    result = true;

                mercurialRepo.Dispose();
            }
            catch
            {

            }

            return result;
        }

        public Watchtower.Models.Repository GetIncomingChanges(Watchtower.Models.Repository repository)
        {
            List<Watchtower.Models.Changeset> changesets = new List<Watchtower.Models.Changeset>();
            Repository mercurialRepo = new Repository(repository.Path);
            IEnumerable<Changeset> incoming = mercurialRepo.Incoming();

            foreach (Changeset cs in incoming)
            {
                Watchtower.Models.Changeset changeset = new Watchtower.Models.Changeset(cs.Branch, cs.Revision, cs.AuthorEmailAddress, cs.Timestamp, cs.AuthorName, cs.CommitMessage);
                changesets.Add(changeset);
            }

            repository.IncomingChangesets = changesets;
            mercurialRepo.Dispose();
            return repository;
        }
        public Watchtower.Models.Repository GetOutgoingChanges(Watchtower.Models.Repository repository)
        {
            List<Watchtower.Models.Changeset> changesets = new List<Watchtower.Models.Changeset>();
            Repository mercurialRepo = new Repository(repository.Path);
            IEnumerable<Changeset> outgoing = mercurialRepo.Outgoing();

            foreach (Changeset cs in outgoing)
            {
                Watchtower.Models.Changeset changeset = new Watchtower.Models.Changeset(cs.Branch, cs.Revision, cs.AuthorEmailAddress, cs.Timestamp, cs.AuthorName, cs.CommitMessage);
                changesets.Add(changeset);
            }

            repository.OutgoingChangesets = changesets;
            mercurialRepo.Dispose();
            return repository;
        }

        public bool PullIncomingChangesets(Watchtower.Models.Repository repository)
        {
            bool result = false;

            try
            {
                Repository mercurialRepo = new Repository(repository.Path);
                mercurialRepo.Pull();
                //mercurialRepo.Update();
                mercurialRepo.Dispose();
                result = true;
            }
            catch
            {

            }

            return result;
        }
        public bool PushOutgoingChangesets(Watchtower.Models.Repository repository)
        {
            bool result = false;

            try
            {
                Repository mercurialRepo = new Repository(repository.Path);
                mercurialRepo.Push();
                mercurialRepo.Dispose();

                result = true;
            }
            catch
            {

            }

            return result;
        }

        public bool Merge(Watchtower.Models.Repository repository)
        {
            bool result = false;

            try
            {
                Repository mercurialRepo = new Repository(repository.Path);
                MergeResult mergeResult = mercurialRepo.Merge();

                if (!mergeResult.HasFlag(MergeResult.UnresolvedFiles))
                    result = true;

                mercurialRepo.Dispose();
            }
            catch
            {

            }

            return result;
        }
        public bool StartMerge(Watchtower.Models.Repository repository)
        {
            throw new NotImplementedException();
        }
    }
}
