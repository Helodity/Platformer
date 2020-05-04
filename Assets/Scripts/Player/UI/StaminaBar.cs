using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    Image _Bar;
    [SerializeField] PlayerMovement _Player;
    [SerializeField] [Range(0, 1)] float _LerpSpeed;
    void Awake()
    {
        _Bar = GetComponent<Image>();
    }
    void FixedUpdate()
    {
        _Bar.fillAmount = Mathf.Lerp(_Bar.fillAmount, _Player.GetCurrentStamina() / _Player.GetMaxStamina(), _LerpSpeed);
    }
}
