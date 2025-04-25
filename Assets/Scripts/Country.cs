using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class Country
{
    public string name;
    public Color color;
    public List<Province> provinces = new List<Province>();

    public Dictionary<string,Color> colorsFromText = new Dictionary<string, Color>{
        ["blue"] = Color.blue,
        ["red"] = Color.red,
        ["green"] = Color.green,
        ["yellow"] = Color.yellow,
        ["black"] = Color.black,
        ["white"] = Color.white,
        ["grey"] = Color.grey
    };
    public Country(){}
    public Country(string _name,Color _color){
        name=_name;
        color=_color;
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

        province.country = this;
        provinces.Add(province);
        if(province.gameObject) province.gameObject.SetColor(color);
        
        if(MapCreatorController.me.countryAdjusted != null)
        {
            // odswierzyc zawartosc listy
        }

    }
}
