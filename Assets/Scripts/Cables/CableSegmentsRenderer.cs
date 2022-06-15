using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cables
{
    public class CableSegmentsRenderer : CableRenderer
    {
        [SerializeField] private GameObject cableSegmentPrefab;
        [SerializeField] private Transform cableSegmentParent;

        private Dictionary<CableNode, LineRenderer> cableSegments = new Dictionary<CableNode, LineRenderer>();

        private Dictionary<CableNode, List<Vector3>> cableSegmentTargetPoints =
            new Dictionary<CableNode, List<Vector3>>();

        #region MonoBehaviour

        private void Update()
        {
            UpdateCableSegments();
        }

        #endregion

        #region CableRenderer

        protected override void OnInitialised()
        {
            base.OnInitialised();

            foreach (var node in Nodes)
            {
                OnNodeCreated(node);
            }

            // TODO: Unlisten
            cable.nodeCreated.AddListener(OnNodeCreated);
            cable.nodeDestroyed.AddListener(OnNodeDestroyed);
            cable.nodeMoved.AddListener(OnNodeMoved);
            
            lineRenderer.enabled = false;
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            
            foreach (var node in Nodes)
            {
                UpdateCableSegmentTargetPoints(node);
            }
        }

        #endregion

        private void UpdateCableSegments()
        {
            foreach (var node in cableSegments.Keys)
            {
                UpdateCableSegment(node, LerpTowardTargetPositions(node));
            }
        }

        private void UpdateCableSegment(CableNode node, List<Vector3> points)
        {
            cableSegments[node].positionCount = points.Count;
            cableSegments[node].SetPositions(points.ToArray());
        }

        private List<Vector3> LerpTowardTargetPositions(CableNode node)
        {
            var targetPoints = cableSegmentTargetPoints[node];
            var pointsArray = new Vector3[cableSegments[node].positionCount];
            cableSegments[node].GetPositions(pointsArray);

            var currentPoints = pointsArray.ToList();

            for (int i = 0; i < currentPoints.Count; i++)
            {
                currentPoints[i] = Vector3.Lerp(currentPoints[i], targetPoints[i], 0.1f);
            }

            return currentPoints;
        }

        private void OnNodeCreated(CableNode node)
        {
            CreateCableSegment(node);
        }

        private void OnNodeMoved(CableNode node)
        {
            UpdateCableSegmentTargetPoints(node);
        }

        private void OnNodeDestroyed(CableNode node)
        {
            DestroyCableSegment(node);
        }

        private void CreateCableSegment(CableNode node)
        {
            var nodeIndex = Nodes.FindIndex(n => n == node);
            
            if (nodeIndex < 1) return;

            var cableSegmentObject = Instantiate(cableSegmentPrefab, cableSegmentParent);
            var cableSegment = cableSegmentObject.GetComponent<LineRenderer>();
            
            SetLineWidth(cableSegment);
            cableSegment.material.mainTexture = cableSprite.texture; 
            
            cableSegments.Add(node, cableSegment);
            cableSegmentTargetPoints.Add(node, new List<Vector3>());

            UpdateCableSegmentTargetPoints(node);
            
            UpdateCableSegment(node, GetTargetPoints(node));
        }

        private void UpdateCableSegmentTargetPoints(CableNode node)
        {
            var targetPoints = GetTargetPoints(node);

            if (targetPoints == null) return;

            cableSegmentTargetPoints[node] = targetPoints;
        }

        private List<Vector3> GetTargetPoints(CableNode node)
        {
            if (!cableSegments.ContainsKey(node)) return null;

            var nodeIndex = Nodes.FindIndex(n => n == node);
            var prevNode = Nodes[nodeIndex - 1];

            var points2D = new List<Vector2>();

            // if (node.Orientation == prevNode.Orientation)
            // {
            //     points2D.AddRange(PointsBetweenPositions(a, b, CurveFunctions.CurveFunction.Sine));
            // }
            // else
            // {
                // points2D.AddRange(PointsBetweenPositions(a, b, CurveFunctions.CurveFunction.Sine, CurveFunctions.CurveFunction.Bezier));
            // }

            points2D.AddRange(PointsBetweenPositions(prevNode, node));
            
            return SetZPositions(points2D, nodeIndex % 2 == 0 ? -1 : 1).ToList();
        }

        private void DestroyCableSegment(CableNode node)
        {
            Destroy(cableSegments[node].gameObject);

            cableSegments.Remove(node);
        }

        private static IEnumerable<Vector3> SetZPositions(IEnumerable<Vector2> points, float zPos)
        {
            return points.Select(p => new Vector3(p.x, p.y, zPos));
        }
    }
}