using UnityEngine;
public class PlayerMovement : MonoBehaviour {

  Rigidbody2D _Rigidbody;

  enum Direction { Left, Right }
  Direction _Direction;

  [Header ("Environment Checks")]
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
  [Space]
  [SerializeField] Vector2 _TopLeftLeftWallJump;
  [SerializeField] Vector2 _BottomRightLeftWallJump;
  [Space]
  [SerializeField] Vector2 _TopLeftRightWallJump;
  [SerializeField] Vector2 _BottomRightRightWallJump;

  [Header ("Movement Speed")]
  [SerializeField] float _DashSpeed;
  [SerializeField] float _WalkingSpeed;
  [SerializeField] float _ClimbSpeed;

  [Header ("Acceleration")]
  [SerializeField] float _Acceleration;
  [SerializeField] float _WallJumpAcceleration;
  [SerializeField] float _ClimbAcceleration;
  [SerializeField] float _AccelerationSmoothRate;
  float _CurrentAcceleration;


  [Header("Gravity")]
  [SerializeField] float _GravityScale;

  [Header ("Dashing")]
  [SerializeField] [Range (0, 1)] float _VelocityAfterDash;
  [SerializeField] int _MaxDashes;
  [SerializeField] float _DashDuration;
  float _DashDurationR;
  [HideInInspector] public bool _IsDashing;
  int _DashesLeft;
  bool _WantToDash;
  Vector2 _DashDirection;

  [Header ("Stamina")]
  [SerializeField] float _MaxStamina;
  [SerializeField] float _StaminaRechargeRate;
  [SerializeField] float _GrabStartStaminaCost;
  [SerializeField] float _GrabJumpStaminaCost;
  float _CurrentStamina;

  [Header("Jumping")]
  [SerializeField] [Range (0, 1)] float _JumpEndVelocityMultiplier;
  [SerializeField] float _GroundJumpForce;
  [Space]
  [SerializeField] float _GrabbingJumpForce;
  [SerializeField] float _GrabbingJumpDuration;
  [Space]
  [SerializeField] Vector2 _WallJumpSlope;
  [SerializeField] float _WallJumpForce;
  [SerializeField] float _WallJumpDuration;
  [Space]
  [SerializeField] Vector2 _NoStaminaWallJumpSlope;
  [SerializeField] float _NoStaminaWallJumpForce;
  float _WallJumpDurationR;
  [SerializeField] float _JumpInputDuration;
  float _JumpInputDurationR;
  bool _ReadyToCut;

  //Grabbing misc
  bool _WantToGrab;
  bool _WantToRelease;
  bool _IsGrabbing;

  void Awake () {
    SaveSystem.Load ();

    Global._Player = gameObject;
    if (Global._HasHitCheckpoint) {
      transform.position = Camera.main.transform.position = Global._CurrentSpawn;
    }

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
    if (collision.CompareTag("Coin"))
    {
      Global._RoundCoins ++;
      collision.gameObject.SetActive(false);
    }
  }

  void HandleWalking () {
    Vector2 velocity = GetDirection () * _WalkingSpeed;
    velocity -= _Rigidbody.velocity;

    float acceleration = Mathf.Lerp(_Acceleration, _WallJumpAcceleration, _WallJumpDurationR / _AccelerationSmoothRate) * Time.deltaTime;

    velocity.x = Mathf.Clamp (velocity.x, -acceleration, acceleration);
    velocity.y = 0;

    _Rigidbody.velocity += velocity;

    if(GetDirection ().x != 0)
    {
      _Direction = (Direction)((GetDirection().x + 1) / 2);
    }
  }

  #region Jumping

  void HandleJumping () {
    if (_JumpInputDurationR > 0 && !_IsDashing && _WallJumpDurationR <= 0) {

      //The jump changes depending on multiple factors
      if (_IsGrabbing){
        Jump(JumpType.Grabbing);
      } else if(!IsGrounded() && CanWallJump() != Wall.None){
        Jump(JumpType.Wall);
      } else if(IsGrounded()){
        Jump(JumpType.Ground);
      }

    }

    if(_Rigidbody.velocity.y < 0 && _WallJumpDurationR > 0)
    {
      _WallJumpDurationR = 0;
    }

    if (_Rigidbody.velocity.y > 0 && _ReadyToCut && !(_IsDashing || (_IsGrabbing && _CurrentStamina > 0)))
    {
      _Rigidbody.velocity = new Vector2(_Rigidbody.velocity.x, _Rigidbody.velocity.y * _JumpEndVelocityMultiplier);
    }

    _ReadyToCut = false;
  }

