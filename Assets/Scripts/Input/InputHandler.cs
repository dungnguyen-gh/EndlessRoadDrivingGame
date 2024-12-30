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
        inputActions.PlayerInput.Exit.performed += OnExitGame;
        inputActions.PlayerInput.Pause.performed += OnPauseGame;
    }

    private void OnEnable() => inputActions?.Enable();
    private void OnDisable() => inputActions?.Disable();

    private void OnResetGame(InputAction.CallbackContext context)
    {
        InitializationManager.Instance.ResetGame();
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
    private void OnExitGame(InputAction.CallbackContext context)
    {
        Application.Quit();
    }
    private void OnPauseGame(InputAction.CallbackContext context)
    {
        InitializationManager.Instance.PauseToggle();
    }
}
