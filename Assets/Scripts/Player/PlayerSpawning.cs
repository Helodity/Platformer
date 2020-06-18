using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawning : MonoBehaviour
{
  void Awake()
  {
    if (Global._HasHitCheckpoint) {
      transform.position = Camera.main.transform.position = Global._CurrentSpawn;
    }
  }
  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.CompareTag("Death"))
    {
      Die();
    }
  }
  public void Die()
  {
    //Todo: add effects
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    Global._RoundCoins = 0;
  }

}
