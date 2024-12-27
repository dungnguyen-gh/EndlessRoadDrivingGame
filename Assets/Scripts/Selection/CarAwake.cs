using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAwake : MonoBehaviour
{
    public GameObject[] cars;
    public CinemachineVirtualCamera virtualCamera;

    private void Awake()
    {
        Vector3 spawnedPosition = new Vector3(0,0,0);

        //get int from PlayerPrefs, set default = 0, if no index is found
        int selectedCarIndex = PlayerPrefs.GetInt("SelectedCarIndex", 0);

        if (selectedCarIndex < 0 || selectedCarIndex >= cars.Length)
        {
            Debug.Log("invalid index, choose first car");
            selectedCarIndex = 0;
        }

        GameObject selectedCar = Instantiate(cars[selectedCarIndex], spawnedPosition, Quaternion.identity);

        //set camera 
        if (virtualCamera != null)
        {
            virtualCamera.Follow = selectedCar.transform;
            virtualCamera.LookAt = selectedCar.transform;
        }
        else
        {
            Debug.Log("no camera");
        }

        InitializationManager.Instance.InitializeGame(selectedCar.transform);
    }
}
