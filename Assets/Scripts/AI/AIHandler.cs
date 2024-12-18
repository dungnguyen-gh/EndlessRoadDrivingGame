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

    float carAheadDistance = 0f;

    //Timing
    WaitForSeconds checkInterval = new WaitForSeconds(0.2f);

    //Lanes
    int laneIndex = 0;

    [Header("SFX")]
    [SerializeField] AudioSource honkHornAS;

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

        //for honking
        if (isCarAhead && carAheadDistance < 10)
        {
            PlayHorn();
        }

        float steerInput = CalculateSteering();

        carHandler.SetInput(new Vector2(steerInput, accelerationInput));
    }
    void PlayHorn()
    {
        if (!honkHornAS.isPlaying)
        {
            honkHornAS.pitch = Random.Range(0.5f, 1.1f);
            honkHornAS.Play();
        }
    }
    private float CalculateSteering()
    {
        //calculate steering based on desired lane index
        float targetPositionX = Utils.CarLanes[laneIndex];

        float distanceToTargetLane = targetPositionX - transform.position.x;

        //is ai is far from target lane
        if (Mathf.Abs(distanceToTargetLane) > 0.05f)
        {
            return Mathf.Clamp(Mathf.Sign(distanceToTargetLane), -1.0f, 1.0f);
        }

        return 0.0f;
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

        if (hitCount > 0)
        {
            carAheadDistance = (transform.position - raycastHits[0].point).magnitude;
        }

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
 