using System.Collections.Generic;
using UnityEngine;

public class Resource : Entity
{
    public Dictionary<ResourceInfo,int> content = new Dictionary<ResourceInfo,int>();

    public Resource()
    {

    }
    public void Fill(List<ResourceInfo> resourceInfos,int minRange,int maxRange)
    {
        foreach(ResourceInfo resourceInfo in resourceInfos){
            content[resourceInfo]=Random.Range(minRange,maxRange);
        }
    }
}
