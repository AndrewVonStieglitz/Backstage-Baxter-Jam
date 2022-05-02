using UnityEngine;

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

        public Vector2 Normal { get; set; }
    }
}
