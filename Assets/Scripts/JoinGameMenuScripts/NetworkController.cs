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
    private NetworkRunner runner;
    public static NetworkController me;

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
        if (runner != null && runner.IsRunning)
        {
            Debug.Log("Room list will be updated automatically");
        }
        else
        {
            await JoinLobby();
        }
    }

    public async void CreateSession(string name)
    {
        int num = UnityEngine.Random.Range(1000, 9999);
        if (runner == null) runner = gameObject.AddComponent<NetworkRunner>();
        await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = name,
            PlayerCount = 6
        });
    }

    public async void ConnectToSession(SessionInfo session) 
    {
        if (runner == null) runner = gameObject.AddComponent<NetworkRunner>();
        await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = session.Name,
        });
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
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
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

/*public class NetworkController : MonoBehaviour, INetworkRunnerCallbacks
{
    SceneRef gameSceneRef;
    private NetworkRunner roomsListRunner;
    private NetworkRunner runnerInstance;
    [SerializeField] GameObject networkRunnerObject;
    public static NetworkController me;
    private string currentLobbyId = "default";

    async void Start()
    {
        me = this;
        DontDestroyOnLoad(this);
        gameSceneRef = SceneRef.FromIndex(SceneUtility.GetBuildIndexByScenePath("Game"));
        await JoinLobby();
    }

    public async Task JoinLobby(string lobbyId = null)
{
    if (!string.IsNullOrEmpty(lobbyId)) currentLobbyId = lobbyId;

    if (roomsListRunner == null)
    {
        roomsListRunner = gameObject.AddComponent<NetworkRunner>();
        roomsListRunner.ProvideInput = false;
        roomsListRunner.AddCallbacks(this);
        if (roomsListRunner.GetComponent<NetworkSceneManagerDefault>() == null)
            roomsListRunner.gameObject.AddComponent<NetworkSceneManagerDefault>();
    }

    if (!roomsListRunner.IsRunning)
    {
        var lobbyResult = await roomsListRunner.JoinSessionLobby(SessionLobby.Shared, currentLobbyId);
        if (!lobbyResult.Ok)
        {
            Debug.LogError("❌ Failed to join session lobby: " + lobbyResult.ShutdownReason);
            return;
        }

        var startResult = await roomsListRunner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = "LobbyWatcher_" + SystemInfo.deviceUniqueIdentifier.Substring(0, 8),
            SceneManager = roomsListRunner.GetComponent<INetworkSceneManager>()
        });

        if (!startResult.Ok)
        {
            Debug.LogError("❌ Failed to start roomsListRunner in Shared mode: " + startResult.ShutdownReason);
        }
        else
        {
            Debug.Log("✅ Started roomsListRunner and joined session lobby");
        }
    }
}


    public async void RefreshRoomList()
    {
        Debug.Log((roomsListRunner != null) + " : " + roomsListRunner.IsRunning);
        if (roomsListRunner != null && roomsListRunner.IsRunning)
        {
            Debug.Log("Room list will be updated automatically");
        }
        else
        {
            await JoinLobby();
        }
    }

    public async void HostGame(string name)
    {
        runnerInstance = networkRunnerObject.GetComponent<NetworkRunner>();
        if (runnerInstance == null) runnerInstance = networkRunnerObject.AddComponent<NetworkRunner>();
        runnerInstance.AddCallbacks(this);
        if (networkRunnerObject.GetComponent<NetworkSceneManagerDefault>() == null) 
            networkRunnerObject.AddComponent<NetworkSceneManagerDefault>();

        if (roomsListRunner != null && roomsListRunner.IsRunning)
            await roomsListRunner.Shutdown();

        var startResult = await runnerInstance.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Host,
            SessionName = name,
            Scene = gameSceneRef,
            SceneManager = runnerInstance.GetComponent<INetworkSceneManager>(),
            SessionProperties = new Dictionary<string, SessionProperty>()
            {
                { "LobbyId", currentLobbyId }
            }
        });

        if (startResult.Ok)
        {
            Debug.Log("HostGame uruchomione");
        }
        else
        {
            Debug.LogError("HostGame niepowodzenie: " + startResult.ShutdownReason);
        }
    }

    public async void JoinGame(SessionInfo session)
    {
        if (!session.IsValid)
        {
            Debug.LogError("Invalid session");
            return;
        }

        if (runnerInstance != null && runnerInstance.IsRunning)
        {
            await runnerInstance.Shutdown();
        }

        runnerInstance = networkRunnerObject.GetComponent<NetworkRunner>();
        if (runnerInstance == null) runnerInstance = networkRunnerObject.AddComponent<NetworkRunner>();
        runnerInstance.AddCallbacks(this);
        if (networkRunnerObject.GetComponent<NetworkSceneManagerDefault>() == null) 
            networkRunnerObject.AddComponent<NetworkSceneManagerDefault>();

        var result = await runnerInstance.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Client,
            SessionName = session.Name,
            SceneManager = runnerInstance.GetComponent<INetworkSceneManager>()
        });

        if (result.Ok)
        {
            Debug.Log($"Joined session: {session.Name}");
            if (roomsListRunner != null && roomsListRunner.IsRunning)
                await roomsListRunner.Shutdown();
        }
        else
        {
            Debug.LogError($"Failed to join session: {result.ShutdownReason}");
        }
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log($"Rooms list update (Count: {sessionList.Count})");

        if (JoinGameMenuController.me)
            JoinGameMenuController.me.UpdateGamesList(sessionList);
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
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
}*/


