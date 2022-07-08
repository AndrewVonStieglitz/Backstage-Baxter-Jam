using System.Collections.Generic;
using UnityEngine;

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
        // TODO: THis is just here temporarily. We need to move all this texture stuff up to the cables system.
        public CableColor cableColor;
        public Texture texture { get; set; }

        private PlugCable pluggableStart;
        private PlugCable pluggableEnd;

        public Connection(PlugCable startObj, CableColor color)
        {
            PluggableStart = startObj;
            cableColor = color;
        }

        private void RecalculatePluggablesList()
        {
            List<PluggablesSO> newPList = new List<PluggablesSO>();
            PlugCable studyPlug = PluggableStart;
            
            //List<PlugCable> seenPlugCables = new List<PlugCable>(); // to prevent loops
            //newPList.Add(pluggableStart.pluggable);
            
            while (studyPlug != null)
            {
                newPList.Add(studyPlug.pluggable);
                
                var instrumentPlug = studyPlug as InstrumentPlugCable;

                if (instrumentPlug != null)
                {
                    Instrument = instrumentPlug.instrument;
                    break;
                }

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

        private void UpdateSprite()
        {
            // cableSprite = connectionIn.PluggableStart.cableSprite;
            // if (connectionOut != null)
            // {
            //     connectionOut.texture = connectionIn.PluggableStart.cableSprite.texture;
            // }
        }
        
        // TODO: Connections need to update their own sprites / texture whenever they change
        
                // Connection studyConnection = connectionOut;
                // while (studyConnection != null)
                // {
                //     studyConnection.texture = cableColorSprites[0].texture;
                //     studyConnection = studyConnection.PluggableEnd.connectionOut;
                // }
    }
}