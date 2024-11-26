using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] bool isAnim;
    [SerializeField] float rotateSpeed = 5f;
    [SerializeField] float bounceSpeed = 5f;
    [SerializeField] float bounceHeight = 0.1f;

    private float initialY;

    //use static to make effect accessible across all coin instances, thus it only initialize once
    private static ParticleSystem collectEffect;

    // Start is called before the first frame update
    void Start()
    {
        initialY = transform.position.y;

        if (collectEffect == null)
        {
            GameObject effect = GameObject.FindGameObjectWithTag("ParticleEffect");
            collectEffect = effect.GetComponent<ParticleSystem>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isAnim)
        {
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime, Space.World);

            //move up down - creates an oscillating value between -height and height
            float newY = initialY + Mathf.Sin(Time.time * bounceSpeed) * bounceHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.CompareTag("Player"))
        {
            Collect();
        }
    }
    private void Collect()
    {
        if (collectEffect)
        {
            collectEffect.transform.position = transform.position;
            collectEffect.Play();
        }

        CoinManager.Instance.AddCoin();
        gameObject.SetActive(false);
    }
}
