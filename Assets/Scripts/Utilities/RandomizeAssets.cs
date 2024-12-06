using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeAssets : MonoBehaviour
{
    public GameObject[] prefabArray;
    private List<Vector3> initialPositions = new List<Vector3>();

    void Start()
    {
        foreach (Transform child in transform)
        {
            initialPositions.Add(child.position);
        }
        ReplaceAssets();
    }

    public void ReplaceAssets()
    {
        if (prefabArray == null || prefabArray.Length == 0)
        {
            return;
        }
        ShuffleArray();
        int prefabIndex = 0;

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            Vector3 childPosition = child.position;

            // get the prefab to instantiate
            GameObject prefabToSpawn = prefabArray[prefabIndex];
            Quaternion childRotation = prefabToSpawn.transform.rotation;

            //check tag for housing
            if (prefabToSpawn.CompareTag("Housing")) 
            {
                if (childPosition.x < 0)
                {
                    childRotation = Random.value > 0.5f ? Quaternion.Euler(0, 90, 0) : childRotation;
                }
                else if (childPosition.x > 0)
                {
                    childRotation = Random.value > 0.5f ? Quaternion.Euler(0, 270, 0) : childRotation;
                }
            }
            //check tag for bench asset
            else if (prefabToSpawn.CompareTag("Bench"))
            {
                //check rotation
                if (childPosition.x < 0)
                {
                    childRotation = Quaternion.Euler(0, 90, 0);
                }
                else if (childPosition.x > 0)
                {
                    childRotation = Quaternion.Euler(0, 270, 0);
                }
            }

            //destroy current
            Destroy(child.gameObject);

            //instantiate the prefab
            GameObject newAsset = Instantiate(prefabToSpawn, childPosition, childRotation);

            newAsset.transform.SetParent(transform);

            prefabIndex = (prefabIndex + 1) % prefabArray.Length;
        }
    }

    //randomize array using Fisher-Yates shuffle
    void ShuffleArray()
    {
        for (int i = prefabArray.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            GameObject temp = prefabArray[i];
            prefabArray[i] = prefabArray[randomIndex];
            prefabArray[randomIndex] = temp;
        }
    }
}
