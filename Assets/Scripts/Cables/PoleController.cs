using UnityEngine;
using static Cables.OrientationUtil;

namespace Cables
{
    public class PoleController : MonoBehaviour
    {
        private Orientation poleOrientation;
        public Orientation PoleOrientation { get => poleOrientation; }
    }
}
