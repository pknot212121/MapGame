using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Fusion;

public class JoinGameMenuController : MonoBehaviour
{
    public List<SessionInfo> currentSessionList = new List<SessionInfo>();
    public Transform gamesListContentTransform;
    public static JoinGameMenuController me;

    [SerializeField] private TMP_InputField gameNameInput;

    void Start()
    {
        me = this;
    }

    public void StartGameClick()
    {
        string name = "";
        if(gameNameInput.text.Length < 6)
        {
            var random = new System.Random();
            char[] chars = {'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};
            for (int i = 0; i < 6; i++)
            {
                name += chars[random.Next(chars.Length)];
            }
        }
        else name = gameNameInput.text;
        NetworkController.me.HostGame(name);
    }

    public void BackClick()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }

    public void UpdateGamesList(List<SessionInfo> sessionList)
    {
        foreach (Transform child in gamesListContentTransform) { Destroy(child.gameObject); }

        GameObject prefab = Resources.Load<GameObject>("Prefabs/GamePanelPrefab");
        foreach(var session in sessionList)
        {
            Debug.Log(session.Name);
            GameObject panel = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            panel.GetComponent<GamePanelPrefab>().Initialise(session);
            panel.transform.SetParent(gamesListContentTransform);
        }
    }
}
