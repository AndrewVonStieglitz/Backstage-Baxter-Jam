using System.Collections.Generic;
using UnityEngine;

namespace Cables
{
    public class CableSegmentsController : MonoBehaviour
    {
        [SerializeField] private CableController cable;
        
        [SerializeField] protected int pointsBetweenNodes;
        [SerializeField] private CurveFunctions.CurveFunction startCurveFunctionID;
        [SerializeField] private CurveFunctions.CurveFunction endCurveFunctionID;
        [SerializeField] private AnimationCurve curveInterpolation;
        [SerializeField] private float catenaryLength;
        
        [Header("Experimental Features")]
        [SerializeField] private bool hangUnsupportedCables;
    
        public List<CableSegment> Segments = new List<CableSegment>();

        private Dictionary<CableNode, CableSegment> nodeSegments = new Dictionary<CableNode, CableSegment>();

        private void OnEnable()
        {
            cable.initialised.AddListener(OnInitialised);
        }

        private void OnDisable()
        {
            cable.initialised.RemoveListener(OnInitialised);
        }

        private void OnInitialised()
        {
            foreach (var node in cable.nodes)
            {
                OnNodeCreated(node);
            }
    
            // TODO: Unlisten
            cable.nodeCreated.AddListener(OnNodeCreated);
            cable.nodeMoved.AddListener(OnNodeMoved);
            cable.nodeDestroyed.AddListener(OnNodeDestroyed);
        }
    
        private void OnNodeCreated(CableNode node)
        {
            CreateCableSegment(node);
        }
    
        private void OnNodeMoved(CableNode node)
        {
            UpdateCableSegment(node);
        }

        private void OnNodeDestroyed(CableNode node)
        {
            DestroyCableSegment(node);
        }

        private void CreateCableSegment(CableNode node)
        {
            var nodeIndex = cable.nodes.FindIndex(n => n == node);

            if (nodeIndex < 1) return;

            var segment = new CableSegment();

            segment.previousNode = cable.nodes[nodeIndex - 1];
            segment.node = node;
            
            // TODO: Turn this into a class or scriptable object
            segment.pointsBetweenNodes = pointsBetweenNodes;
            segment.startCurveFunctionID = startCurveFunctionID;
            segment.endCurveFunctionID = endCurveFunctionID;
            segment.curveInterpolation = curveInterpolation;
            segment.catenaryLength = catenaryLength;
            segment.hangUnsupportedCables = hangUnsupportedCables;

            segment.GeneratePoints();
            
            Segments.Insert(nodeIndex - 1, segment);
            nodeSegments.Add(node, segment);
            
            SetPreviousNodeOfNextSegment(segment, node);
        }

        private void SetPreviousNodeOfNextSegment(CableSegment segment, CableNode node)
        {
            var segmentIndex = Segments.IndexOf(segment);

            if (segmentIndex < Segments.Count - 1)
            {
                var nextSegment = Segments[segmentIndex + 1];

                nextSegment.previousNode = node;

                nextSegment.GeneratePoints();
            }
        }

        private void UpdateCableSegment(CableNode node)
        {
            var segment = nodeSegments[node];

            if (segment == null) return;
            
            segment.GeneratePoints();
        }

        private void DestroyCableSegment(CableNode node)
        {
            var segment = nodeSegments[node];
            
            SetPreviousNodeOfNextSegment(segment, segment.previousNode);

            Segments.Remove(segment);
            nodeSegments.Remove(node);
        }
    }
}