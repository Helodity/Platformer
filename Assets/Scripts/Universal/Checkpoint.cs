using UnityEngine;

public class Checkpoint : MonoBehaviour
{
  [SerializeField] Sprite _CurrentSprite;
  [SerializeField] Sprite _UnusedSprite;
  SpriteRenderer _Renderer;

  private void Awake()
  {
    _Renderer = GetComponent<SpriteRenderer>();
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
      Global._CurrentSpawn = transform.position;
      _Renderer.sprite = _CurrentSprite;
    }
  }
}
