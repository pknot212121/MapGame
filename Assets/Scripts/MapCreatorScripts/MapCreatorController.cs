using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Linq;
using System;


public class MapCreatorController : MonoBehaviour
{
    public Canvas canvas;
    public Canvas provincePointsCanvas;
    public Canvas countryAdjustmentCanvas;
    public Canvas troopInfoAdjustmentCanvas;
    public Canvas resourceInfoAdjustmentCanvas;

    public GameObject temporaryMarks;
    public GameObject cursor;
    public Transform countriesListContentTransform;
    public Transform resourceInfosListContentTransform;
    public Transform troopInfosListContentTransform;

    [SerializeField] private TMP_InputField createProvinceNameInput;
    [SerializeField] private TMP_InputField countryNameInput;
    [SerializeField] private TMP_InputField resourceInfoNameInput;
    [SerializeField] private TMP_InputField troopInfoNameInput;
    [SerializeField] private TMP_InputField countryAdjustmentNameInput;
    [SerializeField] private TMP_InputField countryHexColorInput;
    [SerializeField] private TMP_InputField mapNameInput;

    public Map map;
    public List<ProvinceGameObject> provinceGameObjects = new List<ProvinceGameObject>();
    Stack<ProvinceGameObject> deleted = new Stack<ProvinceGameObject>();

    bool isProvinceShapeFormed = false;
    public Country countryAdjusted = null;
    public TroopInfo troopInfoAdjusted = null;
    public ResourceInfo resourceInfoAdjusted = null;

    List<Vector2> provincePoints = null;
    List<Vector2> allPoints = new List<Vector2>();

    public static MapCreatorController me;

    void Start()
    {
        me = this;
    }

    public void CreateProvinceClick()
    {
        countryAdjusted = null;
        isProvinceShapeFormed = true;
        canvas.gameObject.SetActive(false);
        provincePointsCanvas.gameObject.SetActive(true);
        Cursor.visible = false;
        cursor.SetActive(true);
        provincePoints = new List<Vector2>();
        createProvinceNameInput.text = "";
    }

    public void OkClick()
    {
        string provinceName = createProvinceNameInput.text;
        if(provinceName.Length < 2)
        {
            Debug.Log("Province name must consist of at least 2 characters!");
            return;
        }
        if(ShapeTools.IsIntersecting(provincePoints))
        {
            ProvinceGameObject province = ShapeTools.CreateProvinceGameObject(provinceName, provincePoints);
            if(!province) 
            {
                Debug.Log("Couldn't create the shape!");
                return;
            }
            provinceGameObjects.Add(province);
            map.provinces.Add(new Province(provinceName,province));
            foreach(Vector2 point in provincePoints){allPoints.Add(point);}
        }
        else Debug.Log("Krawędzie się przecinają!");

        isProvinceShapeFormed = false;
        canvas.gameObject.SetActive(true);
        provincePointsCanvas.gameObject.SetActive(false);
        Cursor.visible = true;
        cursor.SetActive(false);

        foreach(Transform t in temporaryMarks.transform)
        {
            Destroy(t.gameObject);
        }
        provincePoints = null;
        
    }

    public void AddCountryClick()
    {
        string countryName = countryNameInput.text;
        if(countryName.Length < 2)
        {
            Debug.Log("Country name must consist of at least 2 characters!");
            return;
        }
        foreach(Country c in map.countries)
        {
            if(c.name == countryName)
            {
                Debug.Log("Country with such name already exists!");
                return;
            }
        }

        Country country = new Country(countryName);
        map.countries.Add(country);

        GameObject prefab = Resources.Load<GameObject>("Prefabs/CountryButton");
        GameObject button = Instantiate(prefab);
        button.transform.SetParent(countriesListContentTransform, false);
        button.GetComponent<CountryButton>().Initialise(country);
        countryNameInput.text = "";
    }

    public void AddResourceInfoClick()
    {
        string resourceInfoName = resourceInfoNameInput.text;
        if(resourceInfoName.Length < 2)
        {
            Debug.Log("Resource name must consist of at least 2 characters!");
            return;
        }
        if(map.resourceInfos.Any(obj => obj.name == resourceInfoName))
        {
            Debug.Log("Resource with such name already exists!");
            return;
        }

        ResourceInfo resourceInfo = new ResourceInfo(resourceInfoName);
        map.resourceInfos.Add(resourceInfo);
        GameObject prefab = Resources.Load<GameObject>("Prefabs/ResourceInfoButton");
        GameObject button = Instantiate(prefab);
        button.transform.SetParent(resourceInfosListContentTransform, false);
        button.GetComponent<ResourceInfoButton>().Initialise(resourceInfo);
        resourceInfoNameInput.text = "";
    }

    public void AddTroopInfoClick()
    {
        string troopInfoName = troopInfoNameInput.text;
        if(troopInfoName.Length < 2)
        {
            Debug.Log("Troop name must consist of at least 2 characters!");
            return;
        }
        if(map.troopInfos.Any(obj => obj.name == troopInfoName))
        {
            Debug.Log("Troop with such name already exists!");
            return;
        }

        TroopInfo troopInfo = new TroopInfo(troopInfoName);
        map.troopInfos.Add(troopInfo);
        GameObject prefab = Resources.Load<GameObject>("Prefabs/TroopInfoButton");
        GameObject button = Instantiate(prefab);
        button.transform.SetParent(troopInfosListContentTransform, false);
        button.GetComponent<TroopInfoButton>().Initialise(troopInfo);
        troopInfoNameInput.text = "";
    }

