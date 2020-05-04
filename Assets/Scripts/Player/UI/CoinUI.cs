using UnityEngine.UI;
using UnityEngine;

public class CoinUI : MonoBehaviour
{
    Text _Text;

    private void Awake()
    {
        _Text = GetComponent<Text>();
    }

    private void Update()
    {
        _Text.text = Global.GetTotalCoins().ToString();
    }
}
