using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flash : MonoBehaviour
{
    [SerializeField] Color _Color1;
    [SerializeField] Color _Color2;
    [SerializeField] float _FlashSpeed;
    float _TimeTillSwitch;
    SpriteRenderer _Renderer;
    bool _Active;

    private void Awake()
    {
        _Renderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (_Active)
        {
            _TimeTillSwitch -= Time.deltaTime;

            if (_TimeTillSwitch <= 0)
            {
                _TimeTillSwitch = _FlashSpeed;
                if (_Renderer.color == _Color1)
                    _Renderer.color = _Color2;
                else
                    _Renderer.color = _Color1;
            }

        }
        else
        {
            _Renderer.color = _Color1;
        }
    }

    public void SetActive(bool active) => _Active = active;


}
