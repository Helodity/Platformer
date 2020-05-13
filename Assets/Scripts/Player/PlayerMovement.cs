using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerMovement : MonoBehaviour {

  /*TODO
   *
   */

  //General parts used by multiple components
  Rigidbody2D _Rigidbody;

  [Header ("Ground and Wall Checks")]
  [SerializeField] LayerMask _WhatIsGround;
  [Space]
  [SerializeField] Vector2 _TopLeftGround;
  [SerializeField] Vector2 _BottomRightGround;
  [Space]
  [SerializeField] Vector2 _TopLeftLeftWall;
  [SerializeField] Vector2 _BottomRightLeftWall;
  [Space]
  [SerializeField] Vector2 _TopLeftRightWall;
  [SerializeField] Vector2 _BottomRightRightWall;

  [Header ("Movement Speed")]
  [SerializeField] float _DashSpeed;
  [SerializeField] float _MovementSpeed;
  [SerializeField] float _ClimbSpeed;

  [Header ("Acceleration")]
  [SerializeField] float _Acceleration;
  [SerializeField] float _WallJumpAcceleration;
  [SerializeField] float _ClimbAcceleration;

  [Header("Gravity")]
  [SerializeField] float _GravityScale;

  [Header ("Timers")]
  [SerializeField] float _WallJumpDuration;
  float _WallJumpDurationR;
  [SerializeField] float _DashDuration;
  float _DashDurationR;

  [Header ("Dashing")]
  [SerializeField] [Range (0, 1)] float _VelocityAfterDash;
  [SerializeField] int _MaxDashes;
  [HideInInspector] public bool _IsDashing;
  int _DashesLeft;
  bool _WantToDash;
  Vector2 _DashDirection;

  [Header ("Stamina")]
  [SerializeField] float _MaxStamina;
  [SerializeField] float _StaminaRechargeRate;
  [SerializeField] float _GrabStartStaminaCost;
  [SerializeField] float _WallJumpStaminaCost;
  float _CurrentStamina;

  [Header("Jumping")]
  [SerializeField] [Range (0, 1)] float _JumpEndVelocityMultiplier;
  [SerializeField] float _JumpForce;
  [SerializeField] float _WallJumpForce;
  [SerializeField] float _JumpInputDuration;
  float _JumpInputDurationR;
  bool _ReadyToCut;

  //Grabbing misc
  bool _WantToGrab;
  bool _WantToRelease;
  bool _IsGrabbing;

  void Awake () {
    SaveSystem.Load();
    Global._Player = gameObject;
    transform.position = Camera.main.transform.position = Global._CurrentSpawn;
    _Rigidbody = GetComponent<Rigidbody2D> ();
    _Rigidbody.gravityScale = _GravityScale;

    _CurrentStamina = _MaxStamina;
  }

  void Update () {
    if (Input.GetKeyDown (PlayerStats._Controls[(int) PlayerStats.PlayerControls.Jump])) {
      _JumpInputDurationR = _JumpInputDuration;
    }

    if (Input.GetKeyDown (PlayerStats._Controls[(int) PlayerStats.PlayerControls.Dash]) && _DashesLeft > 0) {
      _WantToDash = true;
    }

    if (Input.GetKeyUp(PlayerStats._Controls[(int)PlayerStats.PlayerControls.Jump]))
    {
      _ReadyToCut = true;
    }


    _WantToGrab = _WallJumpDurationR <= 0 && Input.GetKey (PlayerStats._Controls[(int) PlayerStats.PlayerControls.Grab]);

    DecrementTimers ();
  }

  void FixedUpdate () {
    HandleClimbing ();
    HandleDash ();
    HandleJumping ();

    if (!_IsGrabbing && !_IsDashing) {
      HandleWalking ();
    }

    if (!_IsDashing && !_IsGrabbing && IsGrounded ()) {
      Land ();
    }
  }

  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.CompareTag("Death"))
    {
      Die ();
    }

    if (collision.CompareTag("Coin"))
    {
      Global._RoundCoins ++;
      collision.gameObject.SetActive(false);
    }
  }

  void HandleWalking () {
    Vector2 velocity = GetDirection () * _MovementSpeed;
    velocity -= _Rigidbody.velocity;

    float acceleration = _WallJumpDurationR <= 0 ? _Acceleration : _WallJumpAcceleration;

    velocity.x = Mathf.Clamp (velocity.x, -acceleration, acceleration);
    velocity.y = 0;

    _Rigidbody.velocity += velocity;
  }

  void HandleJumping () {
    if (_JumpInputDurationR > 0 && (IsGrounded() || _IsGrabbing)) {
      //Store _IsGrabbing as we want to know whether we're doing a wall jump, and StopClimbing() sets _IsGrabbing to false
      bool wallJump = _IsGrabbing;

      if (_IsGrabbing) {
        StopClimbing ();
      }

      //The jump changes depending on whether you're wall jumping
      if (wallJump)
      {
        _WallJumpDurationR = _WallJumpDuration;
        _CurrentStamina -= _WallJumpStaminaCost;

        Vector2 dir = (GetDirection() + Vector2.up).normalized;
        _Rigidbody.velocity = dir * _WallJumpForce;
        return;
      }

      _Rigidbody.velocity = new Vector2(_Rigidbody.velocity.x, _JumpForce);

      _JumpInputDurationR = 0;
    }

    if (_Rigidbody.velocity.y > 0 && _ReadyToCut && !(_IsDashing || (_IsGrabbing && _CurrentStamina > 0)))
    {
      _Rigidbody.velocity = new Vector2(_Rigidbody.velocity.x, _Rigidbody.velocity.y * _JumpEndVelocityMultiplier);
    }

    _ReadyToCut = false;
  }

  #region Wall Climbing

  void HandleClimbing () {
    if ((!_WantToGrab || CanGrab () == Wall.None || IsGrounded () || _CurrentStamina <= 0) && _IsGrabbing) {
      StopClimbing();
    }

    if (_WantToGrab && !_IsGrabbing && CanGrab () != Wall.None && !IsGrounded () && !_IsDashing && _CurrentStamina > 0) {
      StartClimbing ();
    }

    if (_CurrentStamina > 0 && _IsGrabbing) {

      _CurrentStamina -= Time.deltaTime;

      Vector2 velocity = GetDirection () * _ClimbSpeed;
      velocity -= _Rigidbody.velocity;

      velocity.y = Mathf.Clamp (velocity.y, -_ClimbAcceleration, _ClimbAcceleration);
      velocity.x = 0;

      _Rigidbody.velocity += velocity;
    }
  }

  void StartClimbing () {
    _IsGrabbing = true;
    _Rigidbody.gravityScale = 0;
    _CurrentStamina -= _GrabStartStaminaCost;
  }

  void StopClimbing () {
    _IsGrabbing = false;
    _Rigidbody.gravityScale = _GravityScale;
  }

  #endregion

  #region Dashing
  void HandleDash () {
    if (_DashDurationR <= 0 && _IsDashing) {
      EndDash ();
      return;
    }

    if (_IsDashing) {
      _Rigidbody.velocity = _DashDirection * _DashSpeed;
      return;
    }

    if (_WantToDash && _DashesLeft > 0) {
      StartDash ();
    }
  }

  void StartDash () {
    _WantToDash = false;

    _DashDirection = GetDirection ().normalized;

    //Prevent the player from dashing in place
    if (_DashDirection == Vector2.zero)
      return;

    _DashesLeft--;
    _DashDurationR = _DashDuration;
    _Rigidbody.gravityScale = 0;
    _Rigidbody.velocity = Vector2.zero;
    _IsDashing = true;
  }

  void EndDash () {
    _IsDashing = false;
    _Rigidbody.gravityScale = _GravityScale;
    _Rigidbody.velocity *= _VelocityAfterDash;
  }

  #endregion

  #region Misc
  void DecrementTimers () {
    if (_DashDurationR > 0) {
      _DashDurationR -= Time.deltaTime;
    }

    if (_WallJumpDurationR > 0) {
      _WallJumpDurationR -= Time.deltaTime;
    }

    if(_JumpInputDurationR > 0){
      _JumpInputDurationR -= Time.deltaTime;
    }
  }

  void Land () {
    _WallJumpDurationR = 0;
    _DashesLeft = _MaxDashes;
    if (_CurrentStamina < _MaxStamina) {
      _CurrentStamina += _StaminaRechargeRate * Time.deltaTime;
    } else {
      _CurrentStamina = _MaxStamina;
    }
  }

  bool IsGrounded () => Physics2D.OverlapArea (_TopLeftGround + (Vector2) transform.position, _BottomRightGround + (Vector2) transform.position, _WhatIsGround);

  enum Wall { None, Left, Right }
  Wall CanGrab () {
    if (Physics2D.OverlapArea (_TopLeftLeftWall + (Vector2) transform.position, _BottomRightLeftWall + (Vector2) transform.position, _WhatIsGround)) {
      return Wall.Left;
    }

    if (Physics2D.OverlapArea (_TopLeftRightWall + (Vector2) transform.position, _BottomRightRightWall + (Vector2) transform.position, _WhatIsGround)) {
      return Wall.Right;
    }

    return Wall.None;
  }

  //Returns the direction the player is pressing, ignoring the axis
  Vector2 GetDirection () {
    Vector2 output = Vector2.zero;

    if (Input.GetKey (PlayerStats._Controls[(int) PlayerStats.PlayerControls.Left])) {
      output.x--;
    }
    if (Input.GetKey (PlayerStats._Controls[(int) PlayerStats.PlayerControls.Right])) {
      output.x++;
    }

    if (Input.GetKey (PlayerStats._Controls[(int) PlayerStats.PlayerControls.Down])) {
      output.y--;
    }
    if (Input.GetKey (PlayerStats._Controls[(int) PlayerStats.PlayerControls.Up])) {
      output.y++;
    }

    return output;
  }

  public void Die()
  {
    //Todo: add effects
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    Global._RoundCoins = 0;
  }

  #endregion

  #region Getters

  public float GetCurrentStamina () => _CurrentStamina;
  public float GetMaxStamina () => _MaxStamina;

  #endregion
}