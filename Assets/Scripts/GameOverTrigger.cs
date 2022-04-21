using UnityEngine;

namespace DefaultNamespace
{
    [RequireComponent(typeof(Collider2D))]
    public class GameOverTrigger : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D col)
        {
            GameEvents.GameOver();
        }
    }
}