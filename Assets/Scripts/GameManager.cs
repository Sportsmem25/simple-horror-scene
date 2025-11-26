using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerMovement player;
    [SerializeField] private ChaseManager chaseManager;

    private void Update()
    {
        if (Input.GetKey(KeyCode.P))
        {
            Application.Quit();
            Debug.Log("Exit Game");
        }
    }

    public void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}