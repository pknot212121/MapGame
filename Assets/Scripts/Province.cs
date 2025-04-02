using System.Collections.Generic;
using UnityEngine;

public class Province : MonoBehaviour
{
    public new string name;
    public List<Vector2> points = new List<Vector2>();
    public Country country = null;
}
