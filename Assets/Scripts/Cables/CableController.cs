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
        [SerializeField] private float friction;
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
        
        private Vector2 pipeEntryNormal;

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
            
            CreateNode(amp.transform.position, Vector2.zero);
            CreateNode(amp.transform.position, Vector2.zero);
            
            initialised.Invoke();
        }

        public void Initialise(PlugCable startObj)
        {
            this.pluggableStart = startObj;
            if (startObj.tag == "Instrument")
            {
                instrument = startObj.instrument;
            }
            if (startObj.tag == "Pluggable")
            {
                pluggablesList.Add(startObj.pluggable);
                instrument = startObj.GetPathsInstrument();
            }
            
            CreateNode(pluggableStart.transform.position, Vector2.zero);
            CreateNode(pluggableStart.transform.position, Vector2.zero);

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

        public void PipeEnter(Vector2 nodePos, Vector2 normal)
        {
            pipeEntryNormal = normal;

            if (!(Vector2.Dot(nodePos - (Vector2)nodes[nodes.Count - 2].transform.position, normal) > 0)) return;
            
            CreateNode(nodePos, normal);
        }

        public void PipeExit(Vector2 normal)
        {
            if (nodes.Count <= 1) return;

            if (Vector2.Dot(pipeEntryNormal, normal) < 0) return;
            
            DestroyNode(nodes[nodes.Count - 1]);
        }

        private void CreateNode(Vector3 nodePos, Vector2 normal)
        {
            var nodeObject = Instantiate(nodePrefab, nodePos, Quaternion.identity, nodeParent);

            var node = nodeObject.GetComponent<CableNode>();

            if (node == null) throw new Exception("No node component on node prefab.");
            
            node.nodeMoved.AddListener(OnNodeMoved);

            node.Normal = normal;
            
            if (nodes.Count < 1)
                nodes.Add(node);
            else
                nodes.Insert(nodes.Count - 1, node);

            nodeCreated.Invoke(node);
            
            GameEvents.CableWind(this, nodePos);
        }

        private void OnNodeMoved(CableNode node)
        {
            nodeMoved.Invoke(node);
        }

        private void DestroyNode(CableNode node)
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
