using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessLevelHandler : MonoBehaviour
{
    //represent for different road sections
    [SerializeField] GameObject[] sectionsPrefabs;

    // a pool of inactive road instance, used to recycle sections
    GameObject[] sectionsPool = new GameObject[20];

    GameObject[] sections = new GameObject[10];
    //sections will be picked from sectionsPool, active road sections currently using on the track
    //when we reach a certain point, sections will be discard and get another from the pool

    Transform playerCarTransform;

    WaitForSeconds waitFor100ms = new WaitForSeconds(0.1f); //make sure the couroutine not create garbage

    const float sectionLength = 26; //length of each road

    // Start is called before the first frame update
    void Start()
    {
        playerCarTransform = GameObject.FindGameObjectWithTag("Player").transform;

        int prefabIndex = 0;

        //create a pool for our endless sections
        for (int i = 0; i < sectionsPool.Length; i++)
        {
            sectionsPool[i] = Instantiate(sectionsPrefabs[prefabIndex]);
            sectionsPool[i].SetActive(false);

            prefabIndex++;

            //loop the prefab index if we run out of prefabs
            if (prefabIndex > sectionsPrefabs.Length - 1)
                prefabIndex = 0;
        }

        //add the first sections to the road 
        //active first 10 sections and position it in a line
        for (int i = 0; i < sections.Length; i++)
        {
            //get random section
            GameObject randomSection = GetRandomSectionFromPool();

            //move it into position and set it to active
            //space apart by section length (26 unit along the z-axis)
            randomSection.transform.position = new Vector3(sectionsPool[i].transform.position.x, -10, i * sectionLength); 
            randomSection.SetActive(true);

            //store this section in the sections array, representing the currently active segment of the road
            sections[i] = randomSection; 
        }
        //call periodically
        StartCoroutine(UpdateLessOftenCO());

    }

    IEnumerator UpdateLessOftenCO()
    {
        while (true) //infinite
        {
            UpdateSectionPositions();
            yield return waitFor100ms;
        }
    }
    void UpdateSectionPositions()
    {
        for (int i = 0; i < sections.Length; i++)
        {
            //check if section is too far behind
            //if a section is more than -sectionLength units behind the player, meaning the car has moved past it
            if (sections[i].transform.position.z - playerCarTransform.position.z < -sectionLength)
            {
                //store the position of the section and disable it
                Vector3 lastSectionPosition = sections[i].transform.position;
                sections[i].SetActive(false);

                //get new section & enable it & move it forward
                sections[i] = GetRandomSectionFromPool();

                //move the new section into place and active it
                //the new new section is placed in front of the last section by adjusting along z-axis
                sections[i].transform.position = new Vector3(lastSectionPosition.x, -10, lastSectionPosition.z + sectionLength * sections.Length);
                sections[i].SetActive(true);
            }
        }
    }

    GameObject GetRandomSectionFromPool()
    {
        //pick a random index and hope that it is available
        int randomIndex = Random.Range(0, sectionsPool.Length);

        bool isNewSectionFound = false;

        while (!isNewSectionFound)
        {
            //check if the section is not active, in that case we've found a section
            if (!sectionsPool[randomIndex].activeInHierarchy) isNewSectionFound = true;
            else
            {
                //if it was active we need to try to find another one so we increase the index
                randomIndex++;

                //ensure that we loop around if we reach the end of the array
                if (randomIndex > sectionsPool.Length - 1) randomIndex = 0;
            }
        }
        return sectionsPool[randomIndex];
    }
}
