using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {
  [Header ("Stamina")]
  [SerializeField] PlayerMovement _Player;
  [SerializeField][Range (0, 1)] float _StaminaLerpSpeed;
  [SerializeField] Image _StaminaBar;

  [Header ("Coin Counter")]
  [SerializeField] Text _CoinText;

  [Header ("Timer")]
  [SerializeField] Text _TimerText;
  float _Timer;
  bool _TimerPaused = false;

  void FixedUpdate () {
    _StaminaBar.fillAmount = Mathf.Lerp (_StaminaBar.fillAmount, _Player.GetCurrentStamina () / _Player.GetMaxStamina (), _StaminaLerpSpeed);

    _CoinText.text = Global.GetTotalCoins ().ToString ();

    if (!_TimerPaused) {
      _Timer += Time.deltaTime;
      _TimerText.text = "Time: " + MathUtils.RoundToPlace (_Timer, 2);
    }
  }

  #region Timer UI Functions
  public void StartTimer () {
    _TimerPaused = false;
  }
  public void PauseTimer () {
    _TimerPaused = true;
  }
  public void StopTimer () {
    _Timer = 0;
    _TimerPaused = true;
  }
  #endregion
}