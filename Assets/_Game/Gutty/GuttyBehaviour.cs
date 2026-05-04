using UnityEngine;

public class GuttyBehaviour : MonoBehaviour
{


    private void OnCollisionEnter2D(Collision2D collision)
    {
        NutriBehaviour nb = collision.gameObject.GetComponent<NutriBehaviour>();

        if (nb == null)
        {
            Debug.Log("non nutri collsion");
            return;
        }
            HandleNutriMatch(nb);
    }


    private void HandleNutriMatch(NutriBehaviour nb)
    {
        Debug.Log("Eat nutri"); 
        Destroy(nb.gameObject);
    }
}
