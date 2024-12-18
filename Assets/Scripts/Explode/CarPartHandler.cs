using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPartHandler : MonoBehaviour
{
    AudioSource bounceAS;

    private void Awake()
    {
        bounceAS = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!bounceAS.isPlaying)
        {
            bounceAS.pitch = collision.relativeVelocity.magnitude * 0.5f;

            bounceAS.pitch = Mathf.Clamp(bounceAS.pitch, 0.5f, 1.0f);

            bounceAS.Play();
        }
    }
}
