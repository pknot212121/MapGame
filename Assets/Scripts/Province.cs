using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class Province : MonoBehaviour
{
    public new string name;
    public List<Vector2> points = new List<Vector2>();
    public Country country = null;

    public Province(string _name,List<Vector2> _points,Country _country){
        name=_name;
        points=_points;
        country=_country;
    }

    public void SetColor(Color color)
    {
        gameObject.GetComponent<MeshRenderer>().material.color = color;
    }
}
