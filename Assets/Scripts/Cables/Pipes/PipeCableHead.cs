using System;
using UnityEngine;
using Utility;

namespace Cables.Pipes
{
    [RequireComponent(typeof(Collider2D))]
    public class PipeCableHead : MonoBehaviour
    {
        [SerializeField] private CableHead cableHead;
        [SerializeField] private VelocityTracker velocityTracker;
        
        private CablePipeNodeController pipeNodeController;
        private Collider2D collider2D;

        private void Awake()
        {
            collider2D = GetComponent<Collider2D>();
        }

        private void OnEnable()
        {
            cableHead.cableChanged.AddListener(OnCableChanged);
        }

        private void OnDisable()
        {
            cableHead.cableChanged.RemoveListener(OnCableChanged);
        }

        private void OnCableChanged()
        {
            // TODO: Is there any reason the PipeNodeController is on the cable?
            pipeNodeController = cableHead.CurrentCable == null ? null : cableHead.CurrentCable.GetComponent<CablePipeNodeController>();
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (cableHead.CurrentCable == null) return;

            if (!col.CompareTag("Pipe")) return;

            var hit = UtilityFunctions.TriggerCollision(collider2D, velocityTracker.Velocity);

            Vector2 nodePosition = hit.point + hit.normal * cableHead.CurrentCable.cableWidth / 2;

            // Draw collision normals
            Debug.DrawLine(hit.point, hit.point + hit.normal, Color.yellow, 30f);

            pipeNodeController.PipeEnter(nodePosition, hit.normal);
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            if (cableHead.CurrentCable == null) return;
            
            if (!col.CompareTag("Pipe")) return;

            var hit = UtilityFunctions.TriggerCollision(collider2D, -velocityTracker.Velocity);
            
            pipeNodeController.PipeExit(hit.normal);
        }
    }
}