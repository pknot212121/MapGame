using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Map
{
    public List<Province> provinces = new List<Province>();
    public List<Country> countries = new List<Country>();
    public List<ResourceInfo> resourceInfos = new List<ResourceInfo>();
    public List<TroopInfo> troopInfos = new List<TroopInfo>();

    public Country GetCountry(Province province){
        foreach(Country country in countries){
            foreach(Province province1 in country.provinces){ 
                if(province1.name==province.name) return country;

            }
        }
        return null;
    }

    public Country GetCountry(string countryName)
    {
        return countries.FirstOrDefault(c => c.name == countryName);
    }
    public Province GetProvince(string provinceName)
    {
        return provinces.FirstOrDefault(p => p.name == provinceName);
    }
}