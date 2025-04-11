using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UIElements;
using System.Data.Common;
using UnityEngine.Rendering;

public class ShapeTools
{
    public static int[] Triangulate(List<Vector2> points)
    {
        List<int> indices = new List<int>();
        Dictionary<int,Vector2> rest = new Dictionary<int, Vector2>();
        if(IsClockwise(points)){
            for(int i=0;i<points.Count;i++){rest[i]=points[i];}
        }else{
            for(int i=points.Count-1;i>=0;i--){rest[i]=points[i];}
        }

        while(rest.Count>3){
            int size = rest.Count;
            bool removed = false;
            List<int> queue = new List<int>();
            foreach(int index in rest.Keys){
                queue.Add(index);
                if(queue.Count == 3){
                    bool isIntersecting = false;
                    foreach(KeyValuePair<int,Vector2> pair in rest){
                        if(pair.Key != queue[0] &&
                           pair.Key != queue[1] &&
                           pair.Key != queue[2] &&
                           IsPointInTriangle(pair.Value,rest[queue[0]],rest[queue[1]],rest[queue[2]]))
                        {
                            Debug.Log("Punkt w środku!!!");
                            isIntersecting = true;
                            break;
                        }
                    }
                    if(isIntersecting == false && IsConvex(rest[queue[0]],rest[queue[1]],rest[queue[2]])){
                        Debug.Log("Ucho!!!");

                        indices.Add(queue[0]);
                        indices.Add(queue[1]);
                        indices.Add(queue[2]);
                        rest.Remove(queue[1]);
                        removed = true;
                        break;
                    }
                    queue.RemoveAt(0);
                }
            }
            if(!removed) break;
        }
        if(rest.Count == 3){
            foreach(int index in rest.Keys){indices.Add(index);}
        }
        
        
        return indices.ToArray();
    }
    public static bool FasterLineSegmentIntersection (Vector2 line1point1, Vector2 line1point2, Vector2 line2point1, Vector2 line2point2) {
        Vector2 a = line1point2 - line1point1;
        Vector2 b = line2point1 - line2point2;
        Vector2 c = line1point1 - line2point1;

        float alphaNumerator = b.y * c.x - b.x * c.y;
        float betaNumerator  = a.x * c.y - a.y * c.x;
        float denominator    = a.y * b.x - a.x * b.y;

        if (denominator == 0) {
            return false;
        } else if (denominator > 0) {
            if (alphaNumerator < 0 || alphaNumerator > denominator || betaNumerator < 0 || betaNumerator > denominator) {
                return false;
            }
        } else if (alphaNumerator > 0 || alphaNumerator < denominator || betaNumerator > 0 || betaNumerator < denominator) {
            return false;
        }
        return true;
    }

    public static bool IsIntersecting(List<Vector2> points){
        for(int i=0;i<points.Count-3;i++){
            Vector2 A = points[i];
            Vector2 B = points[i+1];
            for(int j=i+2;j<points.Count-1;j++){
                Vector2 C = points[j];
                Vector2 D = points[j+1];
                if(FasterLineSegmentIntersection(A,B,C,D)){return false;}
            }
        }
        return true;
    }

    public static bool IsClockwise(List<Vector2> points)
    {
        float sum = 0;
        for (int i = 0; i < points.Count; i++)
        {
            Vector2 p1 = points[i];
            Vector2 p2 = points[(i + 1) % points.Count];
            sum += p1.x * p2.y - p2.x * p1.y;
        }
        return sum < 0;
    }
    public static bool IsConvex(Vector2 A,Vector2 B,Vector2 C){
        Vector2 vectorBA = A-B;
        Vector2 vectorBC = C-B;
        if(Vector2.SignedAngle(vectorBA,vectorBC)>0){return true;}
        else{return false;}
    }
    public static bool IsPointInTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c){
        double z1 = CrossProduct2D(b - a, p - a);
        double z2 = CrossProduct2D(c - b, p - b);
        double z3 = CrossProduct2D(a - c, p - c);
        bool hasNegative = (z1 < 0) || (z2 < 0) || (z3 < 0);
        bool hasPositive = (z1 > 0) || (z2 > 0) || (z3 > 0);
        return !(hasNegative && hasPositive);
    }
    private static double CrossProduct2D(Vector2 v1, Vector2 v2)
    {
        return v1.x * v2.y - v1.y * v2.x;
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
            if (line.sharedMaterial == null) 
            {
                Material lineMaterial = new Material(Shader.Find("Unlit/Color"));
                Debug.Log("Kolor ustalony!");
                lineMaterial.color = Color.black;
                line.material = lineMaterial;
            }
            for (int i = 0; i < points.Count; i++)
            {
                line.SetPosition(i, new Vector3(points[i].x, points[i].y, -1));
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
