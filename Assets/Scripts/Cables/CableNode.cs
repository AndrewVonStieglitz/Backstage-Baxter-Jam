using UnityEngine;
using UnityEngine.Events;

namespace Cables
{
    public class CableNode : MonoBehaviour
    {
        public UnityEvent<CableNode> nodeMoved = new UnityEvent<CableNode>();

        public Vector2 Normal { get; set; }

        public void MoveNode(Vector3 newPosition)
        {
            transform.position = newPosition;
            
            nodeMoved.Invoke(this);
        }
    }
}
