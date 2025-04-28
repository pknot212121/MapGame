using System;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using TMPro;
using System.Text;



public class NetworkController : MonoBehaviour, INetworkRunnerCallbacks
{
    SceneRef gameSceneRef;
    private NetworkRunner runner;
    public static NetworkController me;

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

    public async void CreateSession(string name, string filename)
    {
        int num = UnityEngine.Random.Range(1000, 9999);
        if (runner == null) runner = gameObject.AddComponent<NetworkRunner>();
        Dictionary<string,SessionProperty> properites = new Dictionary<string, SessionProperty>();
        properites["filename"] = filename;
        await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = name,
            SessionProperties = properites,
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
        await runner.Shutdown();
        SceneManager.LoadScene("JoinGameMenu");
        await JoinLobby();
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        //Debug.Log($"Rooms list update (Count: {sessionList.Count})");
        if (JoinGameMenuController.me) JoinGameMenuController.me.UpdateGamesList(sessionList);
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        //Debug.Log("Connected to server");
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        //Debug.LogWarning($"OnShutdown: {shutdownReason}");
    }


    //public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    //public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer || runner.IsSharedModeMasterClient)
        {

            runner.Spawn(playerPrefab, Vector3.zero, Quaternion.identity, player);

            if (GameManager.Instance == null) runner.Spawn(sessionManagerPrefab,Vector3.zero, Quaternion.identity);

            if(runner.IsSharedModeMasterClient)
            {
                IReadOnlyDictionary<string, SessionProperty> sessionProperties = runner.SessionInfo.Properties;
                string filename = sessionProperties["filename"];
                string mapData = MapManagement.LoadMapFromJson(filename);
                byte[] rawData = MapManagement.Compress(mapData);
                runner.SendReliableDataToPlayer(player,ReliableKey.FromInts(42, 0, 21, 37),Encoding.UTF8.GetBytes(mapData));
            }
        }

     }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) 
    { 
        string receivedMessage = Encoding.UTF8.GetString(data.Array, data.Offset, data.Count);
        Debug.Log("MESSAGE: "+receivedMessage);
        Transform provinceParentObjectTransform = GameObject.Find("Provinces").transform;
        Map allProvinces = JsonUtility.FromJson<Map>(receivedMessage);
        List<ProvinceGameObject> provinceGameObjects = new List<ProvinceGameObject>();

        foreach(Province provinceData in allProvinces.provinces)
        {
            List<Vector2> points = new List<Vector2>();
            foreach (Vector2 point in provinceData.points)
            {
                points.Add(point);
            }
            ProvinceGameObject province = ShapeTools.CreateProvinceGameObject(provinceData.name,points);
            provinceGameObjects.Add(province);
            if(allProvinces.GetCountry(provinceData)==null) province.SetColor(Color.white);
            else province.SetColor(allProvinces.GetCountry(provinceData).color);

            province.gameObject.transform.SetParent(provinceParentObjectTransform);
        }
        GameManager.Instance.SetMapData(allProvinces,provinceGameObjects);
    }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
}
