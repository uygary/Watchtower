using System.Collections.Generic;
using System.Diagnostics;

namespace Watchtower.Model
{
    [DebuggerDisplay("ExtendedRepository: ({Type} - {Path})")]
    public class ExtendedRepository
    {
        public string Type { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public IList<ExtendedChangeset> IncomingChangesets { get; set; }
        public IList<ExtendedChangeset> OutgoingChangesets { get; set; }

        public ExtendedRepository()
        {
            IncomingChangesets = new List<ExtendedChangeset>();
            OutgoingChangesets = new List<ExtendedChangeset>();
        }
        public ExtendedRepository(string type, string path)
            : this()
        {
            Type = type;
            Path = path;
        }
    }
}
