using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereCaster : MonoBehaviour {
	//variables storing how big the sphere is and how far the its cast
	public float sphereRadius;
	public float maxDist;

	//variables storing the origin and direction of the casting
	private Vector3 origin;
	private Vector3 direction;

	//varibale currHitDist returning the distance of the current object from the camera which is only used for the drawing of the debug line    
	private float currHitDist;
	//all objects that are in the scene are stored in this array
	private GameObject[] allObjects;
	private bool getObjects = true;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if(getObjects) {
			//there's a tag that is set on the creation of the objects in "CreateObjects.cs" which allows 
			//us to find the cubes to potentially activate
			allObjects = GameObject.FindGameObjectsWithTag ("findable");
			getObjects = false;
		}

		//set all the cubes to have their targets switched off until the sphere is cast on them
		foreach (GameObject gameObj in allObjects) {
			var cubeComp = gameObj.GetComponent<Cube>();
			cubeComp.Deactivate();
		}

		//set the origin and direction to the camera
		origin = transform.position;
		direction = transform.forward;
		//cast the sphere
		RaycastHit[] hit = Physics.SphereCastAll (origin, sphereRadius, direction, maxDist);
		//for all the cubes in sphere radius set them to active allowing the targets to be shown
		foreach (var _hit in hit) {
			currHitDist = _hit.distance;
			if (_hit.transform.tag == "findable") {
				//activate hit objects
				GameObject cube = _hit.transform.gameObject;
				cube.GetComponent<Cube> ().Activate ();
			} 	
		}
	}

	//displays the casting of the sphere when the camera is clicked in the editor
	private void OnDrawGizmosSelected(){
		Gizmos.color = Color.red;
		Debug.DrawLine (origin, origin + direction * currHitDist);
		Gizmos.DrawWireSphere (origin+direction*currHitDist, sphereRadius);		
	}
}
