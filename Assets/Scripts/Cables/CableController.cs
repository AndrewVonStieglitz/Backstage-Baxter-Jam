using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Cables
{
    public class CableController : MonoBehaviour
    {
        public enum CableState { InProgress, Completed, Abandoned }

        [SerializeField] private int cableID;
        
        public int CableID { get => cableID; }

        public UnityEvent initialised = new UnityEvent();
        public UnityEvent<CableNode> nodeCreated = new UnityEvent<CableNode>();
        public UnityEvent<CableNode> nodeDestroyed = new UnityEvent<CableNode>();
        public UnityEvent<CableNode> nodeMoved = new UnityEvent<CableNode>();

        public CableState state;
        public float cableWidth;
        public List<CableNode> nodes = new List<CableNode>();
        public Sprite Sprite;

        public void Initialise(Transform transform, Sprite sprite)
        {
            Sprite = sprite;
            
            CreateNode(new CableNode(), transform.position);
            CreateNode(new CableNode(), transform.position);
            
            initialised.Invoke();
        }

        public void CreateNode(CableNode node, Vector3 nodePos)
        {
            CreateNodeAtIndex(node, nodePos, nodes.Count);
        }

        public void CreateNodeAtIndex(CableNode node, Vector3 nodePos, int index)
        {
            node.MoveNode(nodePos);
            
            node.nodeMoved.AddListener(OnNodeMoved);

            nodes.Insert(index, node);

            nodeCreated.Invoke(node);
            
            GameEvents.CableWind(this, nodePos);
        }

        private void OnNodeMoved(CableNode node)
        {
            nodeMoved.Invoke(node);
        }

        public void DestroyNode(CableNode node)
        {
            nodes.Remove(node);
            
            nodeDestroyed.Invoke(node);
        }

        public void Complete(Vector3 endPosition)
        {
            nodes.Last().MoveNode(endPosition);
            
            state = CableState.Completed;
        }
    }
}
