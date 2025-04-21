using System;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using TMPro;
using JetBrains.Annotations;

public class MapLoading : NetworkBehaviour{
    [Networked,Capacity(2048)] public string dataJson{get;set;}
    public string filename=null;

    public static MapLoading Instance{get;private set;}
    public override void Spawned()
    {
        if(Instance==null){Instance=this;}
        else if(Instance!=this){Destroy(gameObject);return;}

        IReadOnlyDictionary<string, SessionProperty> sessionProperties = Runner.SessionInfo.Properties;
        filename = sessionProperties["filename"];
        string mapData = MapManagement.LoadMapFromJson(filename);
        Debug.Log(mapData);
        dataJson = mapData;
        Debug.Log(dataJson);
    }
}