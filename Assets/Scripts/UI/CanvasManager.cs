using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager Instance { get; private set; }

    [SerializeField] TMP_Text distanceText;

    [SerializeField] TMP_Text coinText;

    [SerializeField] Slider boostSlider;

    [SerializeField] RectTransform needle;
    private float maxNeedleRotation = -40f;
    private float minNeedleRotation = 215f;

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
            DontDestroyOnLoad(Instance);
        }
    }

    //display coin
    public void UpdateCoinText(int coinAmount)
    {
        coinText.text = "Coins: " + coinAmount;
    }

    //display distance
    public void UpdateDistanceText(float distance)
    {
        distanceText.text = $"Distance traveled: {distance:F1} m";
    }

    //set boost slider
    public void SetMaxBoost(float maxEnergy)
    {
        if (boostSlider != null)
        {
            boostSlider.maxValue = maxEnergy;
        }
    }
    public void UpdateBoost(float currentEnergy)
    {
        if (boostSlider != null)
        {
            boostSlider.value = currentEnergy / 4;
        }
    }

    //set speedometer
    public void UpdateSpeedometer(float speed, float maxSpeed)
    {
        if (needle != null)
        {
            float needleRotation = Mathf.Lerp(minNeedleRotation, maxNeedleRotation, speed / maxSpeed);
            needle.localRotation = Quaternion.Euler(0, 0, needleRotation);
        }
    }
}
