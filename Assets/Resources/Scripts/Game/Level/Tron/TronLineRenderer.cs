using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Game.Level.Tron {

	public class TronLineRenderer : MonoBehaviour {
		Vector3 start;
		Vector3 previous;
		List<Vector3> spots = new List<Vector3> ();

		Mesh mesh;
		MeshCollider meshCollider;
		MeshFilter filter;

		GameObject segment;

		// Keep track off all segments
		List<GameObject> segments = new List<GameObject>();

		// Minimum distance between line and car
		public float minDistance = 10f;
		// Minimum distance between render 'points'.
		public float minStepSize = 0.05f;
		// Center offset, a way to tweak the offset of the center
		public float centerOffset = -2.5f;

		// Height of the line
		public float lineHeight = 5f;
		// Width of the line
		public float lineWidth = 4f;

		// Length of the current segment
		public float currentSegment = 0;
		// Length needed before we split off the segment.
		public float segmentSplitoff = 200;

		// Distance between segments
		public float segmentWaitDistance = 30;
		// Current distance needing to wait before starting next segment
		public float currentSegmentDistance = 0;

		// Material to render the line with
		public Material material;

		void Start () {
			initializeSegment();
		}

		void CreateMaterial (){
			material = new Material (Shader.Find("Transparent/Diffuse"));
		}

		public void setColor (int color) {
			CreateMaterial ();
			Color c = Color.green;
			switch(color) {
				case 0:
					c = Color.red;
					break;
				case 1:
					c = Color.yellow;
					break;
				case 2:
					c = Color.blue;
					break;
				default:
					c = Color.green;
					break;
			}

			material.color = c;
		}

		void initializeSegment() {
			start = previous = transform.position;

			segment = new GameObject(this.name + "-line#" + segments.Count);
			segment.transform.SetParent (transform.parent, true);
			segment.transform.position = transform.position;
			segment.tag = "TronLineSegment";

			mesh = new Mesh ();
			mesh.MarkDynamic();

			MeshRenderer graphics = segment.AddComponent<MeshRenderer> ();
			setColor(0);
			graphics.material = material;

			filter = segment.AddComponent<MeshFilter> ();
			meshCollider = segment.AddComponent<MeshCollider> ();

			filter.mesh = mesh;
			spots.Clear();
			spots.Add (new Vector3(0, 0, 0));

			segments.Add(segment);
		}

		void OnDestroy() {
			foreach (GameObject segment in segments) {
				Destroy(segment);
			}
		}

		// Update is called once per frame
		void Update () {
			Vector3 current = transform.position;
			float diff = Vector3.Distance(previous, transform.position);

			// When starting 2nd+ segment we want to create a 'gap'
			if (currentSegmentDistance > 0) {
				currentSegmentDistance -= diff;
				if (currentSegmentDistance <= 0) {
					start = previous = transform.position;
					segment.transform.position = transform.position;
				}
				return;
			}

			if (diff < minStepSize) {
				return;
			}

			currentSegment += diff;

			// Normalize
			spots.Add(current - start);
			previous = current;

			updateMesh();

			// Start next segment
			if (currentSegment >= segmentSplitoff) {
				initializeSegment();
				currentSegmentDistance = segmentWaitDistance;
				currentSegment = 0;
			}
		}

		Vector3[] vertices;
		int[] triangles;

		void updateMesh() {
			if (spots.Count <= 2) {
				return;
			}
			int offset = 2;
			Vector3 current = transform.position - start;
			for (int i = spots.Count - offset; i >= 0; i -= 1) {
				if (Vector3.Distance(spots[i], current) > minDistance) {
					offset = spots.Count - i;
					break;
				}
				if (i == 0) {
					return;
				}
			}

			int parts = (spots.Count - offset);
			// we need 1 extra vertice 'set' because we need to render between 2 'sets'
			vertices = new Vector3[(parts + 1) * 4];
			/*
			 * Amount of needed triangles needed:
			 * parts * 4 sides * 2 per side * 3 points per triangle
			 * + start(6 points for 2 triangles)
			 * + end(6 points for 2 triangles)
			 */
			triangles = new int[(parts) * 8 * 3 + (3 * 2) + (3 * 2)];

			for (int i = 0; i < (parts + 1); i += 1) {
				calculateVertice(i * 4, spots[i], spots[i + 1]);
			}

			for (int i = 0; i < (parts); i += 1) {
				calculateTriangle(i * 4);
				//calculateNormal(i * 4);
			}

			// first(the figurative bottom of the line)
			triangles[0] = 4;
			triangles[1] = 2;
			triangles[2] = 3;

			triangles[3] = 4;
			triangles[4] = 1;
			triangles[5] = 2;

			// last(the figurative lid of the line)
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

			if (meshCollider != null) {
				meshCollider.sharedMesh = null;
				meshCollider.sharedMesh = mesh;
			}
		}

		void calculateVertice(int index, Vector3 a, Vector3 b) {
			Vector3 ba = b - a;
			Vector3 up = Vector3.up * lineHeight;

			Vector3 cross = Vector3.Cross(up, ba).normalized;
			Vector3 offset = cross * centerOffset;
			a = a + offset;

			vertices[index] = a;
			vertices[index + 1] = a + up;

			vertices[index + 3] = cross * lineWidth + a;
			vertices[index + 2] = vertices[index + 3] + up;
		}

		void calculateTriangle(int index) {
			// front
			// 1 offset for the front line triangles (start of the line)
			int i3 = (index + 1) * 2 * 3;
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

		/**
		    Create a new vector with an dx, dy and dz offset;
		*/
		static Vector3 mod(Vector3 v, float dx, float dy, float dz) {
			return new Vector3(v.x + dx, v.y + dy, v.z + dz);
		}

	}
}