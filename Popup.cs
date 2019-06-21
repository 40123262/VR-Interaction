using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup : MonoBehaviour 
{

	// Use this for initialization
	private GameObject headset;
	public GameObject popup;
	public GameObject text;
	public GameObject arrow;

	//how long the popup is visible for
	public float LIFETIME = 5.0f;
	private float timer = 5.0f;
	//reference to the target so we can hide or show the orbit
	public GameObject target;

	bool visible = false;
	public void show () 
	{
		popup.SetActive (true);
		//text.SetActive(true);
		arrow.SetActive (true);
		//reset timer
		timer = LIFETIME;
		//	if(popup.transform.localScale.y<10.0f)
		//		popup.transform.localScale.y += Time.deltaTime;
		var tarComp = target.GetComponent<Target>();
		tarComp.DrawOrbit();

	}
	public void hide () 
	{
		popup.SetActive (false);
		arrow.SetActive (false);
		var tarComp = target.GetComponent<Target>();
		tarComp.HideOrbit();
	}

	public void setPopup (GameObject popup) {
		this.popup = popup;
	}

	public void setText(GameObject text) {
		this.text = text;
	}

	void Start() {
		headset = GameObject.FindWithTag ("headset");
	}

	void Update()
	{
		var n_popup = headset.transform.position - popup.transform.position;
		popup.transform.rotation = Quaternion.LookRotation (n_popup);
		var n_arrow = headset.transform.position - arrow.transform.position;
		arrow.transform.rotation = Quaternion.LookRotation (n_arrow);

		timer -= Time.deltaTime;


		//when the timer reaches 0, hide the popup
		if(timer <= 0 && popup.activeSelf) {
			hide();
			timer = LIFETIME;
		}
	}

}