using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cables
{
    public class CableController : MonoBehaviour
    {
        [SerializeField] private int cableID;
        public int CableID { get => cableID; }

        private int ampID;
        public int AmpID { get => ampID; }

        public CableController(int cableID, int ampID)
        {
            this.cableID = cableID;
            this.ampID = ampID;
        }

        private enum Direction { Left, Right, Up, Down }
        
        [SerializeField] private GameObject nodePrefab;
        [SerializeField] private float friction;
        
        public float cableWidth;

        public List<CableNode> nodes = new List<CableNode>();

        private Vector3 lastPosition;
        private Vector3 velocity;
        
        // Pole Tracking
        private PoleController previousRegion;
        private PoleController currentRegion;

        private Direction pipeEntryDirection;

        private void Awake()
        {
            lastPosition = transform.position;

            GetComponent<BoxCollider2D>().size = new Vector2(cableWidth, cableWidth);
        }

        private void Update()
        {
            // TODO: Get the angle between the last node and the player
            // TODO: Duplicate code. See CableRenderer.FlatEndedness.
            if (nodes.Count > 1)
            {
                var angle = Vector2.Angle(nodes[nodes.Count - 1].transform.position, transform.position);
                
                // TODO: If the angle exceeds the friction
                // if (angle)
            
                // TODO: Slide the node along the pipe
            }
        }
        
        private void FixedUpdate()
        {
            velocity = (transform.position - lastPosition) / Time.deltaTime;
            lastPosition = transform.position;
        }
        
        private void OnTriggerEnter2D(Collider2D col)
        {
            // if (!col.CompareTag("Region")) return;
            //
            // var region = col.GetComponent<Region>();
            //
            // if (currentRegion == region)
            // {
            //     Debug.Log("New Node in " + col.gameObject.name);
            //
            //     CreateNode(currentRegion);
            // }
            //
            // previousRegion = currentRegion;
            // currentRegion = region;

            if (!col.CompareTag("Pipe")) return;
            
            var regionOrientation = col.GetComponent<PoleController>().PoleOrientation;
            
            pipeEntryDirection = VelocityToDirection(regionOrientation);
            
            // Debug.Log("Pipe Entry Direction: " + pipeEntryDirection + "(" + velocity + ")");

            Vector2 nodePosition = NodePosition(transform.position, regionOrientation, col, pipeEntryDirection);
            
            CreateNode(nodePosition, regionOrientation);
        }

        private Vector2 NodePosition(Vector3 transformPosition, OrientationUtil.Orientation o, Collider2D col, Direction pipeEntryDirection)
        {
            OrientationUtil.OrientedVector2 oPosition = new OrientationUtil.OrientedVector2(transformPosition);

            float fromPipeCentre = 0;

            var bounds = col.bounds;

            // TODO: Clean this up using OrientedVectors
            if (o == OrientationUtil.Orientation.Horizontal)
            {
                if (pipeEntryDirection == Direction.Down)
                {
                    fromPipeCentre = bounds.center.y + bounds.extents.y + cableWidth / 2;
                }
                else
                {
                    fromPipeCentre = bounds.center.y - bounds.extents.y - cableWidth / 2;
                }
            }
            else
            {
                if (pipeEntryDirection == Direction.Left)
                {
                    fromPipeCentre = bounds.center.x + bounds.extents.x + cableWidth / 2;
                }
                else
                {
                    fromPipeCentre = bounds.center.x - bounds.extents.x - cableWidth / 2;
                }
            }
            
            OrientationUtil.OrientedVector2 oVector = new OrientationUtil.OrientedVector2(oPosition.X(o), fromPipeCentre);

            return new Vector2(oVector.X(o), oVector.Y(o));
        }

        private Direction VelocityToDirection(OrientationUtil.Orientation orientation)
        {
            if (orientation == OrientationUtil.Orientation.Horizontal)
            {
                return velocity.y > 0 ? Direction.Up : Direction.Down;
            }
            else
            {
                return velocity.x > 0 ? Direction.Right : Direction.Left;
            }
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            if (!col.CompareTag("Pipe")) return;

            var pipeExitDirection = VelocityToDirection(col.GetComponent<PoleController>().PoleOrientation);

            // Debug.Log("Pipe Exit Direction: " + pipeExitDirection + "(" + velocity + ")");

            if (pipeExitDirection != pipeEntryDirection)
            {
                DestroyNode(nodes.Last());
            }
        }

        private void DestroyNode(CableNode node)
        {
            Destroy(node.gameObject);

            nodes.Remove(node);
        }

        private void CreateNode(Vector3 pointPos, OrientationUtil.Orientation orientation)
        {
            var nodeObject = Instantiate(nodePrefab, pointPos, Quaternion.identity);

            var node = nodeObject.GetComponent<CableNode>();

            if (node == null) throw new Exception("No node component on node prefab.");

            node.Orientation = orientation;
            
            nodes.Add(node);
        }
    }
}
