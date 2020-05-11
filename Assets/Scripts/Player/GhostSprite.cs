using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostSprite : MonoBehaviour
{
  SpriteRenderer _Renderer;

  [SerializeField] Color _MainColor;
  Color _FadedOutColor;
  [SerializeField] [Range(0, 1)] float _FadeOutLerp;
 
  void Awake()
  {
    Debug.Log("setup");
    _Renderer = GetComponent<SpriteRenderer>();
    _FadedOutColor = _MainColor;
    _FadedOutColor.a = 0;

    transform.position = transform.parent.position;
  }

  void FixedUpdate()
  {
    _Renderer.color = Color.Lerp(_Renderer.color, _FadedOutColor, _FadeOutLerp);

    if(_Renderer.color.a < 0.1f)
    {
      gameObject.SetActive(false);
    }

  }

  private void OnEnable()
  {
    _Renderer.color = _MainColor;
  }
}
