using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class BallController : MonoBehaviour
{
  private Rigidbody body;
  [SerializeField] private float speed = 15f;
  [SerializeField] private float maxSpeed = 25f;

  private float movementX = 0f;
  private float movementZ = 0f;

  void Start()
  {
    body = GetComponent<Rigidbody>();
  }

  void Update()
  {
    movementX = Input.GetAxisRaw("Vertical_P1") * -speed;
    movementZ = Input.GetAxisRaw("Horizontal_P1") * speed;
  }


  void FixedUpdate()
  {
    body.AddForce(new Vector3(movementX, 0, movementZ), ForceMode.VelocityChange);
    if (body.velocity.magnitude > maxSpeed) body.velocity = body.velocity.normalized * maxSpeed;
  }

}
