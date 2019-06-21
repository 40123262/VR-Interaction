using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour 
{

	// Use this for initialization
	public Headset headset;
	public GameObject destination;
	public void teleport () {
		headset.transform.position = destination.transform.position;
	}

}