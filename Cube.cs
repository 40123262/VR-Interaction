using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour {
	private GameObject target;
	private float speed = 2.0f;
	private bool activated = false;

	// Use this for initialization
	void Start () {}
	
	// Update is called once per frame
	void Update () {

	}

	//activate the targets 
	public void Activate() {
		var comp = target.GetComponent<Target> ();
		comp.SetActive(true);

		//makes the target visable
		target.transform.localScale = new Vector3 (0.5f, 0.5f, 0.5f);
		activated = true;
	}

	//deactivates the targets
	public void Deactivate() {
		var comp = target.GetComponent<Target> ();
		comp.SetActive(false);
		//make the target invisable
		target.transform.localScale = Vector3.zero;
		activated = false;
		target.GetComponent<Renderer>().material.color = Color.white;
	}

	public void setTarget(GameObject _t) {
		target = _t;
	}

	public GameObject getTarget() {
		return target;
	}

	public bool Activated() {
		return activated;
	}

	public GameObject getCube() {
		return gameObject;
	}
}
