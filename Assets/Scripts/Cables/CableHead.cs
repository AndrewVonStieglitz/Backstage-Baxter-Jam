using System.Linq;
using UnityEngine;

namespace Cables
{
    public class CableHead : MonoBehaviour
    {
        public CableController cable;

        private Vector3 lastPosition;
        public Vector3 velocity;
        private BoxCollider2D boxCollider2D;
        private Collider2D lastOverlappedTrigCollider;
        
        public void NewCable(CableController cable)
        {
            this.cable = cable;

            boxCollider2D = GetComponent<BoxCollider2D>();
            boxCollider2D.size = new Vector2(cable.cableWidth, cable.cableWidth);
            
            cable.cableCompleted.AddListener(OnCableCompleted);
        }

        private void OnCableCompleted()
        {
            cable.cableCompleted.RemoveListener(OnCableCompleted);

            cable = null;
        }

        public void DropCable()
        {
            if (cable)
                Destroy(cable.gameObject);
        }

        public bool TryInteract()
        {
            //print(lastOverlappedTrigCollider.gameObject.GetComponent<PlugCable>());
            if (lastOverlappedTrigCollider != null)
            {
                if (lastOverlappedTrigCollider.TryGetComponent(out PlugCable plugCableInto)) {
                    plugCableInto.Interact();
                    return true;
                }
                //try
                //{
                //    print("Cable head attempting to interact with PlugCable on: " + lastOverlappedTrigCollider.name);
                //    PlugCable plugCableInto = lastOverlappedTrigCollider.gameObject.GetComponent<PlugCable>();
                //    plugCableInto.Interact();
                //    return true;
                //}
                //catch
                //{
                //    print("Cable head Could not find Plugcable on most recent trigger contact");
                //    return false;
                //}
            }
            return false;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            CheckPipeCollision(col);

            CheckCableCollision(col);
        }

        private void CheckCableCollision(Collider2D col)
        {
            if (cable == null) return;
            
            if (!col.CompareTag("Cable")) return;

            var hit = TriggerCollision(velocity);

            // Debug.Log("Player Cable Collision");
            // Debug.DrawLine(hit.point, hit.point + hit.normal, Color.yellow, 30f);

            GameEvents.PlayerCableCollision(hit.point, hit.normal);
        }

        private void CheckPipeCollision(Collider2D col)
        {
            // TODO: Duplicate code. See OnTriggerExit2D.
            lastOverlappedTrigCollider = col;
            if (cable == null) return;

            if (!col.CompareTag("Pipe")) return;

            var hit = TriggerCollision(velocity);

            Vector2 nodePosition = hit.point + hit.normal * cable.cableWidth / 2;

            // Draw collision normals
            Debug.DrawLine(hit.point, hit.point + hit.normal, Color.yellow, 30f);

            cable.PipeEnter(nodePosition, hit.normal);
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            // TODO: Duplicate code. See OnTriggerExit2D.
            if (cable == null) return;
            
            if (!col.CompareTag("Pipe")) return;

            var hit = TriggerCollision(-velocity);
            
            cable.PipeExit(hit.normal);
        }

        private RaycastHit2D TriggerCollision(Vector2 castDirection)
        {
            var hits = new RaycastHit2D[1];
            
            boxCollider2D.Cast(castDirection, hits, 1);

            return hits[0];
        }

        private void FixedUpdate()
        {
            if (cable == null || cable.state != CableController.CableState.InProgress) return;

            velocity = (transform.position - lastPosition) / Time.deltaTime;
            lastPosition = transform.position;

            cable.nodes.Last().MoveNode(transform.position);
        }
    }
}