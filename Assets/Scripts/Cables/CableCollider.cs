using System.Linq;
using Cables.Renderers;
using UnityEngine;

namespace Cables
{
    public class CableCollider : MonoBehaviour
    {
        // TODO: Generalise to CableRenderer.
        [SerializeField] private WholeCableRenderer cableRenderer;
        [SerializeField] private EdgeCollider2D edgeCollider;
        // TODO: Should be segments rather than points.
        [SerializeField] private int pointsToSkipWhenInProgress;

        private void OnEnable()
        {
            cableRenderer.initialised.AddListener(OnInitialised);
            cableRenderer.pointsUpdated.AddListener(OnPointsUpdated);
        }

        private void OnDisable()
        {
            cableRenderer.pointsUpdated.RemoveListener(OnInitialised);
            cableRenderer.pointsUpdated.RemoveListener(OnPointsUpdated);
        }

        private void OnInitialised()
        {
            edgeCollider.edgeRadius = cableRenderer.Cable.cableWidth / 2;
        }

        private void OnPointsUpdated()
        {
            var cableInProgress = cableRenderer.Cable.state == CableController.CableState.InProgress;
            
            var pointsToSkip = cableInProgress ? pointsToSkipWhenInProgress : 0;
            
            if (cableRenderer.Points.Count <= pointsToSkip + 1)
            {
                edgeCollider.enabled = false;

                return;
            }

            edgeCollider.enabled = true;

            edgeCollider.SetPoints(cableRenderer.Points
                .TakeWhile((_, i) => i < cableRenderer.Points.Count - pointsToSkip)
                .Select(p => transform.InverseTransformPoint(p))
                .Select(p => (Vector2)p)
                .ToList()
            );
        }
    }
}