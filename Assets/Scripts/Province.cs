using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class Province : MonoBehaviour
{
    public new string name;
    public List<Vector2> points = new List<Vector2>();
    public Country country = null;

    Dictionary<ResourceInfo,double> resourceGeneration = new Dictionary<ResourceInfo,double>();

    Resource resourceStockpiles;

    int population;

    public void SetColor(Color color)
    {
        gameObject.GetComponent<MeshRenderer>().material.color = color;
    }
    public void Initialise(string name,List<Vector2> points){
        this.name = name;
        this.points = points;
    }

    // public void EvaluateTaxes(){
    //     foreach(ResourceInfo resource in resourceGeneration.Keys){
    //         Resource[resource]+=(int)(population*resourceGeneration[resource]*Random.Range(0.8f,1.2f));

    //     }
    // }
}
