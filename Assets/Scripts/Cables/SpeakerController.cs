using UnityEngine;

namespace Cables
{
    public class SpeakerController : MonoBehaviour
    {
        [SerializeField] private int speakerID;
        
        private int ampID;

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

            var cable = cableHead.cable;

            if (cable == null) return;

            amp = cable.amp;

            cable.Complete(transform.position);
        }

        private void OnEnable()
        {
            GameEvents.onCableConnect += ConnectCable;
            GameEvents.onCableDisconnect += DisconnectCable;
        }

        private void OnDisable()
        {
            GameEvents.onCableConnect -= ConnectCable;
            GameEvents.onCableDisconnect -= DisconnectCable;
        }

        public void ConnectCable(CableController cable, SpeakerController speaker)
        {
            if (speaker == this)
            {
                if (connectedCable)
                {
                    GameEvents.CableDisconnect(connectedCable, speaker);
                }

                connectedCable = cable;
            }
        }

        public void DisconnectCable(CableController cable, SpeakerController speaker)
        {
            if (speaker == this)
            {
                connectedCable = null;
            }
        }

        public void PlayMusic(AudioClip audioclip, int AmpID)
        {
            if (speakerAudio)
            {
                this.ampID = AmpID;
                speakerAudio.clip = audioclip;
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
