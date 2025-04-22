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
    [Networked,Capacity(8100)] public NetworkArray<byte> dataJson{get;}
    public string filename=null;

    public static MapLoading Instance{get;private set;}
    public override void Spawned()
    {
        if(Instance==null){Instance=this;}
        else if(Instance!=this){Destroy(gameObject);return;}
        if (HasStateAuthority){
            IReadOnlyDictionary<string, SessionProperty> sessionProperties = Runner.SessionInfo.Properties;
            filename = sessionProperties["filename"];
            string mapData = MapManagement.LoadMapFromJson(filename);
            Debug.Log(mapData);
            byte[] rawData = MapManagement.Compress(mapData);
            Debug.Log("DŁUGOŚĆ PRZEKAZU:"+rawData.Length);
            for (int i = 0; i < rawData.Length; i++)
            {
                dataJson.Set(i, rawData[i]);
            }
            Debug.Log(dataJson);
        }
    }

}