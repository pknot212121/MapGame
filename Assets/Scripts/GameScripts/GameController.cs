using UnityEngine;
using Fusion;
using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController me;
    public Canvas anteroomCanvas;
    public Canvas gameplayCanvas;
    public Transform playerNicknameDisplayersTransform;
    public Transform provincesTransform;
    public Transform troopsTransform;
    public Button startButton;
    public Button endTurnButton;
    public TMP_Text startCounter;
    public TMP_Text playersCountDisplayer;

    public GameObject troopPrefab;

    public string mapString;
    public Map map;
    public List<ProvinceGameObject> provinceGameObjects = new List<ProvinceGameObject>();
    public List<TroopGO> troopGOs = new List<TroopGO>();

    public List<Action> actions = new List<Action>();

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
        if(Input.GetKeyDown(KeyCode.Tab)) Debug.Log("AKTUALNA ILOŚĆ ENTITIES: "+NetworkManagerGame.Instance.EntityCounter);
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            foreach(Province province in map.provinces)
            {
                Debug.Log(province.countryName);
            }
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            foreach(KeyValuePair<PlayerRef,NetworkString<_32>> keyValuePair in NetworkManagerGame.Instance.PlayersToCountries)
            {
                Debug.Log("GRACZ: "+keyValuePair.Key+" PAŃSTWO: "+keyValuePair.Value);
            }
            foreach(KeyValuePair<PlayerRef,NetworkString<_32>> keyValuePair1 in NetworkManagerGame.Instance.PlayerNicknames)
            {
                Debug.Log("GRACZ: "+keyValuePair1.Key+"NICKNAME: "+keyValuePair1.Value);
            }
        }
        if(Input.GetMouseButtonDown(0))
        {
            if (NetworkManagerGame.Instance != null && map != null)
            {
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                foreach(Province province in map.provinces)
                {
                    if(ShapeTools.IsPointInPolygon((Vector2)worldPosition, province.points.ToArray()))
                    {
                        ProvinceClick(province);
                        break;
                    }
                }
            }
        }
    }

    public void SetUpMap(Map map)
    {
        this.map = map;
        map.Unpack();
        foreach(Province province in map.provinces)
        {
            // Debug.Log("NAZWA KRAJU: "+province.country.name);
            ProvinceGameObject provinceGO = ShapeTools.CreateProvinceGameObject(province, province.points);
            provinceGameObjects.Add(provinceGO);
            if(map.GetCountry(province) == null) provinceGO.SetColor(Color.white);
            else provinceGO.SetColor(map.GetCountry(province).color);
            provinceGO.gameObject.transform.SetParent(provincesTransform);
        }
        Debug.Log("Map has been set up");
    }

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

    // public void EndTurnClick()
    // {
    //     Debug.Log("Próba zakończenia tury");
    //     {
    //         networkPlayer.RPC_EndTurn();
    //     }
    // }

    public void UpdatePlayersCountDisplayer(int current, int max)
    {
        // Debug.Log("ILOŚĆ GRACZY W UPDACIE: "+current+"MAX GRACZY: "+max);
        playersCountDisplayer.text = $"{current}/{max}";
    }

    public void StartClick()
    {
        NetworkManagerGame.Instance.Rpc_StartCountdown();
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

    // Obsługuje odliczanie do startu
    public IEnumerator CountingDownToStart()
    {
        startCounter.gameObject.SetActive(true);
        while(NetworkManagerGame.Instance.StartTimer != 10 && NetworkManagerGame.Instance.StartTimer != -1)
        {
            startCounter.text = "Starting in: " + NetworkManagerGame.Instance.StartTimer;
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

    /*void TrySettingCountryForPlayer()
    {
        if (NetworkManagerGame.Instance != null && NetworkManagerGame.Instance.IsMapDataReady)
        {
            Map currentMap = NetworkManagerGame.Instance.CurrentMapData;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if(Input.GetMouseButtonDown(0))
            {
                foreach(Province province in currentMap.provinces)
                {
                    if(ShapeTools.IsPointInPolygon((Vector2)worldPosition, province.points.ToArray()))
                    {
                        string countryName = currentMap.GetCountry(province).name;
                        PlayerRef owner = NetworkManagerGame.Instance.GetOwnerOfACountry(countryName);
                        if(owner==PlayerRef.None)
                        {
                            NetworkManagerGame.Instance.Rpc_SetPlayerCountry(
                            networkPlayer.Runner.LocalPlayer, 
                            currentMap.GetCountry(province).name);
                            NetworkManagerGame.Instance.PrintCountryOwners();
                        }
                        break;
                    }
                }
            }
        }
    }*/


    void ProvinceClick(Province province)
    {
        if(NetworkManagerGame.Instance.StartTimer == -1) // Gra się już rozpoczęła
        {

        }
        else // Jesteśmy w poczekalni
        {
            PlayerRef owner = NetworkManagerGame.Instance.GetOwnerOfACountry(province.country.name);
            if(owner==PlayerRef.None)
            {
                Debug.Log("PRÓBUJĘ USTAWIĆ WŁAŚCICIELA: "+province.name);
                NetworkManagerGame.Instance.Rpc_SetPlayerCountry(NetworkManagerGame.Instance.Runner.LocalPlayer, province.country.name);
                //NetworkManagerGame.Instance.PrintCountryOwners();
            }
        }
    }

    public void RefreshPlayerNicknameDisplayers()
    {
        Debug.Log("Refreshing nickname displayers");
        foreach(Transform t in playerNicknameDisplayersTransform) Destroy(t.gameObject);
        GameObject prefab = Resources.Load("Prefabs/PlayerNicknameDisplayer") as GameObject;
        foreach(var kvp in NetworkManagerGame.Instance.PlayersToCountries)
        {
            //Debug.Log(kvp);
            if(kvp.Key == PlayerRef.None) continue; // Pomijamy pustych graczy (jeśli tacy są)
            GameObject pnd = Instantiate(prefab);
            Country country = map.GetCountry(kvp.Value.Value);
            Debug.Log(country);
            string nick = NetworkManagerGame.Instance.PlayerNicknames[kvp.Key].Value;
            Debug.Log(nick);
            pnd.GetComponent<PlayerNicknameDisplayer>().Initialise(nick, country);
            pnd.transform.SetParent(playerNicknameDisplayersTransform);
        }
    }
}