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
    public Button endTurnButton;
    public TMP_Text startCounter;
    public TMP_Text playersCountDisplayer;
    public NetworkPlayer networkPlayer;

    void Start()
    {
        me = this;
        // startButton.gameObject.SetActive(false);
        // startCounter.gameObject.SetActive(false);

    }

    void Update()
    {
        if(networkPlayer != null && networkPlayer.Runner != null)
        {
            if (anteroomCanvas != null) TrySettingCountryForPlayer();
            else TryChangingOwnerhipOfAProvince();
        }
    }

    public void EndTurnClick()
    {
        Debug.Log("Próba zakończenia tury");
        {
            networkPlayer.RPC_EndTurn();
        }
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
        NetworkManagerJoin.me.LeaveRoom();
    }

    // Obsługuje odliczanie do startu
    public IEnumerator CountingDownToStart()
    {
        startCounter.gameObject.SetActive(true);
        while(NetworkManagerGame.Instance.StartTimer != 10 && NetworkManagerGame.Instance.StartTimer != -1)
        {
            startCounter.text = "Starting in: " + NetworkManagerGame.Instance.StartTimer;
            yield return null;
        }
        if(NetworkManagerGame.Instance.StartTimer == 10) startCounter.gameObject.SetActive(false);
        if(NetworkManagerGame.Instance.StartTimer == -1)
        {
            anteroomCanvas.gameObject.SetActive(false);
            gameplayCanvas.gameObject.SetActive(true);
        }
    }
    
    void TryChangingOwnerhipOfAProvince()
    {
        if(NetworkManagerGame.Instance != null && NetworkManagerGame.Instance.IsMapDataReady)
        {
            Map currentMap = NetworkManagerGame.Instance.CurrentMapData;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if(Input.GetMouseButtonDown(0))
            {
                foreach(Province province in currentMap.provinces)
                {
                    if(ShapeTools.IsPointInPolygon((Vector2)worldPosition, province.points.ToArray()))
                    {
                        NetworkString<_32> countryName = NetworkManagerGame.Instance.PlayersToCountries[networkPlayer.Runner.LocalPlayer];
                        networkPlayer.Rpc_ChangeProvinceOwnership(countryName,province.name);
                        break;
                    }
                }
            }
            
        }
    }

    void TrySettingCountryForPlayer()
    {
        if (NetworkManagerGame.Instance != null && NetworkManagerGame.Instance.IsMapDataReady)
        {
            Map currentMap = NetworkManagerGame.Instance.CurrentMapData;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if(Input.GetMouseButtonDown(0))
            {
                foreach(Province province in currentMap.provinces)
                {
                    if(ShapeTools.IsPointInPolygon((Vector2)worldPosition, province.points.ToArray()))
                    {
                        string countryName = currentMap.GetCountry(province).name;
                        PlayerRef owner = NetworkManagerGame.Instance.GetOwnerOfACountry(countryName);
                        if(owner==PlayerRef.None)
                        {
                            NetworkManagerGame.Instance.Rpc_SetPlayerCountry(
                            networkPlayer.Runner.LocalPlayer, 
                            currentMap.GetCountry(province).name);
                            NetworkManagerGame.Instance.PrintCountryOwners();
                        }
                        break;
                    }
                }
            }
        }
    }
}