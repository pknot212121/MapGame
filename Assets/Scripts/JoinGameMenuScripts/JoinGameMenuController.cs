using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Fusion;
using System.IO;
using System.Linq;

public class JoinGameMenuController : MonoBehaviour
{
    public List<SessionInfo> currentSessionList = new List<SessionInfo>();
    public Transform gamesListContentTransform;
    public static JoinGameMenuController me;

    [SerializeField] private TMP_InputField gameNameInput;

    [SerializeField] private TMP_InputField mapNameInput;

    void Start()
    {
        me = this;
        if(PlayerPrefs.HasKey("mapName")) mapNameInput.text = PlayerPrefs.GetString("mapName");
    }

    [UnityEngine.Scripting.Preserve] public void StartGameClick()
    {
        string name = "";
        if(gameNameInput.text.Length < 2)
        {
            var random = new System.Random();
            char[] chars = {'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};
            for (int i = 0; i < 6; i++)
            {
                name += chars[random.Next(chars.Length)];
            }
        }
        else name = gameNameInput.text;
        NetworkManagerJoin.me.CreateSession(name);
    }

    public void RefreshClick()
    {
        NetworkManagerJoin.me.RefreshRoomList();
    }

    public void ChooseMapClick(){
        bool isFileFound = false;
        if(mapNameInput.text.Length >= 2)
        {
            foreach(string filePath in Directory.EnumerateFiles(Application.persistentDataPath,"*.json"))
            {
                string fileName = Path.GetFileName(filePath);
                if(mapNameInput.text + ".json"==fileName)
                {
                    isFileFound = true;
                    Debug.Log("Znaleziono mapę!");
                    PlayerPrefs.SetString("mapName", mapNameInput.text);
                    PlayerPrefs.Save();
                }
            }
            if(!isFileFound)
            {
                Debug.Log("Nie znaleziono mapy!");
                return;
            }
            
        }
        else Debug.Log("Nazwa musi mieć conajmniej 2 znaki!");
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
