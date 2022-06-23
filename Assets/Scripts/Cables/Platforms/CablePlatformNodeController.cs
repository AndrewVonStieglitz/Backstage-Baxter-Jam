using System;
using UnityEngine;

namespace Cables.Platforms
{
    public class CablePlatformNodeController : MonoBehaviour
    {
        [SerializeField] private CableController cable;
        [SerializeField] private GameObject platformNodePrefab;
        [SerializeField] private float cableRaycastSize;
        [SerializeField] private LayerMask platformLayerMask;

        private void OnEnable()
        {
            cable.nodeMoved.AddListener(OnNodeMoved);
        }

        private void OnDisable()
        {
            cable.nodeMoved.RemoveListener(OnNodeMoved);
        }

        private void OnNodeMoved(CableNode cableNode)
        {
            CheckForPlatformNodeToCreate();

            CheckForPlatformNodeToDestroy();
        }

        // TODO: Should take the cableNode that was modified. Currently only supports the last cableNode.
        private void CheckForPlatformNodeToCreate()
        {
            if (cable.nodes.Count < 2) return;

            var hit = RaycastBetweenNodes(cable.nodes[cable.nodes.Count - 1], cable.nodes[cable.nodes.Count - 2]);

            if (hit.collider == null) return;
            
            var polyCollider = hit.collider as PolygonCollider2D;
            
            if (polyCollider == null) return;

            var closestVertexIndex = ClosestVertexIndex(polyCollider, hit.point);

            if (closestVertexIndex == -1) return;

            Vector2 closestVertex = WorldSpaceVertex(polyCollider, closestVertexIndex);
            
            var normal = VertexNormal(polyCollider, closestVertexIndex);

            var nodePos = closestVertex + normal.normalized * cable.cableWidth / 2;
            
            var node = cable.CreateNodeAtIndex(platformNodePrefab, nodePos, cable.nodes.Count - 1);

            var platformNode = node as PlatformNode;
            
            if (platformNode == null) throw new Exception($"No {nameof(PlatformNode)} component on node prefab.");

            platformNode.PolyCollider = polyCollider;
            platformNode.VertexIndex = closestVertexIndex;
            platformNode.Normal = normal;
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
            float minDistance = float.MaxValue;
            int closestVertexIndex = -1;

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
        
        // TODO: Should take the cableNode that was modified. Currently only supports the last cableNode.
        private void CheckForPlatformNodeToDestroy()
        {
            if (cable.nodes.Count < 3) return;

            var node = cable.nodes[cable.nodes.Count - 2] as PlatformNode;
            
            if (node is null) return;

            var hit2 = RaycastBetweenNodes(cable.nodes[cable.nodes.Count - 1], cable.nodes[cable.nodes.Count - 3]);

            if (hit2.collider != null) return;

            cable.DestroyNode(cable.nodes[cable.nodes.Count - 2]);
        }

        private RaycastHit2D RaycastBetweenNodes(CableNode fromNode, CableNode toNode)
        {
            Vector2 fromNodePos = fromNode.transform.position;
            Vector2 toNodePos = toNode.transform.position;

            var difference = toNodePos - fromNodePos;

            Vector2 perpendicular = Vector3.Cross(difference, Vector3.forward).normalized;

            var cableSideA = fromNodePos + perpendicular * cable.cableWidth / 2 * cableRaycastSize;
            
            var cableSideAHit = Physics2D.Raycast(cableSideA, difference, difference.magnitude, platformLayerMask);

            if (cableSideAHit.collider != null)
                return cableSideAHit;
            
            var cableSideB = fromNodePos + perpendicular * cable.cableWidth / -2 * cableRaycastSize;

            return Physics2D.Raycast(cableSideB, difference, difference.magnitude, platformLayerMask);
        }
    }
}