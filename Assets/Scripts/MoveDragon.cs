using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDragon : MonoBehaviour
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
    private float jumpScale = 8.0f; // original 4.0 (using AddForce)

    // jump limiter
    private bool userJumped;
    private float distanceToGround;
      
    // model components
    private Rigidbody DragonRigidbody;
    private Transform DragonTransform;

    // animation related
    Animator Animator;

    void Start() {
        DragonRigidbody = GetComponent<Rigidbody>();
        DragonTransform = GetComponent<Transform>();
        CameraTransform = MainCamera.GetComponent<Transform>();
        Animator = GetComponent<Animator>();
        distanceToGround = GetComponent<Collider>().bounds.extents.y;
        // StartCoroutine(printStates());
    }

    void Update() {
        // get keyboard inputs
        wsInput = Input.GetAxis("Vertical");
        adInput = Input.GetAxis("Horizontal");
        userJumped = Input.GetButton("Jump");
    }

    private void FixedUpdate() {
        // decide which direction should the character go (according to camera heading)
        userRotation = DragonTransform.rotation.eulerAngles;
        cameraRotation = CameraTransform.rotation.eulerAngles;
        inputScale = 0;
        
        // move 90 degrees right (press only "D" or "D" + "W" + "S")
        if ((Input.GetKey("d") && !Input.GetKey("w") && !Input.GetKey("s")) || (Input.GetKey("d") && Input.GetKey("w") && Input.GetKey("s"))) {
            userRotation[1] = cameraRotation[1];
            userRotation += new Vector3(0, 90, 0);
            heading = DragonTransform.forward;
            inputScale = Mathf.Abs(adInput);

        // move 90 degrees left (press only "A" or "A" + "W" + "S")
        } else if ((Input.GetKey("a") && !Input.GetKey("w") && !Input.GetKey("s")) || (Input.GetKey("a") && Input.GetKey("w") && Input.GetKey("s"))) {
            userRotation[1] = cameraRotation[1];
            userRotation += new Vector3(0, -90, 0);
            heading = DragonTransform.forward;
            inputScale = Mathf.Abs(adInput);

        // move 0 degree forward (press only "W" or "W" + "A" + "D")
        } else if ((Input.GetKey("w") && !Input.GetKey("a") && !Input.GetKey("d")) || (Input.GetKey("w") && Input.GetKey("a") && Input.GetKey("d"))) {
            userRotation[1] = cameraRotation[1];
            heading = DragonTransform.forward;
            inputScale = Mathf.Abs(wsInput);

        // move 180 degrees backward (press only "S" or "S" + "A" + "D")
        } else if ((Input.GetKey("s") && !Input.GetKey("a") && !Input.GetKey("d")) || (Input.GetKey("s") && Input.GetKey("a") && Input.GetKey("d"))) {
            userRotation[1] = cameraRotation[1];
            userRotation += new Vector3(0, 180, 0);
            heading = DragonTransform.forward;
            inputScale = Mathf.Abs(wsInput);
        
        // move 45 degrees right (press "W" + "D")
        } else if (Input.GetKey("w") && Input.GetKey("d")) {
            userRotation[1] = cameraRotation[1];
            userRotation += new Vector3(0, 45, 0);
            heading = DragonTransform.forward;
            inputScale = (Mathf.Abs(wsInput) + Mathf.Abs(adInput)) / 2.0f;

        // move 45 degrees left (press "W" + "A")
        } else if (Input.GetKey("w") && Input.GetKey("a")) {
            userRotation[1] = cameraRotation[1];
            userRotation += new Vector3(0, -45, 0);
            heading = DragonTransform.forward;
            inputScale = (Mathf.Abs(wsInput) + Mathf.Abs(adInput)) / 2.0f;

        // move 135 degrees right (press "S" + "D")
        } else if (Input.GetKey("s") && Input.GetKey("d")) {
            Debug.Log("135 right!!!!!");
            userRotation[1] = cameraRotation[1];
            userRotation += new Vector3(0, 135, 0);
            heading = DragonTransform.forward;
            inputScale = (Mathf.Abs(wsInput) + Mathf.Abs(adInput)) / 2.0f;

        // move 135 degrees left (press "S" + "A")
        } else if (Input.GetKey("s") && Input.GetKey("a")) {
            Debug.Log("135 left!!!!!");
            userRotation[1] = cameraRotation[1];
            userRotation += new Vector3(0, -135, 0);
            heading = DragonTransform.forward;
            inputScale = (Mathf.Abs(wsInput) + Mathf.Abs(adInput)) / 2.0f;

        // stand still
        } else {
            inputScale = 0;
        }
        
        // rotate character to the right direction
        DragonTransform.rotation = Quaternion.Lerp(DragonTransform.rotation, Quaternion.Euler(userRotation), 0.3f);

        // let the character go forward
        DragonRigidbody.velocity += heading * inputScale * moveScale;
        
        // perform animations
        var norm = euclideanNorm(DragonRigidbody.velocity.x, DragonRigidbody.velocity.z);
        Animator.SetFloat("velocity", norm);
        if (norm != 0 && IsGrounded()) {
            Animator.SetTrigger("triggerWalking");
        } else {
            Animator.SetTrigger("triggerIdle");
        }

        // Only able to jump if you are on the ground
        if (IsGrounded() && userJumped) {
            DragonRigidbody.velocity = Vector3.up * jumpScale;
        }
    }

    IEnumerator printStates() {
        var norm = euclideanNorm(DragonRigidbody.velocity.x, DragonRigidbody.velocity.z);
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
        // Debug.DrawRay(DragonTransform.position, Vector3.down * (distanceToGround + 0.05f), Color.red);
        return Physics.Raycast(DragonTransform.position, Vector3.down, distanceToGround + 0.05f);
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.collider.CompareTag("Obstacle")) {
            print("dragon collided with obstacle");
            StartCoroutine(Slowed());
        }
    }

    public IEnumerator Slowed() {
        print("Dragon will slow");
        moveScale = 0.1f;
        print("Dragon slowed");
        yield return new WaitForSeconds(3);
        print("waiting");
        moveScale = 0.5f; // original 0.5
        print("Dragon back to normal speed");
    }
}
