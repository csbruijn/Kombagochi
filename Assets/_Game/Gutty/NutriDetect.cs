using System;
using UnityEngine;
using UnityEngine.Events;

public class NutriDetect : MonoBehaviour
{
    public static event Action<NutriBehaviour> OnEaten; // shared across all eaters

    private void OnCollisionEnter2D(Collision2D collision)
    {
        NutriBehaviour nb = collision.gameObject.GetComponent<NutriBehaviour>();

        if (nb == null)
            return;

        OnEaten?.Invoke(nb);
    }



}
