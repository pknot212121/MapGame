using System;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class NetworkController : MonoBehaviour, INetworkRunnerCallbacks
{
    SceneRef gameSceneRef;
    private NetworkRunner roomsListRunner;
    private NetworkRunner runnerInstance;
    [SerializeField] GameObject networkRunnerObject;
    public static NetworkController me;

    async void Start()
    {
        me = this;
        DontDestroyOnLoad(this);
        gameSceneRef = SceneRef.FromIndex(SceneUtility.GetBuildIndexByScenePath("Game"));
        
        roomsListRunner = gameObject.AddComponent<NetworkRunner>();
        roomsListRunner.ProvideInput = false;
        roomsListRunner.AddCallbacks(this);
        roomsListRunner.gameObject.AddComponent<NetworkSceneManagerDefault>();

        await roomsListRunner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = "",
            SceneManager = roomsListRunner.GetComponent<INetworkSceneManager>()
        });
    }

    public async void HostGame(string name)
    {
        runnerInstance = networkRunnerObject.AddComponent<NetworkRunner>();
        runnerInstance.AddCallbacks(this);
        networkRunnerObject.AddComponent<NetworkSceneManagerDefault>();

        if (roomsListRunner != null && roomsListRunner.IsRunning) await roomsListRunner.Shutdown();

        await runnerInstance.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Host,
            SessionName = name,
            Scene = gameSceneRef,
            SceneManager = networkRunnerObject.GetComponent<INetworkSceneManager>()
        });
    }

    public async void JoinGame(SessionInfo session)
    {
        if (!session.IsValid)
        {
            Debug.LogError("Invalid session");
        }
        if (runnerInstance != null && runnerInstance.IsRunning)
        {
            await runnerInstance.Shutdown();
            Destroy(runnerInstance.gameObject);
        }

        runnerInstance = networkRunnerObject.AddComponent<NetworkRunner>();
        runnerInstance.AddCallbacks(this);
        networkRunnerObject.AddComponent<NetworkSceneManagerDefault>();

        if (roomsListRunner != null && roomsListRunner.IsRunning) await roomsListRunner.Shutdown();

        var result = await runnerInstance.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Client,
            SessionName = session.Name,
            SceneManager = runnerInstance.GetComponent<INetworkSceneManager>()
        });

        if (result.Ok)
        {
            Debug.Log($"Joined session: {session.Name}");
        }
        else
        {
            Debug.LogError($"Failed to join session: {result.ShutdownReason}");
        }
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        if(JoinGameMenuController.me) JoinGameMenuController.me.UpdateGamesList(sessionList);
    }

    public void OnConnectedToServer(NetworkRunner runner) {}
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) {}
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) {}
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) {}
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) {}
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) {}
    public void OnInput(NetworkRunner runner, NetworkInput input) {}
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) {}
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) {}
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) {}
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, byte[] data) {}
    public void OnSceneLoadDone(NetworkRunner runner) {}
    public void OnSceneLoadStart(NetworkRunner runner) {}
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) {}
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) {}
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) {}
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) {}
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) {}
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) {}
}
