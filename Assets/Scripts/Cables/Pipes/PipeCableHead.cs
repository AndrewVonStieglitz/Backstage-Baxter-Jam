using UnityEngine;
using Utility;

namespace Cables.Pipes
{
    [RequireComponent(typeof(Collider2D))]
    public class PipeCableHead : MonoBehaviour
    {
        [SerializeField] private CableHead cableHead;
        [SerializeField] private VelocityTracker velocityTracker;

        private new Collider2D collider2D;
        private Vector2 pipeEntryNormal;
        private CableController Cable => cableHead.CurrentCable;

        private void Awake()
        {
            collider2D = GetComponent<Collider2D>();
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (Cable == null) return;

            if (!col.CompareTag("Pipe")) return;

            var hit = UtilityFunctions.TriggerCollision(collider2D, velocityTracker.Velocity);

            Vector2 nodePosition = hit.point + hit.normal * Cable.cableWidth / 2;

            // Draw collision normals
            Debug.DrawLine(hit.point, hit.point + hit.normal, Color.yellow, 30f);

            pipeEntryNormal = hit.normal;

            if (!AwayFromPreviousNode(nodePosition, hit.normal)) return;

            var node = new PipeNode(hit.normal);
            
            Cable.CreateNodeAtIndex(node, nodePosition, Cable.nodes.Count - 1);
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            if (Cable == null) return;
            
            if (!col.CompareTag("Pipe")) return;

            var hit = UtilityFunctions.TriggerCollision(collider2D, -velocityTracker.Velocity);
            
            if (Cable.nodes.Count <= 2) return;
            
            if (Vector2.Dot(pipeEntryNormal, hit.normal) < 0) return;
            
            Cable.DestroyNode(Cable.nodes[Cable.nodes.Count - 1]);
        }
        
        private bool AwayFromPreviousNode(Vector2 nodePos, Vector2 normal)
        {
            return Vector2.Dot(nodePos - Cable.nodes[Cable.nodes.Count - 2].Position, normal) > 0;
        }
    }
}