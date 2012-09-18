using System;
using System.Diagnostics;
using System.Drawing;

namespace Watchtower.Model
{
    [DebuggerDisplay("ExtendedChangeset ({Timestamp}:{Revision} - <{AuthorEmail}>{AuthorName} - {CommitMessage})")]
    public class ExtendedChangeset
    {
        /// <summary>
        /// To be filled and used by Watchtower itself.
        /// Do not use while 
        /// </summary>
        public Icon Gravatar { get; set; }

        public string AuthorEmail { get; set; }
        public string AuthorName { get; set; }
        public string Branch { get; set; }
        public string CommitMessage { get; set; }
        public string Revision { get; set; }
        public DateTime Timestamp { get; set; }

        public ExtendedChangeset()
        {

        }
        public ExtendedChangeset(string branch, string revision)
            : this()
        {
            Branch = branch;
            Revision = revision;
        }
        public ExtendedChangeset(string branch, string revision, string authorEmail)
            : this(branch, revision)
        {
            AuthorEmail = authorEmail;
        }
        public ExtendedChangeset(string branch, string revision, string authorEmail, DateTime timestamp)
            : this(branch, revision, authorEmail)
        {
            Timestamp = timestamp;
        }
        public ExtendedChangeset(string branch, string revision, string authorEmail, DateTime timestamp, string authorName, string commitMessage)
            : this(branch, revision, authorEmail, timestamp)
        {
            Timestamp = timestamp;
            AuthorName = authorName;
            CommitMessage = commitMessage;
        }
    }
}
