using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class ProvinceGameObject : MonoBehaviour
{
    public Province data;
    public List<Vector2> points;

    Dictionary<ResourceInfo, double> resourceGeneration = new Dictionary<ResourceInfo,double>();

    Resource resourceStockpiles;

    public void SetColor(Color color)
    {
        gameObject.GetComponent<MeshRenderer>().material.color = color;
    }

    public void Initialise(Province data, List<Vector2> points = null)
    {
        this.data = data;
        data.gameObject = this;
        this.points = points;
        if(points == null) points = new List<Vector2>();
        if(data.country != null) SetColor(data.country.color);
    }

    public void Initialise(string name, List<Vector2> points = null)
    {
        data = new Province(name, this);
        this.points = points;
        if(points == null) points = new List<Vector2>();
    }

    // public void EvaluateTaxes(){
    //     foreach(ResourceInfo resource in resourceGeneration.Keys){
    //         Resource[resource]+=(int)(population*resourceGeneration[resource]*Random.Range(0.8f,1.2f));

    //     }
    // }
}
