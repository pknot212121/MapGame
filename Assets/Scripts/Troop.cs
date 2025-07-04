using UnityEngine;
using System.Collections.Generic;

public class Troop : Entity
{
    [System.NonSerialized]
    public TroopGO go;
    public Dictionary<TroopInfo, int> numbers = new Dictionary<TroopInfo, int>();
    public Resource transportedResources;

    // [System.NonSerialized]
    public Country country; // Właściciel
    // [System.NonSerialized]
    public Province province; // Prowincja na której przebywa (nie zawsze należy ona do właściciela)

    public Troop(){}

    public Troop(int id, int initialTroopsCount, Country country, Province province, List<TroopInfo> troopInfos) // Możliwe że ten konstruktor to tymczasowe rozwiązanie
    {
        this.id = id;
        this.numbers = new Dictionary<TroopInfo, int>();
        foreach(TroopInfo ti in troopInfos)
        {
            numbers[ti] = initialTroopsCount;
        }
        this.country = country;
        this.province = province;
    }
    public Troop(int id, Country country, Province province,int minRange,int maxRange,List<TroopInfo> troopInfos) // Możliwe że ten konstruktor to tymczasowe rozwiązanie
    {
        this.id = id;
        this.numbers = new Dictionary<TroopInfo, int>();
        foreach(TroopInfo ti in troopInfos)
        {
            numbers[ti] = Random.Range(minRange,maxRange);
        }
        this.country = country;
        this.province = province;
    }

    public Troop(int id, Country country, Province province, Dictionary<TroopInfo, int> numbers) // Możliwe że ten konstruktor to tymczasowe rozwiązanie
    {
        this.id = id;
        this.numbers = new Dictionary<TroopInfo, int>();
        this.numbers = numbers;
        this.country = country;
        this.province = province;
    }
    // public void Fill(List<TroopInfo> troopInfos,int minRange,int maxRange)
    // {
    //     foreach(TroopInfo troopInfo in troopInfos)
    //     {
    //         numbers[troopInfo] = Random.Range(minRange,maxRange);
    //     }
    // }

}
