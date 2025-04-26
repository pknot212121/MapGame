using UnityEngine;
using Fusion;
using System.Collections.Generic;
using System;
public class GameController : MonoBehaviour
{
    void Start()
    {
        
    }
    void Update()
    {
        if(PlayerController.Instance!=null && Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("Wartość: "+PlayerController.Instance.playersToCountries.Count);
        }
    }

}