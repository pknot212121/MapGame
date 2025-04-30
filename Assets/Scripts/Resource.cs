using System.Collections.Generic;
using UnityEngine;

public class Resource : Entity
{
    public Dictionary<ResourceInfo,int> content = new Dictionary<ResourceInfo,int>();

    public Resource(List<ResourceInfo> resourceInfos){
        foreach(ResourceInfo resourceInfo in resourceInfos){
            content[resourceInfo]=0;
        }
    }
}
