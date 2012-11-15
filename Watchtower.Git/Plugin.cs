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

        public Model.Repository GetIncomingChanges(Model.Repository repository)
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
                    List<Watchtower.Model.Changeset> commits = new List<Model.Changeset>();
                    foreach(RevCommit rc in lr)
                    {
                        Watchtower.Model.Changeset c = new Model.Changeset(rc.GetParent(0).GetHashCode().ToString(), rc.GetHashCode().ToString(), rc.GetCommitterIdent().GetEmailAddress(), rc.GetCommitterIdent().GetWhen(), rc.GetCommitterIdent().GetName(), rc.GetFullMessage());
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
        public Model.Repository GetOutgoingChanges(Model.Repository repository)
        {
            throw new NotImplementedException();
        }

        public bool PullIncomingChangesets(Model.Repository repository)
        {
            throw new NotImplementedException();
        }
        public bool PushOutgoingChangesets(Model.Repository repository)
        {
            throw new NotImplementedException();
        }

        public bool Merge(Model.Repository repository)
        {
            throw new NotImplementedException();
        }
        public bool StartMerge(Model.Repository repository)
        {
            throw new NotImplementedException();
        }
    }
}
