using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Cables
{
    public class CableHead : MonoBehaviour
    {
        public CableController Cable
        {
            get => cable;
            private set
            {
                cable = value;
                cableChanged.Invoke();
            }
        }

        private Vector3 lastPosition;
        public Vector3 velocity;
        private BoxCollider2D boxCollider2D;
        private Collider2D lastOverlappedTrigCollider;

        public UnityEvent cableChanged = new UnityEvent();
        private CableController cable;

        private void OnEnable()
        {
            GameEvents.onPlayerCableCollision += OnPlayerCableCollision;
        }

        private void OnDisable()
        {
            GameEvents.onPlayerCableCollision -= OnPlayerCableCollision;
        }

        private void OnPlayerCableCollision(Vector2 position, Vector2 normal)
        {
            DropCable();
        }

        public void NewCable(CableController cable)
        {
            Cable = cable;

            boxCollider2D = GetComponent<BoxCollider2D>();
            boxCollider2D.size = new Vector2(cable.cableWidth, cable.cableWidth);
            
            cable.cableCompleted.AddListener(OnCableCompleted);
        }

        private void OnCableCompleted()
        {
            Cable.cableCompleted.RemoveListener(OnCableCompleted);

            Cable = null;
        }

        public void DropCable()
        {
            if (cable)
            {
                cable.pluggableStart.PlayRandomDisconnectSound();
                Destroy(cable.gameObject);
            }
        }

        public bool TryInteract()
        {
            //print("Cable head TryInteract, overlap = " + (lastOverlappedTrigCollider != null));
            if (lastOverlappedTrigCollider != null)
            {
                if (lastOverlappedTrigCollider.TryGetComponent(out PlugCable plugCableInto)) {
                    plugCableInto.Interact();
                    return true;
                }
            }
            return false;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            lastOverlappedTrigCollider = col;
            
            CheckCableCollision(col);
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            if (lastOverlappedTrigCollider == col)
                lastOverlappedTrigCollider = null;
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

        public RaycastHit2D TriggerCollision(Vector2 castDirection)
        {
            var hits = new RaycastHit2D[1];
            
            boxCollider2D.Cast(castDirection, hits, 1);

            return hits[0];
        }

        private void FixedUpdate()
        {
            if (Cable == null || Cable.state != CableController.CableState.InProgress) return;

            velocity = (transform.position - lastPosition) / Time.deltaTime;
            lastPosition = transform.position;

            Cable.nodes.Last().MoveNode(transform.position);
        }
    }
}