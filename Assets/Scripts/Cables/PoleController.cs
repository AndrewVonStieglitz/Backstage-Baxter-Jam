using UnityEngine;
using static Cables.OrientationUtil;

namespace Cables
{
    public class PoleController : MonoBehaviour
    {
        [SerializeField] private Orientation poleOrientation;
        public Orientation PoleOrientation { get => poleOrientation; }
    }
}
