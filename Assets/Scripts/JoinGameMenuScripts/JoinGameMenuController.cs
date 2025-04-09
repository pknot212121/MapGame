using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;

public class JoinGameMenuController : MonoBehaviour
{
    public void StartGameClick()
    {

    }

    public void BackClick()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
}
