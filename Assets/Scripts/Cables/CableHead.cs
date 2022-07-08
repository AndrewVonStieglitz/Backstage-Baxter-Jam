using System.Linq;
using Pluggables;
using UnityEngine;
using UnityEngine.Events;

namespace Cables
{
    public class CableHead : MonoBehaviour
    {
        [SerializeField] private GameObject cablePrefab;
        
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
        // TODO: Use SerializedField.
        private BoxCollider2D boxCollider2D;

        public UnityEvent cableChanged = new UnityEvent();
        private CableController cable;

        private Connection connection;

        private void OnEnable()
        {
            GameEvents.onConnectionStarted += OnConnectionStarted;
            GameEvents.onPlayerCableCollision += OnPlayerCableCollision;
            GameEvents.onConnect += OnConnect;
            GameEvents.onDisconnect += OnDisconnect;
        }

        private void OnDisable()
        {
            GameEvents.onConnectionStarted -= OnConnectionStarted;
            GameEvents.onPlayerCableCollision -= OnPlayerCableCollision;
            GameEvents.onConnect -= OnConnect;
            GameEvents.onDisconnect -= OnDisconnect;
        }

        private void OnConnectionStarted(Connection connection)
        {
            var cableObject = Instantiate(cablePrefab, connection.pluggableStart.transform);
            
            Cable = cableObject.GetComponent<CableController>();

            boxCollider2D = GetComponent<BoxCollider2D>();
            boxCollider2D.size = new Vector2(Cable.cableWidth, Cable.cableWidth);
            
            Cable.Initialise(connection.pluggableStart.transform, connection.cableColor);

            this.connection = connection;
        }

        private void OnPlayerCableCollision(Vector2 position, Vector2 normal)
        {
            DropCable();
        }

        private void OnConnect(Connection connection, PlugCable destination)
        {
            Cable.Complete(destination.transform.position);

            Cable = null;
        }
        
        // TODO: Set the cable state to abandoned and invoke a cableAbandoned event.
        // TODO: Rename this, and some of the other events too.
        private void OnDisconnect(Connection connection, PlugCable endObj)
        {
            if (connection != this.connection) return;
            
            DropCable();
        }

        public void DropCable()
        {
            if (Cable)
            {
                // TODO: Reimplement this somewhere.
                // Cable.pluggableStart.PlayRandomDisconnectSound();
                Destroy(Cable.gameObject);
                
                connection = null;
            }
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            CheckCableCollision(col);
        }

        private void CheckCableCollision(Collider2D col)
        {
            if (Cable == null) return;
            
            if (!col.CompareTag("Cable")) return;

            // TODO: Duplicate code. See PipeCableHead.CheckCableCollision.
            // rushjob code to fit catastrophic bug 
            if (col.GetComponentInParent<CableController>().cableColor == Cable.cableColor) return;

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