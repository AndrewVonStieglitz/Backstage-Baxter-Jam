using System.Collections.Generic;
using UnityEngine;

namespace Cables
{
    public class AmpController : MonoBehaviour
    {
        [SerializeField] private int ampID;
        [SerializeField] private Material cableMaterial;
        [SerializeField] private GameObject cablePrefab;
        [SerializeField] private CableHead cableHead;

        private List<CableController> cables = new List<CableController>();
        
        public int AmpID { get => ampID; }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (!col.CompareTag("CableHead")) return;

            if (cableHead.cable != null && cableHead.cable.amp == this) return;

            var cableObject = Instantiate(cablePrefab, transform);
            var cable = cableObject.GetComponent<CableController>();

            cables.Add(cable);
            
            cableHead.NewCable(cable);

            cable.Initialise(this, cableMaterial);
        }
    }
}
