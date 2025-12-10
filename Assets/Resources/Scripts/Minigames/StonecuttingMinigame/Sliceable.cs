using EzySlice;
using UnityEngine;

public class Sliceable : MonoBehaviour
{
    private Transform knife;
    public Material myMaterial;
    void Start()
    {
        GameObject knifeObject = null;
        if (knife == null)
        {
            knifeObject = GameObject.Find("Knife");
        }
        if (knifeObject != null)
        {
            knife = knifeObject.transform;
        }
        if (myMaterial == null)
        {
            myMaterial = Resources.Load<Material>("Materials/CrystalBlue");
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && knife != null && knife.transform != null)
        {
            GameObject[] hulls = this.gameObject.SliceInstantiate(knife.position, knife.up, myMaterial);
            GameObject bigger;
            GameObject smaller;
            if (hulls != null && hulls.Length == 2)
            {
                if (GetMeshVolume(hulls[0]) >= GetMeshVolume(hulls[1]))
                {
                    bigger = hulls[0];
                    smaller = hulls[1];
                }
                else
                {
                    bigger = hulls[1];
                    smaller = hulls[0];
                }


                Destroy(smaller);
                this.GetComponent<MeshFilter>().mesh = Instantiate(bigger.GetComponent<MeshFilter>().mesh);

                this.GetComponent<MeshRenderer>().sharedMaterials = bigger.GetComponent<MeshRenderer>().sharedMaterials;
                Destroy(bigger);
            }
        }
    }
    float GetMeshVolume(GameObject obj)
    {
        MeshFilter mf = obj.GetComponent<MeshFilter>();
        if (mf == null) return 0f;

        Mesh mesh = mf.sharedMesh;
        if (mesh == null) return 0f;

        float volume = 0f;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        // Sum signed volumes of tetrahedrons relative to origin
        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 p1 = vertices[triangles[i + 0]];
            Vector3 p2 = vertices[triangles[i + 1]];
            Vector3 p3 = vertices[triangles[i + 2]];

            volume += SignedVolumeOfTriangle(p1, p2, p3);
        }

        volume = Mathf.Abs(volume);

        // Scale volume by object’s transform (in case it’s scaled)
        Vector3 scale = obj.transform.lossyScale;
        float scaleFactor = Mathf.Abs(scale.x * scale.y * scale.z);

        return volume * scaleFactor;
    }

    float SignedVolumeOfTriangle(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        return Vector3.Dot(Vector3.Cross(p1, p2), p3) / 6.0f;
    }
}
