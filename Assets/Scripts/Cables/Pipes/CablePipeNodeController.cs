using UnityEngine;

namespace Cables.Pipes
{
    public class CablePipeNodeController : MonoBehaviour
    {
        [SerializeField] private CableController cable;
        
        private Vector2 pipeEntryNormal;
    
        public void PipeEnter(Vector2 nodePos, Vector2 normal)
        {
            pipeEntryNormal = normal;

            if (!AwayFromPreviousNode(nodePos, normal)) return;

            var node = new PipeNode(normal);
            
            cable.CreateNodeAtIndex(node, nodePos, cable.nodes.Count - 1);
        }

        private bool AwayFromPreviousNode(Vector2 nodePos, Vector2 normal)
        {
            return Vector2.Dot(nodePos - cable.nodes[cable.nodes.Count - 2].Position, normal) > 0;
        }

        public void PipeExit(Vector2 normal)
        {
            if (cable.nodes.Count <= 2) return;

            if (Vector2.Dot(pipeEntryNormal, normal) < 0) return;
            
            cable.DestroyNode(cable.nodes[cable.nodes.Count - 1]);
        }
    }
}