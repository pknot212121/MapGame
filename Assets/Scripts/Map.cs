using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.IO.Compression;
using System.Text;
using Unity.VisualScripting;
using Unity.Serialization.Json;

public class Map : Entity
{
    public List<Province> provinces = new List<Province>();
    public List<Country> countries = new List<Country>();
    public List<ResourceInfo> resourceInfos = new List<ResourceInfo>();
    public List<TroopInfo> troopInfos = new List<TroopInfo>();
    public List<Troop> troops = new List<Troop>();
    public int EntityCounter;
    public static Map me;

    public Country GetCountry(Province province){
        foreach(Country country in countries){
            foreach(Province province1 in country.provinces){ 
                if(province1.name==province.name) return country;
            }
        }
        return null;
    }

    public Country GetCountry(string countryName)
    {
        return countries.FirstOrDefault(c => c.name == countryName);
    }
    public Province GetProvince(string provinceName)
    {
        return provinces.FirstOrDefault(p => p.name == provinceName);
    }
    public List<Province> GetNeighboringProvinces(Province province1)
    {
        List<Province> neighbors = new List<Province>();
        foreach(Province province2 in provinces)
        {
            if(ShapeTools.AreTwoProvincesNeighbors(province1,province2)) neighbors.Add(province2);
        }
        return neighbors;
    } 


    public static void LoadMapIntoJson(string filename)
    {
        string path =  Application.persistentDataPath + "/" + filename + ".json";
        MapCreatorController.me.map.EntityCounter = MapCreatorController.me.EntityCounter;
        string json = JsonSerialization.ToJson(MapCreatorController.me.map);
        Debug.Log("Ścieżka zapisu: " + Application.persistentDataPath);
        System.IO.File.WriteAllText(path,json);
    }
    public string SerializeToJson()
    {
        string json = JsonSerialization.ToJson(me);
        return json;
    }
    public static string LoadMapFromJson(string filename)
    {
        string path =  Application.persistentDataPath + "/" + filename + ".json";
        string json = System.IO.File.ReadAllText(path);
        return json;
    }
    public static byte[] Compress(string data)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(data);
        using (var output = new MemoryStream())
        {
            using (var gzip = new GZipStream(output, System.IO.Compression.CompressionLevel.Optimal))
            {
                gzip.Write(bytes, 0, bytes.Length);
            }
            return output.ToArray();
        }
    }
    public static string Decompress(byte[] compressedData)
    {
        using (var input = new MemoryStream(compressedData))
        using (var gzip = new GZipStream(input, CompressionMode.Decompress))
        using (var reader = new StreamReader(gzip))
        {
            return reader.ReadToEnd();
        }
    }
}