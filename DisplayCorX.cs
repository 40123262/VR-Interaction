using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DisplayCorX : MonoBehaviour {

	private Text corXText;  // public if you want to drag your text object in there manually
	public Correlator correlator;

	void Start () {
		corXText = GetComponent<Text>();  // if you want to reference it by code - tag it if you have several texts
	}

	void Update () {
		corXText.text = "CorX: " + correlator.getCorX().ToString();  // make it a string to output to the Text object
	}
}