using UnityEngine;

namespace Cables
{
    public class CableHead : MonoBehaviour
    {
        public CableController cable;

        private Vector3 lastPosition;
        public Vector3 velocity;
        
        public void NewCable(CableController cable)
        {
            this.cable = cable;
            
            GetComponent<BoxCollider2D>().size = new Vector2(cable.cableWidth, cable.cableWidth);
            
            cable.cableCompleted.AddListener(OnCableCompleted);
        }

        private void OnCableCompleted()
        {
            cable.cableCompleted.RemoveListener(OnCableCompleted);

            cable = null;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (cable == null) return;
            
            if (!col.CompareTag("Pipe")) return;

            var pole = col.GetComponent<PoleController>();
            
            var poleOrientation = pole.PoleOrientation;

            var pipeEntryDirection = VelocityToDirection(poleOrientation);
            
            Vector2 nodePosition = NodePosition(transform.position, poleOrientation, col, pipeEntryDirection);
            
            cable.PipeEnter(pole, pipeEntryDirection, nodePosition);
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            if (cable == null) return;
            
            if (!col.CompareTag("Pipe")) return;

            var pole = col.GetComponent<PoleController>();

            var pipeExitDirection = VelocityToDirection(pole.PoleOrientation);
            
            cable.PipeExit(pole, pipeExitDirection);
        }

        private void FixedUpdate()
        {
            if (cable == null || cable.state != CableController.CableState.InProgress) return;

            velocity = (transform.position - lastPosition) / Time.deltaTime;
            lastPosition = transform.position;
        }

        private DirectionUtil.Direction VelocityToDirection(OrientationUtil.Orientation orientation)
        {
            if (orientation == OrientationUtil.Orientation.Horizontal)
            {
                return velocity.y > 0 ? DirectionUtil.Direction.Up : DirectionUtil.Direction.Down;
            }
            else
            {
                return velocity.x > 0 ? DirectionUtil.Direction.Right : DirectionUtil.Direction.Left;
            }
        }

        private Vector2 NodePosition(Vector3 transformPosition, OrientationUtil.Orientation o, Collider2D col, DirectionUtil.Direction pipeEntryDirection)
        {
            OrientationUtil.OrientedVector2 oPosition = new OrientationUtil.OrientedVector2(transformPosition);

            float fromPipeCentre = 0;

            var bounds = col.bounds;

            // TODO: Clean this up using OrientedVectors
            if (o == OrientationUtil.Orientation.Horizontal)
            {
                if (pipeEntryDirection == DirectionUtil.Direction.Down)
                {
                    fromPipeCentre = bounds.center.y + bounds.extents.y + cable.cableWidth / 2;
                }
                else
                {
                    fromPipeCentre = bounds.center.y - bounds.extents.y - cable.cableWidth / 2;
                }
            }
            else
            {
                if (pipeEntryDirection == DirectionUtil.Direction.Left)
                {
                    fromPipeCentre = bounds.center.x + bounds.extents.x + cable.cableWidth / 2;
                }
                else
                {
                    fromPipeCentre = bounds.center.x - bounds.extents.x - cable.cableWidth / 2;
                }
            }
            
            OrientationUtil.OrientedVector2 oVector = new OrientationUtil.OrientedVector2(oPosition.X(o), fromPipeCentre);

            return new Vector2(oVector.X(o), oVector.Y(o));
        }
    }
}