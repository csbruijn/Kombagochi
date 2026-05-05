using Rive.Components;
using UnityEngine;

public class NutriBehaviour : MonoBehaviour
{
    [SerializeField] private RiveWidget ripple; 

    [SerializeField] private float ripplePushCD = .5f;
    private float timer = 0f;


    private void FixedUpdate()
    {
        if (timer < 0f) timer -= Time.deltaTime;
    }
    public void HandleRipplePush()
    {
        if (timer > 0f) return;


        // trigger the rive widget
         
        // set the CD

        timer = ripplePushCD; 


    }
}
