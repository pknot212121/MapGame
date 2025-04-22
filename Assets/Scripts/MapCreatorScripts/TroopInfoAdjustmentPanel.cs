using UnityEngine;
using TMPro;
using System;
using System.Linq;

public class TroopInfoAdjustmentPanel : MonoBehaviour
{
    public static TroopInfoAdjustmentPanel me;
    public TroopInfo adjusted;

    [SerializeField] private TMP_InputField troopInfoNameInput;
    [SerializeField] private TMP_InputField powerMultiplierInput;
    [SerializeField] private Transform unitCostListContentTransform;

    [SerializeField] private GameObject resourceCostPanelPrefab;

    void Awake()
    {
        me = this;
    }

    public void Initialise()
    {
        adjusted = MapCreatorController.me.troopInfoAdjusted;
        troopInfoNameInput.text = adjusted.name;
        powerMultiplierInput.text = adjusted.overallPowerMultiplier.ToString();
        foreach(Transform t in unitCostListContentTransform)
        {
            Destroy(t.gameObject);
        }
        foreach(ResourceInfo ri in MapCreatorController.me.resourceInfos)
        {
            ResourceCostPanel p = Instantiate(resourceCostPanelPrefab).GetComponent<ResourceCostPanel>();
            p.gameObject.transform.SetParent(unitCostListContentTransform, false);
            p.Initialise(ri);
        }
    }

    public void TroopInfoNameInput()
    {
        if(troopInfoNameInput.text.Length >= 2 && !MapCreatorController.me.troopInfos.Any(obj => obj.name == troopInfoNameInput.text))
            adjusted.name = troopInfoNameInput.text;
        else troopInfoNameInput.text = adjusted.name;
    }

    public void PowerMultiplierInput()
    {
        if(float.TryParse(powerMultiplierInput.text, out float result) && result >= 0)
        {
            MapCreatorController.me.troopInfoAdjusted.overallPowerMultiplier = result;
            powerMultiplierInput.text = result.ToString();
        }
        else
        {
            powerMultiplierInput.text = "1";
            MapCreatorController.me.troopInfoAdjusted.overallPowerMultiplier = 1;
        }
    }
}