/*public class NetworkController : MonoBehaviour, INetworkRunnerCallbacks
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

    var startResult = await roomsListRunner.StartGame(new StartGameArgs
    {
        GameMode = GameMode.Server,
        SessionName = "Lobby",
        SceneManager = roomsListRunner.GetComponent<INetworkSceneManager>(),
        SessionProperties = new Dictionary<string, SessionProperty>
        {
            { "Region", "eu" }
        }
    });

    if (startResult.Ok)
    {
        Debug.Log("✅ roomsListRunner started");

        var lobbyResult = await roomsListRunner.JoinSessionLobby(SessionLobby.Shared);
        if (lobbyResult.Ok)
        {
            Debug.Log("✅ Joined session lobby successfully");
        }
        else
        {
            Debug.LogError("❌ Failed to join session lobby: " + lobbyResult.ShutdownReason);
        }
    }
    else
    {
        Debug.LogError("❌ Failed to start roomsListRunner: " + startResult.ShutdownReason);
    }
}


    public async void HostGame(string name)
    {
        runnerInstance = networkRunnerObject.GetComponent<NetworkRunner>();
        if(runnerInstance == null) runnerInstance = networkRunnerObject.AddComponent<NetworkRunner>();
        runnerInstance.AddCallbacks(this);
        if (networkRunnerObject.GetComponent<NetworkSceneManagerDefault>() == null) networkRunnerObject.AddComponent<NetworkSceneManagerDefault>();

        if (roomsListRunner != null && roomsListRunner.IsRunning) await roomsListRunner.Shutdown();

        await runnerInstance.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Host,
            SessionName = name,
            Scene = gameSceneRef,
            SceneManager = networkRunnerObject.GetComponent<INetworkSceneManager>(),
            SessionProperties = new Dictionary<string, SessionProperty>()
            {
                { "Region", "eu" }
            }
        });
    }

    public async void JoinGame(SessionInfo session)
    {
        if (!session.IsValid)
        {
            Debug.LogError("Invalid session");
            return;
        }
        if (runnerInstance != null && runnerInstance.IsRunning)
        {
            await runnerInstance.Shutdown();
            Destroy(runnerInstance.gameObject);
        }

        runnerInstance = networkRunnerObject.GetComponent<NetworkRunner>();
        if(runnerInstance == null) runnerInstance = networkRunnerObject.AddComponent<NetworkRunner>();
        runnerInstance.AddCallbacks(this);
        if (networkRunnerObject.GetComponent<NetworkSceneManagerDefault>() == null) networkRunnerObject.AddComponent<NetworkSceneManagerDefault>();

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
        Debug.Log("Aktualizacja listy pokoi");
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
    public void OnSceneLoadDone(NetworkRunner runner) {}
    public void OnSceneLoadStart(NetworkRunner runner) {}
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) {}
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) {}
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) {}
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) {}
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) {}
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) {}
}*/
