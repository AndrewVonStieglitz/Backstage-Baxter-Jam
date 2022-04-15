using UnityEngine;
using static Cables.OrientationUtil;

namespace Cables
{
    public class CableNode : MonoBehaviour
    {
        private Orientation nodeOrientation;
        public Orientation NodeOrientation { get => nodeOrientation; }

        private CableController cable;
        public CableController CableControllerID { get => cable; }

        public CableNode(Orientation nodeOrientation, CableController cable)
        {
            this.nodeOrientation = nodeOrientation;
            this.cable = cable;
        }
        
        public Orientation Orientation { get; set; }
    }
}
