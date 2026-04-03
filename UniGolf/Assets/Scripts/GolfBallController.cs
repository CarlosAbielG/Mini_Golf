using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GolfBallController : MonoBehaviour
{
    public float moveForce = 20f;
    public float maxSpeed = 8f;
    public Transform cameraTransform;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 moveDirection;

        if (cameraTransform != null)
        {
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;

            camForward.y = 0f;
            camRight.y = 0f;

            camForward.Normalize();
            camRight.Normalize();

            moveDirection = (camForward * v + camRight * h).normalized;
        }
        else
        {
            moveDirection = new Vector3(h, 0f, v).normalized;
        }

        if (moveDirection.magnitude > 0.1f)
        {
            rb.AddForce(moveDirection * moveForce, ForceMode.Acceleration);
        }

        Vector3 flatVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (flatVelocity.magnitude > maxSpeed)
        {
            Vector3 limitedFlatVelocity = flatVelocity.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(
                limitedFlatVelocity.x,
                rb.linearVelocity.y,
                limitedFlatVelocity.z
            );
        }
    }
}