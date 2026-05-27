using Rive;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MicrobeeSimulated : MonoBehaviour
{
    private ViewModelInstanceNumberProperty EyeX;
    private ViewModelInstanceNumberProperty EyeY;
    [SerializeField] private string EyeXName = "eyeX";
    [SerializeField] private string EyeYName = "eyeY";

    bool guttyVisLoaded = false;

    private ViewModelInstanceColorProperty primaryColour;
    [SerializeField] private string colourName = "primaryColour";

    [Header("Dumb way to do colours to check if it works")]

    [SerializeField] private UnityEngine.Color red;
    [SerializeField] private UnityEngine.Color green;
    [SerializeField] private UnityEngine.Color blue;
    [SerializeField] private UnityEngine.Color yellow;

    private void Update()
    {
        if (!IsReady()) return;
        UpdateEyes();
    }

    private void UpdateEyes()
    {
        if (EyeX == null || EyeY == null) return;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 dir = (Vector2)worldPos - (Vector2)transform.position;

        EyeX.Value = dir.x - 10;
        EyeY.Value = -dir.y + 10;
    }



    public bool IsReady()
    {
        if (!guttyVisLoaded) return false;
        return true;
    }
}