    public void BackClick()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }

    public void SaveMapClick(){
        if(mapNameInput.text!=null) MapManagement.LoadMapIntoJson(mapNameInput.text);
        else MapManagement.LoadMapIntoJson("map");
    }

    public void ExitClick()
    {
        countryAdjusted = null;
        troopInfoAdjusted = null;
        resourceInfoAdjusted = null;
        canvas.gameObject.SetActive(true);
        countryAdjustmentCanvas.gameObject.SetActive(false);
        troopInfoAdjustmentCanvas.gameObject.SetActive(false);
        resourceInfoAdjustmentCanvas.gameObject.SetActive(false);
    }

    public void CountryHexColorInput()
    {
        if(countryAdjusted != null) countryAdjusted.SetColorFromHex(countryHexColorInput.text);
    }
    public void ProvinceShapeFormLogic(){
        deleted.Clear();
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if(Input.GetKey(KeyCode.LeftControl)){
            cursor.transform.position = NearestPoint((Vector2)worldPosition,-2);
            if(Input.GetKeyDown(KeyCode.Z) && temporaryMarks.transform.childCount>0){
                Destroy(temporaryMarks.transform.GetChild(temporaryMarks.transform.childCount -1).gameObject);
                provincePoints.RemoveAt(provincePoints.Count-1);
            }
        }
        else
        {
            worldPosition.z = -2;
            cursor.transform.position = worldPosition;
        }
        if(Input.GetMouseButtonDown(0) && !IsPointerOverSpecificCanvas(provincePointsCanvas))
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/PointMark");
            GameObject mark = Instantiate(prefab, cursor.transform.position, Quaternion.identity);
            mark.transform.SetParent(temporaryMarks.transform);
            provincePoints.Add((Vector2)cursor.transform.position);
        }
    }
    public void DeletingAndReturingProvincesLogic(){
        if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Z)){
            ProvinceGameObject lastProvince = provinceGameObjects[provinceGameObjects.Count-1];
            foreach(Vector2 point in lastProvince.points){
                allPoints.Remove(point);
            }
            provinceGameObjects.Remove(lastProvince);
            deleted.Push(lastProvince);
            Destroy(lastProvince.gameObject);
        }
        if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Y) && deleted.Count>0){
            ProvinceGameObject newProvince = deleted.Pop();
            ProvinceGameObject province = ShapeTools.CreateProvinceGameObject(newProvince.name, newProvince.points);
            provinceGameObjects.Add(province);
            foreach(Vector2 point in province.points){
                allPoints.Add(point);
            }
        }
        if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.S)){
            SaveMapClick();
        }
    }
    public void AddingProvincesToCountriesLogic()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if(Input.GetMouseButtonDown(0))
        {
            foreach(ProvinceGameObject province in provinceGameObjects)
            {
                if(ShapeTools.IsPointInPolygon((Vector2)worldPosition, province.points.ToArray()))
                {
                    countryAdjusted.AddProvince(province.data);
                    break;
                }
            }
            
        }
    }

    void Update()
    {
        if(isProvinceShapeFormed)
        {
            ProvinceShapeFormLogic();
        }
        else{
            DeletingAndReturingProvincesLogic();
        }
        if(countryAdjusted != null){
            // Debug.Log("Tryb Edycji Kraju!");
            AddingProvincesToCountriesLogic();
        }
    }


    bool IsPointerOverSpecificCanvas(Canvas c)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.GetComponentInParent<Canvas>() == c)
            {
                return true;
            }
        }
        return false;
    }

    Vector3 NearestPoint(Vector2 worldPosition, int zet){
        double min_distance=double.MaxValue;
        Vector2 nearest_point = worldPosition;
        foreach(Vector2 point in allPoints){
            double distance = Vector2.Distance(worldPosition,point);
            if(distance<min_distance && point!=worldPosition){min_distance=distance;nearest_point=point;}
        }
        return new Vector3(nearest_point.x,nearest_point.y,zet);
    }

    public void EnterCountryAdjustment(Country country)
    {
        countryAdjusted = country;
        Debug.Log("WSZEDŁEM W COUNTRY ADJUSTMENT!!!");
        canvas.gameObject.SetActive(false);
        countryAdjustmentCanvas.gameObject.SetActive(true);
        countryAdjustmentNameInput.text = country.name;
        countryHexColorInput.text = UnityEngine.ColorUtility.ToHtmlStringRGB(country.color);
    }

    public void EnterTroopInfoAdjustment(TroopInfo troopInfo)
    {
        troopInfoAdjusted = troopInfo;
        canvas.gameObject.SetActive(false);
        troopInfoAdjustmentCanvas.gameObject.SetActive(true);
        TroopInfoAdjustmentPanel.me.Initialise();
    }

    public void EnterResourceInfoAdjustment(ResourceInfo resourceInfo)
    {
        resourceInfoAdjusted = resourceInfo;
        canvas.gameObject.SetActive(false);
        resourceInfoAdjustmentCanvas.gameObject.SetActive(true);
        ResourceInfoAdjustmentPanel.me.Initialise();
    }
}