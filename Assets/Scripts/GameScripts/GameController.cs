using UnityEngine;
using Fusion;
using System.Collections.Generic;
using System;

public class GameController : MonoBehaviour
{
    string mapData=null;
    void Start(){

    }
    void Update(){
        if(mapData==null){
            if(MapLoading.Instance!=null){
                Debug.Log("Wykryto instancję!");
                mapData = MapLoading.Instance.dataJson;
                Debug.Log(mapData);
                AllProvincesData allProvinces = JsonUtility.FromJson<AllProvincesData>(mapData);
                foreach(ProvinceData provinceData in allProvinces.allProvinces){
                    Province province = ShapeTools.CreateProvince(provinceData.name,provinceData.points);
                    province.SetColor(provinceData.color);
                }
            }
            else{
                Debug.Log("Nie wykryto Instancji!");
            }
        }
    }

}