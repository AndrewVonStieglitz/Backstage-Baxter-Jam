using UnityEngine;

public class BaxterMovement : MonoBehaviour {
    public float speed;
    public float jumpSpeed;
    bool isGrounded = false;
    float jumping;
    public float jumpingTimer;

    Rigidbody2D rb;
    
    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate() {
        if (isGrounded) {
            if (Input.GetKey(KeyCode.W)) {
                jumping = jumpingTimer;
            }
        } else {
            if (!Input.GetKey(KeyCode.W)) {
                jumping -= 10;
            }
        }

        if (jumping > 0) { 
            jumping -= 1;
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }

        if (Input.GetKey(KeyCode.A)) {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
        }
        if (Input.GetKey(KeyCode.D)) {
            rb.velocity = new Vector2(speed, rb.velocity.y);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Ground") {
            isGrounded = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision) {
        if (collision.gameObject.tag == "Ground") {
            isGrounded = false;
        }
    }
}