using UnityEngine;
using UnityEngine.Events;

namespace Cables
{
    public class CableNode : MonoBehaviour
    {
        public UnityEvent<CableNode> nodeMoved = new UnityEvent<CableNode>();

        public void MoveNode(Vector3 newPosition)
        {
            transform.position = newPosition;
            
            nodeMoved.Invoke(this);
        }
    }
}
