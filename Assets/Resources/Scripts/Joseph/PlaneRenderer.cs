using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlaneRenderer : MonoBehaviour {
	Mesh mesh;
    MeshCollider collider;
    MeshFilter filter;

    public float tileXSize = 10, tileZSize = 10;
    public int xTiles = 10, zTiles = 10;

    bool[,] hide;

    public int randomHoleCnt = 5;

	// Use this for initialization
	void Start () {
        mesh = new Mesh ();

        filter = GetComponent<MeshFilter>();
        filter.mesh = mesh;
		collider = GetComponent<MeshCollider> ();

        hide = new bool[xTiles, zTiles];

        for (int i = 0; i < randomHoleCnt; i += 1)
        {
            hide[Random.Range(1, xTiles - 1), Random.Range(1, zTiles - 2)] = true;
        }
        createMesh();
	}

	Vector3[] vertices, normals;
	int[] triangles;
	void createMesh() {
        Vector3 start = Vector3.zero;

        int idx, xSize = xTiles + 1;
        vertices = new Vector3[xSize * zTiles];
        triangles = new int[xTiles * (zTiles - 1) * 12];

        for (int z = 0; z < zTiles; z += 1) {
            for (int x = 0; x < xSize; x += 1) {
                idx = x + z * xSize;
                vertices[idx] = mod(start, x * tileXSize, 0f, z * tileZSize);
            }
        } 
        for (int z = 0; z < (zTiles - 1); z += 1)
        {
            for (int x = 0; x < xTiles; x += 1)
            {
                if (hide[x, z])
                {
                    continue;
                }
                idx = x + z * xSize;
                renderTileForward((x + z * xTiles) * 6, x + z * xSize);
                renderTileBackward(xTiles * (zTiles - 1) * 6 + (x + z * xTiles) * 6, x + z * xSize);
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();
    }

    void renderTileForward(int idx, int tl)
    {
        int tr = tl + 1;
        int bl = tl + xTiles + 1;
        int br = bl + 1;
        renderTileForward(idx, tl, tr, bl, br); ;
    }

    void renderTileForward(int idx, int tl, int tr, int bl, int br)
    {
        triangles[idx] = bl;
        triangles[idx + 1] = tr;
        triangles[idx + 2] = tl;

        idx += 3;
        triangles[idx] = bl;
        triangles[idx + 1] = br;
        triangles[idx + 2] = tr;
    }

    void renderTileBackward(int idx, int tl)
    {
        int tr = tl + 1;
        int bl = tl + xTiles + 1;
        int br = bl + 1;
        renderTileBackward(idx, tl, tr, bl, br); ;
    }

    void renderTileBackward(int idx, int tl, int tr, int bl, int br)
    {
        triangles[idx] = bl;
        triangles[idx + 1] = tl;
        triangles[idx + 2] = tr;

        idx += 3;
        triangles[idx] = bl;
        triangles[idx + 1] = tr;
        triangles[idx + 2] = br;
    }

    static Vector3 mod(Vector3 v, float dx, float dy, float dz) {
        return new Vector3(v.x + dx, v.y + dy, v.z + dz);
    }
		
}
