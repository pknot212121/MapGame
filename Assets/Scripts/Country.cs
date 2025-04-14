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

    public void SetColorFromHex(string hex)
    {
        if(!Regex.IsMatch(hex, "^#[0-9A-F]{6}$")){
            foreach(string name in colorsFromText.Keys){
                if(hex.ToLower()==name){
                    color=colorsFromText[name];
                    break;
                }
            }
        }
        else{ColorUtility.TryParseHtmlString(hex, out color);}
        
        foreach(Province p in provinces)
        {
            p.SetColor(color);
        }
    }

    public void AddProvince(Province province)
    {
        bool areColorsColliding = false;
        foreach (Country country in MapCreatorController.me.countries)
        {
            if(country != this && country.color == color){
                Debug.Log("Dwa państwa nie mogą mieć takiego samego koloru!!!");
                areColorsColliding=true;
            }
        }
        if(!areColorsColliding){
            province.country = this;
            provinces.Add(province);
            province.SetColor(color);
            foreach (Country country in MapCreatorController.me.countries)
            {
                if(country.provinces.Contains(province)){
                    country.provinces.Remove(province);
                    break;
                }
            }
        }
        
        if(MapCreatorController.me.countryAdjusted != null)
        {
            // odswierzyc zawartosc listy
        }

    }
}
