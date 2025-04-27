using System;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using TMPro;
using System.Text;

public class GameManager : NetworkBehaviour, INetworkRunnerCallbacks
{
    [Networked,Capacity(20)] 
    public NetworkDictionary<PlayerRef, NetworkString<_32>> PlayersToCountries { get; } = new
        NetworkDictionary<PlayerRef, NetworkString<_32>>();
    public static GameManager Instance{get;private set;}
    public Map CurrentMapData { get; private set; }
    public bool IsMapDataReady { get; private set; } = false;
    public event Action OnMapDataReady;

    public override void Spawned()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        Runner.AddCallbacks(this);
    }

    public async void CreateSession(string name, string filename)
    {

    }

    public async void ConnectToSession(SessionInfo session) 
    {

    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log($"Rooms list update (Count: {sessionList.Count})");

        if (JoinGameMenuController.me) JoinGameMenuController.me.UpdateGamesList(sessionList);
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("Connected to server");
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.LogWarning($"OnShutdown: {shutdownReason}");
    }


    //public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    //public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) {

     }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_SetPlayerCountry(PlayerRef player,NetworkString<_32> countryName)
    {
        PlayersToCountries.Set(player,countryName);
    }

    public void SetMapData(Map mapData)
    {
        CurrentMapData = mapData;
        IsMapDataReady = true;
        Debug.Log("Map data set in GameManager.");
        OnMapDataReady?.Invoke(); // Wywołaj event, jeśli ktoś nasłuchuje
    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) {
        if (Runner.IsSharedModeMasterClient) PlayersToCountries.Remove(player);
     }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) 
    { 
        
    }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
}