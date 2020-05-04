using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    // Universal
    PlayerMovement _PlayerMovement;

    [Header("Stamina")]
    [SerializeField] Flash _LowStaminaIndicator;
    [SerializeField] float _LowStaminaThreshold;

    // Dashing
    ParticleSystem _DashParticles;
    bool _IsDashing = false;

    void Awake()
    {
        _PlayerMovement = GetComponent<PlayerMovement>();
        _LowStaminaIndicator.SetActive(false);

        _DashParticles = GetComponent<ParticleSystem>();
        _DashParticles.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        LowStamina();
        DashParticles();
    }

    void LowStamina()
    {
        _LowStaminaIndicator.SetActive(_PlayerMovement.GetCurrentStamina() <= _LowStaminaThreshold);
    }

    void DashParticles()
    {
        if(_IsDashing != _PlayerMovement._IsDashing)
        {
            _IsDashing = !_IsDashing;

            if (_IsDashing)
            {
                _DashParticles.Play();
            }
            else
            {
                _DashParticles.Stop();
            }
        }
    }
}
