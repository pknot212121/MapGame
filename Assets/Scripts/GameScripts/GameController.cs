using UnityEngine;
using Fusion;
using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController me;

    public Canvas anteroomCanvas;
    public Canvas gameplayCanvas;

    public Transform playerNicknameDisplayersTransform;
    public Button startButton;

    public TMP_Text startCounter;
    public TMP_Text playersCountDisplayer;

    void Start()
    {
        me = this;
        startButton.gameObject.SetActive(false);
        startCounter.gameObject.SetActive(false);
    }

    public void UpdatePlayersCountDisplayer(int current, int max)
    {
        playersCountDisplayer.text = $"{current}/{max}";
    }

    public void StartClick()
    {

    }

    public void ReturnToLobbyClick()
    {
        NetworkController.me.LeaveRoom();
    }

    // Obsługuje odliczanie do startu
    public IEnumerator CountingDownToStart()
    {
        startCounter.gameObject.SetActive(true);
        while(GameManager.Instance.StartTimer != 10 && GameManager.Instance.StartTimer != -1)
        {
            startCounter.text = "Starting in: " + GameManager.Instance.StartTimer;
            yield return null;
        }
        if(GameManager.Instance.StartTimer == 10) startCounter.gameObject.SetActive(false);
        if(GameManager.Instance.StartTimer == -1)
        {
            anteroomCanvas.gameObject.SetActive(false);
            gameplayCanvas.gameObject.SetActive(true);
        }
    }
}