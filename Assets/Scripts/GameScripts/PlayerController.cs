using Fusion;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [Networked] public NetworkDictionary<float,string> playersToCountries{get;} = new
        NetworkDictionary<float, string>();
    public static PlayerController Instance{get;private set;}

    public override void Spawned()
    {
        if(Instance==null){Instance=this;}
        else if(Instance!=this){Destroy(gameObject);return;}
    }
    public override void FixedUpdateNetwork()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            
            playersToCountries.Add(Random.Range(1,10),"aaa");
            Debug.Log("Dodano wpis!");
        }
    }
}