using UnityEngine;

namespace Cables.Platforms
{
    public class PlatformNode : CableNode
    {
        public PolygonCollider2D PolyCollider { get; set; }
        public int VertexIndex { get; set; }
        public Vector2 Normal { get; set; }
    }
}