using UnityEngine;
using System.Collections;

public class TerrainGenerator : MonoBehaviour {

    public float detailScale;
    public int seed;

	// Use this for initialization
	void Start () {
   
	}
	
    public void GenerateTerrain() {

        System.Random prng = new System.Random(seed);
        Vector3 octaveOffsets;
        float offsetX = prng.Next(-100000, 100000);
        float offsetY = prng.Next(-100000, 100000);
        octaveOffsets = new Vector3(offsetX, offsetY);

        Mesh mesh = this.gameObject.GetComponent<MeshFilter>().sharedMesh;
        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {

            float sampleX = vertices[i].x + this.transform.position.x;

            float sampleY = vertices[i].z + this.transform.position.z;
            vertices[i].y = Mathf.PerlinNoise(sampleX / detailScale,sampleY / detailScale);

        }
        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        if (!gameObject.GetComponent<MeshCollider>()) {

            this.gameObject.AddComponent<MeshCollider>();

        }

    }
}
