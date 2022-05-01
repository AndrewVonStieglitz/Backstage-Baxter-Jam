using System;
using UnityEngine;
using static Cables.OrientationUtil;

namespace Cables
{
    public class CableNode : MonoBehaviour
    {
        public enum PoleSide
        {
            Over,
            Under
        }

        public PoleSide poleSide;

        private Orientation nodeOrientation;
        
        [Obsolete("Use CableNode.Normal instead.")]
        public Orientation NodeOrientation { get => nodeOrientation; }

        public CableNode(Orientation nodeOrientation)
        {
            this.nodeOrientation = nodeOrientation;
        }
        
        [Obsolete("Use CableNode.Normal instead.")]
        public Orientation Orientation { get; set; }
        
        public Vector2 Normal { get; set; }
    }
}
