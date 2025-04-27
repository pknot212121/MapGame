using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Fusion;
using System.IO;
using System.Linq;


public class MenuController : MonoBehaviour
{
    [SerializeField] private TMP_InputField nicknameInput;

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

    public void NicknameButtonClicked()
    {
        if(nicknameInput.text.Length >= 2)
        {
            PlayerPrefs.SetString("PlayerNickname", nicknameInput.text);
            PlayerPrefs.Save();
        }
        else Debug.Log("Nickname musi mieć conajmniej dwa znaki!");
    }
}
