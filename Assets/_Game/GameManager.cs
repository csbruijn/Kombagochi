using Rive.Components;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance {  get; private set; }

    public GameObject Ripple { get; private set; }

    public bool GameStarted { get; private set; } = true; 

    private void Awake()
    {
        if (instance == null) instance = this;
        else  Destroy(this); 
    }

    public void SetRipple(GameObject rw)
    {
        Ripple = rw;
    }

    public void SceneReset() =>
        SceneManager.LoadScene(1);
}
