using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIHandler : MonoBehaviour
{
    [SerializeField] CarHandler carHandler;

    [SerializeField] LayerMask otherCarsLayerMask;

    [SerializeField] MeshCollider meshCollider;

    RaycastHit[] raycastHits = new RaycastHit[1];

    bool isCarAhead = false;

    //Timing
    WaitForSeconds checkInterval = new WaitForSeconds(0.2f);

    //Lanes
    int laneIndex = 0;
    private void Awake()
    {
        if (CompareTag("Player"))
        {
            Destroy(this);
            return;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CheckForCarAhead());
    }

    // Update is called once per frame
    void Update()
    {
        HandleCarMovement();
    }

    private void HandleCarMovement()
    {
        //slow down if there is a car ahead
        float accelerationInput = isCarAhead ? -1.0f : 1.0f;

        float steerInput = 0.0f;

        //calculate steering based on desired lane index
        float targetPositionX = Utils.CarLanes[laneIndex];

        float distanceToTargetLane = targetPositionX - transform.position.x;

        if (Mathf.Abs(distanceToTargetLane) > 0.05f)
            steerInput = Mathf.Sign(distanceToTargetLane);

        steerInput = Mathf.Clamp(steerInput, -1.0f, 1.0f);

        carHandler.SetInput(new Vector2(steerInput, accelerationInput));
    }
    IEnumerator CheckForCarAhead()
    {
        while (true) 
        {
            isCarAhead = CheckIfOtherCarsIsAhead();
            yield return checkInterval;
        }
    }

    bool CheckIfOtherCarsIsAhead()
    {
        //temporarily disable collider to prevent self collision
        meshCollider.enabled = false;

        Vector3 boxCenter = transform.position + transform.forward;
        Vector3 boxHalfExtents = Vector3.one * 0.25f;
        Quaternion boxOrientation = Quaternion.identity;
        float maxDistance = 2f;

        int hitCount = Physics.BoxCastNonAlloc(boxCenter, boxHalfExtents, transform.forward, raycastHits, boxOrientation, maxDistance, otherCarsLayerMask);
        
        meshCollider.enabled = true;

        return hitCount > 0;
    }

    public void SetLaneIndex(int index)
    {
        laneIndex = index;
    }
    private void OnEnable()
    {
        //random speed
        carHandler.SetMaxSpeed(Random.Range(2, 4));

        //set random lanes
        laneIndex = Random.Range(0, Utils.CarLanes.Length);
    }
}
 