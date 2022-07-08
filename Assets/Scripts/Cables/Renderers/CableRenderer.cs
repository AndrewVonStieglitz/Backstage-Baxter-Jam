using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Cables.Renderers
{
    public abstract class CableRenderer : MonoBehaviour
    {
        [Header("Cable Renderer")]
        [SerializeField] protected CableController cable;
        [SerializeField] protected CableSegmentsController cableSegmentsController;
        
        [Header("Experimental Features")]
        [SerializeField] private bool lerpEnabled;
        [SerializeField] private float lerpSpeed;

        public UnityEvent initialised = new UnityEvent();
        public UnityEvent pointsUpdated = new UnityEvent();
        
        public List<Vector3> Points = new List<Vector3>();
        public CableController Cable => cable;

        protected List<CableSegment> Segments => cableSegmentsController.Segments;
        private Sprite cableSprite;

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

        protected void UpdateLineRendererInstant(LineRenderer lineRenderer, List<Vector3> points)
        {
            Points = points;
            
            lineRenderer.positionCount = points.Count;
            lineRenderer.SetPositions(points.ToArray());
            
            pointsUpdated.Invoke();
        }

        protected void UpdateLineRendererLerp(LineRenderer lineRenderer, List<Vector3> targetPoints)
        {
            if (!lerpEnabled || targetPoints.Count != lineRenderer.positionCount)
            {
                UpdateLineRendererInstant(lineRenderer, targetPoints);

                return;
            }
            
            var pointsArray = new Vector3[lineRenderer.positionCount];
            lineRenderer.GetPositions(pointsArray);
        
            var currentPoints = pointsArray.ToList();
        
            for (int i = 0; i < currentPoints.Count; i++)
            {
                currentPoints[i] = Vector3.Lerp(currentPoints[i], targetPoints[i], lerpSpeed);
            }
        
            UpdateLineRendererInstant(lineRenderer, currentPoints);
        }

        protected virtual void OnInitialised()
        {
            cableSprite = cable.Sprite;
            
            initialised.Invoke();
        }

        protected void InitialiseLineRenderer(LineRenderer lineRenderer)
        {
            lineRenderer.widthCurve = AnimationCurve.Constant(1, 1, cable.cableWidth);
            lineRenderer.sortingLayerName = "Cable";
            
            if (cableSprite != null)
                lineRenderer.material.mainTexture = cableSprite.texture; 
        }
    }
}
