using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceInfoButton : MonoBehaviour
{
    public ResourceInfo resourceInfo;
    [SerializeField] TMP_Text buttonText;

    public void Initialise(ResourceInfo resourceInfo)
    {
        this.resourceInfo = resourceInfo;
        buttonText.text = resourceInfo.name;
    }

    public void Click()
    {
        MapCreatorController.me.EnterResourceInfoAdjustment(resourceInfo);
    }
}