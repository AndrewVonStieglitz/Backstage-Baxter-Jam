using System;
using System.Collections.Generic;
using UnityEngine;
using static Cables.OrientationUtil;

namespace Cables
{
    public abstract class CableRenderer : MonoBehaviour
    {
        [SerializeField] protected int pointsBetweenPins;
        [SerializeField] protected CableController cable;
        [SerializeField] protected float joinCoverUpLength;

        [Header("Player Section")]
        [SerializeField] private CurveFunctions.CurveFunction startCurveFunction;
        [SerializeField] private CurveFunctions.CurveFunction endCurveFunction;
        [SerializeField] private AnimationCurve curveInterpolation;
        [SerializeField] private bool lerpToSine;
        [SerializeField] private float caternaryLength;

        protected List<CableNode> nodes => cable.nodes;
        protected LineRenderer lineRenderer;
        protected CableHead cableHead;
        protected Sprite cableSprite;

        protected virtual void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();

            SetLineWidth(lineRenderer);
            
            // TODO: Temp
            cableHead = FindObjectOfType<CableHead>();
        }
        
        protected virtual void OnEnable()
        {
            cable.initialised.AddListener(OnInitialised);
        }

        protected virtual void OnDisable()
        {
            cable.initialised.RemoveListener(OnInitialised);
        }

        protected virtual void OnInitialised()
        {
            cableSprite = cable.amp.cableSprite;
            lineRenderer.material.mainTexture = cableSprite.texture; 
        }

        protected void SetLineWidth(LineRenderer lineRenderer)
        {
            lineRenderer.widthCurve = AnimationCurve.Constant(1, 1, cable.cableWidth);
        }

        // protected IEnumerable<Vector2> PointsBetweenPositions(Vector2 a, Vector2 b, Orientation orientation)
        // {
        //     var points = new List<Vector2>();
        //     
        //     for (int i = 0; i < pointsBetweenPins; i++)
        //     {
        //         // TODO: How will this work if the orientations of the two nodes are different?
        //         points.Add(CurveFunctions.SinLerp(a, b, i / (float)pointsBetweenPins, orientation));
        //     }
        //
        //     return points;
        // }

        private Func<Vector2, Vector2, float, Vector2> GetCurveFunction(CurveFunctions.CurveFunction curveFunction) =>
            curveFunction switch
            {
                CurveFunctions.CurveFunction.Straight => Vector2.Lerp,
                CurveFunctions.CurveFunction.Sine => SinLerp,
                CurveFunctions.CurveFunction.Catenary => CatenaryLerp,
                CurveFunctions.CurveFunction.Bezier => CurveFunctions.BezierLerp,
                _ => throw new ArgumentOutOfRangeException(nameof(curveFunction), curveFunction, null)
            };

        // TODO: Clean this up
        private Vector2 SinLerp(Vector2 a, Vector2 b, float t)
        {
            return CurveFunctions.SinLerp(a, b, t, nodes[nodes.Count - 1].Orientation);
        }

        private Vector2 CatenaryLerp(Vector2 start, Vector2 end, float t)
        {
            return CurveFunctions.CatenaryLerp(start, end, t, caternaryLength);
        }


        protected IEnumerable<Vector2> PointsBetweenPositions(Vector2 a, Vector2 b, Orientation orientation)
        {
            return PointsBetweenPositions(a, b, orientation, startCurveFunction, endCurveFunction);
        }

        protected IEnumerable<Vector2> PointsBetweenPositions(Vector2 a, Vector2 b, Orientation orientation, CurveFunctions.CurveFunction curveFunctionID)
        {
            return PointsBetweenPositions(a, b, orientation, curveFunctionID, curveFunctionID);
        }

        protected IEnumerable<Vector2> PointsBetweenPositions(Vector2 a, Vector2 b, Orientation orientation,
            CurveFunctions.CurveFunction startCurveID, CurveFunctions.CurveFunction endCurveID)
        {
            var points = new List<Vector2>();
            
            var startCurveFunction = GetCurveFunction(startCurveID);
            var endCurveFunction = GetCurveFunction(endCurveID);
            
            for (int i = 0; i < pointsBetweenPins; i++)
            {
                // TODO: How will this work if the orientations of the two nodes are different?

                var t = i / (float)pointsBetweenPins;

                var startPoint = startCurveFunction(a, b, t);
                var endPoint = endCurveFunction(a, b, t);

                var point = Vector2.Lerp(startPoint, endPoint, curveInterpolation.Evaluate(t));
            
                // TODO: Better way of finding the edge of the pipe?
                // if (nodes.Count > 2 && lerpToSine)
                // {
                //     // Interpolate towards a sine curve as we get closer to the pipe to prevent snapping when a new node is placed.
                //     var distanceToPipe = (b - nodes[nodes.Count - 2].transform.position).y;
                //
                //     var sinPoint = SinLerp(a, b, t);
                //
                //     point = Vector2.Lerp(sinPoint, point, Mathf.Clamp(distanceToPipe, 0, 1));
                // }
                
                points.Add(point);
            }

            return points;
        }

        protected IEnumerable<Vector2> PlayerSegmentPoints(List<Vector2> points)
        {
            if (nodes.Count < 1) return new List<Vector2>();

            var lastNode = nodes[nodes.Count - 1];

            var a = lastNode.transform.position;
            var b = cableHead.transform.position;

            return PointsBetweenPositions(a, b, lastNode.Orientation);
        }
    }
}
