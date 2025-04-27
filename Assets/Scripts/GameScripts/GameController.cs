using UnityEngine;
using Fusion;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController me;

    public Transform playerNicknameDisplayersTransform;
    public Button startButton;

    void Start()
    {
        me = this;
        startButton.interactable = false;
    }
}