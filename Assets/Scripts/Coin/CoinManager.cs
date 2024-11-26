using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }

    private int cointAmount = 0;

    [SerializeField] Text coinText;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    public void AddCoin()
    {
        cointAmount++;
        UpdateCoinText();
    }
    private void UpdateCoinText()
    {
        coinText.text = "Coins: " + cointAmount;
    }
}
