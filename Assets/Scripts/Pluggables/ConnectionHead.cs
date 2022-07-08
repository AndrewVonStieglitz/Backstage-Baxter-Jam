using UnityEngine;

namespace Pluggables
{
    public class ConnectionHead : MonoBehaviour
    {
        private Connection connection;
        
        private Collider2D lastOverlappedTrigCollider;

        private void OnEnable()
        {
            GameEvents.onPlayerCableCollision += OnPlayerCableCollision;
            GameEvents.onConnect += OnConnect;
            GameEvents.onConnectionStarted += OnConnectionStarted;
        }
        
        private void OnDisable()
        {
            GameEvents.onPlayerCableCollision -= OnPlayerCableCollision;
            GameEvents.onConnect -= OnConnect;
            GameEvents.onConnectionStarted -= OnConnectionStarted;
        }

        private void OnConnectionStarted(Connection connection)
        {
            this.connection = connection;
        }

        private void OnConnect(Connection connection, PlugCable plugCable)
        {
            this.connection = null;
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
                    plugCableInto.Interact(connection);
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
            if (this.connection == null) return;
            
            var connection = this.connection;
            
            this.connection = null;
            
            connection.PluggableStart.UnplugOutput();
            
            GameEvents.ConnectionAbandoned(connection);
        }
    }
}