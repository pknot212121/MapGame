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
    public ProvinceGameObject gameObject;

    public string countryName;

    public Province()
    {
    }

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
    public override void Pack()
    {
        if(country != null)
        {
            countryName = country.name;
            country = null;
        }
    }
    public override void Unpack()
    {
        //Debug.Log(countryName);
        if(countryName != null) country = GameController.me.map.GetCountry(countryName);
    }

}