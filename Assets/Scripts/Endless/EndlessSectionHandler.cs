using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessSectionHandler : MonoBehaviour
{
    Transform playerCarTransform;

    // Start is called before the first frame update
    void Start()
    {
        playerCarTransform = InitializationManager.Instance.PlayerTransform;
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = transform.position.z - playerCarTransform.position.z;

        //calculate a percentage value for linear interpolation based on distanceToPlayer
        //distanceToPlayer - 100 <= 0, lerp = 1, object at y = 0, lerp start when object is 100 unit away from player
        //100<distance<250, lerp decrease from 1 to 0, object -10 - 0. divide by 150 so the range of values is spread over 150 units. if 250
        //distance>250, lerp = 0, y = -10
        //1- for inverting value
        float lerpPercentage = 1.0f - ((distanceToPlayer - 100) / 150.0f);
        lerpPercentage = Mathf.Clamp01(lerpPercentage);

        //smooth transition
        transform.position = Vector3.Lerp(new Vector3(transform.position.x, -10, transform.position.z), new Vector3(transform.position.x, 0, transform.position.z), lerpPercentage);

    }
}
