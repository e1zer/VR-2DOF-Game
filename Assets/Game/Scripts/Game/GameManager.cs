using LogitechG29.Sample.Input;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private InputControllerReader inputControllerReader;

    private void Update()
    {
        if (inputControllerReader.EastButton)
            RestartGame();
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
