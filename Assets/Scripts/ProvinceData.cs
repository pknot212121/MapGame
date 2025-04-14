using UnityEngine;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class ProvinceData{
    public string name;
    public List<Vector2> points = new List<Vector2>();
    public string countryName;
}