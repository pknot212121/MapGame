using UnityEngine;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class Map
{
    public List<Province> provinces = new List<Province>();
    public List<Country> countries = new List<Country>();
    public List<ResourceInfo> resourceInfos = new List<ResourceInfo>();
    public List<TroopInfo> troopInfos = new List<TroopInfo>();
}