using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class TroopInfo
{
    public string name;
    public float overallPowerMultiplier;
    public string iconName = "sword_icon";
    public Dictionary<string, float> individualPowerMultiplier;
    public Dictionary<string, int> unitCost;


    public TroopInfo(string name, float overallPowerMultiplier = 1, Dictionary<string, int> unitCost = null, Dictionary<string, float> individualPowerMultiplier = null)
    {
        this.name = name;
        this.overallPowerMultiplier = overallPowerMultiplier;
        this.individualPowerMultiplier = individualPowerMultiplier;
        if(this.individualPowerMultiplier == null) this.individualPowerMultiplier = new Dictionary<string, float>();
        this.unitCost = unitCost;
        if(this.unitCost == null) this.unitCost = new Dictionary<string, int>();
    }

    public void SetIndividualPowerMultiplier(string name, float multiplier)
    {
        try
        {
            individualPowerMultiplier.Add(name, multiplier);
        }
        catch (ArgumentException) // jeśli już istnieje
        {
            individualPowerMultiplier[name] = multiplier;
        }
    }

    public void SetUnitCostValue(string name, int value)
    {
        if(value == 0)
        {
            if(unitCost.ContainsKey(name)) unitCost.Remove("name");
            return;
        }
        try
        {
            unitCost.Add(name, value);
        }
        catch (ArgumentException) // jeśli już istnieje
        {
            unitCost[name] = value;
        }
    }
}
