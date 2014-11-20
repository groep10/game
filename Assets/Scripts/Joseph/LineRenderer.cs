﻿using UnityEngine;
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

        //spots.Add(mod(start, 3f, 0f, 0f));

        //updateMesh();
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 cur = target.transform.position;
        float diff = Vector3.Distance(start, target.transform.position);

        if (diff < 0.01)
        {
            return;
        }

        spots.Add(cur);

        updateMesh();
	}

	Vector3[] vertices;
	int[] triangels;

	void updateMesh() {
		vertices = new Vector3[spots.Count * 8];
		triangels = new int[spots.Count * 12 * 3];

        for (int i = 0; i < (spots.Count - 1); i += 1)
        {
            calculateLine(i * 8, spots[i], spots[i + 1]);
        }
		mesh.vertices = vertices;
		mesh.triangles = triangels;

        //mesh.RecalculateNormals();
        //mesh.RecalculateBounds();
        //mesh.Optimize();
	}

    float lineHeight = 0.5f;
    float lineWidth = 0.25f;
    void calculateLine(int index, Vector3 begin_fl, Vector3 end_fl) {
        Vector3 ba = end_fl - begin_fl;
        Vector3 up = Vector3.up * lineHeight;
        vertices[index] = begin_fl;
        vertices[index + 1] = begin_fl + up;
        
        vertices[index + 3] = Vector3.Cross(up, ba).normalized * lineWidth + begin_fl;
        vertices[index + 2] = vertices[index + 3] + up;
        
        vertices[index + 4] = end_fl;
        vertices[index + 5] = end_fl + up;

        vertices[index + 6] = vertices[index + 2] + ba;
        vertices[index + 7] = vertices[index + 3] + ba;

        //Debug.DrawLine(vertices[index + 0], vertices[index + 1], Color.red, 300);
        //Debug.DrawLine(vertices[index + 1], vertices[index + 2], Color.red, 300);
        //Debug.DrawLine(vertices[index + 2], vertices[index + 3], Color.red, 300);
        //Debug.DrawLine(vertices[index + 3], vertices[index + 0], Color.red, 300);

        //Debug.DrawLine(vertices[index + 4], vertices[index + 5], Color.red, 300);
        //Debug.DrawLine(vertices[index + 5], vertices[index + 6], Color.red, 300);
        //Debug.DrawLine(vertices[index + 6], vertices[index + 7], Color.red, 300);
        //Debug.DrawLine(vertices[index + 7], vertices[index + 4], Color.red, 300);
        //for (int i = 0; i < 7; i += 1)
        //{
        //    Debug.DrawLine(vertices[index + i + 0], vertices[index + i + 1], Color.red, 300);
        //}


        // front
        int i3 = index * 3;
        triangels[i3] = index;
        triangels[i3 + 1] = index + 4;
        triangels[i3 + 2] = index + 5;

        i3 += 3;
        triangels[i3] = index;
        triangels[i3 + 1] = index + 5;
        triangels[i3 + 2] = index + 1;

        // left
        i3 += 3;
        triangels[i3] = index + 0;
        triangels[i3 + 1] = index + 1;
        triangels[i3 + 2] = index + 2;

        i3 += 3;
        triangels[i3] = index + 0;
        triangels[i3 + 1] = index + 2;
        triangels[i3 + 2] = index + 3;

        // top
        i3 += 3;
        triangels[i3] = index + 1;
        triangels[i3 + 1] = index + 5;
        triangels[i3 + 2] = index + 6;

        i3 += 3;
        triangels[i3] = index + 1;
        triangels[i3 + 1] = index + 6;
        triangels[i3 + 2] = index + 2;

        // back
        i3 += 3;
        triangels[i3] = index + 3;
        triangels[i3 + 1] = index + 2;
        triangels[i3 + 2] = index + 6;

        i3 += 3;
        triangels[i3] = index + 3;
        triangels[i3 + 1] = index + 6;
        triangels[i3 + 2] = index + 7;

        // bottom
        i3 += 3;
        triangels[i3] = index + 0;
        triangels[i3 + 1] = index + 3;
        triangels[i3 + 2] = index + 7;

        i3 += 3;
        triangels[i3] = index;
        triangels[i3 + 1] = index + 7;
        triangels[i3 + 2] = index + 4;

        // right
        i3 += 3;
        triangels[i3] = index + 4;
        triangels[i3 + 1] = index + 6;
        triangels[i3 + 2] = index + 5;

        i3 += 3;
        triangels[i3] = index + 4;
        triangels[i3 + 1] = index + 7;
        triangels[i3 + 2] = index + 6;

    }


    static Vector3 mod(Vector3 v, float dx, float dy, float dz) {
        return new Vector3(v.x + dx, v.y + dy, v.z + dz);
    }
		
}
