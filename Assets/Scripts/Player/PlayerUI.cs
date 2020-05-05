using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("Stamina")]
    [SerializeField] PlayerMovement _Player;
    [SerializeField] [Range(0, 1)] float _StaminaLerpSpeed;
    [SerializeField] Image _StaminaBar;

    [Header("Coin Counter")] 
    [SerializeField] Text _CoinText;

    [Header("Timer")]
    [SerializeField] Text _TimerText;
    float _Timer;

    void FixedUpdate()
    {
        _StaminaBar.fillAmount = Mathf.Lerp(_StaminaBar.fillAmount, _Player.GetCurrentStamina() / _Player.GetMaxStamina(), _StaminaLerpSpeed);

        _CoinText.text = Global.GetTotalCoins().ToString();

        _Timer += Time.deltaTime;
        _TimerText.text = "Time: " +MathUtils.RoundToPlace(_Timer, 1);
    }
}
