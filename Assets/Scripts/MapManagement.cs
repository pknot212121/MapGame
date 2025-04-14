using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UIElements;
using System.Data.Common;
using UnityEngine.Rendering;

public class MapManagement{
    public static void LoadMapIntoJson(string filename){
        foreach(Province province in MapCreatorController.me.provinces){
            ProvinceData data = new ProvinceData();
            data.name = province.name;
            data.points = province.points;
            data.countryName = province.country.name;
            string json = JsonUtility.ToJson(data,prettyPrint: true);
            string path =  Application.persistentDataPath + "/" + filename + ".json";
            System.IO.File.WriteAllText(path,json);
        }
        

    }
}