using System;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using System.IO;
using System.Text;



public class NetworkManagerJoin : MonoBehaviour, INetworkRunnerCallbacks
{
    SceneRef gameSceneRef;
    private NetworkRunner runner;
    public static NetworkManagerJoin me;

    [Header("Game Prefabs")]
    public GameObject sessionManagerPrefab;
    public NetworkPrefabRef playerPrefab;



    public NetworkRunner GetRunner()
    {
        return runner;
    }

    async void Start()
    {
        me = this;
        DontDestroyOnLoad(this);
        gameSceneRef = SceneRef.FromIndex(SceneUtility.GetBuildIndexByScenePath("Game"));
        await JoinLobby();
    }

    public async Task JoinLobby()
    {
        if (runner == null) runner = gameObject.AddComponent<NetworkRunner>();
        await runner.JoinSessionLobby(SessionLobby.Shared);
    }


    public async void RefreshRoomList()
    {
        if (runner != null && runner.IsRunning) Debug.Log("Room list will be updated automatically");
        else await JoinLobby();
    }

    public async void CreateSession(string name)
    {
        int num = UnityEngine.Random.Range(1000, 9999);
        if (runner == null) runner = gameObject.AddComponent<NetworkRunner>();
        await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = name,
            PlayerCount = 20,
            Scene = gameSceneRef
        });
    }

    public async void ConnectToSession(SessionInfo session) 
    {
        if (runner == null) runner = gameObject.AddComponent<NetworkRunner>();
        await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = session.Name,
            Scene = gameSceneRef
        });
    }

    public async void LeaveRoom()
    {
        if (runner != null)
        {
            await runner.Shutdown();
        }
        SceneManager.LoadScene("JoinGameMenu");
        await JoinLobby();
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        if (JoinGameMenuController.me) JoinGameMenuController.me.UpdateGamesList(sessionList);
    }
    public void OnConnectedToServer(NetworkRunner runner){ }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason){ }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsSharedModeMasterClient)
        {
            Debug.Log("SPAWNUJE GRACZA: "+player);
            runner.Spawn(playerPrefab, Vector3.zero, Quaternion.identity, player);
            if (NetworkManagerGame.Instance == null)
            {
                
                runner.Spawn(sessionManagerPrefab,Vector3.zero, Quaternion.identity);
            }
        }
     }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
}
