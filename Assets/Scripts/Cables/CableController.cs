using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Cables
{
    public class CableController : MonoBehaviour
    {
        public enum CableState { InProgress, Completed, Abandoned }

        [SerializeField] private int cableID;
        
        public int CableID { get => cableID; }

        public UnityEvent initialised = new UnityEvent();
        public UnityEvent<CableNode> nodeCreated = new UnityEvent<CableNode>();
        public UnityEvent<CableNode> nodeDestroyed = new UnityEvent<CableNode>();
        public UnityEvent cableCompleted = new UnityEvent();
        public UnityEvent<CableNode> nodeMoved = new UnityEvent<CableNode>();

        public CableState state;
        public AmpController amp;
        public PlugCable pluggableStart;
        public PlugCable pluggableEnd;
        public float cableWidth;
        public List<CableNode> nodes = new List<CableNode>();

        public InstrumentSO instrument;
        public List<PluggablesSO> pluggablesList;
        

        private void OnEnable()
        {
            GameEvents.onCableDisconnect += OnCableDisconnect;
            GameEvents.onCableDisconnectPlug += OnCableDisconnectPlug;
        }
        
        private void OnDisable()
        {
            GameEvents.onCableDisconnect -= OnCableDisconnect;
            GameEvents.onCableDisconnectPlug -= OnCableDisconnectPlug;
        }

        public void Initialise(AmpController amp)
        {
            this.amp = amp;
            
            Initialise(amp.transform);
        }

        public void Initialise(PlugCable startObj)
        {
            pluggableStart = startObj;
            
            if (startObj.tag == "Instrument")
            {
                instrument = startObj.instrument;
            }
            
            if (startObj.tag == "Pluggable")
            {
                pluggablesList.Add(startObj.pluggable);
                instrument = startObj.GetPathsInstrument();
            }
            
            Initialise(pluggableStart.transform);
        }

        private void Initialise(Transform transform)
        {
            CreateNode(new CableNode(), transform.position);
            CreateNode(new CableNode(), transform.position);

            initialised.Invoke();
        }

        public void RecalculatePluggablesList()
        {
            PlugCable studyPlug = pluggableStart;
            List<PluggablesSO> newPList = new List<PluggablesSO>();
            InstrumentSO newInstrument = null;
            //newPList.Add(pluggableStart.pluggable);
            int it = 0;
            while (studyPlug != null)
            {
                if (studyPlug.IsInstrument())
                {
                    newInstrument = studyPlug.instrument;
                    break;
                }
                newPList.Add(studyPlug.pluggable);
                studyPlug = studyPlug.GetPrevPlugCable();
            }
            newPList.Reverse();
            if (pluggableEnd != null)
                newPList.Add(pluggableEnd.pluggable);
            instrument = newInstrument;
            pluggablesList = newPList;
            print("Refreshing: " + name + " to instrument: " + (newInstrument != null ? instrument.itemName : "NULL") 
                + ",\tpluggables list contains: " + newPList.Count);
        }

        // TODO: Should set the cable state to abandoned and invoke a cableAbandoned event.
        private void OnCableDisconnect(CableController cable, SpeakerController speaker)
        {
            if (cable != this) return;
            
            Destroy(gameObject);
        }
        private void OnCableDisconnectPlug(CableController cable, PlugCable endObj)
        {
            OnCableDisconnect(cable, null);
        }

        public void CreateNode(CableNode node, Vector3 nodePos)
        {
            CreateNodeAtIndex(node, nodePos, nodes.Count);
        }

        public void CreateNodeAtIndex(CableNode node, Vector3 nodePos, int index)
        {
            node.MoveNode(nodePos);
            
            node.nodeMoved.AddListener(OnNodeMoved);

            nodes.Insert(index, node);

            nodeCreated.Invoke(node);
            
            GameEvents.CableWind(this, nodePos);
        }

        private void OnNodeMoved(CableNode node)
        {
            nodeMoved.Invoke(node);
        }

        public void DestroyNode(CableNode node)
        {
            nodes.Remove(node);
            
            nodeDestroyed.Invoke(node);
        }

        public void Complete()
        {
            state = CableState.Completed;
            
            cableCompleted.Invoke();
        }
    }
}
