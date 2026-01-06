using UnityEngine;
using System.Collections.Generic;

public static class IcoSphere
{
    struct TriangleIndices { public int v1, v2, v3; public TriangleIndices(int a, int b, int c) { v1 = a; v2 = b; v3 = c; } }

    public static Mesh Create(int recursionLevel)
    {
        Mesh mesh = new Mesh();

        Dictionary<long, int> middlePointIndexCache = new Dictionary<long, int>();
        List<Vector3> vertices = new List<Vector3>();
        int AddVertex(Vector3 p) { vertices.Add(p.normalized); return vertices.Count - 1; }

        // create 12 vertices of an icosahedron
        float t = (1f + Mathf.Sqrt(5f)) / 2f;

        AddVertex(new Vector3(-1, t, 0));
        AddVertex(new Vector3(1, t, 0));
        AddVertex(new Vector3(-1, -t, 0));
        AddVertex(new Vector3(1, -t, 0));
        AddVertex(new Vector3(0, -1, t));
        AddVertex(new Vector3(0, 1, t));
        AddVertex(new Vector3(0, -1, -t));
        AddVertex(new Vector3(0, 1, -t));
        AddVertex(new Vector3(t, 0, -1));
        AddVertex(new Vector3(t, 0, 1));
        AddVertex(new Vector3(-t, 0, -1));
        AddVertex(new Vector3(-t, 0, 1));

        List<TriangleIndices> faces = new List<TriangleIndices> {
            new TriangleIndices(0,11,5), new TriangleIndices(0,5,1), new TriangleIndices(0,1,7),
            new TriangleIndices(0,7,10), new TriangleIndices(0,10,11), new TriangleIndices(1,5,9),
            new TriangleIndices(5,11,4), new TriangleIndices(11,10,2), new TriangleIndices(10,7,6),
            new TriangleIndices(7,1,8), new TriangleIndices(3,9,4), new TriangleIndices(3,4,2),
            new TriangleIndices(3,2,6), new TriangleIndices(3,6,8), new TriangleIndices(3,8,9),
            new TriangleIndices(4,9,5), new TriangleIndices(2,4,11), new TriangleIndices(6,2,10),
            new TriangleIndices(8,6,7), new TriangleIndices(9,8,1)
        };

        // refine triangles
        for (int i = 0; i < recursionLevel; i++)
        {
            List<TriangleIndices> faces2 = new List<TriangleIndices>();
            foreach (var tri in faces)
            {
                int a = GetMiddlePoint(tri.v1, tri.v2, ref vertices, ref middlePointIndexCache);
                int b = GetMiddlePoint(tri.v2, tri.v3, ref vertices, ref middlePointIndexCache);
                int c = GetMiddlePoint(tri.v3, tri.v1, ref vertices, ref middlePointIndexCache);

                faces2.Add(new TriangleIndices(tri.v1, a, c));
                faces2.Add(new TriangleIndices(tri.v2, b, a));
                faces2.Add(new TriangleIndices(tri.v3, c, b));
                faces2.Add(new TriangleIndices(a, b, c));
            }
            faces = faces2;
        }

        List<int> triangles = new List<int>();
        foreach (var tri in faces)
        {
            triangles.Add(tri.v1);
            triangles.Add(tri.v2);
            triangles.Add(tri.v3);
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    static int GetMiddlePoint(int p1, int p2, ref List<Vector3> verts, ref Dictionary<long, int> cache)
    {
        bool firstIsSmaller = p1 < p2;
        long key = firstIsSmaller ? ((long)p1 << 32) + p2 : ((long)p2 << 32) + p1;
        if (cache.TryGetValue(key, out int ret)) return ret;

        Vector3 middle = (verts[p1] + verts[p2]) * 0.5f;
        int i = verts.Count;
        verts.Add(middle.normalized);
        cache.Add(key, i);
        return i;
    }
}
