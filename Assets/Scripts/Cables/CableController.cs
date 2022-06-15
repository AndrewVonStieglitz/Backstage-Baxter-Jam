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
        [SerializeField] private float cableRaycastSize;
        
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
            
            var polyCollider = hit.collider as PolygonCollider2D;
            
            if (polyCollider == null) return;

            var closestVertexIndex = ClosestVertexIndex(polyCollider, hit.point);

            if (closestVertexIndex == -1) return;

            Vector2 closestVertex = WorldSpaceVertex(polyCollider, closestVertexIndex);
            
            var normal = VertexNormal(polyCollider, closestVertexIndex);

            var nodePos = closestVertex + normal.normalized * cableWidth / 2;
            
            CreateNodeAtIndex(nodePos, normal, nodes.Count - 1);
        }

        private static Vector2 VertexNormal(PolygonCollider2D polyCollider, int closestVertexIndex)
        {
            Vector2 closestVertex = WorldSpaceVertex(polyCollider, closestVertexIndex);

            var previousVertexIndex = (int) Mathf.Repeat(closestVertexIndex - 1, polyCollider.points.Length);
            var nextVertexIndex = (int) Mathf.Repeat(closestVertexIndex + 1, polyCollider.points.Length);

            Vector2 previousVertex = WorldSpaceVertex(polyCollider, previousVertexIndex);
            Vector2 nextVertex = WorldSpaceVertex(polyCollider, nextVertexIndex);

            var previousEdge = closestVertex - previousVertex;
            var nextEdge = closestVertex - nextVertex;

            Debug.DrawRay(closestVertex, previousEdge, Color.green, 20);
            Debug.DrawRay(closestVertex, nextEdge, Color.green, 20);

            var normal = previousEdge.normalized + nextEdge.normalized;
            
            Debug.DrawRay(closestVertex, normal, Color.blue, 20);

            return normal;
        }

        private static int ClosestVertexIndex(PolygonCollider2D polyCollider, Vector2 point)
        {
            float minDistance = 100000;
            int closestVertexIndex = -1;

            // TODO: Replace with MinBy from MoreLINQ
            for (var vertexIndex = 0; vertexIndex < polyCollider.points.Length; vertexIndex++)
            {
                Vector2 vertex = WorldSpaceVertex(polyCollider, vertexIndex);

                var distance = Vector2.Distance(point, vertex);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestVertexIndex = vertexIndex;
                }
            }

            return closestVertexIndex;
        }

        private static Vector3 WorldSpaceVertex(PolygonCollider2D polyCollider, int vertexIndex)
        {
            var vertexLocalSpace = polyCollider.points[vertexIndex];
            
            return polyCollider.transform.TransformPoint(vertexLocalSpace);
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

            Vector2 perpendicular = Vector3.Cross(difference, Vector3.forward).normalized;

            var cableSideA = fromNodePos + perpendicular * cableWidth / 2 * cableRaycastSize;
            
            var cableSideAHit = Physics2D.Raycast(cableSideA, difference, difference.magnitude, layerMask);

            if (cableSideAHit.collider != null)
                return cableSideAHit;
            
            var cableSideB = fromNodePos + perpendicular * cableWidth / -2 * cableRaycastSize;

            return Physics2D.Raycast(cableSideB, difference, difference.magnitude, layerMask);
        }
    }
}
