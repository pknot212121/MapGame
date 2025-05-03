using UnityEngine;
using Fusion;
using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using UnityEngine.UI;
using Unity.Serialization.Json;
using System.Linq;

public class GameController : MonoBehaviour
{
    public static GameController me;

    #region Transforms
    
    public Transform playerNicknameDisplayersTransform;
    public Transform provincesTransform;
    public Transform troopsTransform;
    public Transform troopInfoListContentTransform;
    public Transform troopListInProvinceContentTransform;

    #endregion
    
    #region UI variables

    public Canvas anteroomCanvas;
    public Canvas gameplayCanvas;
    public Canvas troopMenuCanvas;
    public Canvas provinceMenuCanvas;
    public Button startButton;
    public Button endTurnButton;

    public TMP_Text startCounter;
    public TMP_Text playersCountDisplayer;

    #endregion

    #region Prefabs

    public GameObject troopPrefab;

    #endregion

    #region Entities

    public string mapString;
    public Map map;
    public List<ProvinceGO> provinceGOs = new List<ProvinceGO>();
    public List<TroopGO> troopGOs = new List<TroopGO>();

    public List<Action> myActions = new List<Action>();

    public Action actionPrepared = null; // Akcja którą szykujemy

    #endregion

    #region MonoBehaviour

    void Start()
    {
        me = this;
        startButton.gameObject.SetActive(false);
        startCounter.gameObject.SetActive(false);
    }

    void Update()
    {
        /*if(networkPlayer != null && networkPlayer.Runner != null)
        {
            if (anteroomCanvas != null) TrySettingCountryForPlayer();
            else TryChangingOwnerhipOfAProvince();
        }*/
        // FOR DEBUGGING
        if(Input.GetMouseButtonDown(0) && !ShapeTools.IsPointerOverSpecificCanvas(troopMenuCanvas)) troopMenuCanvas.gameObject.SetActive(false);
        if(Input.GetMouseButtonDown(0) && !ShapeTools.IsPointerOverSpecificCanvas(provinceMenuCanvas)) provinceMenuCanvas.gameObject.SetActive(false);

        if(Input.GetKeyDown(KeyCode.Tab)) Debug.Log("AKTUALNA ILOŚĆ ENTITIES: "+NetworkManagerGame.me.EntityCounter);
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            foreach(Province province in map.provinces)
            {
                Debug.Log(province.countryName);
            }
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            foreach(KeyValuePair<PlayerRef,NetworkString<_32>> keyValuePair in NetworkManagerGame.me.PlayersToCountries)
            {
                Debug.Log("GRACZ: "+keyValuePair.Key+" PAŃSTWO: "+keyValuePair.Value);
            }
            foreach(KeyValuePair<PlayerRef,NetworkString<_32>> keyValuePair1 in NetworkManagerGame.me.PlayerNicknames)
            {
                Debug.Log("GRACZ: "+keyValuePair1.Key+"NICKNAME: "+keyValuePair1.Value);
            }
        }

