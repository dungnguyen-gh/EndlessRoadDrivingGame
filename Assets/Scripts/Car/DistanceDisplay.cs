using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DistanceDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text distanceText;

    //display distance
    public void UpdateDistanceText(float distance)
    {
        distanceText.text = $"Distance traveled: {distance:F1} m";
    }
}
