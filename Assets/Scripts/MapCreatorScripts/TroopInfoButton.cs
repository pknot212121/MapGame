using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TroopInfoButton : MonoBehaviour
{
    public TroopInfo troopInfo;
    [SerializeField] TMP_Text buttonText;

    public void Initialise(TroopInfo troopInfo)
    {
        this.troopInfo = troopInfo;
        buttonText.text = troopInfo.name;
    }

    public void Click()
    {
        MapCreatorController.me.EnterTroopInfoAdjustment(troopInfo);
    }
}