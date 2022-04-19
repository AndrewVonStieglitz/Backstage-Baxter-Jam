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
        
        public enum Direction { Left, Right, Up, Down }

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
        
        private PoleController previousPole;
        private PoleController currentPole;
        private Direction pipeEntryDirection;

        private void OnEnable()
        {
            GameEvents.onCableDisconnect += OnCableDisconnect;
        }
        
        private void OnDisable()
        {
            GameEvents.onCableDisconnect -= OnCableDisconnect;
        }

        // TODO: Should set the cable state to abandoned and invoke a cableAbandoned event.
        private void OnCableDisconnect(CableController cable, SpeakerController speaker)
        {
            if (cable != this) return;
            
            Destroy(gameObject);
        }

        public void PipeEnter(PoleController pole, Direction pipeEntryDirection, Vector2 nodePosition)
        {
            var poleOrientation = pole.PoleOrientation;

            this.pipeEntryDirection = pipeEntryDirection;

            if (currentPole != null && (poleOrientation == currentPole.PoleOrientation || poleOrientation == nodes.Last().Orientation))
            {  
                CreateNode(nodePosition, poleOrientation);
            }
            
            previousPole = currentPole;
            currentPole = pole;
        }

        public void PipeExit(PoleController pole, Direction pipeExitDirection)
        {
            if (pole != currentPole) return;

            if (pole.PoleOrientation != nodes[nodes.Count - 1].Orientation) return;
            
            if (pipeExitDirection == pipeEntryDirection) return;

            if (nodes.Count <= 1) return;
            
            DestroyNode(nodes.Last());
        }

        private void CreateNode(Vector3 nodePos, OrientationUtil.Orientation orientation)
        {
            var nodeObject = Instantiate(nodePrefab, nodePos, Quaternion.identity, nodeParent);

            var node = nodeObject.GetComponent<CableNode>();

            if (node == null) throw new Exception("No node component on node prefab.");

            node.Orientation = orientation;

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

        public void Initialise(AmpController amp)
        {
            this.amp = amp;
            
            CreateNode(amp.transform.position, OrientationUtil.Orientation.Horizontal);
            
            initialised.Invoke();
        }

        public void Complete(Vector3 pos)
        {
            state = CableState.Completed;

            CreateNode(pos, OrientationUtil.Orientation.Horizontal);
            
            cableCompleted.Invoke();
        }
    }
}
