using System.Diagnostics;

namespace Watchtower.Model
{
    [DebuggerDisplay("ConfigData (UpdatePeriod:{UpdatePeriod})")]
    public class ConfigData
    {
        public int UpdatePeriod { get; set; }

        public ConfigData()
        {

        }
        public ConfigData(int updatePeriod)
            : this()
        {
            UpdatePeriod = updatePeriod;
        }
    }
}
