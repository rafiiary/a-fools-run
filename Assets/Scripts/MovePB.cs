using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePB : MonoBehaviour
{
    // user inputs
    private float _playerInput;
    private float _rotationInput;

    // camera and character heading related
    public GameObject MainCamera;
    public Transform CameraTransform;
    private Vector3 _userRot;
    private Vector3 _cameraRot;
    private Vector3 heading;

    // tune sensitivity of controls
    // original mass, drag, angularDrag: 1, 2, 0.05;
    private float MoveScale = 0.5f; // original 0.5
    private const float RotateScale = 3.0f; // original 1.0
    private const float JumpMultiplier = 4.0f; // original 1.6
    private const float MaxSpeed = 5.0f;

    // jump limiter
    private bool _userJumped;
    private bool _jumpInProgress = false;
    private float distanceToGround;

    // model components
    private Rigidbody _rigidbody;
    private Transform _transform;

    // animation related
    Animator animator;
    bool moving_forward;
    bool is_grounded;
    bool jumping;
    bool sprinting;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _transform = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        distanceToGround = GetComponent<Collider>().bounds.extents.y;
        // StartCoroutine(printStates());
        CameraTransform = MainCamera.GetComponent<Transform>();
    }

    void Update()
    {
        // get keyboard inputs
        _playerInput = Input.GetAxis("Vertical");
        _rotationInput = Input.GetAxis("Horizontal");
        _userJumped = Input.GetButton("Jump");

        // play animations according to keyboard inputs
        moving_forward = Input.GetKey("w") || Input.GetKey("s") ||
                          Input.GetKey("a") || Input.GetKey("d") ||
                          Input.GetKey("up") || Input.GetKey("down");
        is_grounded = IsGrounded();
        jumping  = Input.GetKey("space");
        sprinting = Input.GetKey(KeyCode.LeftShift);
    }

    private void FixedUpdate()
    {
        // decide which direction should the character go (according to camera heading)
        _userRot = _transform.rotation.eulerAngles;
        _cameraRot = CameraTransform.rotation.eulerAngles;
        if (Input.GetKey("a")) {
            _userRot[1] = _cameraRot[1];
            _userRot += new Vector3(0, -90, 0);
            heading = _transform.forward;
        } else if (Input.GetKey("d")) {
            _userRot[1] = _cameraRot[1];
            _userRot += new Vector3(0, 90, 0);
            heading = _transform.forward;
        } else if (Input.GetKey("w")) {
            _userRot[1] = _cameraRot[1];
            heading = _transform.forward;
        } else if (Input.GetKey("s")) {
            _userRot[1] = _cameraRot[1];
            _userRot += new Vector3(0, 180, 0);
            heading = _transform.forward;
        }
        
        // rotate character to the right direction
        _transform.rotation = Quaternion.Euler(_userRot);

        //heading = new Vector3(Mathf.Abs(heading.x), Mathf.Abs(heading.y), Mathf.Abs(heading.z));
        float inputScale = 0;
        if (_playerInput != 0 && _rotationInput > 0) {
            inputScale = (Mathf.Abs(_playerInput) + Mathf.Abs(_rotationInput)) / 2.0f;
        } else if (_playerInput != 0 && _rotationInput == 0) {
            inputScale = Mathf.Abs(_playerInput);
        } else if (_playerInput == 0 && _rotationInput != 0) {
            inputScale = Mathf.Abs(_rotationInput);
        } else {
            inputScale = 0;
        }

        // let the character go forward
        if (sprinting)
        {
            _rigidbody.velocity += heading * inputScale * MoveScale * 1.5f;
        }
        else
        {
            _rigidbody.velocity += heading * inputScale * MoveScale;
        }

        // perform animations
        animator.SetBool("isWalking", moving_forward);
        animator.SetBool("isJumping", jumping);
        animator.SetBool("IsGrounded", is_grounded);
        animator.SetBool("isRunning", sprinting);
        animator.SetBool("isIdle", !moving_forward && is_grounded);

        // Only able to jump if you are on the ground
        if (is_grounded && _userJumped)
        {
          _rigidbody.AddForce(Vector3.up * JumpMultiplier, ForceMode.Impulse);
        }
    }

    IEnumerator printStates() {
        var norm = euclideanNorm(_rigidbody.velocity.x, _rigidbody.velocity.z);
        if (norm != 0)
        {
            print("walking...");
            print($"velocity: {norm}");
        }
        else
        {
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
    private bool IsGrounded()
    {
      return Physics.Raycast(transform.position, Vector3.down, distanceToGround - 0.3f);
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.collider.CompareTag("Obstacle")) {
            print("human collided with obstacle");
            StartCoroutine(Slowed());
        }
    }

    public IEnumerator Slowed() {
        print("Human will slow");
        MoveScale = 0.1f;
        print("Human slowed");
        yield return new WaitForSeconds(3);
        print("waiting");
        MoveScale = 0.5f; // original 0.5
        print("Human back to normal speed");
    }
}
