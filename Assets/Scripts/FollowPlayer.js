#pragma strict

//using UnityEngine;
//using System.Collections;

public class FollowPlayer extends MonoBehaviour{
  public var auto : GameObject;
  private var basePlayerPosition : Vector3;
  private var baseCameraPosition : Vector3;

  function Start () {
    basePlayerPosition = auto.transform.position;
	baseCameraPosition = transform.position;
  }
  
  function Update () {
    //transform.position = transform.position + (baseCameraPosition + (auto.transform.position - basePlayerPosition) - transform.position); //* 0.1f;
    transform.position = auto.transform.position;
  }

}

