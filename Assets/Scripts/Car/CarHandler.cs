using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarHandler : MonoBehaviour
{
    [SerializeField] Rigidbody rb;

    [SerializeField] Transform gameModel;
    
    [SerializeField] ExplodeHandler explodeHandler;

    [SerializeField] MeshRenderer carMeshRenderer;

    //max values
    float maxSteerVelocity = 2f;
    float maxForwardVelocity = 12f;

    float carMaxSpeedPercentage = 0f;

    //rates
    float accelerationRate = 1.2f;
    float brakeRate = 10f;
    float steeringRate = 5f;

    Vector2 input = Vector2.zero;

    //Explode state
    bool isExploded = false;

    bool isPlayer = true;

    //distance tracking
    private float distanceTraveled = 0f;
    private Vector3 lastPosition;
    private Coroutine distanceCO;
    WaitForSeconds wait = new WaitForSeconds(0.1f);


    //Emission property
    [Header("Emission")]
    private static readonly int _EmissionColor = Shader.PropertyToID("_EmissionColor");
    Color emissiveColor = Color.red;
    float currentEmissiveColorMultiplier = 0f;
    float targetEmissiveColorMultiplier = 0f;

    [Header("SFX")]
    [SerializeField] AudioSource carEngineAS;

    [SerializeField] AnimationCurve carPitchAnimationCurve;

    [SerializeField] AudioSource carSkidAS;

    [SerializeField] AudioSource carCrashAS;

    [Header("BOOSTING")]
    [SerializeField] private float boostMultiplier = 3.0f;
    [SerializeField] private float maxBoostEnergy = 10f;
    private float boostEnergy;
    public bool isBoosting = false;
    public ParticleSystem[] nitrusSmoke;
    private bool isEmitting = true;


    void Start()
    {
        isPlayer = CompareTag("Player");

        StopEmitter();

        if (!isPlayer)
        {
            return;
        }
        else
        {
            carEngineAS.Play();

            //start measuring distance
            lastPosition = transform.position;

            InititalizeBoosting();

            distanceCO = StartCoroutine(UpdateTraveledDistanceCO());
        }
    }
    void Update()
    {
        if (isExploded) 
        {
            StopEmitter();
            FadeOutCarAudio();
            return;
        }
        

        //rotate car model based on X velocity when turning for visual effect
        gameModel.transform.rotation = Quaternion.Euler(0, rb.velocity.x * 5, 0);

        UpdateCarLight();

        UpdateCarAudio();
    }
    private void FixedUpdate()
    {
        //is exploded
        if (isExploded)
        {
            ApplyExplodedPhysics();
            return;
        }

        HandleDriving();

        HandleSteering();
        //force the car cannot go backwards
        if (rb.velocity.z <= 0)
        {
            rb.velocity = Vector3.zero;
        }
    }
    private void ClampCarXPosition()
    {
        Vector3 clampedPosition = rb.position;


        if (!Mathf.Approximately(clampedPosition.x, Mathf.Clamp(clampedPosition.x, -3.0f, 3.0f)))
        {
            //clamp position x
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, -3.0f, 3.0f);

            rb.position = clampedPosition;

            //reset velocity x to stop pushing force
            rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
        }
    }
    void HandleDriving()
    {
        if (input.y > 0)
        {
            Accelerate();
        }
        else
        {
            rb.drag = 0.2f;
        }

        if (input.y < 0)
        {
            Brake();
        }

        //clamp car's x position
        ClampCarXPosition();


        HandleBoosting();

        HandleSpeedometer();
    }

    void ApplyExplodedPhysics()
    {
        //apply drag
        rb.drag = Mathf.Clamp(rb.velocity.z * 0.1f, 1.5f, 10);

        //move towards after the car has exploded
        rb.MovePosition(Vector3.Lerp(transform.position, new Vector3(0, 0, transform.position.z), Time.deltaTime * 0.5f));

    }
    void Accelerate()
    {
        rb.drag = 0; //not slow down when accelerating

        float maxVelocity = isEmitting ? maxForwardVelocity * boostMultiplier : maxForwardVelocity;

        //apply deceleration after boosting
        if (!isEmitting && rb.velocity.z > maxForwardVelocity)
        {
            rb.velocity = Vector3.Lerp(
                rb.velocity,
                new Vector3(rb.velocity.x, rb.velocity.y, maxForwardVelocity),
                Time.deltaTime * 2);
        }

        //stay within the speed limit
        if (rb.velocity.z >= maxVelocity)
            return;

        //apply force, boosted or normal
        float currentAccelerationRate = isEmitting ? accelerationRate * boostMultiplier : accelerationRate;

        rb.AddForce(rb.transform.forward * currentAccelerationRate * input.y); //get from user input forward
    }
    void Brake()
    {
        //only brake when moving forward
        if (rb.velocity.z <= 0)
            return;

        rb.AddForce(rb.transform.forward * brakeRate * input.y, ForceMode.Force);
    }
    void HandleSteering()
    {
        if (Mathf.Abs(input.x) > 0)
        {
            //move sideways
            float speedSteerFactor = Mathf.Clamp01(rb.velocity.z / 5.0f);

            //apply a force based on input
            rb.AddForce(rb.transform.right * steeringRate * input.x * speedSteerFactor, ForceMode.Force);

            //normalize the x velocity
            float normalizedX = rb.velocity.x / maxSteerVelocity;

            //clamp the magnitude
            normalizedX = Mathf.Clamp(normalizedX, -1.0f, 1.0f);

            //make sure staying speed limit
            rb.velocity = new Vector3(normalizedX * maxSteerVelocity, 0, rb.velocity.z);
        }
        else
        {
            //auto center car
            rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(0, 0, rb.velocity.z), Time.fixedDeltaTime * 3);
        }
    }
    public void SetInput(Vector2 inputVector)
    {
        input = inputVector.normalized;
    }
    public void SetMaxSpeed(float newMaxSpeed)
    {
        maxForwardVelocity = newMaxSpeed;
    }
    IEnumerator SlowDownTimeCO()
    {
        while (Time.timeScale > 0.2f)
        {
            Time.timeScale -= Time.deltaTime * 2;

            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        while (Time.timeScale <= 1.0f)
        {
            Time.timeScale += Time.deltaTime;

            yield return null;
        }

        Time.timeScale = 1.0f;
    }
    //Events Collisions
    private void OnCollisionEnter(Collision collision)
    {
        //ai car will be only exploded when it hit the player or a car part
        if (!isPlayer)
        {
            if (collision.transform.root.CompareTag("Untagged") || collision.transform.root.CompareTag("CarAI"))
                return;
        }
        //get collision point
        Vector3 collisionPoint = collision.contacts[0].point;

        Vector3 velocity = rb.velocity;
        explodeHandler.Explode(velocity * 45);

        isExploded = true;

        StartCoroutine(ResetSpeedCO());

        PlayCrashSound();

        //stop coroutine when exploded
        if (distanceCO != null)
        {
            StopCoroutine(distanceCO);
            distanceCO = null;
        }

        //start slow down effects
        StartCoroutine(SlowDownTimeCO());

        CanvasManager.Instance.ActiveLosePanel();
    }
    void PlayCrashSound()
    {
        carCrashAS.volume = carMaxSpeedPercentage;
        carCrashAS.volume = Mathf.Clamp(carCrashAS.volume, 0.25f, 1.0f);

        carCrashAS.pitch = carMaxSpeedPercentage;
        carCrashAS.pitch = Mathf.Clamp(carCrashAS.pitch, 0.3f, 1.0f);

        carCrashAS.Play();
    }
    IEnumerator UpdateTraveledDistanceCO()
    {
        while (true)
        {
            yield return wait;

            //calculate the distance has moved
            float currentDistance = Vector3.Distance(transform.position, lastPosition);

            //check distance
            if (currentDistance > 1.0f)
            {
                //add to distance
                distanceTraveled += currentDistance;

                //update last position
                lastPosition = transform.position;

                //update UI
                CanvasManager.Instance.UpdateDistanceText(distanceTraveled);
            }

        }
    }
    void UpdateCarLight()
    {
        if (!isPlayer) return;

        if (carMeshRenderer != null)
        {
            bool isBraking = input.y < 0;

            targetEmissiveColorMultiplier = isBraking ? 5.0f : 0.0f;

            // can use Lerp or MoveTowards(smoother)
            currentEmissiveColorMultiplier = Mathf.MoveTowards(currentEmissiveColorMultiplier, targetEmissiveColorMultiplier, Time.deltaTime * 4);

            //cache material for optimization
            Material carMaterial = carMeshRenderer.material;

            //only update if having a change
            if (!Mathf.Approximately(currentEmissiveColorMultiplier, targetEmissiveColorMultiplier))
            {
                carMaterial.SetColor(_EmissionColor, emissiveColor * currentEmissiveColorMultiplier);
            }
        }
    }
    void UpdateCarAudio()
    {
        if (!isPlayer) return;

        carMaxSpeedPercentage = rb.velocity.z / maxForwardVelocity;

        carEngineAS.pitch = carPitchAnimationCurve.Evaluate(carMaxSpeedPercentage);

        if (input.y < 0 && carMaxSpeedPercentage > 0.2f)
        {
            if (!carSkidAS.isPlaying)
                carSkidAS.Play();

            carSkidAS.volume = Mathf.Lerp(carSkidAS.volume, 1.0f, Time.deltaTime * 10);
        }
        else
        {
            carSkidAS.volume = Mathf.Lerp(carSkidAS.volume, 0, Time.deltaTime * 30);
        }
    }
    void FadeOutCarAudio()
    {
        if (!isPlayer) return;
        //happen when car exploded
        carEngineAS.volume = Mathf.Lerp(carEngineAS.volume, 0, Time.deltaTime * 10);

        carSkidAS.volume = Mathf.Lerp(carSkidAS.volume, 0, Time.deltaTime * 10);
    }
    private void OnDestroy()
    {
        if (distanceCO != null)
        {
            StopCoroutine(distanceCO);
        }
    }
    void HandleBoosting()
    {
        if (!isPlayer) return;

        if (!isBoosting && boostEnergy <= maxBoostEnergy)
        {
            boostEnergy += Time.deltaTime * 20f;
        }
        else
        {
            boostEnergy -= (boostEnergy <= 0) ? 0 : Time.deltaTime * 10f;
        }
        boostEnergy = Mathf.Clamp(boostEnergy, 0, maxBoostEnergy);

        if (isBoosting)
        {
            if (boostEnergy > 0) StartEmitter();
            else StopEmitter();
        }
        else StopEmitter();

        //update ui
        CanvasManager.Instance.UpdateBoost(boostEnergy);
    }
    void InititalizeBoosting()
    {
        boostEnergy = maxBoostEnergy;
        CanvasManager.Instance.SetMaxBoost(maxBoostEnergy);
        CanvasManager.Instance.UpdateBoost(boostEnergy);
    }
    void StartEmitter()
    {
        if (isEmitting) return;
        for (int i = 0; i < nitrusSmoke.Length; i++)
        {
            nitrusSmoke[i].Play();
        }
        isEmitting = true;
    }
    void StopEmitter()
    {
        if (!isEmitting) return;
        for (int i = 0; i < nitrusSmoke.Length; i++)
        {
            nitrusSmoke[i].Stop();
        }
        isEmitting = false;
    }
    void HandleSpeedometer()
    {
        if (!isPlayer) return;

        float speed = rb.velocity.magnitude * 3.6f;

        CanvasManager.Instance.UpdateSpeedometer(speed, 180f);
    }
    IEnumerator ResetSpeedCO()
    {
        float duration = 1f;
        float elapsedTime = 0f;
        Vector3 initialVelocity = rb.velocity;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            //reduce velocity to 0 gradually
            rb.velocity = Vector3.Lerp(initialVelocity, Vector3.zero, elapsedTime / duration);

            float speed = rb.velocity.magnitude * 3.6f;

            CanvasManager.Instance.UpdateSpeedometer(speed, 180f);
            yield return null;
        }

        rb.velocity = Vector3.zero;
        CanvasManager.Instance.UpdateSpeedometer(0f, 180f);
    }
    
}
