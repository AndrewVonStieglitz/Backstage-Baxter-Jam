using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace.Pluggables
{
    public class Connection
    {
        public enum ConnectionState {InProgress, Connected}
        
        public PlugCable pluggableStart;
        public PlugCable pluggableEnd;
        public InstrumentSO instrument;
        public List<PluggablesSO> pluggablesList = new List<PluggablesSO>();
        public CableColor cableColor;
        // TODO: THis is just here temporarily. We need to move all this texture stuff up to the cables system.
        public Texture texture { get; set; }
        public ConnectionState state;
        
        public void Initialise(PlugCable startObj, CableColor color)
        {
            pluggableStart = startObj;
            cableColor = color;
            
            if (startObj.tag == "Instrument")
            {
                instrument = startObj.instrument;
            }
            
            if (startObj.tag == "Pluggable")
            {
                pluggablesList.Add(startObj.pluggable);
                instrument = startObj.GetPathsInstrument();
            }
        }
        
        public void RecalculatePluggablesList()
        {
            PlugCable studyPlug = pluggableStart;
            List<PluggablesSO> newPList = new List<PluggablesSO>();
            //List<PlugCable> seenPlugCables = new List<PlugCable>(); // to prevent loops
            InstrumentSO newInstrument = null;
            //newPList.Add(pluggableStart.pluggable);
            while (studyPlug != null)
            {
                if (studyPlug.IsInstrument())
                {
                    newInstrument = studyPlug.instrument;
                    break;
                }
                newPList.Add(studyPlug.pluggable);
                studyPlug = studyPlug.GetPrevPlugCable();
                //if (seenPlugCables.Contains(studyPlug))
                //    return false;
                //seenPlugCables.Add(studyPlug);
            }
            newPList.Reverse();
            if (pluggableEnd != null)
                newPList.Add(pluggableEnd.pluggable);
            instrument = newInstrument;
            pluggablesList = newPList;
            //print("Refreshing: " + name + " to instrument: " + (newInstrument != null ? instrument.itemName : "NULL") 
            //    + ",\tpluggables list contains: " + newPList.Count);
            //return true;
        }
    }
}