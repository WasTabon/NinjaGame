using TMPro;
using UnityEngine;

public class WalletController : MonoBehaviour
{
    public static WalletController Instance;

    public TextMeshProUGUI coinText;
    
    private int _coin;

    public int Coin
    {
        get => _coin;
        set
        {
            _coin = value;
            coinText.text = _coin.ToString();
            PlayerPrefs.SetInt("Coin", _coin);
            PlayerPrefs.Save();
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _coin = PlayerPrefs.GetInt("Coin", 0);
    }
}   