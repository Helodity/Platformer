using UnityEngine;

public class Coin : MonoBehaviour {
  [SerializeField] int _Amount;

  private void OnTriggerEnter2D (Collider2D collision) {
    if (collision.gameObject == Global._Player) {
      Global._RoundCoins += _Amount;
      gameObject.SetActive (false);
    }
  }
}