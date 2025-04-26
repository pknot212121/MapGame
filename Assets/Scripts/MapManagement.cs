using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UIElements;
using System.Data.Common;
using UnityEngine.Rendering;
using System.IO.Compression;
using System.IO;
using System.Text;
using Fusion;
using System.Drawing;

public class MapManagement
{
    public static void LoadMapIntoJson(string filename){
        string path =  Application.persistentDataPath + "/" + filename + ".json";
        string json = JsonUtility.ToJson(MapCreatorController.me.map);
        Debug.Log("Ścieżka zapisu: " + Application.persistentDataPath);
        System.IO.File.WriteAllText(path,json);
    }
    public static string LoadMapFromJson(string filename){
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