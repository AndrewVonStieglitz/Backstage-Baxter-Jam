using System;
using System.Collections.Generic;
using UnityEngine;
using static Cables.OrientationUtil;

namespace Cables
{
    public class CableRenderer : MonoBehaviour
    {
        [SerializeField] private int pointsBetweenPins;
        [SerializeField] private CableController cable;
    
        [Header("Player Section")]
        [SerializeField] private CurveFunctions.CurveFunction startCurveFunction;
        [SerializeField] private CurveFunctions.CurveFunction endCurveFunction;
        [SerializeField] private AnimationCurve curveInterpolation;
        [SerializeField] private bool lerpToSine;
        [SerializeField] private float caternaryLength;

        private List<CableNode> nodes => cable.nodes;
        private LineRenderer lineRenderer;
    
        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
        
            lineRenderer.widthCurve = AnimationCurve.Constant(1, 1, cable.cableWidth);
        }

        private void Update()
        {
            DrawLine();
        }

        private void DrawLine()
        {
            List<Vector3> points = new List<Vector3>();

            // Connections between nodes
            for (var nodeIndex = 0; nodeIndex < nodes.Count - 1; nodeIndex++)
            {
                for (int i = 0; i < pointsBetweenPins; i++)
                {
                    // TODO: How will this work if the orientations of the two nodes are different?
                    var a = nodes[nodeIndex].transform.position;
                    var b = nodes[nodeIndex + 1].transform.position;
                    points.Add(CurveFunctions.SinLerp(a, b, i / (float)pointsBetweenPins, nodes[nodeIndex].Orientation));
                }
            }

            // Connection between the last node and the player
            if (nodes.Count > 0)
            {
                for (int i = 0; i < pointsBetweenPins; i++)
                {
                    // TODO: How will this work if the orientations of the two nodes are different?
                    var a = nodes[nodes.Count - 1].transform.position;
                    var b = transform.position;
                    var t = i / (float)pointsBetweenPins;

                    var startCurve = GetCurveFunction(startCurveFunction);
                    var endCurve = GetCurveFunction(endCurveFunction);

                    var startPoint = startCurve(a, b, t);
                    var endPoint = endCurve(a, b, t);

                    var point = Vector2.Lerp(startPoint, endPoint, curveInterpolation.Evaluate(t));
                
                    // TODO: Better way of finding the edge of the pipe?
                    if (nodes.Count > 2 && lerpToSine)
                    {
                        // Interpolate towards a sine curve as we get closer to the pipe to prevent snapping when a new node is placed.
                        var distanceToPipe = (b - nodes[nodes.Count - 2].transform.position).y;

                        var sinPoint = SinLerp(a, b, t);
                    
                        point = Vector2.Lerp(sinPoint, point, Mathf.Clamp(distanceToPipe, 0, 1));
                    }

                    points.Add(point);
                }
            }

            // TODO: Convert to a pure function.
            SetZPositionsWithSlope(points);

            lineRenderer.positionCount = points.Count;
            lineRenderer.SetPositions(points.ToArray());
        }

        private Func<Vector2, Vector2, float, Vector2> GetCurveFunction(CurveFunctions.CurveFunction curveFunction) =>
            curveFunction switch
            {
                CurveFunctions.CurveFunction.Straight => Vector2.Lerp,
                CurveFunctions.CurveFunction.Sine => SinLerp,
                CurveFunctions.CurveFunction.Catenary => CatenaryLerp,
                CurveFunctions.CurveFunction.Bezier => CurveFunctions.BezierLerp,
                _ => throw new ArgumentOutOfRangeException(nameof(curveFunction), curveFunction, null)
            };

        private void SetZPositionsWithSlope(List<Vector3> points)
        {
            // For every pair of points, use the slope to set the Z position
            for (var pointIndex = 0; pointIndex < points.Count - 1; pointIndex++)
            {
                var point = points[pointIndex];
                var nextPoint = points[pointIndex + 1];

                var node = nodes[Mathf.FloorToInt(pointIndex / (float) pointsBetweenPins)];

                OrientedVector2 vector = new OrientedVector2(nextPoint - point);
            
                float vectorSlope;

                // Prevent division by zero
                if (vector.X(node.Orientation) == 0)
                {
                    vectorSlope = 20;
                }
                else
                {
                    vectorSlope = vector.Y(node.Orientation) / vector.X(node.Orientation);
                }

                points[pointIndex] = new Vector3(point.x, point.y, Sigmoid(vectorSlope) - 0.5f);
            }
        }

        // TODO: Clean this up
        private Vector2 SinLerp(Vector2 a, Vector2 b, float t)
        {
            return CurveFunctions.SinLerp(a, b, t, nodes[nodes.Count - 1].Orientation);
        }

        private Vector2 CatenaryLerp(Vector2 start, Vector2 end, float t)
        {
            return CurveFunctions.CatenaryLerp(start, end, t, caternaryLength);
        }

        private static float Sigmoid(double value) { return 1.0f / (1.0f + (float) Math.Exp(-value)); }
    }
}
