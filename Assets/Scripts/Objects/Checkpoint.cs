using UnityEngine;

public class Checkpoint : MonoBehaviour
{
  [SerializeField] Sprite _CurrentSprite;
  [SerializeField] Sprite _UnusedSprite;
  ParticleSystem _Particles;
  SpriteRenderer _Renderer;

  private void Awake()
  {
    _Renderer = GetComponent<SpriteRenderer> ();
    _Particles = GetComponent<ParticleSystem> ();
  }

  private void Update()
  {
    if(Global._CurrentSpawn != transform.position)
    {
      _Renderer.sprite = _UnusedSprite;
    }
  }

  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.CompareTag("Player"))
    {
      if(_Renderer.sprite != _CurrentSprite)
      {
        Global._CurrentSpawn = transform.position;
        _Renderer.sprite = _CurrentSprite;
        _Particles.Play();
      }
    }
  }
}
