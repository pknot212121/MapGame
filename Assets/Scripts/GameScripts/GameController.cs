using UnityEngine;
using Fusion;
using System.Collections.Generic;
using System;

public class GameController : MonoBehaviour
{
    byte[] mapDataEncoded;
    string mapData=null;
    void Start(){

    }
    void Update(){
        if(mapData==null){
            if(MapLoading.Instance!=null){
                Debug.Log("Wykryto instancję!");
                mapDataEncoded = new byte[MapLoading.Instance.dataJson.Length];
                for (int i = 0; i < MapLoading.Instance.dataJson.Length; i++)
                {
                    mapDataEncoded[i] = MapLoading.Instance.dataJson[i];
                }
                mapData = MapManagement.Decompress(mapDataEncoded);
                Debug.Log(mapData);
                AllProvincesData allProvinces = JsonUtility.FromJson<AllProvincesData>(mapData);
                foreach(ProvinceData provinceData in allProvinces.allProvinces){
                    List<Vector2> points = new List<Vector2>();
                    foreach (string text in provinceData.points)
                    {
                        string[] czesci = text.Split(new char[] { ';' }, 2);
                        points.Add(new Vector2(float.Parse(czesci[0]),float.Parse(czesci[1])));
                    }
                    Province province = ShapeTools.CreateProvince(provinceData.name,points);
                    province.SetColor(provinceData.color);
                }
            }
            else{
                Debug.Log("Nie wykryto Instancji!");
            }
        }
    }

}