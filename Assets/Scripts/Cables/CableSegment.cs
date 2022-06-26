﻿using System;
using System.Collections.Generic;
using System.Linq;
using Cables.Pipes;
using Cables.Platforms;
using UnityEngine;

namespace Cables
{
    public class CableSegment
    {
        // TODO: Work out how these are going to be set, since they can't be serialised
        public int pointsBetweenNodes;
        public CurveFunctions.CurveFunction startCurveFunctionID;
        public CurveFunctions.CurveFunction endCurveFunctionID;
        public AnimationCurve curveInterpolation;
        public float catenaryLength;
        public bool hangUnsupportedCables;
    
        public CableNode previousNode;
        public CableNode node;

        public List<Vector2> points = new List<Vector2>();

        public void GeneratePoints()
        {
            points.Clear();
            
            if (SegmentIsSupported() || !hangUnsupportedCables)
            {
                if (TestForSin())
                {
                    points.AddRange(PointsBetweenPositions(previousNode, node, CurveFunctions.CurveFunction.Sine, 20));
                }
                else
                {
                    points.AddRange(PointsBetweenPositions(previousNode, node));
                }
            }
            else
            {
                var pointsBetweenPositions = PointsBetweenPositions(previousNode, node, CurveFunctions.CurveFunction.Catenary, 20);
            
                // Duplicate points to prevent tearing
                // TODO: Need to find another way to check this without nodeIndex. Maybe node type terminal? Or just bring index in here.
                // if (nodeIndex > 0)
                {
                    points.Add(pointsBetweenPositions.First());
                    points.Add(pointsBetweenPositions.First());
                }

                points.AddRange(pointsBetweenPositions);
            }
        }

        private bool TestForSin()
        {
            var previousXYNode = previousNode as PipeNode;
            var xyNode = node as PipeNode;
            
            if (previousXYNode is null || xyNode is null) return false;
            // if (previousXYNode.Normal != xyNode.Normal * -1) return false;


            return true;
        }

        private bool SegmentIsSupported()
        {
            var previousPlatformNode = previousNode as PlatformNode;
            var platformNode = node as PlatformNode;
            
            if (previousPlatformNode is null || platformNode is null) return false;

            if (previousPlatformNode.PolyCollider == null || platformNode.PolyCollider == null) return false;
        
            if (previousPlatformNode.PolyCollider != platformNode.PolyCollider) return false;
        
            if (!CyclicPointsAreAdjacent(previousPlatformNode.VertexIndex, platformNode.VertexIndex, previousPlatformNode.PolyCollider.points.Length)) return false;
        
            if (!VectorPointsUp(previousPlatformNode.Normal.normalized + platformNode.Normal.normalized)) return false;

            return true;
        }

        private static bool CyclicPointsAreAdjacent(int indexA, int indexB, int cycleLength)
        {
            var absDiff = Mathf.Abs(indexA - indexB);

            return absDiff == 1 || absDiff == cycleLength - 1;
        }

        private static bool VectorPointsUp(Vector2 vector)
        {
            return Vector2.Dot(vector, Vector2.up) > 0;
        }

        // TODO: Move this to CurveFunctions
        protected Vector2 PointWithQuartic(PipeNode a, PipeNode b, float t)
        {
            var aPos = a.Position;
            var dPos = b.Position;

            var aTangent = (Vector2) Vector3.Cross(a.Normal.normalized, Vector3.forward);
            var dTangent = (Vector2) Vector3.Cross(b.Normal.normalized, Vector3.back);

            var bPos = Vector2.Distance(aPos + aTangent, dPos) > Vector2.Distance(aPos, dPos) ? aPos - aTangent : aPos + aTangent;
            var cPos = Vector2.Distance(dPos + dTangent, aPos) > Vector2.Distance(dPos, aPos) ? dPos - dTangent : dPos + dTangent;
        
            // Draw anchors
            Debug.DrawLine(aPos, bPos, Color.red, 30f);
            Debug.DrawLine(cPos, dPos, Color.red, 30f);

            return QuarticBezier(aPos, bPos, cPos, dPos, t);
        }

        private Vector2 QuarticBezier(Vector2 a, Vector2 b, Vector2 c, Vector2 d, float t)
        {
            var ab = Vector2.Lerp(a, b, t);
            var bc = Vector2.Lerp(b, c, t);
            var cd = Vector2.Lerp(c, d, t);

            var abc = Vector2.Lerp(ab, bc, t);
            var bcd = Vector2.Lerp(bc, cd, t);

            return Vector2.Lerp(abc, bcd, t);
        }

        private Func<CableNode, CableNode, float, Vector2> GetCurveFunction(CurveFunctions.CurveFunction curveFunction)
        {
            switch (curveFunction)
            {
                case CurveFunctions.CurveFunction.Straight:
                    return (a, b, t) => Vector2.Lerp(a.Position, b.Position, t);
                case CurveFunctions.CurveFunction.Sine:
                    return (a, b, t) => CurveFunctions.SinLerp(a.Position, b.Position, t,
                        NodeOrientation((PipeNode) previousNode).Inverse());
                case CurveFunctions.CurveFunction.Catenary:
                    return (a, b, t) =>
                        CurveFunctions.CatenaryLerp(a.Position, b.Position, t, catenaryLength);
                case CurveFunctions.CurveFunction.RightAngleCubic:
                    return (a, b, t) => CurveFunctions.BezierLerp(a.Position, b.Position, t);
                case CurveFunctions.CurveFunction.TangentQuartic:
                    return (a, b, t) => PointWithQuartic((PipeNode) a, (PipeNode) b, t);
                default:
                    throw new ArgumentOutOfRangeException(nameof(curveFunction), curveFunction, null);
            }
        }

        protected IEnumerable<Vector2> PointsBetweenPositions(CableNode a, CableNode b, int pointsBetweenNodes = 0)
        {
            return PointsBetweenPositions(a, b, startCurveFunctionID, endCurveFunctionID, pointsBetweenNodes);
        }

        protected IEnumerable<Vector2> PointsBetweenPositions(CableNode a, CableNode b, CurveFunctions.CurveFunction curveFunctionID, int pointsBetweenNodes = 0)
        {
            return PointsBetweenPositions(a, b, curveFunctionID, curveFunctionID, pointsBetweenNodes);
        }

        protected IEnumerable<Vector2> PointsBetweenPositions(CableNode a, CableNode b,
            CurveFunctions.CurveFunction startCurveID, CurveFunctions.CurveFunction endCurveID, int pointsBetweenNodes = 0)
        {
            if (pointsBetweenNodes == 0)
                pointsBetweenNodes = this.pointsBetweenNodes;
        
            var points = new List<Vector2>();
        
            var startCurveFunction = GetCurveFunction(startCurveID);
            var endCurveFunction = GetCurveFunction(endCurveID);
        
            for (int i = 0; i < pointsBetweenNodes; i++)
            {
                var t = i / (float)pointsBetweenNodes;

                var startPoint = startCurveFunction(a, b, t);
                var endPoint = endCurveFunction(a, b, t);

                var point = Vector2.Lerp(startPoint, endPoint, curveInterpolation.Evaluate(t));
            
                points.Add(point);
            }

            return points;
        }

        protected static OrientationUtil.Orientation NodeOrientation(PipeNode node)
        {
            return OrientationUtil.VectorToOrientation(node.Normal);
        }
    }
}