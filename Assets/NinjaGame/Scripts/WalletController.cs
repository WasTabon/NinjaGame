using System;
using TMPro;
using UnityEngine;

public class WalletController : MonoBehaviour
{
    public static WalletController Instance;

    public TextMeshProUGUI _cointText;
    
    public ArcJumpCurve2D player;

    public GameObject buyPanel;
    
    public GameObject revivePanel;
    public GameObject losePanel;
    
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

    private void Update()
    {
        if (coinText.gameObject.activeSelf)
        {
            coinText.text = $"{_coin.ToString()} <sprite=0>";
        }
    }

    public void BuyRevive()
    {
        if (_coin >= 50)
        {
            _coin -= 50;
            player.Revive();
            losePanel.SetActive(false);
            revivePanel.SetActive(false);
        }
        else
        {
            buyPanel.SetActive(true);
        }
    }
}   