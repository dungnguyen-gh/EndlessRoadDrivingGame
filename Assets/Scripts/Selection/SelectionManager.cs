using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{
    public GameObject[] cars;
    private int currentCarIndex = 0;
    private PlayerInputAction inputActions;
    //ui
    [SerializeField] Button prevButton;
    [SerializeField] Button nextButton;
    [SerializeField] Button startButton;

    //animation
    [SerializeField] float transitionDuration = 0.5f;
    [SerializeField] float transitionDistance = 10f;
     
    private bool isSliding = false;

    [SerializeField] float rotateSpeed = 20f;

    private void Awake()
    {
        inputActions = new PlayerInputAction();

        inputActions.PlayerInput.Exit.performed += OnExitGame;
    }

    // Start is called before the first frame update
    void Start()
    {
        //deactive all cars, active the first one
        for (int i = 0; i < cars.Length; i++)
        {
            cars[i].SetActive(i == currentCarIndex);
        }

        prevButton.onClick.AddListener(() => ShowPreviousCar());
        nextButton.onClick.AddListener(() => ShowNextCar());

        startButton.onClick.AddListener(StartCar);
    }
    private void Update()
    {
        if (!isSliding) 
        { 
            RotateSelectedCar();
        }
    }
    public void StartCar()
    {
        //save selected car in PlayerPrefs
        PlayerPrefs.SetInt("SelectedCarIndex", currentCarIndex);
        PlayerPrefs.Save();

        //load scene
        SceneManager.LoadScene("DrivingScene");
    }
    public void ShowNextCar()
    {
        StartCoroutine(SlideCarList(1));
        //cars[currentCarIndex].SetActive(false);
        //currentCarIndex = (currentCarIndex + 1) % cars.Length;
        //cars[currentCarIndex].SetActive(true);
    }
    public void ShowPreviousCar()
    {
        StartCoroutine(SlideCarList(-1));
        //cars[currentCarIndex].SetActive(false);
        //currentCarIndex = (currentCarIndex - 1 + cars.Length) % cars.Length;
        //cars[currentCarIndex].SetActive(true);
    }
    private IEnumerator SlideCarList(int direction)
    {
        //prevent multiple slidings
        if (isSliding) yield break;

        isSliding = true;

        // get next car based on direction
        int nextIndex = (currentCarIndex + direction + cars.Length) % cars.Length;


        //set next car active and position it off-screen
        cars[nextIndex].SetActive(true);
        cars[nextIndex].transform.localPosition = new Vector3(-direction * transitionDistance, 0, 0);

        float elapsedTime = 0;

        // get current and next transform
        Transform currentCar = cars[currentCarIndex].transform;
        Transform nextCar = cars[nextIndex].transform;

        //slide animation with lerp
        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / transitionDuration;

            //switching position
            currentCar.localPosition = Vector3.Lerp(Vector3.zero, new Vector3(direction * transitionDistance, 0, 0), t);
            nextCar.localPosition = Vector3.Lerp(new Vector3(-direction * transitionDistance, 0, 0), Vector3.zero, t);

            yield return null;
        }

        //set position after transition
        currentCar.localPosition = new Vector3(direction * transitionDistance, 0, 0);
        nextCar.localPosition = Vector3.zero;

        //deactive previous car
        cars[currentCarIndex].SetActive(false);

        //reset rotation of current car
        cars[currentCarIndex].transform.localRotation = Quaternion.Euler(0, 210f, 0);

        //update index
        currentCarIndex = nextIndex;

        isSliding = false;
    }
    private void RotateSelectedCar()
    {
        if (cars[currentCarIndex].activeSelf)
        {
            cars[currentCarIndex].transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
        }
    }
    private void OnExitGame(InputAction.CallbackContext context)
    {
        Application.Quit();
    }
    public void Exit()
    {
        Application.Quit();
    }
}
