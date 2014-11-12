#pragma strict

//using UnityEngine;
//using UnityEngine.UI;
//using System.Collections;

public class Playerbeginsel extends MonoBehaviour {
  private var speed : float; //snelheid door het spel heen
  public var dt : float; //stapgrootte
  public var ds : float; //stapgroote van snelheid
  public var dzr : float; //stapgroote van rotatie
  public var start : Vector3; //startpositie
  public var startAngle : Vector3; //Beginhoeken
  private var zrotation: float;
 

  function Start () {
    speed = 0.0;
    transform.position = start;
    transform.eulerAngles = startAngle;

  }

  function Update () {
    //Beweging player
    //if (pause == false) { Als we een pauze ingesteld hebben
      if (Input.GetKey(KeyCode.UpArrow)) {
          speed = speed + ds;  //snelheid vergroten
          transform.position = new Vector3 (transform.position.x + speed*dt, transform.position.y, transform.position.z);
		  print(transform.position.x);
	  }
	 	
	  else if(Input.GetKey(KeyCode.DownArrow)){
	    speed = speed - ds;
	    transform.position = new Vector3 (transform.position.x + speed*dt, transform.position.y, transform.position.z);
	  }
	  else if(Input.GetKey(KeyCode.LeftArrow)){
	    if(zrotation + dzr>=360){ zrotation = zrotation + dzr;   }
	    else{ zrotation = zrotation + dzr -360;}
	    transform.eulerAngles = new Vector3 (transform.eulerAngles.x, zrotation, transform.eulerAngles.y);
	    //http://unity3d.com/learn/tutorials/modules/beginner/scripting/translate-and-rotate
	  
	  }
	  else if(Input.GetKey(KeyCode.RightArrow)){}
	  else{
	    transform.position = new Vector3 (transform.position.x + speed*dt, transform.position.y, transform.position.z);
	  }
	}
	
  //}
  

}

						 
						
