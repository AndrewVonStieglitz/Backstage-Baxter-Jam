using UnityEngine;

namespace Pluggables
{
    public class ConnectionHead : MonoBehaviour
    {
        public Connection Connection;
        
        private Collider2D lastOverlappedTrigCollider;

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
            AbandonConnection();
        }

        public bool TryInteract()
        {
            if (lastOverlappedTrigCollider != null)
            {
                if (lastOverlappedTrigCollider.TryGetComponent(out PlugCable plugCableInto)) {
                    plugCableInto.Interact(Connection);
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

        public void AbandonConnection()
        {
            if (Connection == null) return;
            
            var connection = Connection;
            
            Connection = null;
            
            connection.PluggableStart.UnplugOutput();
            
            GameEvents.ConnectionAbandoned(connection);
        }
    }
}