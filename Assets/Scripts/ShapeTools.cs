using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UIElements;
using System.Data.Common;
using UnityEngine.Rendering;
using System.Drawing;
using System.Linq;
using UnityEngine.EventSystems;

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
                            //Debug.Log("Punkt w środku!!!");
                            isIntersecting = true;
                            break;
                        }
                    }
                    if(isIntersecting == false && IsConvex(rest[queue[0]],rest[queue[1]],rest[queue[2]])){
                        //Debug.Log("Ucho!!!");

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
    public static Vector2 CentroidOfACountry(Country country)
    {
        HashSet<Vector2> allPoints = new HashSet<Vector2>();
        foreach(Province province in country.provinces)
        {
            foreach(Vector2 point in province.points) allPoints.Add(point);
        }
        return CentroidOfAPolygon(allPoints.ToList());
    }

    public static Vector2 CentroidOfAPolygon(List<Vector2> poly)
    {
        float sumX=0;
        float sumY=0;
        int count = poly.Count();
        foreach(Vector2 point in poly)
        {
            sumX+=point.x;
            sumY+=point.y;
        }
        sumX/=count;
        sumY/=count;
        return new Vector2(sumX,sumY);
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

    public static bool AreTwoProvincesNeighborsOneWay(Province province1, Province province2)
    {
        foreach(Vector2 P3 in province1.points)
        {
            for(int i=1;i<province2.points.Count();i++)
            {
                float eps = 0.001f;
                Vector2 P1 = province2.points[i-1];
                Vector2 P2 = province2.points[i];
                if((Math.Abs(P1.x-P3.x)<eps && Math.Abs(P1.y-P3.y)<eps) || (Math.Abs(P2.x-P3.x)<eps && Math.Abs(P2.y-P3.y)<eps)) return true;
                float deltaX = P2.x-P1.x;
                float deltaY = P2.y-P1.y;
                if(Math.Abs(deltaX)<eps && Math.Abs(P3.x-P1.x)<eps)
                {
                    if((P3.y<P2.y && P3.y>P1.y) || (P3.y>P2.y && P3.y<P1.y)) return true;
                }
                else
                {
                    float a = deltaY/deltaX;
                    float b = P1.y-a*P1.x;
                    if(Math.Abs(a*P3.x+b-P3.y)<eps) return true;
                }
            }
        }
        return false;
    }
    public static bool AreTwoProvincesNeighbors(Province province1, Province province2)
    {
        if(province1 == null || province2 == null) return false;
        if(province1.name == province2.name) return false;
        return AreTwoProvincesNeighborsOneWay(province1,province2) || AreTwoProvincesNeighborsOneWay(province2,province1);
    }
    public static float[] PointOnLine(Vector2 p0,Vector2 p1,Vector2 q) 
    {

        // p0 and p1 define the line segment
        // q is the reference point (aka mouse)
        // returns point on the line closest to px

        if (p0.x == p1.x && p0.y == p1.y) p0.x -= 0.00001f;

        float Unumer = ((q.x - p0.x) * (p1.x - p0.x)) + ((q.y - p0.y) * (p1.y - p0.y));
        float Udenom = (float)(Math.Pow(p1.x - p0.x, 2) + Math.Pow(p1.y - p0.y, 2));
        float U = Unumer / Udenom;

        Vector2 r = new Vector2(p0.x + (U * (p1.x - p0.x)),p0.y + (U * (p1.y - p0.y)));

        float minx = Math.Min(p0.x, p1.x);
        float maxx = Math.Max(p0.x, p1.x);
        float miny = Math.Min(p0.y, p1.y);
        float maxy = Math.Max(p0.y, p1.y);

        bool isValid = (r.x >= minx && r.x <= maxx) && (r.y >= miny && r.y <= maxy);
        float[] r2 = {r.x,r.y};
        return isValid ? r2 : null;
    }
    public static bool IsPointerOverSpecificCanvas(Canvas c)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.GetComponentInParent<Canvas>() == c)
            {
                return true;
            }
        }
        return false;
    }
    public static float Distance(Vector2 A, Vector2 B)
    {
        return (A.x-B.x)*(A.x-B.x)+(A.y-B.y)*(A.y-B.y);
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
    public static bool IsPointInPolygon( Vector2 p, Vector2[] polygon )
    {
        double minX = polygon[ 0 ].x;
        double maxX = polygon[ 0 ].x;
        double minY = polygon[ 0 ].y;
        double maxY = polygon[ 0 ].y;
        for ( int i = 1 ; i < polygon.Length ; i++ )
        {
            Vector2 q = polygon[ i ];
            minX = Math.Min( q.x, minX );
            maxX = Math.Max( q.x, maxX );
            minY = Math.Min( q.y, minY );
            maxY = Math.Max( q.y, maxY );
        }

        if ( p.x < minX || p.x > maxX || p.y < minY || p.y > maxY )
        {
            return false;
        }
        bool inside = false;
        for ( int i = 0, j = polygon.Length - 1 ; i < polygon.Length ; j = i++ )
        {
            if ( ( polygon[ i ].y > p.y ) != ( polygon[ j ].y > p.y ) &&
                p.x < ( polygon[ j ].x - polygon[ i ].x ) * ( p.y - polygon[ i ].y ) / ( polygon[ j ].y - polygon[ i ].y ) + polygon[ i ].x )
            {
                inside = !inside;
            }
        }

        return inside;
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
                //Debug.Log("Kolor ustalony!");
                lineMaterial.color = UnityEngine.Color.black;
                line.material = lineMaterial;
            }
            for (int i = 0; i < points.Count; i++)
            {
                line.SetPosition(i, new Vector3(points[i].x, points[i].y, -1));
            }
        }
        meshRenderer.material = new Material(Shader.Find("Unlit/Color"));

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

    public static ProvinceGO CreateProvinceGO(string name, List<Vector2> points)
    {
        GameObject shape = ShapeTools.CreateShape("Province", points, 0.2f);
        if(!shape) return null;

        ProvinceGO province = shape.AddComponent<ProvinceGO>();
        province.Initialise(name, points);
        GameObject parent = GameObject.Find("Provinces");
        if(parent) shape.transform.SetParent(parent.transform);
        return province;
    }

    public static ProvinceGO CreateProvinceGO(Province data, List<Vector2> points)
    {
        GameObject shape = ShapeTools.CreateShape("Province", points, 0.2f);
        if(!shape) return null;

        ProvinceGO province = shape.AddComponent<ProvinceGO>();
        province.Initialise(data, points);
        GameObject parent = GameObject.Find("Provinces");
        if(parent) shape.transform.SetParent(parent.transform);
        return province;
    }
}
