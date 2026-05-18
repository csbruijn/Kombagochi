using System;
using UnityEngine;
using UnityEngine.Events;

public class NutriDetect : MonoBehaviour
{
    public static event Action<NutriBehaviour, GuttyBehaviour> OnEaten;
    private NutriType m_nutriCompetibility;
    private GuttyBehaviour gb;

    private void Start()
    {
        if (TryGetComponent<GuttyBehaviour>(out gb)) m_nutriCompetibility = gb.nutriCompetibility;
        else Debug.LogError($"Component GuttyBehaviour  '{gb}' not found.\", this");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        NutriBehaviour nb = collision.gameObject.GetComponent<NutriBehaviour>();

        if (nb == null)
            return;

        if (nb.nutriType != m_nutriCompetibility)
            return; 

        OnEaten?.Invoke(nb, gb);
    }
}
