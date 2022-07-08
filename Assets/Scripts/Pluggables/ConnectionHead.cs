using UnityEngine;

namespace DefaultNamespace.Pluggables
{
    public class ConnectionHead : MonoBehaviour
    {
        public Connection Connection;
        
        private Collider2D lastOverlappedTrigCollider;
        
        public bool TryInteract()
        {
            //print("Cable head TryInteract, overlap = " + (lastOverlappedTrigCollider != null));
            //if ((lastOverlappedTrigCollider != null))
            //    print("with: " + lastOverlappedTrigCollider.name);
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
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            if (lastOverlappedTrigCollider == col)
                lastOverlappedTrigCollider = null;
        }
    }
}