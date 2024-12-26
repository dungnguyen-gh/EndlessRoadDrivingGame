using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [SerializeField] CarHandler carHandler;
    private PlayerInputAction inputActions;
    private void Awake()
    {
        if (!CompareTag("Player"))
        {
            Destroy(this);
            return;
        }

        inputActions = new PlayerInputAction();

        inputActions.PlayerInput.Reset.performed += OnResetGame;
        inputActions.PlayerInput.Boost.started += OnBoostStart;
        inputActions.PlayerInput.Boost.canceled += OnBoostEnd;
        inputActions.PlayerInput.Move.performed += OnMove;
        inputActions.PlayerInput.Move.canceled += OnMove;
    }

    private void OnEnable() => inputActions?.Enable();
    private void OnDisable() => inputActions?.Disable();

    private void OnResetGame(InputAction.CallbackContext context)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    private void OnBoostStart(InputAction.CallbackContext context)
    {
        carHandler.isBoosting = true;
    }
    private void OnBoostEnd(InputAction.CallbackContext context)
    {
        carHandler.isBoosting = false;
    }
    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        carHandler.SetInput(input);
    }
}
