using Rive.Components;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance {  get; private set; }

    public GameObject Ripple { get; private set; }

    private void Awake()
    {
        if (instance == null) instance = this;
        else  Destroy(this); 
    }

    public void SetRipple(GameObject rw)
    {
        Ripple = rw;
    }

}
