using Rive.Components;
using UnityEngine;

public class Ripple : MonoBehaviour
{
    private void Start()
    {
        GameManager.instance.SetRipple(gameObject);
    }
}
