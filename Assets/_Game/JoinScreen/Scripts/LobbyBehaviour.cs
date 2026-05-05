using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyBehaviour : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnGameStart()
    {
        Debug.Log("StartGame");

        SceneManager.LoadScene(1); 
    }
}
