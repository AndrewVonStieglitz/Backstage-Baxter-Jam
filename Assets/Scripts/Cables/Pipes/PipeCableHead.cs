﻿using UnityEngine;

namespace Cables.Pipes
{
    public class PipeCableHead : MonoBehaviour
    {
        [SerializeField] private CableHead cableHead;
        
        private CablePipeNodeController pipeNodeController;

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
            pipeNodeController = cableHead.CurrentCable == null ? null : cableHead.CurrentCable.GetComponent<CablePipeNodeController>();
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            CheckPipeCollision(col);
        }

        private void CheckPipeCollision(Collider2D col)
        {
            // TODO: Duplicate code. See OnTriggerExit2D.
            if (cableHead.CurrentCable == null) return;

            if (!col.CompareTag("Pipe")) return;

            var hit = cableHead.TriggerCollision(cableHead.Velocity);

            Vector2 nodePosition = hit.point + hit.normal * cableHead.CurrentCable.cableWidth / 2;

            // Draw collision normals
            Debug.DrawLine(hit.point, hit.point + hit.normal, Color.yellow, 30f);

            pipeNodeController.PipeEnter(nodePosition, hit.normal);
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            // TODO: Duplicate code. See OnTriggerEnter2D.
            if (cableHead.CurrentCable == null) return;
            
            if (!col.CompareTag("Pipe")) return;

            var hit = cableHead.TriggerCollision(-cableHead.Velocity);
            
            pipeNodeController.PipeExit(hit.normal);
        }
    }
}