using UnityEngine;
using System.Collections.Generic;

public class ShapeTools
{
    public static int[] Triangulate(List<Vector2> points)
    {
        List<int> indices = new List<int>();

        for (int i = 1; i < points.Count - 1; i++)
        {
            indices.Add(0);
            indices.Add(i);
            indices.Add(i + 1);
        }

        return indices.ToArray();
    }

    public static GameObject CreateShape(string name, List<Vector2> points, float stroke)
    {
        if (points.Count < 3) return null;
        
        GameObject shapeObject = new GameObject(name);
        shapeObject.transform.position = Vector3.zero;
        
        MeshFilter meshFilter = shapeObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = shapeObject.AddComponent<MeshRenderer>();
        if(stroke > 0)
        {
            LineRenderer line = shapeObject.AddComponent<LineRenderer>();
            line.positionCount = points.Count;
            line.loop = true;
            line.widthMultiplier = stroke;
            for (int i = 0; i < points.Count; i++)
            {
                line.SetPosition(i, new Vector3(points[i].x, points[i].y, 0));
            }
        }
        meshRenderer.material = new Material(Shader.Find("Standard"));

        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;

        Vector3[] vertices = new Vector3[points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            vertices[i] = new Vector3(points[i].x, points[i].y, 0);
        }

        int[] triangles = Triangulate(points);

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return shapeObject;
    }

    public static Province CreateProvince(string name, List<Vector2> points)
    {
        GameObject shape = ShapeTools.CreateShape("Province", points, 0.2f);
        if(!shape) return null;

        Province province = shape.AddComponent<Province>();
        province.points = points;
        province.name = name;
        GameObject parent = GameObject.Find("Provinces");
        if(parent) shape.transform.SetParent(parent.transform);
        return province;
    }
}
