using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class Province : Entity
{
    public string name;
    public List<Vector2> points = new List<Vector2>();
    [System.NonSerialized]
    public Country country;

    [System.NonSerialized]
    public ProvinceGO go;

    public string countryName;

    public Resource resourceGeneration = new Resource();
    public Resource resourceStockpiles = new Resource();
    public List<Troop> troops = new List<Troop>();

    public Province()
    {
    }

    public Province(string name)
    {
        this.name = name;
    }
    public Province(string name, ProvinceGO go)
    {
        this.name = name;
        this.go = go;
        points = go.points;
    }
    public Province(int id,string name, ProvinceGO go)
    {
        this.id = id;
        this.name = name;
        this.go = go;
        points = go.points;
    }

}