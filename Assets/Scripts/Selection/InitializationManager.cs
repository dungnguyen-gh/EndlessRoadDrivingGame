using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitializationManager : MonoBehaviour
{
    public static InitializationManager Instance { get; private set; }

    public static event Action OnInitializationComplete;

    public Transform PlayerTransform { get; private set; }

    private bool isInitialized = false;

    private bool isPaused = false;

    private List<AudioSource> audioSources = new List<AudioSource>();

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    public void InitializeGame(Transform playerTransform)
    {
        if (isInitialized) return;

        PlayerTransform = playerTransform;
        isInitialized = true;

        //inform all listeners
        OnInitializationComplete?.Invoke();
    }

    public void PauseToggle()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
        CanvasManager.Instance.SetPauseMenu(isPaused);
        if (isPaused) PauseAudio();
        else ResumeAudio();
    }

    public void BackToMain()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("SelectionScene");
    }
    public void ResetGame()
    {
        Time.timeScale = 1f;
        CanvasManager.Instance.SetPauseMenu(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    private void PauseAudio()
    {
        var audios = FindObjectsOfType<AudioSource>();
        foreach (var audio in audios)
        {
            if (audio.isPlaying)
            {
                audio.Pause();
                audioSources.Add(audio);
            }
        }
    }
    private void ResumeAudio()
    {
        foreach (var audio in audioSources)
        {
            audio.Play();
        }
        audioSources.Clear();
    }
}
