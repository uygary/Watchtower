using System.Diagnostics;

namespace Watchtower.Model
{
    [DebuggerDisplay("ConfigData (UpdatePeriod:{UpdatePeriod} - UpdateSequentially:{UpdateSequentially})")]
    public class ConfigData
    {
        public int UpdatePeriod { get; set; }
        public bool SequentialUpdate { get; set; }

        public ConfigData()
        {

        }
        public ConfigData(int updatePeriod)
            : this()
        {
            UpdatePeriod = updatePeriod;
        }
        public ConfigData(int updatePeriod, bool sequentialUpdate)
            : this(updatePeriod)
        {
            SequentialUpdate = sequentialUpdate;
        }
    }
}
