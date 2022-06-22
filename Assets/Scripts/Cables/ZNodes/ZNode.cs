using UnityEngine;

namespace Cables
{
    public class ZNode : CableNode
    {
        public PolygonCollider2D PolyCollider { get; set; }
        public int VertexIndex { get; set; }
        public Vector2 ZAxisNormal { get; set; }
    }
}