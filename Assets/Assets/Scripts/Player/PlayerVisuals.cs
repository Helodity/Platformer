using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    PlayerMovement _PlayerMovement;

    [Header("Stamina")]
    [SerializeField] Flash _LowStaminaIndicator;
    [SerializeField] float _Threshold;

    void Awake()
    {
        _PlayerMovement = GetComponent<PlayerMovement>();
        _LowStaminaIndicator.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        _LowStaminaIndicator.SetActive(_PlayerMovement.GetCurrentStamina() <= _Threshold);
    }
}
