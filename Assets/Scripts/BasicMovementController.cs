using UnityEngine;

public class BasicMovementController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rididbody;
    [SerializeField] private float speed;

    private void Update()
    {
        var direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        
        rididbody.AddForce(direction * Time.deltaTime * speed);
    }
}
