using System.Collections.Generic;
using System.Linq;
using Pluggables;
using UnityEngine;
using UnityEngine.Events;

namespace Cables
{
    public class CableHead : MonoBehaviour
    {
        [SerializeField] private GameObject cablePrefab;
        
        public CableController CurrentCable
        {
            get => currentCurrentCable;
            private set
            {
                currentCurrentCable = value;
                cableChanged.Invoke();
            }
        }

        private Vector3 lastPosition;
        public Vector3 velocity;
        // TODO: Use SerializedField.
        private BoxCollider2D boxCollider2D;

        public UnityEvent cableChanged = new UnityEvent();
        private CableController currentCurrentCable;

        private Connection connection;

        private Dictionary<Connection, CableController> cables = new Dictionary<Connection, CableController>();

        private void OnEnable()
        {
            GameEvents.onConnectionStarted += OnConnectionStarted;
            GameEvents.onConnect += OnConnect;
            GameEvents.onDisconnect += OnDisconnect;
            GameEvents.onConnectionAbandoned += OnConnectionAbandoned;
        }

        private void OnConnectionAbandoned(Connection connection)
        {
            DestroyCable(connection);
        }

        private void OnDisable()
        {
            GameEvents.onConnectionStarted -= OnConnectionStarted;
            GameEvents.onConnect -= OnConnect;
            GameEvents.onDisconnect -= OnDisconnect;
        }

        private void OnConnectionStarted(Connection connection)
        {
            var cableObject = Instantiate(cablePrefab, connection.PluggableStart.transform);
            
            CurrentCable = cableObject.GetComponent<CableController>();
            
            cables.Add(connection, CurrentCable);

            boxCollider2D = GetComponent<BoxCollider2D>();
            boxCollider2D.size = new Vector2(CurrentCable.cableWidth, CurrentCable.cableWidth);
            
            CurrentCable.Initialise(connection.PluggableStart.transform, connection.cableColor);

            this.connection = connection;
        }

        private void OnConnect(Connection connection, PlugCable destination)
        {
            CurrentCable.Complete(destination.transform.position);

            CurrentCable = null;
        }
        
        // TODO: Set the cable state to abandoned and invoke a cableAbandoned event.
        // TODO: Rename this, and some of the other events too.
        private void OnDisconnect(Connection connection, PlugCable endObj)
        {
            DestroyCable(connection);
        }

        private void DestroyCable(Connection connection)
        {
            if (!cables.ContainsKey(connection)) return;
            
            Destroy(cables[connection].gameObject);

            cables.Remove(connection);
            
            if (connection == this.connection) this.connection = null;
        }

        private void DestroyCable(CableController cable)
        {
            DestroyCable(cables.First(c => c.Value == cable).Key);
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            CheckCableCollision(col);
        }

        private void CheckCableCollision(Collider2D col)
        {
            if (CurrentCable == null) return;
            
            if (!col.CompareTag("Cable")) return;

            // rushjob code to fit catastrophic bug 
            if (col.GetComponentInParent<CableController>().cableColor == CurrentCable.cableColor) return;

            var hit = TriggerCollision(velocity);

            Debug.Log("Player Cable Collision");
            Debug.DrawLine(hit.point, hit.point + hit.normal, Color.yellow, 30f);
            
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
            if (CurrentCable == null || CurrentCable.state != CableController.CableState.InProgress) return;

            velocity = (transform.position - lastPosition) / Time.deltaTime;
            lastPosition = transform.position;

            CurrentCable.nodes.Last().MoveNode(transform.position);
        }
    }
}