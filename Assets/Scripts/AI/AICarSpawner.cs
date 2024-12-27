using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICarSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] carAIPrefabs;

    //detecting other cars
    [SerializeField] LayerMask otherCarsLayerMask;

    GameObject[] carAIPool = new GameObject[20];

    Transform playerCarTransform;

    //timing and delay
    float lastCarSpawnedTime = 0f;
    WaitForSeconds wait = new WaitForSeconds(0.5f);

    //overlap check to avoid on spawning on other cars
    Collider[] overlappedCheckCollider = new Collider[1];


    private void OnEnable()
    {
        InitializationManager.OnInitializationComplete += OnInitializationFinish;
    }
    private void OnDisable()
    {
        InitializationManager.OnInitializationComplete -= OnInitializationFinish;
    }
    private void OnInitializationFinish()
    {
        playerCarTransform = InitializationManager.Instance.PlayerTransform;

        InitializeCarPool();

        StartCoroutine(SpawnCars());
    }

    private void InitializeCarPool()
    {
        int prefabIndex = 0;

        for (int i = 0; i < carAIPool.Length; i++)
        {
            carAIPool[i] = Instantiate(carAIPrefabs[prefabIndex]);
            carAIPool[i].SetActive(false);

            //increment prefab index
            prefabIndex = (prefabIndex + 1) % carAIPrefabs.Length;
        }
    }
    IEnumerator SpawnCars()
    {
        while (true)
        {
            CleanUpCarsOutOfView();
            TrySpawnNewCars();

            yield return wait;
        }
    }

    void TrySpawnNewCars()
    {
        if (Time.time - lastCarSpawnedTime < 2)
            return;

        GameObject carToSpawn = GetInactiveCarFromPool();

        //no car available to spawn
        if (carToSpawn == null)
            return;

        //set position and rotation for the spawned car
        Vector3 spawnPosition = new Vector3(0, 0, playerCarTransform.transform.position.z + 100);

        if (Physics.OverlapBoxNonAlloc(spawnPosition, Vector3.one * 2, overlappedCheckCollider, Quaternion.identity, otherCarsLayerMask) > 0)
            return;

        //activate car
        carToSpawn.transform.position = spawnPosition;
        carToSpawn.SetActive(true);

        lastCarSpawnedTime = Time.time;
    }
    private GameObject GetInactiveCarFromPool()
    {
        //find a car to spawn
        foreach (GameObject aiCar in carAIPool)
        {
            //select active cars
            if (!aiCar.activeInHierarchy)
                return aiCar;
        }
        return null;
    }

    void CleanUpCarsOutOfView()
    {
        foreach (GameObject aiCar in carAIPool)
        {
            //skip inactive cars
            if (!aiCar.activeInHierarchy)
                continue;

            //disable car if too far from the player
            float distanceFromPlayer = aiCar.transform.position.z - playerCarTransform.position.z;

            //check if AI car is too far ahead or check if AI car is too far behind 
            if (distanceFromPlayer > 200 || distanceFromPlayer < -50)
                aiCar.SetActive(false);
        }
    }
}

