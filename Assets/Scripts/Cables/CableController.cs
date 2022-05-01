using System;
using System.Collections.Generic;
using System.Linq;
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
        public UnityEvent<int> nodeCreated = new UnityEvent<int>();
        public UnityEvent<CableNode> nodeDestroyed = new UnityEvent<CableNode>();
        public UnityEvent cableCompleted = new UnityEvent();

        public CableState state;
        public AmpController amp;
        public float cableWidth;
        public List<CableNode> nodes = new List<CableNode>();
        
        private Vector2 pipeEntryNormal;

        private void OnEnable()
        {
            GameEvents.onCableDisconnect += OnCableDisconnect;
        }
        
        private void OnDisable()
        {
            GameEvents.onCableDisconnect -= OnCableDisconnect;
        }

        public void Initialise(AmpController amp)
        {
            this.amp = amp;
            
            CreateNode(amp.transform.position, OrientationUtil.Orientation.Horizontal, Vector2.zero);
            
            initialised.Invoke();
        }

        // TODO: Should set the cable state to abandoned and invoke a cableAbandoned event.
        private void OnCableDisconnect(CableController cable, SpeakerController speaker)
        {
            if (cable != this) return;
            
            Destroy(gameObject);
        }

        public void PipeEnter(PoleController pole, Vector2 nodePosition, Vector2 normal)
        {
            var poleOrientation = pole.PoleOrientation;

            pipeEntryNormal = normal;

            if (!(Vector2.Dot(nodePosition - (Vector2)nodes.Last().transform.position, normal) > 0)) return;
            
            CreateNode(nodePosition, poleOrientation, normal);
        }

        public void PipeExit(PoleController pole, Vector2 normal)
        {
            if (nodes.Count <= 1) return;

            if (Vector2.Dot(pipeEntryNormal, normal) < 0) return;
            
            DestroyNode(nodes.Last());
        }

        private void CreateNode(Vector3 nodePos, OrientationUtil.Orientation orientation, Vector2 normal)
        {
            var nodeObject = Instantiate(nodePrefab, nodePos, Quaternion.identity, nodeParent);

            var node = nodeObject.GetComponent<CableNode>();

            if (node == null) throw new Exception("No node component on node prefab.");

            node.Orientation = orientation;
            node.Normal = normal;

            node.poleSide = nodes.Count < 1 || nodes[nodes.Count - 1].poleSide == CableNode.PoleSide.Under
                ? CableNode.PoleSide.Over
                : CableNode.PoleSide.Under;
            
            nodes.Add(node);
            
            nodeCreated.Invoke(nodes.Count - 1);
            
            GameEvents.CableWind(this, orientation, nodePos);
        }

        private void DestroyNode(CableNode node)
        {
            Destroy(node.gameObject);

            nodes.Remove(node);
            
            nodeDestroyed.Invoke(node);
        }

        public void Complete(Vector3 pos)
        {
            state = CableState.Completed;

            CreateNode(pos, OrientationUtil.Orientation.Horizontal, Vector2.zero);
            
            cableCompleted.Invoke();
        }
    }
}
