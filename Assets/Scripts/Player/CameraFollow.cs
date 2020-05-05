using UnityEngine;

public class CameraFollow : MonoBehaviour {
  [SerializeField] PlayerMovement _Player;

  [Header ("Positioning")]
  [SerializeField][Range (0, 1)] float _PositionLerpSpeed;

  [Header ("Size")]
  [SerializeField] float _StandardCameraSize;
  [SerializeField] float _DashingCameraSize;
  [SerializeField][Range (0, 1)] float _SizeLerpSpeed;
  float _TargetCameraSize;

  Camera _Camera;

  private void Awake () {
    _Camera = GetComponent<Camera> ();
  }

  void FixedUpdate () {
    // Follows the player's position
    Vector3 targetPosition = _Player.transform.position;
    targetPosition.z = -10;
    transform.position = Vector3.Lerp (transform.position, targetPosition, _PositionLerpSpeed);

    //Increases the FOV if the player is dashing
    _TargetCameraSize = _Player._IsDashing ? _DashingCameraSize : _StandardCameraSize;
    _Camera.orthographicSize = Mathf.Lerp (_Camera.orthographicSize, _TargetCameraSize, _SizeLerpSpeed);
  }
}