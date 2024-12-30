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

    [SerializeField] GameObject tutorialMenu;
    [SerializeField] GameObject pausePanel;

    [SerializeField] GameObject losePanel;
    [SerializeField] TMP_Text scoreText;

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
    private void Start()
    {
        SetPauseMenu(false);
    }
    //display coin
    public void UpdateCoinText(int coinAmount)
    {
        coinText.text = coinAmount.ToString();
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
    public void SetPauseMenu(bool isShowing)
    {
        pausePanel.SetActive(isShowing);
        if (!isShowing && tutorialMenu.activeSelf) tutorialMenu.SetActive(false);
    }

    public void ActiveLosePanel()
    {
        StartCoroutine(ActiveLosePanelCO());
    }
    IEnumerator ActiveLosePanelCO()
    {
        yield return new WaitForSeconds(3);
        losePanel.SetActive(true);
        scoreText.text = coinText.text;
    }
}
