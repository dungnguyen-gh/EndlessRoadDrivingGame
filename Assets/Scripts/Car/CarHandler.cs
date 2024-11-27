using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarHandler : MonoBehaviour
{
    [SerializeField] Rigidbody rb;

    [SerializeField] Transform gameModel;
    
    [SerializeField] ExplodeHandler explodeHandler;

    //max values
    float maxSteerVelocity = 2f;
    float maxForwardVelocity = 15f;

    //rates
    float accelerationRate = 1.5f;
    float brakeRate = 10f;
    float steeringRate = 5f;

    Vector2 input = Vector2.zero;

    //Explode state
    bool isExploded = false;

    bool isPlayer = true;

    void Start()
    {
        isPlayer = CompareTag("Player");
    }
    void Update()
    {
        if (isExploded) return;

        //rotate car model based on X velocity when turning for visual effect
        gameModel.transform.rotation = Quaternion.Euler(0, rb.velocity.x * 5, 0);
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

        Steer();

        ////force the car cannot go backwards
        //if (rb.velocity.z <= 0)
        //{
        //    rb.velocity = Vector3.zero;
        //}

        //force the car cannot go backwards
        if (rb.velocity.z <= 0)
        {
            rb.velocity = Vector3.zero;
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

        //stay within the speed limit
        if (rb.velocity.z >= maxForwardVelocity)
            return;

        rb.AddForce(rb.transform.forward * accelerationRate * input.y); //get from user input forward
    }
    void Brake()
    {
        //only brake when moving forward
        if (rb.velocity.z <= 0)
            return;

        rb.AddForce(rb.transform.forward * brakeRate * input.y);
    }
    void Steer()
    {
        if (Mathf.Abs(input.x) > 0)
        {
            //move sideways
            float speedBaseSteerLimit = rb.velocity.z / 5.0f;
            speedBaseSteerLimit = Mathf.Clamp01(speedBaseSteerLimit);

            //apply a force based on input
            rb.AddForce(rb.transform.right * steeringRate * input.x * speedBaseSteerLimit);

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

        StartCoroutine(SlowDownTimeCO());
    }
}
