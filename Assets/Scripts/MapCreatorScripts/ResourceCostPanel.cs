using UnityEngine;
using System;
using System.Linq;
using TMPro;

public class ResourceCostPanel : MonoBehaviour
{
    private ResourceInfo resourceInfo;

    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_InputField costInput;

    public void Initialise(ResourceInfo resourceInfo)
    {
        this.resourceInfo = resourceInfo;
        nameText.text = resourceInfo.name;
        if(TroopInfoAdjustmentPanel.me.adjusted.unitCost.ContainsKey(resourceInfo.name))
            costInput.text = TroopInfoAdjustmentPanel.me.adjusted.unitCost[resourceInfo.name].ToString();
        else costInput.text = "0";
    }

    public void Input()
    {
        if(int.TryParse(costInput.text, out int result) && result >= 0)
        {
            TroopInfoAdjustmentPanel.me.adjusted.SetUnitCostValue(resourceInfo.name, result);
        }
        else
        {
            if(TroopInfoAdjustmentPanel.me.adjusted.unitCost.ContainsKey(resourceInfo.name))
                costInput.text = TroopInfoAdjustmentPanel.me.adjusted.unitCost[resourceInfo.name].ToString();
            else costInput.text = "0";
        }
    }
}
