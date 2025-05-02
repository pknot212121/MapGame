using UnityEngine;
using UnityEngine.UI; 
using TMPro;          
using System.Collections.Generic;

public class TroopInProvincePrefab : MonoBehaviour
{
    [SerializeField] private TMP_Text mostCommonTroopName;
    [SerializeField] private TMP_Text summarisedTroopCount;
    public Troop troop1;

    public void Initialise(Troop troop,TroopInfo troopInfo,int count)
    {
        mostCommonTroopName.text = troopInfo.name;
        summarisedTroopCount.text = count.ToString();
        troop1 = troop;
    }
    public void OnButtonClick()
    {
        GameController.me.ShowTroopMenu(troop1);
    }

}