using UnityEngine;
using UnityEngine.Events;

namespace Cables
{
    public class CableNode
    {
        public Vector2 Position { get; private set; }
        
        public UnityEvent<CableNode> nodeMoved = new UnityEvent<CableNode>();

        public void MoveNode(Vector3 newPosition)
        {
            Position = newPosition;
            
            nodeMoved.Invoke(this);
        }
    }
}
