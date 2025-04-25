using UnityEngine;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class Province
{
    public string name;
    public Country country;
    public List<string> points = new List<string>();

    public ProvinceGameObject gameObject;

    public Province(string name)
    {
        this.name = name;
    }
}