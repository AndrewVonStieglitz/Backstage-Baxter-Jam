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
        public Orientation NodeOrientation { get => nodeOrientation; }

        public CableNode(Orientation nodeOrientation)
        {
            this.nodeOrientation = nodeOrientation;
        }
        
        public Orientation Orientation { get; set; }
    }
}
