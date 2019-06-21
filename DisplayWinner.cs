using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayWinner : MonoBehaviour {
	private Text winnerText;
	public Correlator corr;

	// Use this for initialization
	void Start () {
		winnerText = GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		winnerText.text = "Winnner: " +  corr.getWinner ().ToString();
	}
}
