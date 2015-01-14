using UnityEngine;
using System.Collections;

public class LevelTour : MonoBehaviour
{
		public GameObject start;
		public GameObject first;
		public GameObject second;
		public GameObject end;
	public Transform target;

		private GameObject current;
		private GameObject next;
		private Vector3 dist;
		
	private float curve;
		private int count;
		public Vector3 viewpoint = new Vector3(0,0,0);
		public Vector3 temp;

		private float xstap;
		private float ystap;
		private float zstap;
		private int teller;
	public float height=3;
	public int delta=100;

		// Use this for initialization
		void Start ()
		{
		count = 1;
		current = start;
		next = first;
		transform.position = new Vector3(current.transform.position.x,current.transform.position.y+height,current.transform.position.z);
		determinedist ();
		teller = 0;
		//print (height == 3);

		}
	
		// Update is called once per frame
		void Update ()
		{
		transform.LookAt(target);
		//if (transform.position.x == next.transform.position.x && transform.position.z == next.transform.position.z) {
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
		
		void determinedist(){

		dist = new Vector3 (next.transform.position.x - current.transform.position.x, 0, next.transform.position.z - current.transform.position.z);

		xstap = dist.x / delta;
		//ystap = dist.y / 100f;
		zstap = dist.z / delta;
		print (dist);

		print (xstap);
		print (zstap);
		print (transform.position);
		print (next.transform.position);
		print (current.transform.position);
		}
		
		void interpolate(){
		//dist = new Vector3 (current.transform.position.x - next.transform.position.x, 0, current.transform.position.x - next.transform.position.x);
		transform.position = new Vector3(current.transform.position.x + xstap*teller,transform.position.y,current.transform.position.z + zstap*teller); 
		//print (current.transform.position.z);
		if (teller == 100) {
						print (teller);
						print (transform.position.z);
						print (next.transform.position.z);
						float temp = (float) transform.position.z;
						float temp2 = (float) next.transform.position.z;
						if (temp == temp2) {
								print (true);
						}
				}
		teller = teller + 1;

				
				//curve = Vector3.Angle(transform.forward, viewpoint);
				//this.transform.rotation = Quaternion.SetFromToRotation(this.transform.rotation, new Vector3(transform.rotation.x+curve,0,0));
		//temp = transform.forward;
		//transform.rotation = Quaternion.SetLookRotation(viewpoint, temp); 

		}
}

