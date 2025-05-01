using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using TMPro;
using System.Text;
using System.Linq;
using Unity.Serialization.Json;
using UnityEngine.InputSystem;


public class NetworkManagerGame : NetworkBehaviour, INetworkRunnerCallbacks
{
    [Networked,Capacity(20)] public NetworkDictionary<PlayerRef, NetworkString<_32>> PlayersToCountries { get; } = new NetworkDictionary<PlayerRef, NetworkString<_32>>();
    [Networked, Capacity(20)] public NetworkDictionary<PlayerRef, NetworkString<_32>> PlayerNicknames { get; }
    [Networked] public int StartTimer { get; set; } = 10; // 10 - poczekalnia, 5-1 - odliczanie, -1 - rozpoczęta gra
    [Networked] public PlayerRef ActivePlayer {get;set;}
    [Networked] public PlayerRef Master { get; set; }
    [Networked] public int EntityCounter {get;set;}  = 0; //Do przydzielania kolejnych id


    public static NetworkManagerGame Instance{get;private set;}
    //public Map CurrentMapData { get; private set; }
    //public List<ProvinceGameObject> provinceGameObjects {get; private set;}
    //public bool IsMapDataReady { get; private set; } = false;
    //public bool IsFirstActivePlayerSet {get; private set;} = false;
    //public event Action OnMapDataReady;

