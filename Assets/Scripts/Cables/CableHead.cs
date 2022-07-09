using System.Collections.Generic;
using System.Linq;
using Pluggables;
using UnityEngine;
using UnityEngine.Events;
using Utility;

namespace Cables
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class CableHead : MonoBehaviour
    {
        [SerializeField] private GameObject cablePrefab;
        [Tooltip("Indexed according to Enums.Colours.")]
        [SerializeField] private List<Sprite> coloredCableSprites;
        [SerializeField] private VelocityTracker velocityTracker;

        public CableController CurrentCable
        {
            get => currentCurrentCable;
            private set
            {
                currentCurrentCable = value;
                cableChanged.Invoke();
            }
        }

        public UnityEvent cableChanged = new UnityEvent();
        
        private CableController currentCurrentCable;
        private Connection connection;
        private Dictionary<Connection, CableController> cables = new Dictionary<Connection, CableController>();
        private BoxCollider2D boxCollider2D;

        private void Awake()
        {
            boxCollider2D = GetComponent<BoxCollider2D>();
        }

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

            boxCollider2D.size = new Vector2(CurrentCable.cableWidth, CurrentCable.cableWidth);
            
            CurrentCable.Initialise(connection.PluggableStart.transform, coloredCableSprites[(int) connection.Color]);

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

        private void OnTriggerEnter2D(Collider2D col)
        {
            CheckCableCollision(col);
        }

        private void CheckCableCollision(Collider2D col)
        {
            if (CurrentCable == null) return;
            
            if (!col.CompareTag("Cable")) return;

            // Cable should not collide with other cables of the same color
            if (col.GetComponentInParent<CableController>().Sprite == CurrentCable.Sprite) return;

            var hit = UtilityFunctions.TriggerCollision(boxCollider2D, velocityTracker.Velocity);

            // Debug.Log("Player Cable Collision");
            // Debug.DrawLine(hit.point, hit.point + hit.normal, UnityEngine.Color.yellow, 30f);
            
            GameEvents.PlayerCableCollision(hit.point, hit.normal);
        }
        
        private void FixedUpdate()
        {
            if (CurrentCable == null || CurrentCable.state != CableController.CableState.InProgress) return;

            CurrentCable.nodes.Last().MoveNode(transform.position);
        }
    }
}