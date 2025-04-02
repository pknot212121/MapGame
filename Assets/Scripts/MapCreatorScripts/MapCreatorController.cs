using System.Collections.Generic;
using UnityEngine;

public class MapCreatorController : MonoBehaviour
{
    public GameObject canvas;
    public GameObject provincePointsCanvas;
    public GameObject temporaryMarks;
    Map map = new Map();
    bool isProvinceShapeFormed = false;
    List<Vector2> provincePoints = null;

    public void CreateProvinceClick()
    {
        isProvinceShapeFormed = true;
        canvas.SetActive(false);
        provincePointsCanvas.SetActive(true);
        provincePoints = new List<Vector2>();
    }

    public void OkClick()
    {
        isProvinceShapeFormed = false;
        canvas.SetActive(true);
        provincePointsCanvas.SetActive(false);


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
            if(Input.GetMouseButtonDown(0))
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
}