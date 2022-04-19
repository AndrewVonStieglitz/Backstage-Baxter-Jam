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

            for (var index = 0; index < cable.nodes.Count; index++)
            {
                OnNodeCreated(index);
            }

            cable.nodeCreated.AddListener(OnNodeCreated);
            cable.nodeDestroyed.AddListener(OnNodeDestroyed);
        }

        // protected override void OnDisable()
        // {
        //     base.OnDisable();
        //     
        //     cable.nodeCreated.RemoveListener(OnNodeCreated);
        //     cable.nodeDestroyed.RemoveListener(OnNodeDestroyed);
        // }

        private void Update()
        {
            if (cable.state == CableController.CableState.InProgress)
            {
                lineRenderer.enabled = true;

                DrawPlayerSegment();
            }
            else
                lineRenderer.enabled = false;
        }

        private void DrawPlayerSegment()
        {
            if (nodes.Count < 1) return;
            
            var points = new List<Vector2>();
            
            points.AddRange(PlayerSegmentPoints(points));

            var lastNode = nodes[nodes.Count - 1];
            List<Vector3> points3D = new List<Vector3>();

            if (cableSegments.Count > 0)
            {
                var lastCableSegment = cableSegments[nodes[nodes.Count - 1]];
                var lastPoint = lastCableSegment.GetPosition(lastCableSegment.positionCount - 1);
                
                points3D.Add(new Vector3(lastPoint.x - joinCoverUpLength, lastPoint.y, lastPoint.z));
            }
            
            points3D = SetZPositions(points, lastNode.poleSide == CableNode.PoleSide.Over ? 1 : -1).ToList();
            

            lineRenderer.positionCount = points3D.Count;
            lineRenderer.SetPositions(points3D.ToArray());
        }

        private void OnNodeCreated(int nodeIndex)
        {
            CreateNodeSegment(nodeIndex);
        }

        private void OnNodeDestroyed(CableNode node)
        {
            DestroyNodeSegment(node);
        }

        private void CreateNodeSegment(int nodeIndex)
        {
            if (cable.nodes.Count < 2) return;

            var cableSegmentObject = Instantiate(cableSegmentPrefab, cableSegmentParent);
            var cableSegment = cableSegmentObject.GetComponent<LineRenderer>();
            
            SetLineWidth(cableSegment);
            cableSegment.material.mainTexture = cableSprite.texture; 

            var node = cable.nodes[nodeIndex];
            var prevNode = nodes[nodes.Count - 2];

            cableSegments.Add(node, cableSegment);

            var points2D = new List<Vector2>();

            var a = prevNode.transform.position;
            var b = node.transform.position;

            points2D.Add(a);

            if (node.Orientation == prevNode.Orientation)
            {
                points2D.AddRange(PointsBetweenPositions(a, b, prevNode.Orientation, CurveFunctions.CurveFunction.Sine));
            }
            else
            {
                points2D.AddRange(PointsBetweenPositions(a, b, prevNode.Orientation, CurveFunctions.CurveFunction.Sine, CurveFunctions.CurveFunction.Bezier));
            }
            
            points2D.Add(b);
            
            // TODO Orient
            points2D.Add(new Vector2(b.x + joinCoverUpLength, b.y));

            var points3D = SetZPositions(points2D, node.poleSide == CableNode.PoleSide.Over ? -1 : 1).ToList();
            
            cableSegment.positionCount = points3D.Count;
            cableSegment.SetPositions(points3D.ToArray());
        }

        private void DestroyNodeSegment(CableNode node)
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