using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveChicken : MonoBehaviour
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
    private float moveScale = 0.5f; // original 0.5
    private float jumpScale = 20.0f; // original 4.0 (using AddForce)

    // jump limiter
    private bool userJumped;
    private float distanceToGround;

    // model components
    private Rigidbody ChickenRigidbody;
    private Transform ChickenTransform;

    // animation related
    Animator Animator;
    bool movingForward;
    bool isGrounded;
    bool jumping;
    bool sprinting;
    bool hasFallen;

    void Start() {
        ChickenRigidbody = GetComponent<Rigidbody>();
        ChickenTransform = GetComponent<Transform>();
        CameraTransform = MainCamera.GetComponent<Transform>();
        Animator = GetComponent<Animator>();
        distanceToGround = GetComponent<Collider>().bounds.extents.y;
        hasFallen = false;
        isGrounded = true;
        //StartCoroutine(printStates());
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
                Animator.SetBool("fallen", hasFallen);
    }

    private void FixedUpdate() {
        // decide which direction should the character go (according to camera heading)
        userRotation = ChickenTransform.rotation.eulerAngles;
        cameraRotation = CameraTransform.rotation.eulerAngles;
        inputScale = 0;

        Animator.SetBool("isWalking", movingForward && !hasFallen);
        Animator.SetBool("isJumping", !isGrounded && !hasFallen);
        Animator.SetBool("isGrounded", isGrounded && !hasFallen);
        Animator.SetBool("isRunning", sprinting && !hasFallen);
        Animator.SetBool("isIdle", !movingForward && isGrounded && !hasFallen);

        if (!hasFallen)
        {
          // move 90 degrees right (press only "D" or "D" + "W" + "S")
          if ((Input.GetKey("d") && !Input.GetKey("w") && !Input.GetKey("s")) || (Input.GetKey("d") && Input.GetKey("w") && Input.GetKey("s"))) {
              userRotation[1] = cameraRotation[1];
              userRotation += new Vector3(0, 90, 0);
              heading = ChickenTransform.forward;
              inputScale = Mathf.Abs(adInput);

          // move 90 degrees left (press only "A" or "A" + "W" + "S")
          } else if ((Input.GetKey("a") && !Input.GetKey("w") && !Input.GetKey("s")) || (Input.GetKey("a") && Input.GetKey("w") && Input.GetKey("s"))) {
              userRotation[1] = cameraRotation[1];
              userRotation += new Vector3(0, -90, 0);
              heading = ChickenTransform.forward;
              inputScale = Mathf.Abs(adInput);

          // move 0 degree forward (press only "W" or "W" + "A" + "D")
          } else if ((Input.GetKey("w") && !Input.GetKey("a") && !Input.GetKey("d")) || (Input.GetKey("w") && Input.GetKey("a") && Input.GetKey("d"))) {
              userRotation[1] = cameraRotation[1];
              heading = ChickenTransform.forward;
              inputScale = Mathf.Abs(wsInput);

          // move 180 degrees backward (press only "S" or "S" + "A" + "D")
          } else if ((Input.GetKey("s") && !Input.GetKey("a") && !Input.GetKey("d")) || (Input.GetKey("s") && Input.GetKey("a") && Input.GetKey("d"))) {
              userRotation[1] = cameraRotation[1];
              userRotation += new Vector3(0, 180, 0);
              heading = ChickenTransform.forward;
              inputScale = Mathf.Abs(wsInput);

          // move 45 degrees right (press "W" + "D")
          } else if (Input.GetKey("w") && Input.GetKey("d")) {
              userRotation[1] = cameraRotation[1];
              userRotation += new Vector3(0, 45, 0);
              heading = ChickenTransform.forward;
              inputScale = (Mathf.Abs(wsInput) + Mathf.Abs(adInput)) / 2.0f;

          // move 45 degrees left (press "W" + "A")
          } else if (Input.GetKey("w") && Input.GetKey("a")) {
              userRotation[1] = cameraRotation[1];
              userRotation += new Vector3(0, -45, 0);
              heading = ChickenTransform.forward;
              inputScale = (Mathf.Abs(wsInput) + Mathf.Abs(adInput)) / 2.0f;

          // move 135 degrees right (press "S" + "D")
          } else if (Input.GetKey("s") && Input.GetKey("d")) {
              Debug.Log("135 right!!!!!");
              userRotation[1] = cameraRotation[1];
              userRotation += new Vector3(0, 135, 0);
              heading = ChickenTransform.forward;
              inputScale = (Mathf.Abs(wsInput) + Mathf.Abs(adInput)) / 2.0f;

          // move 135 degrees left (press "S" + "A")
          } else if (Input.GetKey("s") && Input.GetKey("a")) {
              Debug.Log("135 left!!!!!");
              userRotation[1] = cameraRotation[1];
              userRotation += new Vector3(0, -135, 0);
              heading = ChickenTransform.forward;
              inputScale = (Mathf.Abs(wsInput) + Mathf.Abs(adInput)) / 2.0f;

          // stand still
          } else {
              inputScale = 0;
          }
        }
        // rotate character to the right direction
        ChickenTransform.rotation = Quaternion.Lerp(ChickenTransform.rotation, Quaternion.Euler(userRotation), 0.3f);

        // let the character go forward
        if (sprinting && !hasFallen) {
          ChickenRigidbody.velocity += heading * inputScale * moveScale * 1.1f;
        } else {
          ChickenRigidbody.velocity += heading * inputScale * moveScale;
        }


        // only able to jump if you are on the ground
        if (isGrounded && userJumped && !hasFallen) {
            ChickenRigidbody.velocity = Vector3.up * jumpScale;
        }
    }

    IEnumerator printStates() {
        var norm = euclideanNorm(ChickenRigidbody.velocity.x, ChickenRigidbody.velocity.z);
        if (norm != 0) {
            print("Running");
            print($"velocity: {norm}");
        } else {
            print("Flying");
        }
        yield return new WaitForSeconds(5);
    }

    /** Return the euclidean norm of x and y */
    private float euclideanNorm (float x, float y) {
    return Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2));
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.collider.CompareTag("Obstacle")) {
            print("chicken collided with obstacle");
            hasFallen = true;
            print("Fallen set to true");
            StartCoroutine(Slowed());
        }
    }

    /** Send a raycast to check if player is grounded and returns true if
    the player is on some sort of ground */
    private bool IsGrounded() {
        // Debug.DrawRay(ChickenTransform.position, Vector3.down * 0.05f, Color.red);
        return Physics.Raycast(ChickenTransform.position, Vector3.down, 0.6f);
    }

    public IEnumerator Slowed() {
        print("Chicken will slow");
        moveScale = 0.1f;
        print("Chicken slowed");
        yield return new WaitForSeconds(2);
        hasFallen = false;
        print("waiting");
        moveScale = 0.5f; // original 0.5
        print("Chicken back to normal speed");
    }
}
