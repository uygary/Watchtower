using System.Collections.Generic;
using System.Diagnostics;

namespace Watchtower.Model
{
    [DebuggerDisplay("Repository: ({Type} - {Path})")]
    public class Repository
    {
        public string Type { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public IList<Changeset> IncomingChangesets { get; set; }
        public IList<Changeset> OutgoingChangesets { get; set; }

        public Repository()
        {
            IncomingChangesets = new List<Changeset>();
            OutgoingChangesets = new List<Changeset>();
        }
        public Repository(string type, string path)
            : this()
        {
            Type = type;
            Path = path;
        }
    }
}
