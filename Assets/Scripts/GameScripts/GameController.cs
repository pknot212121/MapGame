using UnityEngine;
using Fusion;
using System.Collections.Generic;
using System;
public class GameController : MonoBehaviour
{
    public static GameController me;

    public Transform playerNicknameDisplayersTransform;

    void Start()
    {
        me = this;
    }
}