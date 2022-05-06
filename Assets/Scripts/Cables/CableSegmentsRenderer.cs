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

        protected override void OnInitialised()
        {
            base.OnInitialised();

            foreach (var node in Nodes)
            {
                OnNodeCreated(node);
            }

            cable.nodeCreated.AddListener(OnNodeCreated);
            cable.nodeDestroyed.AddListener(OnNodeDestroyed);
            cable.nodeMoved.AddListener(OnNodeMoved);
            
            lineRenderer.enabled = false;
        }

        private void OnNodeMoved(CableNode node)
        {
            UpdateCableSegment(node);
        }

        // protected override void OnDisable()
        // {
        //     base.OnDisable();
        //     
        //     cable.nodeCreated.RemoveListener(OnNodeCreated);
        //     cable.nodeDestroyed.RemoveListener(OnNodeDestroyed);
        // }

        private void OnNodeCreated(CableNode node)
        {
            CreateCableSegment(node);
        }

        private void OnNodeDestroyed(CableNode node)
        {
            DestroyCableSegment(node);
        }

        private void CreateCableSegment(CableNode node)
        {
            var nodeIndex = Nodes.FindIndex(n => n == node);
            
            if (nodeIndex < 1) return;

            var prevNode = Nodes[nodeIndex - 1];

            var cableSegmentObject = Instantiate(cableSegmentPrefab, cableSegmentParent);
            var cableSegment = cableSegmentObject.GetComponent<LineRenderer>();
            
            SetLineWidth(cableSegment);
            cableSegment.material.mainTexture = cableSprite.texture; 
            
            cableSegments.Add(node, cableSegment);

            UpdateCableSegment(prevNode);
            UpdateCableSegment(node);
        }

        private void UpdateCableSegment(CableNode node)
        {
            if (!cableSegments.ContainsKey(node)) return;

            var nodeIndex = Nodes.FindIndex(n => n == node);
            var prevNode = Nodes[nodeIndex - 1];

            var cableSegment = cableSegments[node];
            
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
            
            var points3D = SetZPositions(points2D, nodeIndex % 2 == 0 ? -1 : 1).ToList();
            
            cableSegment.positionCount = points3D.Count;
            cableSegment.SetPositions(points3D.ToArray());
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

        protected override void OnValidate()
        {
            base.OnValidate();
            
            foreach (var node in Nodes)
            {
                UpdateCableSegment(node);
            }
        }
    }
}