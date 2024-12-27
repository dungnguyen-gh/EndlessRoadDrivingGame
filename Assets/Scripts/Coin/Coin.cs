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

    AudioSource coinAS;

    MeshRenderer coinRenderer;

    CoinSpawner coinSpawner;
    // Start is called before the first frame update
    void Start()
    {
        initialY = transform.position.y;

        coinAS = GetComponent<AudioSource>();
        coinRenderer = GetComponent<MeshRenderer>();

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
            PlayCoinSound();
            
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
        coinRenderer.enabled = false;

        coinSpawner.AddCoinPoint();

        StartCoroutine(DeactiveAfterSound());
    }
    private IEnumerator DeactiveAfterSound()
    {
        //wait for the sound finished
        yield return new WaitWhile(() => coinAS.isPlaying);

        gameObject.SetActive(false);
        coinRenderer.enabled = true;
    }
    private void PlayCoinSound()
    {
        //play sound depending on player's speed
        if (!coinAS.isPlaying)
        {
            coinAS.pitch = 1.0f;
            coinAS.volume = 1.0f;
            coinAS.Play();
        }
    }
    public void SetCoinSpawner(CoinSpawner coin)
    {
        coinSpawner = coin;
    }
}
