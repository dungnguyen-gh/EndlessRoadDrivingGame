using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputHandler : MonoBehaviour
{
    [SerializeField] CarHandler carHandler;

    private void Awake()
    {
        if (!CompareTag("Player"))
        {
            Destroy(this);
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 input = Vector2.zero;

        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");

        carHandler.SetInput(input);

        //reset game
        if (Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        // set boosting state
        carHandler.isBoosting = Input.GetKey(KeyCode.Space);
    }
}
