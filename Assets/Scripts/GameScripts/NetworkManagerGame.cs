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
    [Networked] public int Seed { get; set; } = 10;
    [Networked] public PlayerRef ActivePlayer { get; set; }
    [Networked] public PlayerRef Master { get; set; }
    [Networked] public int EntityCounter {get;set;}  = 0; // Do przydzielania kolejnych id


    public static NetworkManagerGame me{get;private set;}
    public List<List<Action>> allPlayersActions = null;
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
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_SetNickname(NetworkString<_32> nickname, RpcInfo info = default)
    {
        string validatedNick = nickname.Value.Trim();
        if (this != null)
        {
            PlayerNicknames.Set(info.Source, validatedNick);
            Log.Info($"RPC: Zaktualizowano GameManager.PlayerNicknames dla {info.Source}.");
        }
        else
        {
            Log.Error("RPC: GameManager.Instance jest null, nie można zaktualizować centralnego słownika nicków.");
        }
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

    /*[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_GameBegins()
    {
        GameController.me.RefreshPlayerNicknameDisplayers();
    }*/



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
        byte[] buffer = new byte[4];
        System.Security.Cryptography.RandomNumberGenerator.Fill(buffer);
        Seed = BitConverter.ToInt32(buffer, 0);
        Debug.Log("Generated seed: " + Seed);

        // Podstawowe jednostki
        List<Action> actions = new List<Action>();
        foreach (Province province in GameController.me.map.provinces)
        {
            if(province!=null)
            {
                Troop troop = new Troop(++EntityCounter, 15, province.country, province, GameController.me.map.troopInfos);
                Dictionary<int, int> convertedDict = troop.numbers
                .ToDictionary(
                    kvp => kvp.Key.id,
                    kvp => kvp.Value
                );
                Debug.Log(province.country); // TU JEST NULL Z JAKIEGOS POWODU
                var data = (countryId: province.country.id, provinceId: province.id, convertedNumbers: convertedDict);
                Action action = new Action(Action.ActionType.RaiseTroop, 0, troop.id, JsonSerialization.ToJson(data));
                actions.Add(action);
            }
        }

        string json = JsonSerialization.ToJson(actions);
        Debug.Log(json);
        DistributeMessage("InitialActions", json);

        // Przypisanie państw
        List<PlayerRef> unassignedPlayers = Runner.ActivePlayers.ToList();
        List<Country> unassignedCountries = new List<Country>(GameController.me.map.countries);
        foreach(var kvp in PlayersToCountries)
        {
            if(kvp.Key != PlayerRef.None)
            {
                unassignedPlayers.RemoveAll(p => p == kvp.Key);
                unassignedCountries.RemoveAll(c => c.name == kvp.Value.Value);
            }
        }
        if(unassignedPlayers.Count > unassignedCountries.Count) 
        {
            Debug.Log("Too few countries to distribute, can't start");
            return;
        }
        Debug.Log(unassignedPlayers.Count + " : " + unassignedCountries.Count);
        while(unassignedPlayers.Count > 0)
        {
            int randomCountryIndex = UnityEngine.Random.Range(0, unassignedCountries.Count);
            Debug.Log("Random index: " + randomCountryIndex);
            PlayersToCountries.Add(unassignedPlayers[0], unassignedCountries[randomCountryIndex].name);
            unassignedPlayers.RemoveAt(0);
            unassignedCountries.RemoveAt(randomCountryIndex);
        }
        Rpc_RefreshPlayerNicknameDisplayers();

    }

    public void ProcessAllPlayersActions() // Przeplata ruchy graczy
    {
        List<Queue<Action>> actionsQueues = allPlayersActions.Select(list => new Queue<Action>(list)).ToList();
        List<Action> actions = new List<Action>();
        bool goAgain = true;
        while(goAgain)
        {
            goAgain = false;
            foreach(Queue<Action> queue in actionsQueues)
            {
                if (queue.Count == 0) continue;
                actions.Add(queue.Dequeue());
                if (queue.Count > 0) goAgain = true;
            }
        }
        allPlayersActions.Clear();
        DistributeMessage("ProcessedActions", JsonSerialization.ToJson(actions));
    }

    public void SendReliableMessageToPlayer(PlayerRef player, string title, string content)
    {
        Runner.SendReliableDataToPlayer(player, ReliableKey.FromInts(42, 0, 21, 37), Encoding.UTF8.GetBytes(title + " " + content));
    }

    public void DistributeMessage(string title, string content) // Wyślij wiadomość do wszystkich
    {
        foreach(PlayerRef player in Runner.ActivePlayers)
        Runner.SendReliableDataToPlayer(player, ReliableKey.FromInts(42, 0, 21, 37), Encoding.UTF8.GetBytes(title + " " + content));
    }


    public override void Spawned()
    {
        if (me == null) me = this;
        else Destroy(gameObject);
        Runner.AddCallbacks(this);
    }
    public override void Render() //obsługuje licznik graczy - lepsza
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
                string chosenNickname = PlayerPrefs.GetString("PlayerNickname", $"Player_{UnityEngine.Random.Range(100, 999)}");
                Debug.Log("USTAWIONO NICKNAME GRACZA: "+Runner.LocalPlayer+" JAKO: "+chosenNickname+" W SPAWNED");
                Rpc_SetNickname(chosenNickname);
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
        Rpc_RefreshPlayerNicknameDisplayers();
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
        Rpc_RefreshPlayerNicknameDisplayers();
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
            GameController.me.SetUpMap(map);
            string chosenNickname = PlayerPrefs.GetString("PlayerNickname", $"Player_{UnityEngine.Random.Range(100, 999)}");
            Debug.Log("USTAWIONO NICKNAME GRACZA: "+Runner.LocalPlayer+" JAKO: "+chosenNickname+" W SPAWNED");
            Rpc_SetNickname(chosenNickname);
        }
        else if(title == "InitialActions")
        {
            List<Action> actions = JsonSerialization.FromJson<List<Action>>(content);
            GameController.me.HandleActions(actions);
        }
        else if(title == "MyActions") // Wysyłane przez graczy do authority do przetworzenia
        {
            if(allPlayersActions == null) allPlayersActions = new List<List<Action>>();
            List<Action> actions = JsonSerialization.FromJson<List<Action>>(content);
            if(actions == null) Debug.Log("Received broken actions list, can't process");
            allPlayersActions.Add(actions);
            if(allPlayersActions.Count == Runner.ActivePlayers.ToList().Count) ProcessAllPlayersActions();
        }
        else if(title == "ProcessedActions") // Przetworzone akcje wysyłane przez authority do graczy
        {
            List<Action> actions = JsonSerialization.FromJson<List<Action>>(content);
            GameController.me.HandleActions(actions);
        }
    }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
}