  enum JumpType {Ground, Grabbing, Wall}
  void Jump(JumpType type)
  {
    Vector2 dir;
    _JumpInputDurationR = 0;
    // CanWallJump() has to return 1 or 2 as this jump can't be triggered when there's no wall that could be grabbed, the rest of the math formats it to -1 or 1
    int wall = ((int)CanWallJump() - 1) * 2 - 1;
    switch (type)
    {
      case JumpType.Ground:
        _Rigidbody.velocity = new Vector2(_Rigidbody.velocity.x, _GroundJumpForce);
      break;

      case JumpType.Wall:
        _WallJumpDurationR = _WallJumpDuration;
        if(_CurrentStamina > 0)
        {
          dir = new Vector2(-wall * _WallJumpSlope.x, _WallJumpSlope.y).normalized;
          _Rigidbody.velocity = dir * _WallJumpForce;
        }
        else {
          dir = new Vector2(-wall * _NoStaminaWallJumpSlope.x, _NoStaminaWallJumpSlope.y).normalized;
          _Rigidbody.velocity = dir * _NoStaminaWallJumpForce;
        }
      break;

      case JumpType.Grabbing:
        StopClimbing();
        _WallJumpDurationR = _GrabbingJumpDuration;
        _CurrentStamina -= _GrabJumpStaminaCost;
        dir = new Vector2(GetDirection().x, 1);
        if(dir.x == wall) { dir.x = 0; }
        dir = dir.normalized;
        _Rigidbody.velocity = dir * _GrabbingJumpForce;
      break;
    }
  }

  #endregion

  #region Wall Climbing

  void HandleClimbing () {
    if ((!_WantToGrab || CanGrab () == Wall.None  || _CurrentStamina <= 0) && _IsGrabbing) {
      StopClimbing();
    }

    if (_WantToGrab && !_IsGrabbing && CanGrab () != Wall.None && !_IsDashing && _CurrentStamina > 0) {
      StartClimbing ();
    }

    if (_CurrentStamina > 0 && _IsGrabbing) {
      if(Mathf.Abs(_Rigidbody.velocity.y) >= 0.5f)
      {
        _CurrentStamina -= Time.deltaTime;
      }

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
    if(!IsGrounded())
    {
      _CurrentStamina -= _GrabStartStaminaCost;
    }
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
      _DashDirection = Vector2.right * (((int)_Direction * 2) - 1);

    if (_IsGrabbing)
    {
      StopClimbing ();
    }

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

  #region Wall
  enum Wall { None, Left, Right }
  // Returns the wall the player can grab
  Wall CanGrab()
  {
    if (Physics2D.OverlapArea(_TopLeftLeftWall + (Vector2)transform.position, _BottomRightLeftWall + (Vector2)transform.position, _WhatIsGround))
    {
      return Wall.Left;
    }

    if (Physics2D.OverlapArea(_TopLeftRightWall + (Vector2)transform.position, _BottomRightRightWall + (Vector2)transform.position, _WhatIsGround))
    {
      return Wall.Right;
    }

    return Wall.None;
  }

  Wall CanWallJump()
  {
    if (Physics2D.OverlapArea(_TopLeftLeftWallJump + (Vector2)transform.position, _BottomRightLeftWallJump + (Vector2)transform.position, _WhatIsGround))
    {
      return Wall.Left;
    }

    if (Physics2D.OverlapArea(_TopLeftRightWallJump + (Vector2)transform.position, _BottomRightRightWallJump + (Vector2)transform.position, _WhatIsGround))
    {
      return Wall.Right;
    }

    return Wall.None;
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

    if (_JumpInputDurationR > 0){
      _JumpInputDurationR -= Time.deltaTime;
    }
  }

  void Land () {
    _WallJumpDurationR = 0;
    _DashesLeft = _MaxDashes;
    _CurrentStamina = _MaxStamina;
  }

  bool IsGrounded () => Physics2D.OverlapArea (_TopLeftGround + (Vector2) transform.position, _BottomRightGround + (Vector2) transform.position, _WhatIsGround);

  // Returns the direction the player is pressing, ignoring the axis
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

  #endregion

  #region Getters

  public float GetCurrentStamina () => _CurrentStamina;
  public float GetMaxStamina () => _MaxStamina;

  #endregion
}