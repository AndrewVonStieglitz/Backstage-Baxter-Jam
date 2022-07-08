using System.Collections.Generic;
using UnityEngine;

namespace Pluggables
{
    public class PlugCable : MonoBehaviour
    {
        private PluggableMB PMB;
        public PluggablesSO pluggable;

        // TODO: Remove this dependency.
        [SerializeField] private ConnectionHead connectionHead;
        [SerializeField] private PluggableType pluggableType;
        public Connection connectionIn;
        public Connection connectionOut;

        [SerializeField] protected CableColor itemColor;

        protected void Awake()
        {
            PMB = GetComponent<PluggableMB>();
            PMB.itemColor = itemColor;
            pluggable = PMB.GetIdentifierSO();
            PMB.Init();
        }
        
        public void Interact(Connection connection)
        {
            if (GameManager.currentGameState != GameManager.GameState.playing) return;
            
            bool hasConnection = connectionHead.Connection != null;

            if (hasConnection && pluggableType != PluggableType.Instrument)
                CompleteConnection();
            else if (!hasConnection && pluggableType != PluggableType.Speaker)
                StartConnection();
        }

        private void StartConnection()
        {
            if (connectionOut != null)
            {
                connectionOut.PluggableEnd.UnplugInput();

                var oldConnectionOut = connectionOut;

                connectionOut = null;
                
                GameEvents.Disconnect(oldConnectionOut, this);
            }

            Connection connection = new Connection(this, itemColor);
            
            connectionOut = connection;
            
            connectionHead.Connection = connection;

            GameEvents.ConnectionStarted(connection);
        }

        private void CompleteConnection()
        {
            var connection = connectionHead.Connection;
            
            // Check for connection failure
            if (!ConnectionAllowed(connection))
            {
                // TODO: Call the connection failure event.

                return;
            }
            
            if (connectionIn != null)
            {
                connectionIn.PluggableStart.UnplugOutput();

                var oldConnectionIn = connectionIn;

                connectionIn = null;
                
                GameEvents.Disconnect(oldConnectionIn, this);
            }

            connectionIn = connection;

            connection.PluggableEnd = this;

            connectionHead.Connection = null;

            GameEvents.Connect(connection, this);
        }

        private bool ConnectionAllowed(Connection connection)
        {
            // TODO: Pretty sure this case is covered by ContainsLoops.
            if (connection.PluggableStart == this) return false;

            if (connection.cableColor != itemColor) return false;

            if (ContainsLoops(connection)) return false;

            return true;
        }

        public void UnplugOutput()
        {
            connectionOut = null;
        }

        public void UnplugInput()
        {
            connectionIn = null;
        }
        
        public PlugCable PrevPlugCable()
        {
            return connectionIn?.PluggableStart;
        }

        public PlugCable NextPlugCable()
        {
            return connectionOut?.PluggableEnd;
        }

        private bool ContainsLoops(Connection cable)
        {
            //print("Checking for loops on: " + name);
            List<PlugCable> seenPlugCables = new List<PlugCable>();
            seenPlugCables.Add(this);
            PlugCable studyCable = cable.PluggableStart;
            while (studyCable != null)
            {
                if (seenPlugCables.Contains(studyCable))
                    return true;
                seenPlugCables.Add(studyCable);
                studyCable = studyCable.PrevPlugCable();
            }
            //print("Found no loops");
            return false;
        }
    }
}