using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeDisplay : MonoBehaviour
{

    private TextMeshProUGUI mTMP;
    private float timer = 12*60;

    void Start()
    {
        mTMP = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        int minutes = Mathf.FloorToInt(timer % 60);
        int hours24 = Mathf.FloorToInt((timer / 60) % 24);

        string amPm = hours24 >= 12 ? "PM" : "AM";

        int hours12 = hours24 % 12;

        if (hours12 == 0)
            hours12 = 12;

        mTMP.text = $"{hours12:00}:{minutes:00} {amPm}";
    }   
}
