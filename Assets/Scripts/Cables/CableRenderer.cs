using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Cables
{
    public abstract class CableRenderer : MonoBehaviour
    {
        [FormerlySerializedAs("pointsBetweenPins")] [SerializeField] protected int pointsBetweenNodes;
        [SerializeField] protected CableController cable;
        [SerializeField] protected float joinCoverUpLength;

        [FormerlySerializedAs("startCurveFunction")]
        [Header("Player Section")]
        [SerializeField] private CurveFunctions.CurveFunction startCurveFunctionID;
        [FormerlySerializedAs("endCurveFunction")] [SerializeField] private CurveFunctions.CurveFunction endCurveFunctionID;
        [SerializeField] private AnimationCurve curveInterpolation;
        [SerializeField] private bool lerpToSine;
        [SerializeField] private float caternaryLength;

        protected List<CableNode> Nodes => cable.nodes;
        protected LineRenderer lineRenderer;
        protected Sprite cableSprite;

        protected virtual void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();

            SetLineWidth(lineRenderer);
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

        private Func<CableNode, CableNode, float, Vector2> GetCurveFunction(CurveFunctions.CurveFunction curveFunction) =>
            curveFunction switch
            {
                CurveFunctions.CurveFunction.Straight => (a, b, t) => Vector2.Lerp(a.transform.position, b.transform.position, t),
                CurveFunctions.CurveFunction.Sine => (a, b, t) => CurveFunctions.SinLerp(a.transform.position, b.transform.position, t, NodeOrientation(Nodes[Nodes.Count - 1])),
                CurveFunctions.CurveFunction.Catenary => (a, b, t) => CurveFunctions.CatenaryLerp(a.transform.position, b.transform.position, t, caternaryLength),
                CurveFunctions.CurveFunction.RightAngleCubic => (a, b, t) => CurveFunctions.BezierLerp(a.transform.position, b.transform.position, t),
                CurveFunctions.CurveFunction.TangentQuartic => PointWithQuartic,
                _ => throw new ArgumentOutOfRangeException(nameof(curveFunction), curveFunction, null)
            };

        protected IEnumerable<Vector2> PointsBetweenPositions(CableNode a, CableNode b)
        {
            return PointsBetweenPositions(a, b, startCurveFunctionID, endCurveFunctionID);
        }

        protected IEnumerable<Vector2> PointsBetweenPositions(CableNode a, CableNode b, CurveFunctions.CurveFunction curveFunctionID)
        {
            return PointsBetweenPositions(a, b, curveFunctionID, curveFunctionID);
        }

        protected IEnumerable<Vector2> PointsBetweenPositions(CableNode a, CableNode b,
            CurveFunctions.CurveFunction startCurveID, CurveFunctions.CurveFunction endCurveID)
        {
            var points = new List<Vector2>();
            
            var startCurveFunction = GetCurveFunction(startCurveID);
            var endCurveFunction = GetCurveFunction(endCurveID);
            
            for (int i = 0; i <= pointsBetweenNodes; i++)
            {
                // TODO: How will this work if the orientations of the two nodes are different?

                var t = i / (float)pointsBetweenNodes;

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

        protected static OrientationUtil.Orientation NodeOrientation(CableNode node)
        {
            return OrientationUtil.VectorToOrientation(node.Normal);
        }

        protected virtual void OnValidate() { }

        // TODO: Move this to CurveFunctions
        protected Vector2 PointWithQuartic(CableNode a, CableNode b, float t)
        {
            var aPos = (Vector2) a.transform.position;
            var dPos = (Vector2) b.transform.position;

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
    }
}
