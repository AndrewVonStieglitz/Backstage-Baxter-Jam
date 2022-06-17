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

        protected override void UpdateLineRenderers()
        {
            foreach (var segment in lineRenderers.Keys)
            {
                UpdateLineRenderer(segment, LerpTowardTargetPositions(segment));
            }
        }

        #endregion

        private void UpdateLineRenderer(CableSegment segment, List<Vector3> points)
        {
            UpdateLineRenderer(lineRenderers[segment], points);
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
            
            InitialiseLineRenderer(lineRenderer);
            
            lineRenderers.Add(segment, lineRenderer);
            
            UpdateLineRenderer(segment, GetTargetPoints(segment));
        }
        
        private void DestroyCableSegmentRenderer(CableSegment segment)
        {
            Destroy(lineRenderers[segment].gameObject);
        
            lineRenderers.Remove(segment);
        }

        private List<Vector3> GetTargetPoints(CableSegment segment)
        {
            var points = segment.points;
            
            points.Add(segment.node.transform.position);

            var points3D = SetZPositions(segment, points).ToList();
            
            return points3D;
        }

        // TODO: Needs to be tested. Need to get XYNodes working again first.
        private IEnumerable<Vector3> SetZPositions(CableSegment segment, List<Vector2> points)
        {
            var segmentIndex = Segments.IndexOf(segment);
            
            var zPos = segmentIndex % 2 == 0 ? -1 : 1;
            
            return points.Select(p => new Vector3(p.x, p.y, zPos));
        }
    }
}