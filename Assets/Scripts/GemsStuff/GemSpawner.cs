using System.Collections.Generic;
using UnityEngine;

public class GemSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject gemPrefab;
    [SerializeField]
    //Material boulderMaterial;

    /*[Header("Stone Settings")]
    public int resolution = 3;          // how many times to subdivide
    public float noiseStrength = 1f;  // how rough the rock is
    public float baseRadius = 2f;     // size
    GameObject stone;
    GameObject[] targetShapes;*/

    public List<Transform> spawnPositions;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //targetShapes = Resources.LoadAll<GameObject>("Prefabs/TargetShapes/");

        foreach (Transform child in transform)
        {
            spawnPositions.Add(child);
        }

        for(int i = 0; i < spawnPositions.Count; i++)
        {
            GameObject gem = Instantiate(gemPrefab, spawnPositions[i]);

            //GameObject gem = InstantiateRock(spawnPositions[i].position);
        }
    }


    /*GameObject InstantiateRock(Vector3 position)
    {
        // Create a new GameObject to hold the mesh
        GameObject stone = new GameObject("Stone");
        stone.tag = "Gem";

        // Add required components
        MeshFilter mf = stone.AddComponent<MeshFilter>();
        MeshRenderer mr = stone.AddComponent<MeshRenderer>();

        // Assign your generated mesh
        mf.mesh = GenerateStone();

        // Assign a material (important!)
        mr.material = boulderMaterial;
        mr.material.color = Color.Lerp(Color.gray, Color.black, Random.value);

        // Position it
        stone.transform.position = position;

        // Add mouse rotation behaviour
        stone.AddComponent<Sliceable>();

        GameObject targetShape = Instantiate(targetShapes[Random.Range(0, targetShapes.Length)], new Vector3(0, 0, 0), Quaternion.identity);
        targetShape.transform.position = stone.transform.position;
        targetShape.name = "TargetShape";
        GameObject parent = new GameObject("Boulder");
        parent.transform.position = stone.transform.position;
        stone.transform.parent = parent.transform;
        targetShape.transform.parent = parent.transform;
        parent.AddComponent<Rotateable>();
        this.stone = parent;
        //rescale to match scene (first merge)
        stone.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        targetShape.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);

        return stone;
    }

    Mesh GenerateStone()
    {
        // Start from an icosahedron (better than cube for uniformity)
        Mesh mesh = IcoSphere.Create(resolution);
        Vector3[] verts = mesh.vertices;

        for (int i = 0; i < verts.Length; i++)
        {
            // Add random "roughness" to each vertex
            Vector3 dir = verts[i].normalized;
            float offset = Mathf.PerlinNoise(verts[i].x * 2f, verts[i].y * 2f) * noiseStrength;
            verts[i] = dir * (baseRadius + offset);
        }

        mesh.vertices = verts;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();


        return mesh;
    }*/


}
