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
  [SerializeField] [Range(0, 50)] float _TimerSpeed = 1;
  float _Timer;
  bool _TimerPaused = false;

  void FixedUpdate () {
    _StaminaBar.fillAmount = Mathf.Lerp (_StaminaBar.fillAmount, _Player.GetCurrentStamina () / _Player.GetMaxStamina (), _StaminaLerpSpeed);
    _StaminaBar.color = _StaminaColor.Evaluate(_Player.GetCurrentStamina() / _Player.GetMaxStamina());

    _CoinText.text = Global.GetTotalCoins ().ToString ();

    if (!_TimerPaused) {
      UpdateTimer ();
    }
  }

  #region Timer UI Functions
  void UpdateTimer()
  {
    _Timer += Time.deltaTime * _TimerSpeed;

    int minutes = (int)MathUtils.FloorToPlace(_Timer, 0) / 60;
    string minuteZeros = "";

    int seconds = (int)MathUtils.FloorToPlace(_Timer, 0) - (minutes * 60);
    string secondZeros = "";

    int miliseconds = (int)((MathUtils.FloorToPlace(_Timer, 2) - MathUtils.FloorToPlace(_Timer, 0)) * 100);
    string miliZeros = "";

    if (minutes < 10)
    {
      minuteZeros = "0";
    }

    if (seconds < 10)
    {
      secondZeros = "0";
    }

    if (miliseconds < 10)
    {
      miliZeros = "0";
    }

    _TimerText.text = "Time: " + minuteZeros + minutes + ":" + secondZeros + seconds + ":" + miliZeros + miliseconds;
  }



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