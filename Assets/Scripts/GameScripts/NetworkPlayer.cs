using System;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using TMPro;
using System.Text;

public class NetworkPlayer : NetworkBehaviour
{
    private bool mapRelatedInitializationDone = false;
    public override void Spawned()
    {
        TryInitializeWithMapData();
    }

    void Update()
    {
        if (!mapRelatedInitializationDone)
        {
            TryInitializeWithMapData();
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            foreach (var entry in GameManager.Instance.PlayersToCountries)
            {
                Debug.Log($"Player {entry.Key}: {entry.Value}");
            }
        }
        
    }
    void TryInitializeWithMapData()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsMapDataReady)
        {
            Map currentMap = GameManager.Instance.CurrentMapData;
            foreach(Country country in currentMap.countries)
            {
                if(!GameManager.Instance.PlayersToCountries.ContainsValue(country.name))
                {
                    GameManager.Instance.Rpc_SetPlayerCountry(
                    Object.Runner.LocalPlayer, 
                    country.name);
                }
                break;
            }
            mapRelatedInitializationDone = true;
        }
    }
}