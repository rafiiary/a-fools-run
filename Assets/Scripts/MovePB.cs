using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePB : MonoBehaviour
{
    // user inputs
    private float wsInput;
    private float adInput;
    private float inputScale;

    // camera and character heading related
    public GameObject MainCamera;
    public Transform CameraTransform;
    private Vector3 userRotation;
    private Vector3 cameraRotation;
    private Vector3 heading;

    // tune sensitivity of controls
    // original mass, drag, angularDrag: 1, 2, 0.05
    private float moveScale = 0.6f; // original 0.5
    private float jumpScale = 8.0f; // original 4.0 (using AddForce)

    // jump limiter
    [SerializeField] private LayerMask PlatformLayerMask;
    public Collider HumanCollider;
    private bool userJumped;
    private float distanceToGround;

    // model components
    private Rigidbody HumanRigidbody;
    private Transform HumanTransform;

    // Colliders
    public Collider normalCollider;

    // animation related
    Animator Animator;
    bool movingForward;
    bool isGrounded;
    bool jumping;
    bool sprinting;
    bool hasFallen;

    void Start() {
        HumanRigidbody = GetComponent<Rigidbody>();
        HumanTransform = GetComponent<Transform>();
        CameraTransform = MainCamera.GetComponent<Transform>();
        Animator = GetComponent<Animator>();
        HumanCollider = normalCollider;
        distanceToGround = normalCollider.bounds.extents.y;
        hasFallen = false;
        // StartCoroutine(printStates());
    }

    void Update() {
        // get keyboard inputs
        wsInput = Input.GetAxis("Vertical");
        adInput = Input.GetAxis("Horizontal");
        userJumped = Input.GetButton("Jump");

        // play animations according to keyboard inputs
        movingForward = Input.GetKey("w") || Input.GetKey("s") ||
                          Input.GetKey("a") || Input.GetKey("d");
        isGrounded = IsGrounded();
        jumping = Input.GetKey("space");
        sprinting = Input.GetKey(KeyCode.LeftShift);

    }

    private void FixedUpdate() {
        // decide which direction should the character go (according to camera heading)
        userRotation = HumanTransform.rotation.eulerAngles;
        cameraRotation = CameraTransform.rotation.eulerAngles;
        inputScale = 0;

        // perform animations
        Animator.SetBool("isWalking", movingForward && !hasFallen);
        Animator.SetBool("isJumping", !isGrounded && !hasFallen);
        Animator.SetBool("isGrounded", isGrounded && !hasFallen);
        Animator.SetBool("isRunning", sprinting && !hasFallen);
        Animator.SetBool("isIdle", !movingForward && isGrounded && !hasFallen);
        Animator.SetBool("fallen", hasFallen);

        // move 90 degrees right (press only "D" or "D" + "W" + "S")
        if (!hasFallen)
        {
          if ((Input.GetKey("d") && !Input.GetKey("w") && !Input.GetKey("s")) || (Input.GetKey("d") && Input.GetKey("w") && Input.GetKey("s"))) {
              userRotation[1] = cameraRotation[1];
              userRotation += new Vector3(0, 90, 0);
              heading = HumanTransform.forward;
              inputScale = Mathf.Abs(adInput);

          // move 90 degrees left (press only "A" or "A" + "W" + "S")
          } else if ((Input.GetKey("a") && !Input.GetKey("w") && !Input.GetKey("s")) || (Input.GetKey("a") && Input.GetKey("w") && Input.GetKey("s"))) {
              userRotation[1] = cameraRotation[1];
              userRotation += new Vector3(0, -90, 0);
              heading = HumanTransform.forward;
              inputScale = Mathf.Abs(adInput);

          // move 0 degree forward (press only "W" or "W" + "A" + "D")
          } else if ((Input.GetKey("w") && !Input.GetKey("a") && !Input.GetKey("d")) || (Input.GetKey("w") && Input.GetKey("a") && Input.GetKey("d"))) {
              userRotation[1] = cameraRotation[1];
              heading = HumanTransform.forward;
              inputScale = Mathf.Abs(wsInput);

          // move 180 degrees backward (press only "S" or "S" + "A" + "D")
          } else if ((Input.GetKey("s") && !Input.GetKey("a") && !Input.GetKey("d")) || (Input.GetKey("s") && Input.GetKey("a") && Input.GetKey("d"))) {
              userRotation[1] = cameraRotation[1];
              userRotation += new Vector3(0, 180, 0);
              heading = HumanTransform.forward;
              inputScale = Mathf.Abs(wsInput);

          // move 45 degrees right (press "W" + "D")
          } else if (Input.GetKey("w") && Input.GetKey("d")) {
              userRotation[1] = cameraRotation[1];
              userRotation += new Vector3(0, 45, 0);
              heading = HumanTransform.forward;
              inputScale = (Mathf.Abs(wsInput) + Mathf.Abs(adInput)) / 2.0f;

          // move 45 degrees left (press "W" + "A")
          } else if (Input.GetKey("w") && Input.GetKey("a")) {
              userRotation[1] = cameraRotation[1];
              userRotation += new Vector3(0, -45, 0);
              heading = HumanTransform.forward;
              inputScale = (Mathf.Abs(wsInput) + Mathf.Abs(adInput)) / 2.0f;

          // move 135 degrees right (press "S" + "D")
          } else if (Input.GetKey("s") && Input.GetKey("d")) {
              userRotation[1] = cameraRotation[1];
              userRotation += new Vector3(0, 135, 0);
              heading = HumanTransform.forward;
              inputScale = (Mathf.Abs(wsInput) + Mathf.Abs(adInput)) / 2.0f;

          // move 135 degrees left (press "S" + "A")
          } else if (Input.GetKey("s") && Input.GetKey("a")) {
              userRotation[1] = cameraRotation[1];
              userRotation += new Vector3(0, -135, 0);
              heading = HumanTransform.forward;
              inputScale = (Mathf.Abs(wsInput) + Mathf.Abs(adInput)) / 2.0f;

          // stand still
          } else {
              inputScale = 0;
          }
        }

        // rotate character to the right direction
        HumanTransform.rotation = Quaternion.Lerp(HumanTransform.rotation, Quaternion.Euler(userRotation), 0.3f);

        // let the character go forward
        if (sprinting && !hasFallen) {
            HumanRigidbody.velocity += heading * inputScale * moveScale * 1.3f;
        } else {
            HumanRigidbody.velocity += heading * inputScale * moveScale;
        }


        // only able to jump if you are on the ground
        if (isGrounded && userJumped && !hasFallen) {
            HumanRigidbody.velocity = Vector3.up * jumpScale;
        }
    }

    IEnumerator printStates() {
        var norm = euclideanNorm(HumanRigidbody.velocity.x, HumanRigidbody.velocity.z);
        if (norm != 0) {
            print("walking...");
            print($"velocity: {norm}");
        } else {
            print("idle...");
        }
        yield return new WaitForSeconds(5);
    }

    /** Return the euclidean norm of x and y */
    private float euclideanNorm (float x, float y) {
        return Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2));
    }

    /** Send a raycast to check if player is grounded and returns true if
     the player is on some sort of ground */
    private bool IsGrounded() {
        // boxcast not working now
        // float extraHeight = 0.05f;
        // bool hitGround = Physics.BoxCast(HumanCollider.bounds.center, HumanTransform.lossyScale, HumanTransform.up * -1, Quaternion.Euler(Vector3.zero), HumanTransform.lossyScale.y + extraHeight);
        bool hitGround = Physics.Raycast(HumanTransform.position, Vector3.down, distanceToGround - 0.36f);

        /*
        Color rayColor;
        if (hitGround) {
            rayColor = Color.green;
        } else {
            rayColor = Color.red;
        }
        Debug.DrawRay(HumanTransform.position, Vector3.down * (distanceToGround - 0.36f), rayColor);
        */

        return hitGround;
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.collider.CompareTag("Obstacle")) {
            print("human collided with obstacle");
            hasFallen = true;
            StartCoroutine(Slowed());
        }
    }

    public IEnumerator Slowed() {
        print("Human will slow");
        moveScale = 0.1f;
        print("Human slowed");
        yield return new WaitForSeconds(3);
        hasFallen = false;
        print("waiting");
        moveScale = 0.5f; // original 0.5
        print("Human back to normal speed");
    }
}
