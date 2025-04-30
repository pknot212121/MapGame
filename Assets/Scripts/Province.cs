using UnityEngine;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class Province : Entity
{
    public string name;
    public List<Vector2> points = new List<Vector2>();
    [System.NonSerialized]
    public Country country;

    [System.NonSerialized]
    public ProvinceGameObject gameObject;

    public Province(string name)
    {
        this.name = name;
    }

    public Province(string name, ProvinceGameObject gameObject)
    {
        this.name = name;
        this.gameObject = gameObject;
        points = gameObject.points;
    }

}