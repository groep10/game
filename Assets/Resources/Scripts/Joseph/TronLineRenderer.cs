using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TronLineRenderer : MonoBehaviour {
	Vector3 start;
	List<Vector3> spots = new List<Vector3> ();

	Mesh mesh;
    MeshCollider meshCollider;
    MeshFilter filter;

	public GameObject target;

	// Use this for initialization
	void Start () {
		start = target.transform.position;
		
        mesh = new Mesh ();
        mesh.MarkDynamic();

        filter = GetComponent<MeshFilter>();
        filter.mesh = mesh;
		meshCollider = GetComponent<MeshCollider> ();
        //gameObject.rigidbody.constraints = RigidbodyConstraints.FreezeAll;
		spots.Add (start);


        AccountController.getInstance().register("test", "test");
        Debug.Log("test");
	}

    void OnCollisionEnter()
    {
        print("Collision");
    }

	// Update is called once per frame
	void Update () {
        Vector3 cur = target.transform.position;
        float diff = Vector3.Distance(start, target.transform.position);

        if (diff < 0.05)
        {
            return;
        }

        spots.Add(cur);

        updateMesh();
	}

	Vector3[] vertices;
	int[] triangles;

    static float minDistance = 2;

	void updateMesh() {
        if (spots.Count <= 2) {
            return;
        }
        int offset = 2;
        for (int i = spots.Count - offset; i > 0; i -= 1)
        {
            if (Vector3.Distance(spots[i], target.transform.position) > minDistance)
            {
                offset = spots.Count - i;
                break;
            }
            if (i == 0)
            {
                return;
            }
        }
        
        vertices = new Vector3[(spots.Count - offset + 1) * 4];
        triangles = new int[(spots.Count - offset) * 8 * 3 + (3 * 2)];

        for (int i = 0; i < (spots.Count - offset + 1); i += 1) {
            calculateVertice(i * 4, spots[i], spots[i + 1]);
        }

        for (int i = 0; i < (spots.Count - offset); i += 1) {
            calculateTriangle(i * 4);
            //calculateNormal(i * 4);
        }
        
        // last
        int idx = (spots.Count - offset) * 8 * 3;
        triangles[idx] = vertices.Length - 4;
        triangles[idx + 1] = vertices.Length - 2;
        triangles[idx + 2] = vertices.Length - 3;

        triangles[idx + 3] = vertices.Length - 4;
        triangles[idx + 4] = vertices.Length - 1;
        triangles[idx + 5] = vertices.Length - 2;    

        mesh.Clear();
		mesh.vertices = vertices;
		mesh.triangles = triangles;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();

        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = mesh;
    }

    float lineHeight = 0.5f;
    float lineWidth = 0.25f;

    void calculateVertice(int index, Vector3 a, Vector3 b) {
        Vector3 ba = b - a;
        Vector3 up = Vector3.up * lineHeight;
        vertices[index] = a;
        vertices[index + 1] = a + up;

        vertices[index + 3] = Vector3.Cross(up, ba).normalized * lineWidth + a;
        vertices[index + 2] = vertices[index + 3] + up;
    }

    void calculateTriangle(int index) {
        // front
        int i3 = index * 2 * 3;
        triangles[i3] = index;
        triangles[i3 + 1] = index + 4;
        triangles[i3 + 2] = index + 5;

        i3 += 3;
        triangles[i3] = index;
        triangles[i3 + 1] = index + 5;
        triangles[i3 + 2] = index + 1;

        // top
        i3 += 3;
        triangles[i3] = index + 1;
        triangles[i3 + 1] = index + 5;
        triangles[i3 + 2] = index + 6;

        i3 += 3;
        triangles[i3] = index + 1;
        triangles[i3 + 1] = index + 6;
        triangles[i3 + 2] = index + 2;

        // back
        i3 += 3;
        triangles[i3] = index + 3;
        triangles[i3 + 1] = index + 2;
        triangles[i3 + 2] = index + 6;

        i3 += 3;
        triangles[i3] = index + 3;
        triangles[i3 + 1] = index + 6;
        triangles[i3 + 2] = index + 7;

        // bottom
        i3 += 3;
        triangles[i3] = index + 0;
        triangles[i3 + 1] = index + 3;
        triangles[i3 + 2] = index + 7;

        i3 += 3;
        triangles[i3] = index;
        triangles[i3 + 1] = index + 7;
        triangles[i3 + 2] = index + 4;
    }

    /*
    We let unity do this for us
    void calculateNormal(int index) {
        int i2 = index * 2;
        normals[i2] = Vector3.Cross(vertices[index + 4] - vertices[index + 0], vertices[index + 1] - vertices[index + 0]);
        normals[i2 + 1] = normals[i2];
        i2 += 2;

        normals[i2] = Vector3.Cross(vertices[index + 5] - vertices[index + 1], vertices[index + 2] - vertices[index + 1]);
        normals[i2 + 1] = normals[i2];
        i2 += 2;

        normals[i2] = Vector3.Cross(vertices[index + 6] - vertices[index + 2], vertices[index + 3] - vertices[index + 2]);
        normals[i2 + 1] = normals[i2];
        i2 += 2;

        normals[i2] = Vector3.Cross(vertices[index + 7] - vertices[index + 3], vertices[index + 0] - vertices[index + 3]);
        normals[i2 + 1] = normals[i2];
    }*/

    static Vector3 mod(Vector3 v, float dx, float dy, float dz) {
        return new Vector3(v.x + dx, v.y + dy, v.z + dz);
    }
		
}
