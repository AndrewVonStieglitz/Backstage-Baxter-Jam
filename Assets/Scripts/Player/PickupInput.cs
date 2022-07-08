using System.Collections;
using Pluggables;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PickupInput : MonoBehaviour
    {
        // TODO: Remove this dependency.
        [SerializeField] private Cables.CableHead cableHead;
        [SerializeField] private ConnectionHead connectionHead;
        [SerializeField] private float maxHoldPickup;

        private PlayerControls playerControls;
        private float pickupHeldFrom = 0f;
        private bool pickupBeingHeld = false;

        private void Awake()
        {
            playerControls = new PlayerControls();
            playerControls.Baxter.Enable();
        }

        private void OnEnable()
        {
            playerControls.Baxter.PickupRelease.performed += PickupPressDown;
            playerControls.Baxter.PickupRelease.canceled += PickupPressUp;
        }
        
        private void OnDisable()
        {
            playerControls.Baxter.PickupRelease.performed -= PickupPressDown;
            playerControls.Baxter.PickupRelease.canceled -= PickupPressUp;
        }

        private void PickupPressDown(InputAction.CallbackContext context)
        {
            pickupHeldFrom = Time.time;
            pickupBeingHeld = true;
            StopCoroutine(countdownPickupHold());
            StartCoroutine(countdownPickupHold());
        }

        private void PickupPressUp(InputAction.CallbackContext context)
        {
            float pickupHeldTime= Time.time - pickupHeldFrom;
            pickupBeingHeld = false;
            StopCoroutine(countdownPickupHold());
            if (pickupHeldTime < maxHoldPickup)
            {
                // pick up the cable
                connectionHead.TryInteract();
            }
            else
            {
                // release the cable
                cableHead.DropCable();
            }
        }

        private IEnumerator countdownPickupHold()
        {
            float startTime = Time.time;
            yield return new WaitForSeconds(maxHoldPickup);
            if (pickupBeingHeld && Time.time - pickupHeldFrom >= maxHoldPickup)
            {
                // release cable
                //print("Coroutine force releasing cable\tat: " + Time.time + ",\t started at: " + startTime);
                cableHead.DropCable();
                pickupBeingHeld = false;
            }
        }
    }
}