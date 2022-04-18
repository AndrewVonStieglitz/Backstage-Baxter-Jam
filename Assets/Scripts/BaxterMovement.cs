using UnityEngine;

public class BaxterMovement : MonoBehaviour {

    bool LeftP = false;
    bool rightP = false;
    bool jumpP = false;

    Rigidbody2D rb;

    private void Start() {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
    }
    private void Update() {
        LeftP = Input.GetKey(KeyCode.LeftArrow);
        rightP = Input.GetKey(KeyCode.RightArrow);
        jumpP = Input.GetKey(KeyCode.UpArrow);
    }
    private void FixedUpdate() {
        if (LeftP) {
            rb.AddForce(new Vector2(-1, 0) * 10);
        } 
        if (rightP) {
            rb.AddForce(new Vector2(1, 0) * 10);
        }
        if (jumpP) {
            rb.AddForce(new Vector2(0, 1) * 10);
        }
    }
}