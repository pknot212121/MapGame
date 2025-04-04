using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

public class MapCreatorController : MonoBehaviour
{
    public Canvas canvas;
    public Canvas provincePointsCanvas;
    public GameObject temporaryMarks;
    public TMP_InputField nameInput;
    Map map = new Map();
    bool isProvinceShapeFormed = false;
    List<Vector2> provincePoints = null;
    private Keyboard keyboard;
    List<Vector2> allPoints = new List<Vector2>();

    public void CreateProvinceClick()
    {
        isProvinceShapeFormed = true;
        canvas.gameObject.SetActive(false);
        provincePointsCanvas.gameObject.SetActive(true);
        provincePoints = new List<Vector2>();
        nameInput.text = "";
    }

    public void OkClick()
    {
        string provinceName = nameInput.text;
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
        foreach(Vector2 point in provincePoints){allPoints.Add(point);}

        isProvinceShapeFormed = false;
        canvas.gameObject.SetActive(true);
        provincePointsCanvas.gameObject.SetActive(false);

        foreach(Transform t in temporaryMarks.transform)
        {
            Destroy(t.gameObject);
        }
        provincePoints = null;
    }

    void Update()
    {
        keyboard = Keyboard.current;
        if(isProvinceShapeFormed)
        {
            if(Input.GetMouseButtonDown(0) && !IsPointerOverSpecificCanvas(provincePointsCanvas))
            {
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                worldPosition.z = 0;
                GameObject prefab = Resources.Load<GameObject>("Prefabs/PointMark");
                Vector2 position = (Vector2)worldPosition;
                if(keyboard.leftCtrlKey.isPressed){
                    Debug.Log(Nearestpoint(position));
                    position = Nearestpoint(position);
                }
                GameObject mark = Instantiate(prefab, new Vector3(position.x,position.y,0), Quaternion.identity);
                mark.transform.SetParent(temporaryMarks.transform);
                provincePoints.Add(position);
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

    Vector2 Nearestpoint(Vector2 worldPosition){
        double min_distance=double.MaxValue;
        Vector2 nearest_point = worldPosition;
        foreach(Vector2 point in allPoints){
            double distance = Vector2.Distance(worldPosition,point);
            if(distance<min_distance && point!=worldPosition){min_distance=distance;nearest_point=point;}
        }
        return nearest_point;
    }
}