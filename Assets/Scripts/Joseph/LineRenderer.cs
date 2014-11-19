using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LineRenderer : MonoBehaviour {
	Vector3 start, previous;
	List<Vector3> spots = new List<Vector3> ();

	Mesh mesh;

	public GameObject target;

	// Use this for initialization
	void Start () {
		mesh = new Mesh ();
		previous = start = target.transform.position;
		GetComponent<MeshFilter> ().mesh = mesh;
		GetComponent<MeshCollider> ().sharedMesh = mesh;
		spots.Add (start);
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 cur = target.transform.position;
		float diff = Vector3.Distance (start, target.transform.position);

		if (diff < 0.01) {
			return;
		}

		spots.Add (cur);

		updateMesh ();
	}

	Vector3[] vertices;
	int[] triangels;

	void updateMesh() {
		vertices = new Vector3[spots.Count * 8];
		triangels = new int[vertices.Length * 3];
		/*
		for (int i = 0; i < spots.Count; i += 1) {
			vertices[i * 2] = spots[i];
			vertices[i * 2 + 1] = new Vector3(spots[i].x, spots[i].y + 0.5f, spots[i].z);
		}
		for (int i = 0; i < (spots.Count - 1); i += 1) {
			triangels[i * 6] = i * 2;
			triangels[i * 6 + 1] = i * 2 + 1;
			triangels[i * 6 + 2] = i * 2 + 2;

			triangels[i * 6 + 3] = i * 2 + 1;
			triangels[i * 6 + 4] = i * 2 + 3;
			triangels[i * 6 + 5] = i * 2 + 2;
		}
		*/
        for (int i = 0; i < (spots.Count - 1); i += 1)
        {
            calculateLine(i * 8, spots[i], spots[i + 1]);
        }
		mesh.vertices = vertices;
		mesh.triangles = triangels;
	}

    float lineHeight = 0.5f;
    float lineWidth = 0.25f;
    void calculateLine(int index, Vector3 begin_fl, Vector3 end_fl) {
        Vector3 begin_fh = mod(begin_fl, 0f, lineHeight, 0f);
        Vector3 end_fh = mod(end_fl, 0f, lineHeight, 0f);

        Vector3 begin_bl = mod(begin_fl, lineWidth, 0f, 0f);
        Vector3 begin_bh = mod(begin_fl, lineWidth, lineHeight, 0f);
        Vector3 end_bl = mod(end_fl, lineWidth, 0f, 0f);
        Vector3 end_bh = mod(end_fl, lineWidth, lineHeight, 0f);

        vertices[index] = begin_fl;
        vertices[index + 1] = begin_fh;
        vertices[index + 2] = begin_bl;
        vertices[index + 3] = begin_bh;
        vertices[index + 4] = end_fl;
        vertices[index + 5] = end_fh;
        vertices[index + 6] = end_bl;
        vertices[index + 7] = end_bh;

        // front
        int i3 = index * 3;
        triangels[i3] = index;
        triangels[i3 + 1] = index + 5;
        triangels[i3 + 2] = index + 4;
        
        i3 += 3;
        triangels[i3] = index;
        triangels[i3 + 1] = index + 5;
        triangels[i3 + 2] = index + 1;

        // top
        i3 += 3;
        triangels[i3] = index + 1;
        triangels[i3 + 1] = index + 6;
        triangels[i3 + 2] = index + 5;

        i3 += 3;
        triangels[i3] = index + 1;
        triangels[i3 + 1] = index + 6;
        triangels[i3 + 2] = index + 2;

        // back
        i3 += 3;
        triangels[i3] = index + 3;
        triangels[i3 + 1] = index + 6;
        triangels[i3 + 2] = index + 7;

        i3 += 3;
        triangels[i3] = index + 3;
        triangels[i3 + 1] = index + 6;
        triangels[i3 + 2] = index + 2;

        // bottom
        i3 += 3;
        triangels[i3] = index;
        triangels[i3 + 1] = index + 7;
        triangels[i3 + 2] = index + 4;

        i3 += 3;
        triangels[i3] = index + 0;
        triangels[i3 + 1] = index + 7;
        triangels[i3 + 2] = index + 3;
	}


    static Vector3 mod(Vector3 v, float dx, float dy, float dz) {
        return new Vector3(v.x + dx, v.y + dy, v.z + dz);
    }
		
}
