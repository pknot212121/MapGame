using UnityEngine;
using Fusion;
using System.Collections.Generic;
using System;

public class GameController : MonoBehaviour
{
    List<Province> provinces = new List<Province>();
    
    void Start(){
        NetworkRunner runner = NetworkController.me.GetRunner();
        IReadOnlyDictionary<string, SessionProperty> sessionProperties = runner.SessionInfo.Properties;
        if (sessionProperties.TryGetValue("mapdata", out SessionProperty mapDataProperty))
        {
            string mapDataJson = mapDataProperty;
            Debug.Log(mapDataJson);
            List<Province> provinces = new List<Province>();
            AllProvincesData allProvincesData = JsonUtility.FromJson<AllProvincesData>(mapDataJson);
            foreach(ProvinceData data in allProvincesData.allProvinces){
                Province province = ShapeTools.CreateProvince(data.name,data.points);
                provinces.Add(province);
            }
        }
    }
}