    [SerializeField] private GameObject textEntryPrefab;
    [SerializeField] private RectTransform textContainer;
    private int messageCounter = 0;



    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_SetPlayerCountry(PlayerRef player, NetworkString<_32> countryName)
    {
        if(PlayersToCountries.ContainsKey(player)) PlayersToCountries.Set(player, countryName);
        else PlayersToCountries.Add(player, countryName);
        Rpc_RefreshPlayerNicknameDisplayers();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_RefreshPlayerNicknameDisplayers()
    {
        GameController.me.RefreshPlayerNicknameDisplayers();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_StartCountdown()
    {
        StartTimer = 5;
        Rpc_CountdownHasStarted();
        StartCoroutine(CountDown());
    }

    public IEnumerator CountDown()
    {
        while(StartTimer > 0)
        {
            yield return new WaitForSeconds(1);
            StartTimer--;
        }
        if(StartTimer == 0)
        {
            StartTimer = -1;
            PrepareGame();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_CountdownHasStarted()
    {
        GameController.me.StartCoroutine(GameController.me.CountingDownToStart());
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_GameBegins()
    {
        GameController.me.RefreshPlayerNicknameDisplayers();
    }



    /*public void SetMapData(Map mapData, List<ProvinceGameObject> provinceGameObjects)
    {
        CurrentMapData = mapData;
        IsMapDataReady = true;
        this.provinceGameObjects = provinceGameObjects;
        Debug.Log("Map data set in GameManager.");
        OnMapDataReady?.Invoke();
    }*/

    public void SetNewMaster(PlayerRef master) // To ma być teoretycznie wywołane tylko przez obecnego mastera, nie należy wywoływać u innych
    {
        if(!Runner.IsSharedModeMasterClient) return;
        Master = master;
        GameController.me.ShowMasterCanvas();
        Debug.Log(Master + " has been assigned as new master");
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


    public void PrepareGame()
    {
        List<Action> actions = new List<Action>();
        foreach (Province province in GameController.me.map.provinces)
        {
            if(province!=null)
            {
                Troop troop = new Troop(++EntityCounter, 15, province.country, province);
                Action action = new Action(++EntityCounter,Action.ActionType.RaiseTroop, null, troop, null);
                // action.Pack();
                Troop troop2 = (Troop)action.entity2;
                foreach(KeyValuePair<TroopInfo,int> keyValuePair in troop2.numbers)
                {
                    Debug.Log("Nazwa: "+keyValuePair.Key.name+" Ilość: "+keyValuePair.Value);
                }
                actions.Add(action);
            }
        }

        // Tu trzeba dorobić wysyłanie listy
        string json = JsonSerialization.ToJson(actions);
        Debug.Log(json);
        DistributeMessage("InitialActions", json);
    }

    public void DistributeMessage(string title, string content) // Wyślij wiadomość do wszystkich
    {
        foreach(PlayerRef player in Runner.ActivePlayers)
        Runner.SendReliableDataToPlayer(player, ReliableKey.FromInts(42, 0, 21, 37), Encoding.UTF8.GetBytes(title + " " + content));
    }


    public override void Spawned()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        Runner.AddCallbacks(this);
    }
    public override void Render() //obsługuje licznik graczy - lepsza synchronizacja
    {
        if (Runner == null || !Runner.IsRunning || GameController.me == null)
        {
            return;
        }
        if (Runner.SessionInfo != null)
        {
            int currentPlayers = Runner.ActivePlayers.Count();
            int maxPlayers = Runner.SessionInfo.MaxPlayers;
            GameController.me.UpdatePlayersCountDisplayer(currentPlayers, maxPlayers);
        }
    }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        //Debug.Log($"Rooms list update (Count: {sessionList.Count})");
        //if (JoinGameMenuController.me) JoinGameMenuController.me.UpdateGamesList(sessionList);
    }
    public void OnConnectedToServer(NetworkRunner runner)
    {
        //Debug.Log("Connected to server");
    }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        //Debug.LogWarning($"OnShutdown: {shutdownReason}");
    }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if(Runner.IsSharedModeMasterClient) // Tylko host wykonuje zmiany
        {
            if(Runner.LocalPlayer == player) // Jesli to host dołącza (tworzy pokój)
            {
                SetNewMaster(player);
                string filename = PlayerPrefs.GetString("mapName");
                GameController.me.mapString = Map.LoadMapFromJson(filename);
                Map map = JsonSerialization.FromJson<Map>(GameController.me.mapString);
                GameController.me.map = map;
                EntityCounter = map.EntityCounter;
                map.Unpack();
                GameController.me.SetUpMap(map);
                Debug.Log("Set up host scene");
            }
            else // Jeśli to nie host dołącza to wysyła wcześniej ustawione dane z GameController
            {
                runner.SendReliableDataToPlayer(player, ReliableKey.FromInts(42, 0, 21, 37),Encoding.UTF8.GetBytes("InitialMap " + GameController.me.mapString));
                Debug.Log("Sent reliable data to " + player);
            }
            StartTimer = 10;
        }
        // Debug.Log("ILOŚĆ GRACZY: "+Runner.ActivePlayers.ToList().Count());
        // GameController.me.UpdatePlayersCountDisplayer(Runner.ActivePlayers.ToList().Count(), runner.SessionInfo.MaxPlayers);
    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (Runner.IsSharedModeMasterClient)
        {
            PlayersToCountries.Remove(player);
            Debug.Log("Usunięto przypisanie gracza do państwa!");
            PlayerNicknames.Remove(player);
            Debug.Log("Usunięcie przypisania gracza do nickname");
            StartTimer = 10;
            if(Master != Runner.LocalPlayer) SetNewMaster(Runner.LocalPlayer);
        }
        // GameController.me.UpdatePlayersCountDisplayer(Runner.ActivePlayers.ToList().Count(), runner.SessionInfo.MaxPlayers);
    }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) 
    {
        Debug.Log("Received reliable data");
        string receivedMessage = Encoding.UTF8.GetString(data.Array, data.Offset, data.Count);
        string[] parts = receivedMessage.Split(new[] {' '}, 2);
        string title = parts[0];
        string content = parts[1];
        if(title == "InitialMap")
        {
            GameController.me.mapString = content;
            Map map = JsonSerialization.FromJson<Map>(content);
            GameController.me.map = map;
            map.Unpack();
            GameController.me.SetUpMap(map);
            GameController.me.RefreshPlayerNicknameDisplayers();
        }
        else if(title == "InitialActions")
        {
            // Tu wczytać akcje z jsona
            
            List<Action> actions = JsonSerialization.FromJson<List<Action>>(content);
            GameController.me.HandleActions(actions);
        }
    }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
}