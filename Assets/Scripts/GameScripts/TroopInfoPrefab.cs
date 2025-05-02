using UnityEngine;
using UnityEngine.UI; 
using TMPro;          
using System.Collections.Generic;

public class TroopInfoPrefab : MonoBehaviour
{
    [SerializeField] private TMP_Text troopName;
    [SerializeField] private TMP_Text troopCount;

    public void Initialise(TroopInfo troopInfo,int count)
    {
        troopName.text = troopInfo.name;
        troopCount.text = count.ToString();
    }

}