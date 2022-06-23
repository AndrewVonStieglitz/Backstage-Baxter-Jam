using UnityEngine;

namespace Cables.Platforms
{
    public class PlatformNode : CableNode
    {
        public PolygonCollider2D PolyCollider { get; }
        public int VertexIndex { get; }
        public Vector2 Normal { get; }

        public PlatformNode(PolygonCollider2D polyCollider, int vertexIndex, Vector2 normal)
        {
            PolyCollider = polyCollider;
            VertexIndex = vertexIndex;
            Normal = normal;
        }
    }
}