        if(Input.GetMouseButtonDown(0)) // Ten gość robi returny, stawiać rzeczy przed nim
        {
            if (NetworkManagerGame.me == null || map == null) return;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            foreach(TroopGO troopGO in troopGOs)
            {
                if(Vector2.Distance((Vector2)worldPosition, (Vector2)troopGO.transform.position) < 0.75f)
                {
                    TroopClick(troopGO.data);
                    return;
                }
            }

            foreach(Province province in map.provinces)
            {
                if(ShapeTools.IsPointInPolygon((Vector2)worldPosition, province.points.ToArray()))
                {
                    ProvinceClick(province);
                    return;
                }
            }
        }
    }

    #endregion
    public void SetUpMap(Map map)
    {
        this.map = map;
        foreach(Province province in map.provinces)
        {
            // Debug.Log("NAZWA KRAJU: "+province.country.name);
            ProvinceGO provinceGO = ShapeTools.CreateProvinceGO(province, province.points);
            provinceGOs.Add(provinceGO);
            if(map.GetCountry(province) == null) provinceGO.SetColor(Color.white);
            else provinceGO.SetColor(map.GetCountry(province).color);
            provinceGO.gameObject.transform.SetParent(provincesTransform);
        }
        Debug.Log("Map has been set up");
    }
    #region CanvasUpdates
    public void UpdateTroopInfoList(Troop troop)
    {
        foreach (Transform child in troopInfoListContentTransform) { Destroy(child.gameObject); }
        GameObject prefab = Resources.Load<GameObject>("Prefabs/troopInfoPrefab");
        foreach(KeyValuePair<TroopInfo,int> keyValuePair in troop.numbers)
        {
            GameObject panel = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            panel.GetComponent<TroopInfoPrefab>().Initialise(keyValuePair.Key,keyValuePair.Value);
            panel.transform.SetParent(troopInfoListContentTransform, false);
        }
    }
    public void UpdateTroopListInProvince(Province province)
    {
        foreach (Transform child in troopListInProvinceContentTransform) { Destroy(child.gameObject); }
        GameObject prefab = Resources.Load<GameObject>("Prefabs/TroopInProvincePrefab");
        foreach(TroopGO troopGO in troopGOs)
        {
            if(troopGO.data.province.id == province.id)
            {
                int sum=troopGO.data.numbers.Sum(x => x.Value);
                var maxEntry = troopGO.data.numbers.Aggregate((x, y) => x.Value > y.Value ? x : y);
                GameObject panel = Instantiate(prefab, Vector3.zero, Quaternion.identity);
                panel.GetComponent<TroopInProvincePrefab>().Initialise(troopGO.data,maxEntry.Key,sum);
                panel.transform.SetParent(troopListInProvinceContentTransform, false);
            }
        }
        
    }
    #endregion

    #region Actions handling

    public void HandleAction(Action action)
    {
        if(action.type == Action.ActionType.RaiseTroop)
        {
            TroopGO troopGO = Instantiate(troopPrefab).GetComponent<TroopGO>();
            Troop troop = (Troop)action.entity2;
            troopGO.Initialise(troop);
            troopGO.transform.SetParent(troopsTransform);
            troopGOs.Add(troopGO);
        }
    }

    public void HandleActions(List<Action> actions)
    {
        foreach(Action action in actions) HandleAction(action);
    }

    #endregion

    // public void EndTurnClick()
    // {
    //     Debug.Log("Próba zakończenia tury");
    //     {
    //         networkPlayer.RPC_EndTurn();
    //     }
    // }

    #region UI methods

    public void UpdatePlayersCountDisplayer(int current, int max)
    {
        // Debug.Log("ILOŚĆ GRACZY W UPDACIE: "+current+"MAX GRACZY: "+max);
        playersCountDisplayer.text = $"{current}/{max}";
    }

    public void StartClick()
    {
        NetworkManagerGame.me.Rpc_StartCountdown();
        startButton.gameObject.SetActive(false);
    }

    public void ReturnToLobbyClick()
    {
        NetworkManagerJoin.me.LeaveRoom();
    }

    public void ShowMasterCanvas()
    {
        startButton.gameObject.SetActive(true);
    }

    public void ShowProvinceMenu(Province province)
    {
        troopMenuCanvas.gameObject.SetActive(false);
        provinceMenuCanvas.gameObject.SetActive(true);
        UpdateTroopListInProvince(province);
    }

    public void ShowTroopMenu(Troop troop)
    {
        provinceMenuCanvas.gameObject.SetActive(false);
        troopMenuCanvas.gameObject.SetActive(true);
        UpdateTroopInfoList(troop);
    }

    public void EndTurnClick()
    {
        NetworkManagerGame.me.SendReliableMessageToPlayer(NetworkManagerGame.me.Object.StateAuthority, "MyActions", JsonSerialization.ToJson(myActions));
    }

    // public void EndTurnClick()
    // {
    //     NetworkManagerGame.Instance.SendReliableMessageToPlayer(NetworkManagerGame.Instance.Object.StateAuthority, "MyActions", JsonSerialization.ToJson(myActions));
    // }

    #endregion

    #region IEnumerators

    // Obsługuje odliczanie do startu
    public IEnumerator CountingDownToStart()
    {
        startCounter.gameObject.SetActive(true);
        while(NetworkManagerGame.me.StartTimer != 10 && NetworkManagerGame.me.StartTimer != -1)
        {
            startCounter.text = "Starting in: " + NetworkManagerGame.me.StartTimer;
            yield return null;
        }
        startCounter.gameObject.SetActive(false);
        /*if(NetworkManagerGame.Instance.StartTimer == 10) startCounter.gameObject.SetActive(false);
        if(NetworkManagerGame.Instance.StartTimer == -1)
        {
            anteroomCanvas.gameObject.SetActive(false);
            gameplayCanvas.gameObject.SetActive(true);
            Debug.Log("Game has started");
        }*/
    }

    #endregion
    
    /*void TryChangingOwnerhipOfAProvince()
    {
        if(NetworkManagerGame.Instance != null && NetworkManagerGame.Instance.IsMapDataReady)
        {
            Map currentMap = NetworkManagerGame.Instance.CurrentMapData;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if(Input.GetMouseButtonDown(0))
            {
                foreach(Province province in currentMap.provinces)
                {
                    if(ShapeTools.IsPointInPolygon((Vector2)worldPosition, province.points.ToArray()))
                    {
                        NetworkString<_32> countryName = NetworkManagerGame.Instance.PlayersToCountries[networkPlayer.Runner.LocalPlayer];
                        networkPlayer.Rpc_ChangeProvinceOwnership(countryName,province.name);
                        break;
                    }
                }
            }
            
        }
    }*/

    #region Entities clicks

    void TroopClick(Troop troop)
    {
        Country country = null;
        foreach(KeyValuePair<PlayerRef,NetworkString<_32>> keyValuePair in NetworkManagerGame.me.PlayersToCountries)
        {
            if(keyValuePair.Key == NetworkManagerGame.me.Runner.LocalPlayer)
            {
                country = map.GetCountry((string)keyValuePair.Value);
                break;
            }
        }
        if(country!=null && troop.country.id == country.id)
        {
            Debug.Log("Troop click");
            if(actionPrepared != null && actionPrepared.type == Action.ActionType.MoveTroop && actionPrepared.entity1.id == troop.id) // Drugie kliknięcie wchodzi w menu jednostki
            {
                actionPrepared = null;
                ShowTroopMenu(troop);
            }
            else // Pierwsze kliknięcie wybiera atak
            {
                actionPrepared = new Action(Action.ActionType.MoveTroop, troop, null, null);
                foreach(Province p in map.GetNeighboringProvinces(troop.province)) p.go.StartCoroutine(p.go.HighlightForSelection());
            }
        }
    }

    void ProvinceClick(Province province)
    {
        Debug.Log("Province click");
        if(NetworkManagerGame.me.StartTimer == -1) // Gra się już rozpoczęła
        {
            if(actionPrepared != null && actionPrepared.type == Action.ActionType.MoveTroop && ShapeTools.AreTwoProvincesNeighbors(province, ((Troop)actionPrepared.entity1).province))
            {
                actionPrepared.entity2 = province;
                myActions.Add(actionPrepared);
                actionPrepared = null;
                Debug.Log("Added move action");
            }
            else
            {
                actionPrepared = null;
                ShowProvinceMenu(province);
            }
        }
        else // Jesteśmy w poczekalni
        {
            PlayerRef owner = NetworkManagerGame.me.GetOwnerOfACountry(province.country.name);
            if(owner==PlayerRef.None)
            {
                Debug.Log("PRÓBUJĘ USTAWIĆ WŁAŚCICIELA: "+province.name);
                NetworkManagerGame.me.Rpc_SetPlayerCountry(NetworkManagerGame.me.Runner.LocalPlayer, province.country.name);
                //NetworkManagerGame.Instance.PrintCountryOwners();
            }
        }
    }

    #endregion

    public void RefreshPlayerNicknameDisplayers()
    {
        Debug.Log("Refreshing nickname displayers");
        foreach(Transform t in playerNicknameDisplayersTransform) Destroy(t.gameObject);
        GameObject prefab = Resources.Load("Prefabs/PlayerNicknameDisplayer") as GameObject;
        foreach(var kvp in NetworkManagerGame.me.PlayersToCountries)
        {
            //Debug.Log(kvp);
            if(kvp.Key == PlayerRef.None) continue; // Pomijamy pustych graczy (jeśli tacy są)
            GameObject pnd = Instantiate(prefab);
            Country country = map.GetCountry(kvp.Value.Value);
            Debug.Log(country);
            string nick = NetworkManagerGame.me.PlayerNicknames[kvp.Key].Value;
            Debug.Log(nick);
            pnd.GetComponent<PlayerNicknameDisplayer>().Initialise(nick, country);
            pnd.transform.SetParent(playerNicknameDisplayersTransform);
        }
    }
}