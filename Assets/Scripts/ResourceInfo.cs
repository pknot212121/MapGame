using UnityEngine;

public class ResourceInfo
{
    public string name;
    public float value;

    public ResourceInfo(string name, float value = 1)
    {
        this.name = name;
        this.value = value;
    }
}
