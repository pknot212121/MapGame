using UnityEngine;
using System.Collections.Generic;

public class Troop : Entity
{
    int id = 0; // Identyfikator jednostki, może być używany do wysyłki
    public Dictionary<TroopInfo, int> numbers;
    public Resource transportedResources;

    public Country country; // Właściciel
    public Province province; // Prowincja na której przebywa (nie zawsze należy ona do właściciela)

    public string countryName = null; // Używane przy wysyłce
    public string provinceName = null; // Używane przy wysyłce

    public Troop(int id, int numberOfEachTroopInfo, Country country, Province province) // Możliwe że ten konstruktor to tymczasowe rozwiązanie
    {
        this.id = id;
        this.numbers = new Dictionary<TroopInfo, int>();
        foreach(TroopInfo ti in GameController.me.map.troopInfos)
        {
            numbers.Add(ti, numberOfEachTroopInfo);
        }
        this.country = country;
        this.province = province;
    }

    public override void Pack() 
    {
        if(country != null) 
        {
            countryName = country.name;
            country = null;
        }
        if(province != null) 
        {
            provinceName = province.name;
            province = null;
        }
    }

    public override void Unpack()
    {
        if(countryName != null) 
        {
            GameController.me.map.GetCountry(countryName);
        }
        if(provinceName != null) 
        {
            GameController.me.map.GetProvince(provinceName);
        }
    }
}
