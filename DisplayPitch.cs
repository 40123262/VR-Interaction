using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DisplayPitch : MonoBehaviour {

	private Text pitchText;  // public if you want to drag your text object in there manually
	public Headset headset;

	void Start () {
		pitchText = GetComponent<Text>();  // if you want to reference it by code - tag it if you have several texts
	}

	void Update () {
		pitchText.text = "Pitch: " + headset.getPitch().ToString();  // make it a string to output to the Text object
	}
}