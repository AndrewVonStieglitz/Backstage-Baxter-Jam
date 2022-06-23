using UnityEngine;

namespace Cables.Pipes
{
    public class PipeNode : CableNode
    {
        public Vector2 Normal { get; }

        public PipeNode(Vector2 normal)
        {
            Normal = normal;
        }
    }
}