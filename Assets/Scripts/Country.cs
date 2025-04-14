using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class Country
{
    public string name;
    public Color color;
    public List<Province> provinces = new List<Province>();

    public void SetColorFromHex(string hex)
    {
        if(!Regex.IsMatch(hex, "^#[0-9A-F]{6}$")) return;
        ColorUtility.TryParseHtmlString(hex, out color);

        foreach(Province p in provinces)
        {
            p.SetColor(color);
        }
    }

    public void AddProvince(Province province)
    {
        province.country = this;
        provinces.Add(province);
        province.SetColor(color);
        if(MapCreatorController.me.countryAdjusted != null)
        {
            // odswierzyc zawartosc listy
        }
    }
}
