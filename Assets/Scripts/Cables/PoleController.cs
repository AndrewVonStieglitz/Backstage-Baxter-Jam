using System;
using UnityEngine;
using static Cables.OrientationUtil;

namespace Cables
{
    public class PoleController : MonoBehaviour
    {
        [SerializeField] private Orientation poleOrientation;
        
        [Obsolete]
        public Orientation PoleOrientation { get => poleOrientation; }
    }
}
