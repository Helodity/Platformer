using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    /*TODO
     * Find some better organization for variables
     * Make Stamina not refill instantly
    */

    //General parts used by multiple components
    Rigidbody2D _Rigidbody;

    [Header("Ground")]
    [SerializeField] LayerMask _WhatIsGround;
    [SerializeField] Vector2 _TopLeftGround;
    [SerializeField] Vector2 _BottomRightGround;

    [Header("Walking")]
    [SerializeField] float _MovementSpeed;
    [SerializeField] float _Acceleration;
    [SerializeField] float _WallJumpAcceleration;

    [Header("Jumping")]
    [SerializeField] KeyCode _JumpKey = KeyCode.Space;
    [SerializeField] float _JumpForce;
    [SerializeField] float _FallMultiplier;
    [SerializeField] float _LowJumpMultiplier;

    // Wall Jumping
    [SerializeField] float _WallJumpStaminaDrain;
    [SerializeField] float _WallJumpTime;
    float _WallJumpTimeLeft;
    bool _WantToJump;

    [Header("Dashing")]
    [SerializeField] float _DashSpeed;
    [SerializeField] [Range(0, 1)] float _VelocityAfterDash;
    [SerializeField] float _DashLength;
    [SerializeField] int _MaxDashes;
    [SerializeField] KeyCode _DashKey;
    bool _WantToDash;
    [HideInInspector] public bool _IsDashing;
    float _DashTimeLeft;
    int _DashesLeft;
    Vector2 _DashDirection;

    [Header("Wall Climb")]
    [SerializeField] KeyCode _GrabKey = KeyCode.K;
    [SerializeField] float _MaxStamina;
    [SerializeField] float _ClimbSpeed;
    [SerializeField] float _ClimbAcceleration;
    [SerializeField] float _WallCircleRadii;
    [SerializeField] Vector3 _LeftGrabCircleOffset;
    [SerializeField] Vector3 _RightGrabCircleOffset;
    bool _WantToGrab;
    bool _WantToRelease;
    bool _IsGrabbing;
    float _CurrentStamina;

    void Awake() {
        _Rigidbody = GetComponent<Rigidbody2D>();

        _CurrentStamina = _MaxStamina;
    }

    void Update() {
        if (Input.GetKeyDown(_JumpKey) && (IsGrounded() || _IsGrabbing)) {
            _WantToJump = true;
        }

        if (Input.GetKeyDown(_DashKey) && _DashesLeft > 0) {
            _WantToDash = true;
        }

        _WantToGrab = (_WallJumpTimeLeft <= _WallJumpTime - 0.5f) && Input.GetKey(_GrabKey);

        DecrementTimers();
    }

    void FixedUpdate() {
        HandleClimbing();
        HandleDash();
        HandleJumping();

        if (!_IsGrabbing && !_IsDashing) {
            HandleWalking();
        }

        if (!_IsDashing && !_IsGrabbing && IsGrounded()) {
            Land();
        }
    }

    void HandleWalking() {
        Vector2 targetVelocity = GetDirection() * _MovementSpeed;
        Vector2 velocityChange = targetVelocity - _Rigidbody.velocity;

        float acceleration = _WallJumpTimeLeft <= 0 ? _Acceleration : _WallJumpAcceleration;

        velocityChange.x = Mathf.Clamp(velocityChange.x, -acceleration, acceleration);
        velocityChange.y = 0;

        _Rigidbody.velocity += velocityChange;
    }

    void HandleJumping() {
        if (_WantToJump) {
            //Store _IsGrabbing as we want to know whether we're doing a wall jump, and StopClimbing() sets _IsGrabbing to false
            bool wallJump = _IsGrabbing;

            if (_IsGrabbing) {
                StopClimbing();
            }

            Jump(wallJump);

            _WantToJump = false;
        }

        // Dashing and grabbing disable gravity, so we need to skip setting the gravity scale
        if (_IsDashing || (_IsGrabbing && _CurrentStamina > 0))
            return;

        // Increase gravity if not jumping or while falling to reduce floatiness
        if (_Rigidbody.velocity.y < 0)
            _Rigidbody.gravityScale = _FallMultiplier;
        else if (_Rigidbody.velocity.y > 0 && !Input.GetKey(_JumpKey)) {
            _Rigidbody.gravityScale = _LowJumpMultiplier;
        } else {
            _Rigidbody.gravityScale = 1;
        }
    }

    void Jump(bool offWall) {
        if (offWall) {
            _WallJumpTimeLeft = _WallJumpTime;
            _CurrentStamina -= _WallJumpStaminaDrain;

            Vector2 dir = (GetDirection() + Vector2.up).normalized;
            _Rigidbody.AddForce(dir * _JumpForce, ForceMode2D.Impulse);
            return;
        }
        //If the player is falling, add extra force so we get the same lift
        float fallCounter = _Rigidbody.velocity.y < 0 ? -_Rigidbody.velocity.y : 0;
        _Rigidbody.AddForce(Vector2.up * (_JumpForce + fallCounter), ForceMode2D.Impulse);
    }

    #region Wall Climbing

    void HandleClimbing() {
        if (!_WantToGrab || CanGrab() == Wall.None || IsGrounded()) {
            StopClimbing();
            return;
        }

        if (_WantToGrab && !_IsGrabbing && CanGrab() != Wall.None && !IsGrounded()) {
            StartClimbing();
        }

        if (_CurrentStamina > 0) {
            _CurrentStamina -= Time.deltaTime;
            Vector2 targetVelocity = GetDirection() * _ClimbSpeed;
            Vector2 velocityChange = targetVelocity - _Rigidbody.velocity;

            velocityChange.y = Mathf.Clamp(velocityChange.y, -_ClimbAcceleration, _ClimbAcceleration);
            velocityChange.x = 0;

            _Rigidbody.velocity += velocityChange;
            return;
        }

        _Rigidbody.gravityScale = 1;
    }

    void StartClimbing() {
        _IsGrabbing = true;
        _Rigidbody.gravityScale = 0;
    }

    void StopClimbing() {
        _IsGrabbing = false;
        _Rigidbody.gravityScale = 1;
    }

    #endregion

    #region Dashing
    void HandleDash() {
        if (_DashTimeLeft <= 0 && _IsDashing) {
            EndDash();
            return;
        }

        if (_IsDashing) {
            _Rigidbody.velocity = _DashDirection * _DashSpeed;
            return;
        }

        if (_WantToDash && _DashesLeft > 0) {
            StartDash();
        }
    }

    void StartDash() {
        _DashDirection = GetDirection().normalized;
        _WantToDash = false;

        //Prevent the player from dashing in place
        if (_DashDirection == Vector2.zero)
            return;

        _DashesLeft--;
        _DashTimeLeft = _DashLength;
        _IsDashing = true;

        _Rigidbody.gravityScale = 0;
        _Rigidbody.velocity = Vector2.zero;
    }

    void EndDash() {
        _IsDashing = false;
        _Rigidbody.gravityScale = 1;
        _Rigidbody.velocity *= _VelocityAfterDash;
    }

    #endregion

    #region misc

    void DecrementTimers() {
        if (_DashTimeLeft > 0) {
            _DashTimeLeft -= Time.deltaTime;
        }

        if (_WallJumpTimeLeft > 0) {
            _WallJumpTimeLeft -= Time.deltaTime;
        }
    }

    void Land() {
        _WallJumpTimeLeft = 0;
        _DashesLeft = _MaxDashes;
        _CurrentStamina = _MaxStamina;
    }

    bool IsGrounded() {
        return Physics2D.OverlapArea(_TopLeftGround + (Vector2)transform.position, _BottomRightGround + (Vector2)transform.position, _WhatIsGround);
    }

  enum Wall{ None, Left, Right }

  Wall CanGrab() {
    if (Physics2D.OverlapCircle(transform.position + _LeftGrabCircleOffset, _WallCircleRadii, _WhatIsGround)) {
            return Wall.Left;
    }

    if (Physics2D.OverlapCircle(transform.position + _RightGrabCircleOffset, _WallCircleRadii, _WhatIsGround)) {
      return Wall.Right;
        }

    return Wall.None;
  }

  //Returns the direction the player is pressing, ignoring the axis
  Vector2 GetDirection() {
    Vector2 output = Vector2.zero;
    if (Input.GetKey (KeyCode.A))
      output.x--;
    if (Input.GetKey (KeyCode.D))
      output.x++;

    if (Input.GetKey (KeyCode.S))
      output.y--;
    if (Input.GetKey (KeyCode.W))
      output.y++;

    return output;
  }

  #endregion

  #region Getters

  public float GetCurrentStamina() { return _CurrentStamina; }
  public float GetMaxStamina() { return _MaxStamina; }
    #endregion
}