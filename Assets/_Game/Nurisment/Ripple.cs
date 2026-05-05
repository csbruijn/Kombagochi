using Rive.Components;
using UnityEngine;

public class Ripple : MonoBehaviour
{
    private void Awake()
    {
        GameManager.instance.SetRipple(gameObject);
    }
}
