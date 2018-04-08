using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Ball : MonoBehaviour
{
  private Rigidbody body;
  public float minSpeed = 15f;
  public float maxSpeed = 25f; // this should increase with game time

  void Start()
  {
    body = GetComponent<Rigidbody>();
  }


  void Update()
  {
    if (body.velocity.magnitude > maxSpeed) body.velocity = body.velocity.normalized * maxSpeed;
    if (body.velocity.magnitude < minSpeed) body.velocity = body.velocity.normalized * minSpeed;
  }
}
