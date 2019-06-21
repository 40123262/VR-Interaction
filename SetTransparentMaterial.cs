using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTransparentMaterial : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Debug.Log ("Material");
		GetComponent<MeshRenderer> ().material.color = new Color (0.5f, 0.5f, 0.5f, 0.5f);
	}

	// Update is called once per frame
	void Update () {

	}
}