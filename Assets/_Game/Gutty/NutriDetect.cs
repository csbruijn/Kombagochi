using UnityEngine;

public class NutriDetect : MonoBehaviour
{
    public GuttyBehaviour GB; 

    private void OnCollisionEnter2D(Collision2D collision)
    {
        NutriBehaviour nb = collision.gameObject.GetComponent<NutriBehaviour>();

        if (nb == null)
        {
            Debug.Log("non nutri collsion");
            return;
        }
            GB.HandleNutriMatch(nb);
    }



}
