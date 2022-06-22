using System.Collections.Generic;
using UnityEngine;

namespace Cables
{
    public class AmpController : MonoBehaviour
    {
        [SerializeField] private int ampID;
        [SerializeField] private GameObject cablePrefab;
        [SerializeField] private CableHead cableHead;

        public Sprite cableSprite;
        
        public int AmpID { get => ampID; }
        
        private readonly List<CableController> cables = new List<CableController>();

        private void OnEnable()
        {
            GameEvents.onCableDisconnect += OnCableDisconnect;
        }

        private void OnDisable()
        {
            GameEvents.onCableDisconnect -= OnCableDisconnect;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (!col.CompareTag("CableHead")) return;

            if (cableHead.Cable != null) return;

            var cableObject = Instantiate(cablePrefab, transform);
            var cable = cableObject.GetComponent<CableController>();

            cables.Add(cable);
            
            cableHead.NewCable(cable);

            cable.Initialise(this);
        }
        
        // TODO: This should be listening for the cable getting destroyed, not disconnected.
        private void OnCableDisconnect(CableController cable, SpeakerController speaker)
        {
            if (!cables.Contains(cable)) return;

            cables.Remove(cable);
        }
    }
}
