using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {
  [Header ("Stamina")]
  [SerializeField] PlayerMovement _Player;
  [SerializeField][Range (0, 1)] float _StaminaLerpSpeed;
  [SerializeField] Image _StaminaBar;
  [SerializeField] Gradient _StaminaColor;

  [Header ("Coin Counter")]
  [SerializeField] Text _CoinText;

  [Header ("Timer")]
  [SerializeField] Text _TimerText;
  float _Timer;
  bool _TimerPaused = false;

  void FixedUpdate () {
    _StaminaBar.fillAmount = Mathf.Lerp (_StaminaBar.fillAmount, _Player.GetCurrentStamina () / _Player.GetMaxStamina (), _StaminaLerpSpeed);
    _StaminaBar.color = _StaminaColor.Evaluate(_Player.GetCurrentStamina() / _Player.GetMaxStamina());

    _CoinText.text = Global.GetTotalCoins ().ToString ();

    if (!_TimerPaused) {
      _Timer += Time.deltaTime;

      float seconds = MathUtils.FloorToPlace(_Timer, 0);
      int miliseconds = (int)((MathUtils.FloorToPlace(_Timer, 2) - seconds) * 100);
      int startZeros = 0;

      if (miliseconds < 10)
        startZeros = 1;

      string start = "";
      for (int i = 0; i < startZeros; i++) { start += "0"; }

      _TimerText.text = "Time: " + seconds + ":" + start + miliseconds;
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