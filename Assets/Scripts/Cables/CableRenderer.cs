using System.Collections.Generic;
using UnityEngine;

namespace Cables
{
    public abstract class CableRenderer : MonoBehaviour
    {
        [SerializeField] protected CableController cable;
        [SerializeField] protected CableSegmentsController cableSegmentsController;

        protected List<CableSegment> Segments => cableSegmentsController.Segments;
        protected Sprite cableSprite;

        protected virtual void OnEnable()
        {
            cable.initialised.AddListener(OnInitialised);
        }

        protected virtual void OnDisable()
        {
            cable.initialised.RemoveListener(OnInitialised);
        }
        
        protected virtual void Update()
        {
            UpdateLineRenderers();
        }

        protected abstract void UpdateLineRenderers();

        protected static void UpdateLineRenderer(LineRenderer lineRenderer, List<Vector3> points)
        {
            lineRenderer.positionCount = points.Count;
            lineRenderer.SetPositions(points.ToArray());
        }

        protected virtual void OnInitialised()
        {
            if (cable.amp) 
                cableSprite = cable.amp.cableSprite;
            else if (cable.pluggableStart)
                cableSprite = cable.pluggableStart.cableSprite;
        }

        protected void InitialiseLineRenderer(LineRenderer lineRenderer)
        {
            lineRenderer.widthCurve = AnimationCurve.Constant(1, 1, cable.cableWidth);
            lineRenderer.material.mainTexture = cableSprite.texture; 
        }
    }
}
