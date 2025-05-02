using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class ProvinceGO : MonoBehaviour
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
        data.go = this;
        this.points = points;
        if(points == null) points = new List<Vector2>();
    }

    public void Initialise(string name, List<Vector2> points = null)
    {
        data = new Province(name, this);
        this.points = points;
        data.points = points;
        if(points == null) points = new List<Vector2>();
    }

    public IEnumerator HighlightForSelection()
    {
        Color originalColor = Color.grey;
        if(data.country != null) originalColor = data.country.color;
        var material = gameObject.GetComponent<MeshRenderer>().material;
        while(GameController.me.actionPrepared != null)
        {
            material.color = Color.white;
            yield return new WaitForSeconds(0.5f);
            material.color = originalColor;
            yield return new WaitForSeconds(0.5f);
        }
        material.color = originalColor;
        yield return null;
    }

    // public void EvaluateTaxes(){
    //     foreach(ResourceInfo resource in resourceGeneration.Keys){
    //         Resource[resource]+=(int)(population*resourceGeneration[resource]*Random.Range(0.8f,1.2f));

    //     }
    // }
}
