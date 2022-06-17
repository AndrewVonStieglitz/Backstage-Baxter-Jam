using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cables
{
    public class CableSegmentsRenderer : CableRenderer
    {
        [SerializeField] private GameObject cableSegmentPrefab;
        [SerializeField] private Transform cableSegmentParent;

        private readonly Dictionary<CableSegment, LineRenderer> lineRenderers = new Dictionary<CableSegment, LineRenderer>();

        #region MonoBehaviour
        
        private void Update()
        {
            UpdateCableSegmentRenderers();
        }
        
        #endregion
        
        #region CableRenderer
        
        protected override void OnInitialised()
        {
            base.OnInitialised();
        
            foreach (var segment in cableSegmentsController.Segments)
            {
                OnSegmentCreated(segment);
            }
        
            // TODO: Unlisten
            cableSegmentsController.cableSegmentCreated.AddListener(OnSegmentCreated);
            cableSegmentsController.cableSegmentDestroyed.AddListener(OnSegmentDestroyed);
        }
        
        #endregion
        
        private void UpdateCableSegmentRenderers()
        {
            foreach (var segment in lineRenderers.Keys)
            {
                UpdateCableSegmentRenderer(segment, LerpTowardTargetPositions(segment));
            }
        }
        
        private void UpdateCableSegmentRenderer(CableSegment segment, List<Vector3> points)
        {
            lineRenderers[segment].positionCount = points.Count;
            lineRenderers[segment].SetPositions(points.ToArray());
        }
        
        private List<Vector3> LerpTowardTargetPositions(CableSegment segment)
        {
            var pointsArray = new Vector3[lineRenderers[segment].positionCount];
            lineRenderers[segment].GetPositions(pointsArray);
        
            var currentPoints = pointsArray.ToList();
            var targetPoints = GetTargetPoints(segment);
        
            for (int i = 0; i < currentPoints.Count; i++)
            {
                currentPoints[i] = Vector3.Lerp(currentPoints[i], targetPoints[i], 0.1f);
            }
        
            return currentPoints;
        }
        
        private void OnSegmentCreated(CableSegment segment)
        {
            CreateCableSegmentRenderer(segment);
        }
        
        private void OnSegmentDestroyed(CableSegment segment)
        {
            DestroyCableSegmentRenderer(segment);
        }
        
        private void CreateCableSegmentRenderer(CableSegment segment)
        {
            var lineRendererObject = Instantiate(cableSegmentPrefab, cableSegmentParent);
            var lineRenderer = lineRendererObject.GetComponent<LineRenderer>();
            
            SetLineWidth(lineRenderer);
            lineRenderer.material.mainTexture = cableSprite.texture; 
            
            lineRenderers.Add(segment, lineRenderer);
            
            UpdateCableSegmentRenderer(segment, GetTargetPoints(segment));
        }
        
        private void DestroyCableSegmentRenderer(CableSegment segment)
        {
            Destroy(lineRenderers[segment].gameObject);
        
            lineRenderers.Remove(segment);
        }

        private List<Vector3> GetTargetPoints(CableSegment segment)
        {
            var points = segment.points.Select(p => (Vector3)p).ToList();
            
            points.Add(segment.node.transform.position);
            
            return points;
        }
    }
}