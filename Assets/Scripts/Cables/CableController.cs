using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Cables
{
    public class CableController : MonoBehaviour
    {
        public enum CableState { InProgress, Completed, Abandoned }

        [SerializeField] private int cableID;
        [SerializeField] private GameObject nodePrefab;
        [SerializeField] private Transform nodeParent;
        
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
            CreateNode(nodePrefab, transform.position);
            CreateNode(nodePrefab, transform.position);

            initialised.Invoke();
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

        public CableNode CreateNode(GameObject nodePrefab, Vector3 nodePos)
        {
            return CreateNodeAtIndex(nodePrefab, nodePos, nodes.Count);
        }

        public CableNode CreateNodeAtIndex(GameObject nodePrefab, Vector3 nodePos, int index)
        {
            var nodeObject = Instantiate(nodePrefab, nodePos, Quaternion.identity, nodeParent);

            var node = nodeObject.GetComponent<CableNode>();

            if (node == null) throw new Exception($"No {nameof(CableNode)} component on node prefab.");

            node.MoveNode(nodePos);
            
            node.nodeMoved.AddListener(OnNodeMoved);

            nodes.Insert(index, node);

            nodeCreated.Invoke(node);
            
            GameEvents.CableWind(this, nodePos);

            return node;
        }

        private void OnNodeMoved(CableNode node)
        {
            nodeMoved.Invoke(node);
        }

        public void DestroyNode(CableNode node)
        {
            Destroy(node.gameObject);

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
