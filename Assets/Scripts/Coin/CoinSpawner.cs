using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    [SerializeField] GameObject coinPrefab;

    //coin poll
    GameObject[] coinPool = new GameObject[10];
    Transform playerTransform;

    //coin spacing
    float lastCoinZPosition = 0f;
    float minSpacing = 10f;
    float maxSpacing = 25f;

    //delay
    WaitForSeconds wait = new WaitForSeconds(1f);

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        InitializeCoinPool();
        StartCoroutine(SpawnCoins());
    }

    void InitializeCoinPool()
    {
        for (int i = 0; i < coinPool.Length; i++)
        {
            coinPool[i] = Instantiate(coinPrefab);
            coinPool[i].SetActive(false);
        }
    }

    IEnumerator SpawnCoins()
    {
        while (true)
        {
            CleanUpCoinsOutOfView();
            TrySpawnNewCoin();

            yield return wait;
        }
    }

    void TrySpawnNewCoin()
    {
        GameObject coinToSpawn = GetInactiveCoinFromPool();
        if (coinToSpawn == null)
            return;
        
        //for the first time, the first coin will be far away from the player
        if (lastCoinZPosition == 0)
        {
            lastCoinZPosition = playerTransform.position.z + 50f;
        }

        //ensure last position is always ahead of the player
        //if the last spawn point lies behind the player, move it to ahead
        if (lastCoinZPosition < playerTransform.position.z)
        {
            lastCoinZPosition = playerTransform.position.z + minSpacing;
        }

        //get random space
        float randomCoinSpacing = Random.Range(minSpacing, maxSpacing);

        //get z position
        float spawnZPosition = lastCoinZPosition + randomCoinSpacing;

        //define spawn x position
        float[] xPositions = { -0.3f, 0.3f };

        //select a x position randomly
        float selectedXPosition = xPositions[Random.Range(0, xPositions.Length)];

        //set spawn position
        Vector3 spawnPosition = new Vector3(selectedXPosition, 0.2f, spawnZPosition);

        coinToSpawn.transform.position = spawnPosition;

        coinToSpawn.SetActive(true);

        //update the position
        lastCoinZPosition = spawnZPosition;
    }

    private GameObject GetInactiveCoinFromPool()
    {
        foreach (GameObject coin in coinPool)
        {
            if (!coin.activeInHierarchy)
                return coin;
        }
        return null;
    }

    private void CleanUpCoinsOutOfView()
    {
        foreach (GameObject coin in coinPool)
        {
            //skip active coin
            if (!coin.activeInHierarchy)
                continue;

            //disable coin if too far
            float distanceFromPlayer = coin.transform.position.z - playerTransform.position.z;

            if (distanceFromPlayer < -50)
            {
                coin.SetActive(false);
            }
        }
    }
}
