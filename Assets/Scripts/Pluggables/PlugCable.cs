using System.Collections.Generic;
using UnityEngine;

namespace Pluggables
{
    public class PlugCable : MonoBehaviour
    {
        private PluggableMB PMB;
        public PluggablesSO pluggable;

        [SerializeField] private PluggableType pluggableType;

        private Connection connectionIn;
        private Connection connectionOut;

        [SerializeField] public ColorEnum color;

        protected void Awake()
        {
            PMB = GetComponent<PluggableMB>();
            PMB.color = color;
            pluggable = PMB.GetIdentifierSO();
            PMB.Init();
        }
        
        public void Interact(Connection headConnection)
        {
            if (GameManager.currentGameState != GameManager.GameState.playing) return;
            
            bool hasConnection = headConnection != null;

            if (hasConnection && pluggableType != PluggableType.Instrument)
                CompleteConnection(headConnection);
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

            Connection connection = new Connection(this);
            
            connectionOut = connection;
            
            GameEvents.ConnectionStarted(connection);
        }

        private void CompleteConnection(Connection headConnection)
        {
            if (!ConnectionAllowed(headConnection))
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

            connectionIn = headConnection;

            headConnection.PluggableEnd = this;

            GameEvents.Connect(headConnection, this);
        }

        private bool ConnectionAllowed(Connection connection)
        {
            // TODO: Pretty sure this case is covered by ContainsLoops.
            if (connection.PluggableStart == this) return false;

            // Prevent connections between different colours
            // TODO: Pretty sure this duplicates logic in the GameManager.
            if (connection.Color != color) return false;

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
            return false;
        }
    }
}