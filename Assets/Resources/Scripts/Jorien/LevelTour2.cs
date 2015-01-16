using UnityEngine;
using System.Collections;

public class LevelTour2 : MonoBehaviour
{

		[Header ("Points")]	
		public GameObject start; //points which will be visited by the camera
		public GameObject first;
		public GameObject second;
		public GameObject end;
		public Transform target; //point of vision for the camera
		public GameObject camera;	

		[Header ("Values")]	
		public float height=3; //amount camera is higher than plane
		public int delta=100; //maximum amount of stap
	
	
		private GameObject current; //point camera is leaving 
		private GameObject next; //camera moves towards this point
		private Vector3 dist; //distance between current and next
		private int count; //count nummerates per point
		private int teller; //teller nummerates every time interpolate() is called
		private float xstap; //amount x position camera is increased every time interpolate() is called
		private float zstap; //amount z position camera is increased every time interpolate() is called
		



		void beginTour ()
		{ //initialiseren
		//camera.enabled = true;
		count = 1;
		current = start;
		next = first;
		transform.position = new Vector3(current.transform.position.x,current.transform.position.y+height,current.transform.position.z);
		determinedist (); 
		teller = 0;

		}
	
		// Update is called once per frame
		void Update ()
		{
			transform.LookAt(target); //Makes sure camera always looks at the right direction
			if(delta==teller){
				if (count == 1) {
					count = count +1;
					next = second;
					current = first;
					transform.position = new Vector3(current.transform.position.x,current.transform.position.y+height,current.transform.position.z);
					determinedist();
					teller=0;
				} else if(count == 2){
					count = count +1;
					next = end;
					current = second;
					transform.position = new Vector3(current.transform.position.x,current.transform.position.y+height,current.transform.position.z);
					determinedist();
					teller=0;
				} else if(count == 3){
					count = count +1;
					next = start;
					current = end;
					transform.position = new Vector3(current.transform.position.x,current.transform.position.y+height,current.transform.position.z);
					determinedist();
					teller=0;
				}else {
					return;
				}
			}
			interpolate ();	
		}
		
		void determinedist(){ //function which calculates distance, xstap and zstap
			dist = new Vector3 (next.transform.position.x - current.transform.position.x, 0, next.transform.position.z - current.transform.position.z);
			xstap = dist.x / delta;
			zstap = dist.z / delta;
		}
		
		void interpolate(){ //function that makes the camera move smooth from current to next
		transform.position = new Vector3(current.transform.position.x + xstap*teller,transform.position.y,current.transform.position.z + zstap*teller); 
		teller = teller + 1;
		}
}

