using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] PlayerMovement _Player;

    [Header("Settings")]
    [SerializeField] [Range(0, 1)] float _LerpSpeed;

    Image _Bar;
    void Awake()
    {
        _Bar = GetComponent<Image>();
    }
    void FixedUpdate()
    {
        _Bar.fillAmount = Mathf.Lerp(_Bar.fillAmount, _Player.GetCurrentStamina() / _Player.GetMaxStamina(), _LerpSpeed);
    }
}
