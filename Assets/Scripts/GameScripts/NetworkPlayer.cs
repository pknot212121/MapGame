using System;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using TMPro;
using System.Text;
using Unity.VisualScripting;
using UnityEngine.Assertions.Must;

public class NetworkPlayer : NetworkBehaviour
{
    private bool mapRelatedInitializationDone = false;

    [Networked, Capacity(32)]
    public NetworkString<_32> Nickname { get; private set; }
    public override void Spawned()
    {
        string chosenNickname = PlayerPrefs.GetString("PlayerNickname", $"Player_{UnityEngine.Random.Range(100, 999)}");
        Rpc_SetNickname(chosenNickname);

        TryInitializeWithMapData();
    }

    void Update()
    {
        if (!mapRelatedInitializationDone)
        {
            TryInitializeWithMapData();
        }
        else TryChangingOwnerhipOfAProvince();
        
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            foreach (var entry in GameManager.Instance.PlayersToCountries)
            {
                Debug.Log($"Player {entry.Key}: {entry.Value}");
            }
            foreach (var entry in GameManager.Instance.PlayerNicknames)
            {
                Debug.Log($"Player {entry.Key}: {entry.Value}");
            }
        }
        
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_SetNickname(NetworkString<_32> nickname, RpcInfo info = default)
    {
        string validatedNick = nickname.Value.Trim();
        Nickname = validatedNick;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerNicknames.Set(info.Source, validatedNick);
            Log.Info($"RPC: Zaktualizowano GameManager.PlayerNicknames dla {info.Source}.");
        }
         else
        {
            Log.Error("RPC: GameManager.Instance jest null, nie można zaktualizować centralnego słownika nicków.");
        }
    }

    [Rpc]
    public void Rpc_ChangeProvinceOwnership(NetworkString<_32> winningSideName, NetworkString<_32> provinceName)
    {
        Transform provinceParentObjectTransform = GameObject.Find("Provinces").transform;
        Map currentMap = GameManager.Instance.CurrentMapData;
        Country winningSide = currentMap.GetCountry(winningSideName.ToString());
        Province province = currentMap.GetProvince(provinceName.ToString());
        Country losingSide = currentMap.GetCountry(province);
        foreach(Country country in currentMap.countries)
        {
            if(country.name == losingSide.name) country.provinces.Remove(province);
            if(country.name == winningSideName.ToString()) country.provinces.Add(province);
        }
        foreach(ProvinceGameObject provinceGameObject in GameManager.Instance.provinceGameObjects)
        {
            if(provinceGameObject.data.name == province.name) provinceGameObject.SetColor(winningSide.color);
            Debug.Log("Sprawdzam: "+ province.name);
        }
        
    }
    
    void TryInitializeWithMapData()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsMapDataReady)
        {
            Map currentMap = GameManager.Instance.CurrentMapData;
            // foreach(Country country in currentMap.countries)
            // {
            //     if(!GameManager.Instance.PlayersToCountries.ContainsValue(country.name))
            //     {
            //         GameManager.Instance.Rpc_SetPlayerCountry(
            //         Object.Runner.LocalPlayer, 
            //         country.name);
            //     }
            //     break;
            // // }
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if(Input.GetMouseButtonDown(0))
            {
                foreach(Province province in currentMap.provinces)
                {
                    if(ShapeTools.IsPointInPolygon((Vector2)worldPosition, province.points.ToArray()))
                    {
                        GameManager.Instance.Rpc_SetPlayerCountry(
                        Object.Runner.LocalPlayer, 
                        currentMap.GetCountry(province).name);
                        mapRelatedInitializationDone = true;
                        Debug.Log("Państwo wybrane!");
                        break;
                    }
                }
            }
        }
    }
    void TryChangingOwnerhipOfAProvince()
    {
        if(GameManager.Instance != null && GameManager.Instance.IsMapDataReady)
        {
            Map currentMap = GameManager.Instance.CurrentMapData;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if(Input.GetMouseButtonDown(0))
            {
                foreach(Province province in currentMap.provinces)
                {
                    if(ShapeTools.IsPointInPolygon((Vector2)worldPosition, province.points.ToArray()))
                    {
                        NetworkString<_32> countryName = GameManager.Instance.PlayersToCountries[Runner.LocalPlayer];
                        Rpc_ChangeProvinceOwnership(countryName,province.name);
                        break;
                    }
                }
            }
            
        }
    }
}