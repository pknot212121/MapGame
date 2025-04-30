using UnityEngine;
public class ResourceInfo : Entity
{
    public string name;
    public float value;

    public ResourceInfo(){}
    public ResourceInfo(string name, float value = 1)
    {
        this.name = name;
        this.value = value;
    }

    public override void Pack()
    {
        base.Pack();
    }
    public override void Unpack()
    {
        base.Unpack();
    }
}
