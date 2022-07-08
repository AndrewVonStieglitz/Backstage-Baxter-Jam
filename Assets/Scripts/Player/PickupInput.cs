using System.Collections;
using Pluggables;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PickupInput : MonoBehaviour
    {
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
            StopCoroutine(CountdownPickupHold());
            StartCoroutine(CountdownPickupHold());
        }

        private void PickupPressUp(InputAction.CallbackContext context)
        {
            if (!pickupBeingHeld) return;
            
            pickupBeingHeld = false;
            
            StopCoroutine(CountdownPickupHold());
            
            connectionHead.TryInteract();
        }

        private IEnumerator CountdownPickupHold()
        {
            yield return new WaitForSeconds(maxHoldPickup);
            
            if (pickupBeingHeld && Time.time - pickupHeldFrom >= maxHoldPickup)
            {
                connectionHead.AbandonConnection();
                
                pickupBeingHeld = false;
            }
        }
    }
}