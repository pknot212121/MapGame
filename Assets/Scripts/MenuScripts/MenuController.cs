using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void JoinGameClick()
    {
        SceneManager.LoadScene("JoinGameMenu", LoadSceneMode.Single);
    }

    public void MapCreatorClick()
    {
        SceneManager.LoadScene("MapCreator", LoadSceneMode.Single);
    }

    public void QuitGameClick()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
