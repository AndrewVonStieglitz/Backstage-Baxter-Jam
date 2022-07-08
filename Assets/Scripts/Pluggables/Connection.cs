using System.Collections.Generic;

namespace Pluggables
{
    public class Connection
    {
        public PlugCable PluggableStart
        {
            get => pluggableStart;
            set
            {
                pluggableStart = value;
                RecalculatePluggablesList();
            }
        }

        public PlugCable PluggableEnd
        {
            get => pluggableEnd;
            set
            {
                pluggableEnd = value;
                RecalculatePluggablesList();
            }
        }

        public InstrumentSO Instrument { get; private set; }
        public List<PluggablesSO> pluggablesList = new List<PluggablesSO>();
        public ColorEnum Color { get; private set; }

        private PlugCable pluggableStart;
        private PlugCable pluggableEnd;

        public Connection(PlugCable startObj)
        {
            PluggableStart = startObj;
        }

        private void RecalculatePluggablesList()
        {
            List<PluggablesSO> newPList = new List<PluggablesSO>();
            PlugCable studyPlug = PluggableStart;
            
            //List<PlugCable> seenPlugCables = new List<PlugCable>(); // to prevent loops
            //newPList.Add(pluggableStart.pluggable);
            
            while (studyPlug != null)
            {
                var instrumentPlug = studyPlug as InstrumentPlugCable;

                if (instrumentPlug != null)
                {
                    Instrument = instrumentPlug.instrument;
                    Color = instrumentPlug.color;
                    break;
                }

                newPList.Add(studyPlug.pluggable);
                studyPlug = studyPlug.PrevPlugCable();

                //if (seenPlugCables.Contains(studyPlug))
                //    return false;
                //seenPlugCables.Add(studyPlug);
            }

            newPList.Reverse();
            
            if (PluggableEnd != null)
                newPList.Add(PluggableEnd.pluggable);
            
            pluggablesList = newPList;
        }
    }
}