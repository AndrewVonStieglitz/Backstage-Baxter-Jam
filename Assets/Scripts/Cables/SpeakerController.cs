// DEPRICATED
using System.Linq;
using UnityEngine;

namespace Cables
{
    public class SpeakerController : MonoBehaviour
    {
        [SerializeField] private int speakerID;
        
        public int AmpID
        {
            get => ampID;
        }

        public int SpeakerID
        {
            get => speakerID;
        }

        private CableController connectedCable;
        private AudioSource speakerAudio;
        private AmpController amp;
        private int ampID;

        private void Awake()
        {
            speakerAudio = GetComponent<AudioSource>();

            //Adds component if does not currently exist
            if (!speakerAudio)
            {
                speakerAudio = gameObject.AddComponent<AudioSource>();
                speakerAudio.loop = true;
                speakerAudio.playOnAwake = false;
                speakerAudio.Stop();
            }
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (!col.CompareTag("CableHead")) return;

            var cableHead = col.GetComponent<CableHead>();

            if (cableHead == null) return;

            var cable = cableHead.Cable;

            if (cable == null) return;

            // DEPRICATED
            //amp = cable.amp;
            
            cable.nodes.Last().MoveNode(transform.position);

            cable.Complete();

            if (connectedCable)
            {
                GameEvents.CableDisconnect(connectedCable, this);
            }

            connectedCable = cable;
            
            GameEvents.CableConnect(cable, this);
        }

        public void PlayMusic(AudioClip audioclip, int AmpID, float time)
        {
            if (speakerAudio)
            {
                this.ampID = AmpID;
                speakerAudio.clip = audioclip;
                speakerAudio.time = time % audioclip.length;
                speakerAudio.Play();
            }
            else
            {
                Debug.Log("Speaker is missing audio source");
            }
        }

        public void StopMusic()
        {
            if (speakerAudio)
            {
                speakerAudio.clip = null;
                speakerAudio.Stop();
            }
            else
            {
                Debug.Log("Speaker is missing audio source");
            }
        }
    }
}
