using UnityEngine;
using Fusion;
using System.Collections.Generic;
using System;

public class GameController : MonoBehaviour
{
    byte[] mapDataEncoded;
    string mapData=null;
    void Start()
    {

    }
    void Update(){
    //     if(mapData==null){
    //         if(MapLoading.Instance!=null){
    //             Debug.Log("Wykryto instancję!");
    //             mapDataEncoded = new byte[MapLoading.Instance.dataJson.Length];
    //             for (int i = 0; i < MapLoading.Instance.dataJson.Length; i++)
    //             {
    //                 mapDataEncoded[i] = MapLoading.Instance.dataJson[i];
    //             }
    //             mapData = MapManagement.Decompress(mapDataEncoded);
    //             Debug.Log(mapData);
    //             Transform provinceParentObjectTransform = GameObject.Find("Provinces").transform;
    //             Map allProvinces = JsonUtility.FromJson<Map>(mapData);
    //             foreach(Province provinceData in allProvinces.provinces){
    //                 List<Vector2> points = new List<Vector2>();
    //                 foreach (Vector2 point in provinceData.points)
    //                 {
    //                     points.Add(point);
    //                 }
    //                 ProvinceGameObject province = ShapeTools.CreateProvinceGameObject(provinceData.name,points);
    //                 if(allProvinces.GetCountry(provinceData)==null) province.SetColor(Color.white);
    //                 else province.SetColor(allProvinces.GetCountry(provinceData).color);
    //                 province.gameObject.transform.SetParent(provinceParentObjectTransform);
    //             }
    //         }
    //         else{
    //             Debug.Log("Nie wykryto Instancji!");
    //         }
    //     }
    // }
    }

}