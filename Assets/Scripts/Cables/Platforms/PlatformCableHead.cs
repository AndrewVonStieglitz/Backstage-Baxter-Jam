using UnityEngine;

namespace Cables.Platforms
{
    public class PlatformCableHead : MonoBehaviour
    {
        [SerializeField] private CableHead cableHead;
        [SerializeField] private float cableRaycastSize;
        [SerializeField] private LayerMask platformLayerMask;

        private CableController cable;

        private void OnEnable()
        {
            cableHead.cableChanged.AddListener(OnCableChanged);
        }

        private void OnDisable()
        {
            cableHead.cableChanged.RemoveListener(OnCableChanged);
        }

        private void OnCableChanged()
        {
            if (cable != null)
                cable.nodeMoved.RemoveListener(OnNodeMoved);
            
            cable = cableHead.CurrentCable;

            if (cable != null)
                cable.nodeMoved.AddListener(OnNodeMoved);
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

            var node = new PlatformNode(polyCollider, closestVertexIndex, normal);
            
            cable.CreateNodeAtIndex(node, nodePos, cable.nodes.Count - 1);
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

            var cableNode = cable.nodes[cable.nodes.Count - 2];
            
            var platformNode = cableNode as PlatformNode;
            
            if (platformNode is null) return;

            var inVector = cable.nodes[cable.nodes.Count - 3].Position - cableNode.Position;
            var outVector = cable.nodes[cable.nodes.Count - 1].Position - cableNode.Position;

            var inAngle = Vector2.Angle(inVector, platformNode.Normal);
            var outAngle = Vector2.Angle(outVector, platformNode.Normal);

            if (inAngle + outAngle > 180) return;

            cable.DestroyNode(cableNode);
        }

        private RaycastHit2D RaycastBetweenNodes(CableNode fromNode, CableNode toNode)
        {
            var fromNodePos = fromNode.Position;
            var toNodePos = toNode.Position;

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