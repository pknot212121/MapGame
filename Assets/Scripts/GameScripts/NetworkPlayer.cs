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
using System.Linq;
using UnityEngine.UI;

public class NetworkPlayer : NetworkBehaviour
{
    [Networked, Capacity(32)]
    public NetworkString<_32> Nickname { get; private set; } = "";
    public override void Spawned()
    {
        if (Object.HasStateAuthority){
            Debug.Log("GRACZ: "+Runner.LocalPlayer+" ZESPAWNOWALNY");
        // if(NetworkManagerGame.Instance!= null && Nickname=="")
        }
        
        // {
        //     string chosenNickname = PlayerPrefs.GetString("PlayerNickname", $"Player_{UnityEngine.Random.Range(100, 999)}");
        //     Debug.Log("USTAWIONO NICKNAME GRACZA: "+Runner.LocalPlayer+" JAKO: "+chosenNickname+" W SPAWNED");
        //     Rpc_SetNickname(chosenNickname);
        // }
    }
    public void Update()
    {
        if(Object.HasStateAuthority && Nickname=="")
        {
            string chosenNickname = PlayerPrefs.GetString("PlayerNickname", $"Player_{UnityEngine.Random.Range(100, 999)}");
            Debug.Log("USTAWIONO NICKNAME GRACZA: "+Runner.LocalPlayer+" JAKO: "+chosenNickname+" W UPDATE");
            Rpc_SetNickname(chosenNickname);
        }
        
    }

    [Rpc(RpcSources.All,RpcTargets.StateAuthority)]
    public void RPC_EndTurn()
    {
        if(NetworkManagerGame.Instance.ActivePlayer==Runner.LocalPlayer && Runner.ActivePlayers.ToList().Count()>1)
        {
            foreach(PlayerRef player in Runner.ActivePlayers.ToList())
            {
                if(player!=Runner.LocalPlayer)
                {
                    NetworkManagerGame.Instance.ActivePlayer = player;
                    Debug.Log("Koniec Tury gracza: "+Runner.LocalPlayer);
                    Debug.Log("Początek tury gracza: "+player);
                }
            }
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_SetNickname(NetworkString<_32> nickname, RpcInfo info = default)
    {
        string validatedNick = nickname.Value.Trim();
        Nickname = validatedNick;
        if (NetworkManagerGame.Instance != null)
        {
            NetworkManagerGame.Instance.PlayerNicknames.Set(info.Source, validatedNick);
            Log.Info($"RPC: Zaktualizowano GameManager.PlayerNicknames dla {info.Source}.");
        }
         else
        {
            Log.Error("RPC: GameManager.Instance jest null, nie można zaktualizować centralnego słownika nicków.");
        }
    }

    /*[Rpc]
    public void Rpc_ChangeProvinceOwnership(NetworkString<_32> winningSideName, NetworkString<_32> provinceName)
    {
        Map currentMap = GameController.me.map;
        Country winningSide = currentMap.GetCountry(winningSideName.Value);
        Province province = currentMap.GetProvince(provinceName.Value);
        Country losingSide = currentMap.GetCountry(province);
        foreach(Country country in currentMap.countries)
        {
            if(country.name == losingSide.name) country.provinces.Remove(province);
            if(country.name == winningSideName.ToString()) country.provinces.Add(province);
        }
        foreach(ProvinceGameObject provinceGameObject in NetworkManagerGame.Instance.provinceGameObjects)
        {
            if(provinceGameObject.data.name == province.name) provinceGameObject.SetColor(winningSide.color);
            Debug.Log("Podbito: "+ province.name);
        }
    }*/
}