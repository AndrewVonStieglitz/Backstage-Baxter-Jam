using UnityEngine;

namespace Utility
{
    public class VelocityTracker : MonoBehaviour
    {
        public Vector3 Velocity { get; private set; }
        
        private Vector3 lastPosition;

        private void FixedUpdate()
        {
            var position = transform.position;
            
            Velocity = (position - lastPosition) / Time.deltaTime;
            
            lastPosition = position;
        }
    }
}