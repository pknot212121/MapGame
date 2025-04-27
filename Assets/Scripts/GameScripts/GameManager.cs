using System;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using TMPro;
using System.Text;
using System.Linq;

public class GameManager : NetworkBehaviour, INetworkRunnerCallbacks
{
    [Networked,Capacity(20)] 
    public NetworkDictionary<PlayerRef, NetworkString<_32>> PlayersToCountries { get; } = new
        NetworkDictionary<PlayerRef, NetworkString<_32>>();

    [Networked, Capacity(20)]
    public NetworkDictionary<PlayerRef, NetworkString<_32>> PlayerNicknames { get; }
    [Networked]
    public PlayerRef Owner { get; set; }
    [Networked]
    public int StartTimer { get; set; } // 10 - poczekalnia, 5-1 - odliczanie, -1 - rozpoczęta gra
    public static GameManager Instance{get;private set;}
    public Map CurrentMapData { get; private set; }
    public bool IsMapDataReady { get; private set; } = false;
    public event Action OnMapDataReady;

    [Header("UI References")]
    [Tooltip("Prefab pola tekstowego do instancjonowania (musi mieć TextEntryUI)")]
    [SerializeField] private GameObject textEntryPrefab;

    [Tooltip("Kontener UI, do którego będą dodawane pola tekstowe (z VerticalLayoutGroup)")]
    [SerializeField] private RectTransform textContainer;

    [Header("Testowanie")]
    [SerializeField] private string defaultTestMessage = "To jest testowa wiadomość nr ";
    private int messageCounter = 0;

    public override void Spawned()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        if (textEntryPrefab == null)
        {
            Debug.LogError("Prefab pola tekstowego nie jest przypisany w DynamicTextAdder!");
        }
        if (textContainer == null)
        {
            Debug.LogError("Kontener tekstu nie jest przypisany w DynamicTextAdder!");
        }
        Runner.AddCallbacks(this);
    }

    /*public async void CreateSession(string name, string filename)
    {

    }

    public async void ConnectToSession(SessionInfo session) 
    {

    }*/

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

    public void SetMapData(Map mapData)
    {
        CurrentMapData = mapData;
        IsMapDataReady = true;
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

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
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
        
    }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
}