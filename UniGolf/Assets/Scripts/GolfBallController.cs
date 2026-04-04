using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class GolfBallShotController : MonoBehaviour
{
    public float rotationSpeed = 120f;
    public float maxPower = 15f;
    public float powerMultiplier = 0.03f;
    public float stopThreshold = 0.08f;
    public float aimLineBaseLength = 2f;

    public Transform cameraTransform;

    private Rigidbody rb;
    private LineRenderer lineRenderer;

    private bool isDragging = false;
    private Vector3 aimDirection = Vector3.forward;
    private float shotPower = 0f;
    private float dragStartY = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        SetupLineRenderer();
    }

    void Start()
    {
        rb.linearDamping = 1.5f;
        rb.angularDamping = 0.5f;

        if (cameraTransform != null)
        {
            Vector3 forward = cameraTransform.forward;
            forward.y = 0f;

            if (forward.sqrMagnitude > 0.01f)
            {
                aimDirection = forward.normalized;
            }
        }
    }

    void Update()
    {
        ForceStopIfSlow();

        if (BallIsStopped())
        {
            RotateAim();
            HandleShotInput();
            UpdateAimLine();
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }

    void SetupLineRenderer()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.08f;
        lineRenderer.endWidth = 0.04f;
        lineRenderer.useWorldSpace = true;
        lineRenderer.enabled = true;

        if (lineRenderer.material == null)
        {
            Shader shader = Shader.Find("Sprites/Default");
            if (shader != null)
            {
                lineRenderer.material = new Material(shader);
            }
        }

        lineRenderer.startColor = Color.yellow;
        lineRenderer.endColor = Color.red;
    }

    void RotateAim()
    {
        float turn = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            {
                turn -= 1f;
            }

            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            {
                turn += 1f;
            }

            if (Keyboard.current.cKey.wasPressedThisFrame && cameraTransform != null)
            {
                Vector3 camForward = cameraTransform.forward;
                camForward.y = 0f;

                if (camForward.sqrMagnitude > 0.01f)
                {
                    aimDirection = camForward.normalized;
                }
            }
        }

        if (turn != 0f)
        {
            Quaternion rotation = Quaternion.Euler(0f, turn * rotationSpeed * Time.deltaTime, 0f);
            aimDirection = rotation * aimDirection;
            aimDirection.Normalize();
        }
    }

    void HandleShotInput()
    {
        if (Mouse.current == null)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            isDragging = true;
            dragStartY = Mouse.current.position.ReadValue().y;
            shotPower = 0f;
        }

        if (isDragging && Mouse.current.leftButton.isPressed)
        {
            float currentY = Mouse.current.position.ReadValue().y;
            float dragDistance = dragStartY - currentY;

            shotPower = Mathf.Clamp(dragDistance * powerMultiplier, 0f, maxPower);
        }

        if (isDragging && Mouse.current.leftButton.wasReleasedThisFrame)
        {
            isDragging = false;

            if (shotPower > 0.05f)
            {
                rb.AddForce(aimDirection * shotPower, ForceMode.Impulse);
            }

            shotPower = 0f;
        }
    }

    void UpdateAimLine()
    {
        lineRenderer.enabled = true;

        Vector3 start = transform.position + Vector3.up * 0.15f;
        float lineLength = aimLineBaseLength + shotPower * 0.35f;
        Vector3 end = start + aimDirection * lineLength;

        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }

    void ForceStopIfSlow()
    {
        if (rb.linearVelocity.magnitude < stopThreshold &&
            rb.angularVelocity.magnitude < stopThreshold)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    bool BallIsStopped()
    {
        return rb.linearVelocity.magnitude < stopThreshold &&
               rb.angularVelocity.magnitude < stopThreshold;
    }
}