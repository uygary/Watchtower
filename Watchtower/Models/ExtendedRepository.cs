using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Watchtower.Models
{
    [DebuggerDisplay("ExtendedRepository: ({Type} - {Path})")]
    public class ExtendedRepository
    {
        public string Type { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public ICollection<ExtendedChangeset> IncomingChangesets { get; set; }
        public ICollection<ExtendedChangeset> OutgoingChangesets { get; set; }

        public ExtendedRepository()
        {
            IncomingChangesets = new ObservableCollection<ExtendedChangeset>();
            OutgoingChangesets = new ObservableCollection<ExtendedChangeset>();
        }
        public ExtendedRepository(string type, string path)
            : this()
        {
            Type = type;
            Path = path;
        }
    }
}
