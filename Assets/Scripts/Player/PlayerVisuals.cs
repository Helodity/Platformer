using UnityEngine;

public class PlayerVisuals : MonoBehaviour {
  // Universal
  PlayerMovement _PlayerMovement;
  SpriteRenderer _PlayerRenderer;

  [Header ("Stamina")]
  [SerializeField] Flash _LowStaminaIndicator;
  [SerializeField] float _LowStaminaThreshold;


  [Header("Ghost Sprites")]
  [SerializeField] GameObject _GhostSpritePrefab;
  SpriteRenderer[] _GhostSprites;
  [SerializeField] int _GhostSpriteAmount;
  int _CurrentGhostSprite;
  [SerializeField] float _GhostSpriteSpawnRate;
  float _GhostSpriteSpawnRateR;

  // Dash particles
  ParticleSystem _DashParticles;
  bool _IsDashing = false;

  void Start () {
    _DashParticles = GetComponent<ParticleSystem> ();
    _DashParticles.Stop ();

    _PlayerMovement = GetComponent<PlayerMovement> ();
    _PlayerRenderer = GetComponent<SpriteRenderer> ();
    _LowStaminaIndicator.SetActive (false);

    _GhostSprites = new SpriteRenderer[_GhostSpriteAmount];
    for(int i = 0; i < _GhostSpriteAmount; i++)
    {
      _GhostSprites[i] = Instantiate (_GhostSpritePrefab, transform.parent).GetComponent<SpriteRenderer> ();
      _GhostSprites[i].gameObject.SetActive (false);
    }
  }

  // Update is called once per frame
  void Update () {
    //Makes the low stamina overlay flash if the player's stamina is below the threshold
    _LowStaminaIndicator.SetActive (_PlayerMovement.GetCurrentStamina () <= _LowStaminaThreshold);

    //Plays and stops the dash particles
    if (_IsDashing != _PlayerMovement._IsDashing) {
      _IsDashing = !_IsDashing;
      _GhostSpriteSpawnRateR = _GhostSpriteSpawnRate;

      if (_IsDashing) {
        _DashParticles.Play ();
      } else {
        _DashParticles.Stop ();
      }
    }

    //Shows the ghost sprites that fade out
    if (_IsDashing)
    {
      _GhostSpriteSpawnRateR -= Time.deltaTime;

      if(_GhostSpriteSpawnRateR <= 0)
      {
        _GhostSpriteSpawnRateR = _GhostSpriteSpawnRate;
        _CurrentGhostSprite++;

        if(_CurrentGhostSprite >= _GhostSpriteAmount) {  _CurrentGhostSprite = 0; }

        _GhostSprites[_CurrentGhostSprite].gameObject.SetActive (true);
        _GhostSprites[_CurrentGhostSprite].transform.position = _PlayerMovement.gameObject.transform.position;
        _GhostSprites[_CurrentGhostSprite].sprite = _PlayerRenderer.sprite;
      }
    }
  }
}