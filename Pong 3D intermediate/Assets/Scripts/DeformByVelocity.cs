using UnityEngine;
using DG.Tweening;

public class DeformByVelocity : MonoBehaviour
{
  public float minVelocity = 1f;
  public float strength = 0.01f;
  public new Rigidbody rigidbody;
  private Vector3 startScale;

  private void Start()
  {
    startScale = transform.localScale;
  }

  private void Update()
  {
    var velocity = rigidbody.velocity.magnitude;
    if (velocity < minVelocity) {
      transform.localScale = startScale;
      return;
    }

    var rotation = transform.eulerAngles;
    var angle = Vector3.SignedAngle(Vector3.forward, rigidbody.velocity, Vector3.up);
    rotation.y = angle;
    transform.eulerAngles = rotation;

    var amount = 1f + velocity * strength;
    var inverseAmount = (1f / amount);
    transform.localScale = new Vector3(inverseAmount, 1f, amount);
  }
}