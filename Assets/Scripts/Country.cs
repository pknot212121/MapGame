using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class Country : Entity
{
    public string name;
    public Color color;
    public List<Province> provinces = new List<Province>();
    // public Map map;

    public Dictionary<string,Color> colorsFromText = new Dictionary<string, Color>{
        ["blue"] = Color.blue,
        ["red"] = Color.red,
        ["green"] = Color.green,
        ["yellow"] = Color.yellow,
        //["black"] = Color.black, // to po prostu nie
        //["white"] = Color.white, // biały to kolor prowincji niczyjej więc państwo nie może go mieć
        ["grey"] = Color.grey
    };
    public Country(){}
    public Country(int id,string name)
    {
        this.id = id;
        this.name = name;
        SetRandomColor();
    }
    public override void Pack()
    {
        // foreach(Province province in provinces)
        // {
        //     province.Pack();
        // }
        base.Pack();
    }
    public override void Unpack()
    {
        // foreach(Province province in provinces)
        // {
        //     province.country = this;
        //     province.Unpack();
        // }
        base.Pack();
    }


    public void SetRandomColor()
    {
        color = Color.white;
        while(color == Color.white)
        {
            color = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
            foreach(Country c in MapCreatorController.me.map.countries)
            {
                if(color == c.color && c != this) color = Color.white;
            }
        }
        foreach(Province p in provinces)
        {
            if(p.gameObject) p.gameObject.SetColor(color);
        }
    }

    public Country(string name, Color color)
    {
        this.name = name;
        this.color = color;
    }

    public void SetColorFromHex(string hex)
    {
        Color c = Color.grey;
        if(Regex.IsMatch(hex, "^#[0-9A-Fa-f]{6}$")) ColorUtility.TryParseHtmlString(hex, out c);
        else if(Regex.IsMatch(hex, "^[0-9A-Fa-f]{6}$")) ColorUtility.TryParseHtmlString("#" + hex, out c);
        else 
        {
            foreach(string name in colorsFromText.Keys)
            {
                if(hex.ToLower()==name)
                {
                    c = colorsFromText[name];
                    break;
                }
            }
        }
        if(c == null)
        {
            Debug.Log("Error applying color");
            return;
        }
        foreach(Country country in MapCreatorController.me.map.countries)
        {
            if(country.color == c && country != this)
            {
                Debug.Log("2 countries can't have the same color");
                return;
            }
        }
        color = c;
        
        foreach(Province p in provinces)
        {
            if(p.gameObject) p.gameObject.SetColor(color);
        }
    }

    public void AddProvince(Province province)
    {
        foreach (Country country in MapCreatorController.me.map.countries)
        {
            if(country.provinces.Contains(province))
            {
                country.provinces.Remove(province);
                break;
            }
        }

        provinces.Add(province);
        if(province.gameObject) province.gameObject.SetColor(color);
        province.country = this;
        if(MapCreatorController.me.countryAdjusted != null)
        {
            // odswierzyc zawartosc listy
        }

    }
}
