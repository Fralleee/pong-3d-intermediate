using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AIController : MonoBehaviour
{
  private Transform ball;
  private Rigidbody body;
  [SerializeField] private float speed = 50f;
  [SerializeField] private float maxSpeed = 15f;
  private float movementX = 0f;
  private float movementZ = 0f;

  void Start()
  {
    body = GetComponent<Rigidbody>();
    body.maxAngularVelocity = 500f;
    ball = GameObject.FindGameObjectWithTag("Ball").transform;
  }

  void Update()
  {
    // check distance between pady and bally
    float posX = ball.position.x - transform.position.x;
    movementX = posX * speed;
  }

  void FixedUpdate()
  {
    body.AddForce(new Vector3(movementX, 0, movementZ), ForceMode.VelocityChange);
    if (body.velocity.magnitude > maxSpeed) body.velocity = body.velocity.normalized * maxSpeed;
  }
}
