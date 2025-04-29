using System;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using TMPro;
using System.Text;
using System.Linq;


public class NetworkManagerGame : NetworkBehaviour, INetworkRunnerCallbacks
{
    [Networked,Capacity(20)] public NetworkDictionary<PlayerRef, NetworkString<_32>> PlayersToCountries { get; } = new NetworkDictionary<PlayerRef, NetworkString<_32>>();
    [Networked, Capacity(20)] public NetworkDictionary<PlayerRef, NetworkString<_32>> PlayerNicknames { get; }
    [Networked] public int StartTimer { get; set; } = 10; // 10 - poczekalnia, 5-1 - odliczanie, -1 - rozpoczęta gra
    [Networked] public PlayerRef ActivePlayer {get;set;}


    public static NetworkManagerGame Instance{get;private set;}
    public Map CurrentMapData { get; private set; }
    public List<ProvinceGameObject> provinceGameObjects {get; private set;}
    public bool IsMapDataReady { get; private set; } = false;
    public bool IsFirstActivePlayerSet {get; private set;} = false;
    public event Action OnMapDataReady;

    [SerializeField] private GameObject textEntryPrefab;
    [SerializeField] private RectTransform textContainer;
    private int messageCounter = 0;



    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_SetPlayerCountry(PlayerRef player,NetworkString<_32> countryName)
    {
        PlayersToCountries.Set(player,countryName);
        Rpc_RefreshPlayerNicknameDisplayers();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_RefreshPlayerNicknameDisplayers()
    {
        Debug.Log("Refreshing nickname displayers");
        foreach(Transform t in GameController.me.playerNicknameDisplayersTransform) Destroy(t.gameObject);
        GameObject prefab = Resources.Load("Prefabs/PlayerNicknameDisplayer") as GameObject;
        foreach(var kvp in PlayersToCountries)
        {
            GameObject pnd = Instantiate(prefab);
            pnd.GetComponent<PlayerNicknameDisplayer>().Initialise(PlayerNicknames[kvp.Key].Value, CurrentMapData.GetCountry(kvp.Value.Value));
            pnd.transform.SetParent(GameController.me.playerNicknameDisplayersTransform);
        }
    }



    public void SetMapData(Map mapData, List<ProvinceGameObject> provinceGameObjects)
    {
        CurrentMapData = mapData;
        IsMapDataReady = true;
        this.provinceGameObjects = provinceGameObjects;
        Debug.Log("Map data set in GameManager.");
        OnMapDataReady?.Invoke();
        
    }
    public void AddNewTextEntry(string message)
    {
        if (textEntryPrefab == null || textContainer == null)
        {
            Debug.LogError("Nie można dodać tekstu - brakuje prefabu lub kontenera.");
            return;
        }
        GameObject newEntry = Instantiate(textEntryPrefab, textContainer);
        newEntry.name = $"TextEntry_{messageCounter}";
        TextMeshProUGUI tmpText = newEntry.GetComponentInChildren<TextMeshProUGUI>();
        if (tmpText != null) {
            tmpText.text = message;
        } else Debug.LogError("Nie znaleziono komponentu TextMeshProUGUI na instancji prefabu!", newEntry);
         messageCounter++;
    }
    
    public PlayerRef GetOwnerOfACountry(NetworkString<_32> countryName)
    {
        foreach(KeyValuePair<PlayerRef,NetworkString<_32>> keyValuePair in PlayersToCountries)
        {
            if(keyValuePair.Value==countryName) return keyValuePair.Key;
        }
        return PlayerRef.None;
    }

    public void PrintCountryOwners()
    {
        foreach(KeyValuePair<PlayerRef,NetworkString<_32>> keyValuePair in PlayersToCountries)
        {
            Debug.Log("Gracz: "+keyValuePair.Key+" Państwo: "+keyValuePair.Value);
        }
    }



    public override void Spawned()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        Runner.AddCallbacks(this);
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
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if(runner.IsSharedModeMasterClient)
        {
            IReadOnlyDictionary<string, SessionProperty> sessionProperties = runner.SessionInfo.Properties;
            string filename = PlayerPrefs.GetString("mapName");
            string mapData = Map.LoadMapFromJson(filename);
            byte[] rawData = Map.Compress(mapData);
            runner.SendReliableDataToPlayer(player,ReliableKey.FromInts(42, 0, 21, 37),Encoding.UTF8.GetBytes(mapData));
        }
        if(!IsFirstActivePlayerSet && runner.IsSharedModeMasterClient)
        {
            ActivePlayer = player;
            IsFirstActivePlayerSet=true;
        }
        GameController.me.UpdatePlayersCountDisplayer(Runner.ActivePlayers.ToList().Count(), runner.SessionInfo.MaxPlayers);
    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        GameController.me.UpdatePlayersCountDisplayer(Runner.ActivePlayers.ToList().Count(), runner.SessionInfo.MaxPlayers);
        if (Runner.IsSharedModeMasterClient)
        {
            PlayersToCountries.Remove(player);
            Debug.Log("Usunięto przypisanie gracza do państwa!");
            PlayerNicknames.Remove(player);
            Debug.Log("Usunięcie przypisania gracza do nickname");
        }
    }
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
        NetworkManagerGame.Instance.SetMapData(allProvinces,provinceGameObjects);
    }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
}