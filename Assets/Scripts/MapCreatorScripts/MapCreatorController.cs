using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Linq;

public class MapCreatorController : MonoBehaviour
{
    public Canvas canvas;
    public Canvas provincePointsCanvas;
    public Canvas countryAdjustmentCanvas;

    public GameObject temporaryMarks;
    public GameObject cursor;
    public GameObject countriesListContent;

    [SerializeField] private TMP_InputField createProvinceNameInput;
    [SerializeField] private TMP_InputField countryNameInput;
    [SerializeField] private TMP_InputField countryAdjustmentNameInput;
    [SerializeField] private TMP_InputField countryHexColorInput;

    List<Province> provinces = new List<Province>();
    List<Country> countries = new List<Country>();

    Stack<Province> deleted = new Stack<Province>();

    Map map = new Map();

    bool isProvinceShapeFormed = false;
    public Country countryAdjusted = null;

    List<Vector2> provincePoints = null;
    private Keyboard keyboard;
    List<Vector2> allPoints = new List<Vector2>();

    public static MapCreatorController me;

    void Start()
    {
        me = this;
    }

    public void CreateProvinceClick()
    {
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

        Province province = ShapeTools.CreateProvince(provinceName, provincePoints);
        if(!province) 
        {
            Debug.Log("Couldn't create the shape!");
            return;
        }
        provinces.Add(province);
        foreach(Vector2 point in provincePoints){allPoints.Add(point);}

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
        foreach(Country c in countries)
        {
            if(c.name == countryName)
            {
                Debug.Log("Country with such name already exists!");
                return;
            }
        }

        Country country = new Country();
        country.name = countryName;
        countries.Add(country);

        GameObject prefab = Resources.Load<GameObject>("Prefabs/CountryButton");
        GameObject button = Instantiate(prefab);
        button.transform.SetParent(countriesListContent.transform, false);
        button.GetComponent<CountryButton>().Initialise(country);
        countryNameInput.text = "";
    }

    public void BackClick()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }

    public void ExitClick()
    {
        countryAdjusted = null;
        canvas.gameObject.SetActive(true);
        countryAdjustmentCanvas.gameObject.SetActive(false);
    }

    public void CountryHexColorInput()
    {
        if(countryAdjusted != null) countryAdjusted.SetColorFromHex(countryHexColorInput.text);
    }

    void Update()
    {
        if(isProvinceShapeFormed)
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if(Input.GetKey(KeyCode.LeftControl)){
                cursor.transform.position = NearestPoint((Vector2)worldPosition);
                if(Input.GetKeyDown(KeyCode.Z) && temporaryMarks.transform.childCount>0){
                    Destroy(temporaryMarks.transform.GetChild(temporaryMarks.transform.childCount -1).gameObject);
                    provincePoints.RemoveAt(provincePoints.Count-1);

                }
            }
            else
            {
                worldPosition.z = 0;
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
        else{
            
            if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Z)){
                Province lastProvince = provinces[provinces.Count-1];
                foreach(Vector2 point in lastProvince.points){
                    allPoints.Remove(point);
                }
                provinces.Remove(lastProvince);
                deleted.Push(lastProvince);
                Destroy(lastProvince.gameObject);
            }
            if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Y) && deleted.Count>0){
                Province newProvince = deleted.Pop();
                Province province = ShapeTools.CreateProvince(newProvince.name, newProvince.points);
                provinces.Add(province);
                foreach(Vector2 point in province.points){
                    allPoints.Add(point);
                }
                
            }

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

    Vector2 NearestPoint(Vector2 worldPosition){
        double min_distance=double.MaxValue;
        Vector2 nearest_point = worldPosition;
        foreach(Vector2 point in allPoints){
            double distance = Vector2.Distance(worldPosition,point);
            if(distance<min_distance && point!=worldPosition){min_distance=distance;nearest_point=point;}
        }
        return nearest_point;
    }

    public void EnterCountryAdjustment(Country country)
    {
        countryAdjusted = country;
        canvas.gameObject.SetActive(false);
        countryAdjustmentCanvas.gameObject.SetActive(true);
        countryAdjustmentNameInput.text = country.name;
    }
}