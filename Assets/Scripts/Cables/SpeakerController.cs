using UnityEngine;

namespace Cables
{
    public class SpeakerController : MonoBehaviour
    {
        [SerializeField] private int speakerID;
        public int SpeakerID { get => speakerID; }

        private AmpController amp;

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (!col.CompareTag("CableHead")) return;

            var cableHead = col.GetComponent<CableHead>();

            if (cableHead == null) return;
            
            var cable = cableHead.cable;

            if (cable == null) return;

            amp = cable.amp;

            cable.Complete(transform.position);
        }
    }
}
