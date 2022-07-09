using UnityEngine;

namespace Utility
{
    public static class UtilityFunctions
    {
        // Allows OnTriggerEnter2D to provide detailed collision information like OnCollisionEnter2D.
        public static RaycastHit2D TriggerCollision(Collider2D collider2D, Vector2 castDirection)
        {
            var hits = new RaycastHit2D[1];
            
            collider2D.Cast(castDirection, hits, 1);

            return hits[0];
        }
    }
}