using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DisplayYaw : MonoBehaviour {

	private Text yawText;  // public if you want to drag your text object in there manually
	public Headset headset;

	void Start () {
		yawText = GetComponent<Text>();  // if you want to reference it by code - tag it if you have several texts
	}

	void Update () {
		yawText.text = "Yaw: " + headset.getYaw().ToString();  // make it a string to output to the Text object
	}
}