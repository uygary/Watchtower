using System;
using System.Collections.Generic;
using NGit;
using NGit.Api;
using NGit.Revwalk;
using NGit.Transport;
using Sharpen;

using Watchtower.Core;

namespace Watchtower.Git
{
    public class Plugin : IPlugin
    {
        public string RepositoryType { get { return Constants.RepositoryType; } }

        public bool VerifyRepository(string path)
        {
            bool result = false;

            try
            {
                NGit.Api.Git git = NGit.Api.Git.Open(path);
                Repository repo = git.GetRepository();

                ////Repository repo = new FileRepository(new Sharpen.FilePath(path));
                ////NGit.Api.Git git = new NGit.Api.Git(repo);
                //FetchCommand fc = git.Fetch();
                //FetchResult fr = fc.Call();
                //LogCommand lc = git.Log();
                //Iterable<RevCommit> lr = lc.Call();


                string indexFile = repo.GetIndexFile();
                //repo.GetRef();
                //repo.ReadOrigHead();

                if (null != git && null != repo && !string.IsNullOrEmpty(indexFile))
                {
                    result = true;
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public Models.Repository GetIncomingChanges(Models.Repository repository)
        {
            try
            {
                NGit.Api.Git git = NGit.Api.Git.Open(repository.Path);
                Repository repo = git.GetRepository();
                repo.GetIndexFile();

                //Repository repo = new FileRepository(new Sharpen.FilePath(path));
                //NGit.Api.Git git = new NGit.Api.Git(repo);
                FetchCommand fc = git.Fetch();
                FetchResult fr = fc.Call();
                LogCommand lc = git.Log();
                Iterable<RevCommit> lr = lc.Call();



                if ( git != null && repo != null && fr != null && lr != null )
                {
                    List<Watchtower.Models.Changeset> commits = new List<Models.Changeset>();
                    foreach(RevCommit rc in lr)
                    {
                        Watchtower.Models.Changeset c = new Models.Changeset(rc.GetParent(0).GetHashCode().ToString(), rc.GetHashCode().ToString(), rc.GetCommitterIdent().GetEmailAddress(), rc.GetCommitterIdent().GetWhen(), rc.GetCommitterIdent().GetName(), rc.GetFullMessage());
                        commits.Add(c);
                    }
                    repository.IncomingChangesets = commits;
                }
            }
            catch
            {
            }
            return repository;
        }
        public Models.Repository GetOutgoingChanges(Models.Repository repository)
        {
            throw new NotImplementedException();
        }

        public bool PullIncomingChangesets(Models.Repository repository)
        {
            throw new NotImplementedException();
        }
        public bool PushOutgoingChangesets(Models.Repository repository)
        {
            throw new NotImplementedException();
        }

        public bool Merge(Models.Repository repository)
        {
            throw new NotImplementedException();
        }
        public bool StartMerge(Models.Repository repository)
        {
            throw new NotImplementedException();
        }
    }
}
