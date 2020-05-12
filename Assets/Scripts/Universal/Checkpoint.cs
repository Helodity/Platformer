using UnityEngine;

public class Checkpoint : MonoBehaviour
{
  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.CompareTag("Player"))
    {
      Debug.Log("Set spawn");
      Global._CurrentSpawn = transform.position;
    }
  }
}
