using UnityEngine;
using System.Collections;

public class LevelTour : MonoBehaviour
{
		public GameObject start;
		public GameObject first;
		public GameObject second;
		public GameObject end;
		private GameObject current;
		private GameObject next;
		private Vector3 dist;
		private float curve;
		private int count;
		public static Vector3 viewpoint = new Vector3(0,0,0);
		public static Vector3 temp;

		// Use this for initialization
		void Start ()
		{
		count = 1;
		current = start;
		next = first;


		}
	
		// Update is called once per frame
		void Update ()
		{
		if (transform.position == next.transform.position) {
						if (count == 1) {
							count = count +1;
							next = second;
							current = first;
						} else if(count == 2){
							count = count +1;
							next = end;
							current = second;
						} else {
								return;
						}
		}
		interpolate ();
	
		}
		
		void interpolate(){
				dist = new Vector3 (current.transform.position.x - next.transform.position.x, 0, current.transform.position.x - next.transform.position.x);
				transform.position = transform.position + dist * Time.deltaTime;  
				//curve = Vector3.Angle(transform.forward, viewpoint);
				//this.transform.rotation = Quaternion.SetFromToRotation(this.transform.rotation, new Vector3(transform.rotation.x+curve,0,0));
		//temp = transform.forward;
		//transform.rotation = Quaternion.SetLookRotation(viewpoint, temp); 

		}
}

