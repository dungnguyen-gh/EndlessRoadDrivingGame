using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeHandler : MonoBehaviour
{
    [SerializeField] GameObject originalObject;

    [SerializeField] GameObject model;

    Rigidbody[] rigidbodies;

    [SerializeField] GameObject explosionEffect;

    private void Awake()
    {
        rigidbodies = model.GetComponentsInChildren<Rigidbody>(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        //Explode(Vector3.forward);
    }

    public void Explode(Vector3 externalForce)
    {
        originalObject.SetActive(false);

        foreach (Rigidbody rb in rigidbodies)
        {
            rb.transform.parent = null;

            rb.GetComponent<MeshCollider>().enabled = true;

            rb.gameObject.SetActive(true);
            rb.isKinematic = false;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.AddForce(Vector3.up * 200 + externalForce, ForceMode.Force);
            rb.AddTorque(Random.insideUnitSphere * 0.5f, ForceMode.Impulse);

            
            //change the tag so other objects can explode when being hit by car parts
            rb.gameObject.tag = "CarPart";
        }
        TriggerEffect();
    }

    void TriggerEffect()
    {
        Vector3 center = CalculateCenter();

        Instantiate(explosionEffect, center, Quaternion.identity);
    }
    private Vector3 CalculateCenter()
    {
        Vector3 center = Vector3.zero;
        int activePartsCount = 0;

        foreach (Rigidbody rb in rigidbodies)
        {
            if (rb.gameObject.activeInHierarchy)
            {
                center += rb.transform.position;
                activePartsCount++;
            }
        }
        if (activePartsCount > 0)
        {
            //calculate average position
            center /= activePartsCount; 
        }
        return center;
    }
}
