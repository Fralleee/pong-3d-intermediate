using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/*
 *  Flytta ut basic grejer till egen komponent som både padcontroller och AI controller ärver ifrån
 * 
 */

public enum PlayerController {
  PLAYER1,
  PLAYER2
}

[RequireComponent(typeof(Rigidbody))]
public class PadController : MonoBehaviour
{
  private Rigidbody body;
  [SerializeField] private PlayerController player = PlayerController.PLAYER1;
  [SerializeField] private float speed = 50f;
  [SerializeField] private float maxSpeed = 15f;
  [SerializeField] private float spinCooldown = .2f;
  [SerializeField] private Vector2 zBounds;
  [SerializeField] private Vector2 xBounds;

  private float movementX = 0f;
  private float movementZ = 0f;
  private float nextSpin = 0f;

  private string verticalControlIdentifier;
  private string horizontalControlIdentifier;
  private string fireControlIdentifier;

  #region Monobehaviour methods
  void Start()
  {
    body = GetComponent<Rigidbody>();
    body.maxAngularVelocity = 500f;
    verticalControlIdentifier = player == PlayerController.PLAYER1 ? "Vertical_P1" : "Vertical_P2";
    horizontalControlIdentifier = player == PlayerController.PLAYER1 ? "Horizontal_P1" : "Horizontal_P2";
    fireControlIdentifier = player == PlayerController.PLAYER1 ? "Fire_P1" : "Fire_P2";
  }

  void Update()
  {
    movementX = Input.GetAxisRaw(verticalControlIdentifier) * -speed;
    movementZ = Input.GetAxisRaw(horizontalControlIdentifier) * speed;
    if (Input.GetButtonDown(fireControlIdentifier) && Time.time > nextSpin)
    {
      Spin();
      nextSpin = Time.time + spinCooldown;
    }
  }


  void FixedUpdate()
  {
    body.AddForce(new Vector3(movementX, 0, movementZ), ForceMode.VelocityChange);
    if (body.velocity.magnitude > maxSpeed) body.velocity = body.velocity.normalized * maxSpeed;
  }

  void LateUpdate()
  {
    ClampPosition();
  }
  #endregion

  void ClampPosition()
  {
    var pos = transform.position;
    pos.x = Mathf.Clamp(transform.position.x, xBounds.x, xBounds.y);
    pos.z = Mathf.Clamp(transform.position.z, zBounds.x, zBounds.y);
    transform.position = pos;
  }

  void Spin()
  {
    transform.localScale = new Vector3(6f, 1f, 1f);
    transform.DOScaleX(3f, .8f).SetEase(Ease.OutBack);
    transform.DORotate(new Vector3(0, 180f, 0), .8f, RotateMode.LocalAxisAdd).SetEase(Ease.OutBack);
  }

  void OnCollisionEnter(Collision collision)
  {
    if(collision.gameObject.tag == "Ball" && GetComponent<PadController>().enabled)
    {
      Rigidbody ballRb = collision.gameObject.GetComponent<Rigidbody>();
      Vector3 newForce = new Vector3(-Input.GetAxis(verticalControlIdentifier), 0, Input.GetAxis(horizontalControlIdentifier));
      ballRb.AddForce(newForce, ForceMode.VelocityChange);
    }
  }
}
