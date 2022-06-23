using System;
using UnityEngine;

namespace Cables.Pipes
{
    public class CablePipeNodeController : MonoBehaviour
    {
        [SerializeField] private CableController cable;
        [SerializeField] private GameObject xYNodePrefab;
        
        private Vector2 pipeEntryNormal;
    
        public void PipeEnter(Vector2 nodePos, Vector2 normal)
        {
            pipeEntryNormal = normal;

            if (!AwayFromPreviousNode(nodePos, normal)) return;
            
            var node = cable.CreateNodeAtIndex(xYNodePrefab, nodePos, cable.nodes.Count - 1);

            var xyNode = node as PipeNode;
            
            if (xyNode is null) throw new Exception($"No {nameof(PipeNode)} component on node prefab.");
            
            xyNode.Normal = normal;
        }

        private bool AwayFromPreviousNode(Vector2 nodePos, Vector2 normal)
        {
            return Vector2.Dot(nodePos - (Vector2) cable.nodes[cable.nodes.Count - 2].transform.position, normal) > 0;
        }

        public void PipeExit(Vector2 normal)
        {
            if (cable.nodes.Count <= 2) return;

            if (Vector2.Dot(pipeEntryNormal, normal) < 0) return;
            
            cable.DestroyNode(cable.nodes[cable.nodes.Count - 1]);
        }
    }
}