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
        _DashParticles = GetComponent<ParticleSystem>();
        _DashParticles.Stop();

        _PlayerMovement = GetComponent<PlayerMovement>();
        _LowStaminaIndicator.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //Makes the low stamina overlay flash if the player's stamina is below the threshold
        _LowStaminaIndicator.SetActive(_PlayerMovement.GetCurrentStamina() <= _LowStaminaThreshold);

        //Plays and stops the dash particles
        if (_IsDashing != _PlayerMovement._IsDashing)
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
