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
            
            CreateNode(amp.transform.position, Vector2.zero);
            CreateNode(amp.transform.position, Vector2.zero);
            
            initialised.Invoke();
        }

        // TODO: Should set the cable state to abandoned and invoke a cableAbandoned event.
        private void OnCableDisconnect(CableController cable, SpeakerController speaker)
        {
            if (cable != this) return;
            
            Destroy(gameObject);
        }

        public void PipeEnter(Vector2 nodePos, Vector2 normal)
        {
            pipeEntryNormal = normal;

            if (!(Vector2.Dot(nodePos - (Vector2)nodes[nodes.Count - 2].transform.position, normal) > 0)) return;
            
            CreateNode(nodePos, normal);
        }

        public void PipeExit(Vector2 normal)
        {
            if (nodes.Count <= 2) return;

            if (Vector2.Dot(pipeEntryNormal, normal) < 0) return;
            
            DestroyNode(nodes[nodes.Count - 1]);
        }

        private void CreateNode(Vector3 nodePos, Vector2 normal)
        {
            CreateNodeAtIndex(nodePos, normal, nodes.Count);
        }

        private void CreateNodeAtIndex(Vector3 nodePos, Vector2 normal, int index)
        {
            var nodeObject = Instantiate(nodePrefab, nodePos, Quaternion.identity, nodeParent);

            var node = nodeObject.GetComponent<CableNode>();

            if (node == null) throw new Exception("No node component on node prefab.");

            node.Normal = normal;
            node.MoveNode(nodePos);
            
            node.nodeMoved.AddListener(OnNodeMoved);

            nodes.Insert(index, node);

            nodeCreated.Invoke(node);
            
            GameEvents.CableWind(this, nodePos);
        }

        private void OnNodeMoved(CableNode node)
        {
            UpdateZAxis(node);
            
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
        
        // TODO: Needs a better name, and refactoring
        private void UpdateZAxis(CableNode cableNode)
        {
            CreateZAxisNode();

            DestroyZAxisNode();
        }

        private void CreateZAxisNode()
        {
            if (nodes.Count < 2) return;

            var hit = RaycastBetweenNodes(nodes[nodes.Count - 1], nodes[nodes.Count - 2], 1 << 6);

            if (hit.collider == null) return;

            // Find the nearest vertex
            var polyCollider = hit.collider as PolygonCollider2D;

            if (polyCollider == null) return;

            float minDistance = 100000;
            Vector2 closestVertex = new Vector2();

            // TODO: Replace with MinBy from MoreLINQ
            foreach (var vertex in polyCollider.points)
            {
                var vertexWorldSpace = polyCollider.transform.TransformPoint(vertex);

                var distance = Vector2.Distance(hit.point, vertexWorldSpace);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestVertex = vertexWorldSpace;
                }
            }

            if (closestVertex == Vector2.zero) return;

            // Offset the closest point by the width.
            // TODO: This is only an approximation of the normal
            var normalish = closestVertex - (Vector2)polyCollider.bounds.center;

            var nodePos = closestVertex + normalish.normalized * cableWidth / 2;

            Debug.DrawRay(closestVertex, normalish, Color.blue, 20);


            CreateNodeAtIndex(nodePos, normalish, nodes.Count - 1);
        }

        private void DestroyZAxisNode()
        {
            if (nodes.Count < 3) return;

            var hit2 = RaycastBetweenNodes(nodes[nodes.Count - 1], nodes[nodes.Count - 3], 1 << 6);

            if (hit2.collider != null) return;

            // TODO: I'm not sure there's any point specifiying which node to remove, sine I'm pretty sure anything but hte last node isn't supported anyway
            DestroyNode(nodes[nodes.Count - 1]);
        }

        private RaycastHit2D RaycastBetweenNodes(CableNode fromNode, CableNode toNode, int layerMask)
        {
            Vector2 fromNodePos = fromNode.transform.position;
            Vector2 toNodePos = toNode.transform.position;

            var difference = toNodePos - fromNodePos;

            return Physics2D.Raycast(fromNodePos, difference, difference.magnitude, layerMask);
        }
    }
}
