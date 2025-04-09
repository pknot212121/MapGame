using UnityEngine;
using UnityEngine.SceneManagement;

public class JoinGameMenuController : MonoBehaviour
{
    public void BackClick()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
}
