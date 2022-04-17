using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Cables
{
    public class CableController : MonoBehaviour
    {
        public enum CableState
        {
            InProgress,
            Completed,
            Abandoned
        }
        
        private int ampID;

        public int AmpID
        {
            get => ampID;
        }

        public CableState state;
        
        [SerializeField] private int cableID;
        public int CableID { get => cableID; }

        public AmpController amp;

        public UnityEvent initialised = new UnityEvent();
        public UnityEvent<int> nodeCreated = new UnityEvent<int>();
        public UnityEvent<CableNode> nodeDestroyed = new UnityEvent<CableNode>();
        public UnityEvent cableCompleted = new UnityEvent();

        public enum Direction { Left, Right, Up, Down }
        
        [SerializeField] private GameObject nodePrefab;
        [SerializeField] private float friction;
        [SerializeField] public Material cableMaterial;
        [SerializeField] private Transform nodeParent;
        
        public float cableWidth;

        public List<CableNode> nodes = new List<CableNode>();
        
        // Pole Tracking
        private PoleController previousPole;
        private PoleController currentPole;

        private Direction pipeEntryDirection;

        private void Update()
        {
            // TODO: Get the angle between the last node and the player
            // TODO: Duplicate code. See CableRenderer.FlatEndedness.
            // if (nodes.Count > 1)
            // {
            //     var angle = Vector2.Angle(nodes[nodes.Count - 1].transform.position, player.position);
            //     
            //     // TODO: If the angle exceeds the friction
            //     // if (angle)
            //
            //     // TODO: Slide the node along the pipe
            // }
        }

        public void PipeEnter(PoleController pole, Direction pipeEntryDirection, Vector2 nodePosition)
        {
            var poleOrientation = pole.PoleOrientation;

            this.pipeEntryDirection = pipeEntryDirection;

            if (previousPole == null || poleOrientation == currentPole.PoleOrientation)
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
            
            if (pipeExitDirection != pipeEntryDirection)
            {
                DestroyNode(nodes.Last());
            }
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

        public void Initialise(AmpController amp, Material cableMaterial)
        {
            this.amp = amp;
            this.cableMaterial = cableMaterial;
            
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
