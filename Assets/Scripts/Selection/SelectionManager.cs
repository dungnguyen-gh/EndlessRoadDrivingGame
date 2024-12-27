using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{
    public GameObject[] cars;
    private int currentCarIndex = 0;
    [SerializeField] Button prevButton;
    [SerializeField] Button nextButton;
    [SerializeField] Button startButton;

    // Start is called before the first frame update
    void Start()
    {
        //deactive all cars, active the first one
        for (int i = 0; i < cars.Length; i++)
        {
            cars[i].SetActive(i == currentCarIndex);
        }
        prevButton.onClick.AddListener(ShowPreviousCar);
        nextButton.onClick.AddListener(ShowNextCar);
        startButton.onClick.AddListener(StartCar);
    }
    public void StartCar()
    {
        //save selected car in PlayerPrefs
        PlayerPrefs.SetInt("SelectedCarIndex", currentCarIndex);
        PlayerPrefs.Save();

        //load scene
        SceneManager.LoadScene("DrivingScene");
    }
    public void ShowNextCar()
    {
        cars[currentCarIndex].SetActive(false);
        currentCarIndex = (currentCarIndex + 1) % cars.Length;
        cars[currentCarIndex].SetActive(true);
    }
    public void ShowPreviousCar()
    {
        cars[currentCarIndex].SetActive(false);
        currentCarIndex = (currentCarIndex - 1 + cars.Length) % cars.Length;
        cars[currentCarIndex].SetActive(true);
    }
}
