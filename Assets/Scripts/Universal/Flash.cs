using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flash : MonoBehaviour {
  SpriteRenderer _Renderer;

  [Header ("Colors")]
  [SerializeField] Color _Color1;
  [SerializeField] Color _Color2;
  Color _CurrentColor;

  [Header ("Settings")]
  [SerializeField] float _FlashSpeed;

  float _TimeTillSwitch;
  bool _IsActive;

  private void Awake () {
    _Renderer = GetComponent<SpriteRenderer> ();
  }

  void Update () {
    if (_IsActive) {
      _TimeTillSwitch -= Time.deltaTime;

      if (_TimeTillSwitch <= 0) {
        _TimeTillSwitch = _FlashSpeed;
        if (_CurrentColor == _Color1) {
          _CurrentColor = _Color2;
        } else {
          _CurrentColor = _Color1;
        }
        _Renderer.color = _CurrentColor;
      }

    }
  }

  public void SetActive (bool active) {
    _IsActive = active;
    if (!_IsActive) {
      _Renderer.color = _Color1;
    }
  }
}