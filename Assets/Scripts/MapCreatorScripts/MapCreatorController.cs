using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapCreatorController : MonoBehaviour
{
    public Canvas canvas;
    public Canvas provincePointsCanvas;
    public GameObject temporaryMarks;
    Map map = new Map();
    bool isProvinceShapeFormed = false;
    List<Vector2> provincePoints = null;

    public void CreateProvinceClick()
    {
        isProvinceShapeFormed = true;
        canvas.gameObject.SetActive(false);
        provincePointsCanvas.gameObject.SetActive(true);
        provincePoints = new List<Vector2>();
    }

    public void OkClick()
    {
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
        if(isProvinceShapeFormed)
        {
            if(Input.GetMouseButtonDown(0) && !IsPointerOverSpecificCanvas(provincePointsCanvas))
            {
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                worldPosition.z = 0;
                GameObject prefab = Resources.Load<GameObject>("Prefabs/PointMark");
                GameObject mark = Instantiate(prefab, worldPosition, Quaternion.identity);
                mark.transform.SetParent(temporaryMarks.transform);
                provincePoints.Add((Vector2)worldPosition);
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
}