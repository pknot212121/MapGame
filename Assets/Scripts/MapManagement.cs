using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UIElements;
using System.Data.Common;
using UnityEngine.Rendering;

public class MapManagement{
    public static void LoadMapIntoJson(string filename){
        string path =  Application.persistentDataPath + "/" + filename + ".json";
        AllProvincesData allData = new AllProvincesData();
        foreach(Province province in MapCreatorController.me.provinces){
            ProvinceData data = new ProvinceData();
            data.name = province.name;
            data.points = province.points;
            if(province.country==null){data.countryName="";data.color=Color.white;}
            else{data.countryName = province.country.name;data.color=province.country.color;}
            allData.allProvinces.Add(data);
        }
        string json = JsonUtility.ToJson(allData);
        Debug.Log("Ścieżka zapisu: " + Application.persistentDataPath);
        System.IO.File.WriteAllText(path,json);
    }
    public static string LoadMapFromJson(string filename){
        string path =  Application.persistentDataPath + "/" + filename + ".json";
        string json = System.IO.File.ReadAllText(path);
        return json;

    }
}