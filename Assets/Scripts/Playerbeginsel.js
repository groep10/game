#pragma strict

//using UnityEngine;
//using UnityEngine.UI;
//using System.Collections;

public class Playerbeginsel extends MonoBehaviour {
  private var speed : float; //snelheid door het spel heen
  public var dt : float; //stapgrootte Dit kan ook opgevangen worden met Time.deltaTime
  public var ds : float; //stapgroote van snelheid
  public var start : Vector3; //startpositie
  public var startAngle : Vector3; //Beginhoeken
  public var rotationspeed : float; //stapgrootte bij draaing
 

  function Start () {
    speed = 0.0; //beginwaarde snelheid
    transform.position = start; //Hier kunnen we beginpositie mee invoeren
    transform.eulerAngles = startAngle; //Hier kunnen we beginpositie mee invoeren
    
  }

  function Update () {
    //Beweging player
    //if (pause == false) { Als we een pauze ingesteld hebben
      if (Input.GetKey(KeyCode.UpArrow)) {
          if(-30<speed && speed<30) {speed = speed + ds;}  //snelheid vergroten, maar wel binnen bepaalde limieten
          transform.Translate(Vector3.forward * speed*dt); 
      }
	  else if(Input.GetKey(KeyCode.DownArrow)){ //zelfde manier als uparrow
	    if(-30<speed && speed<30) {speed = speed - ds;}
	    transform.Translate(Vector3.forward * speed*dt);
	  }
	  else if(Input.GetKey(KeyCode.LeftArrow)){
	    transform.Rotate(Vector3.up, -rotationspeed*dt); //draait met een vaste grootte
	    transform.Translate(Vector3.forward * speed*dt); //zorgt dat autootje voortbeweegt tijdens draaiing
	  }
	  else if(Input.GetKey(KeyCode.RightArrow)){ // zelfde manier als leftarrow
	    transform.Rotate(Vector3.up, rotationspeed*dt);
	    transform.Translate(Vector3.forward * speed*dt);
	  }
	  else{
	   transform.Translate(Vector3.forward * speed*dt); //zorgt dat gasknop niet ingedrukt hoeft te blijven.
	   }
	}
	
  //}
  
  
  	/*function OnTriggerEnter (other : Collider) {

	  if (other.gameObject.tag == "schans") {
	    //transform.position = (transform.position.x, other.transform.position.y, transform.position.z);
			

		}
   }*/
   function OnTriggerStay (other : Collider) {
		if (other.attachedRigidbody) {
			other.attachedRigidbody.AddForce(Vector3.up * 10);
		}
	}

}

						 
						
