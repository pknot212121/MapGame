using UnityEngine;
using TMPro;
using System;
using System.Linq;

public class ResourceInfoAdjustmentPanel : MonoBehaviour
{
    public static ResourceInfoAdjustmentPanel me;
    public ResourceInfo adjusted;

    [SerializeField] private TMP_InputField resourceInfoNameInput;
    [SerializeField] private TMP_InputField valueInput;

    void Awake()
    {
        me = this;
    }

    public void Initialise()
    {
        adjusted = MapCreatorController.me.resourceInfoAdjusted;
        resourceInfoNameInput.text = adjusted.name;
        valueInput.text = adjusted.value.ToString();
    }

    public void ResourceInfoNameInput()
    {
        if(resourceInfoNameInput.text.Length >= 2 && !MapCreatorController.me.resourceInfos.Any(obj => obj.name == resourceInfoNameInput.text))
            adjusted.name = resourceInfoNameInput.text;
        else resourceInfoNameInput.text = adjusted.name;
    }

    public void ValueInput()
    {
        if(float.TryParse(valueInput.text, out float result) && result > 0)
        {
            MapCreatorController.me.resourceInfoAdjusted.value = result;
            valueInput.text = result.ToString();
        }
        else
        {
            valueInput.text = "1";
            MapCreatorController.me.resourceInfoAdjusted.value = 1;
        }
    }
}
