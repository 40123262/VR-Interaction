using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DisplayCorY : MonoBehaviour {

	private Text corYText;  // public if you want to drag your text object in there manually
	public Correlator correlator;

	void Start () {
		corYText = GetComponent<Text>();  // if you want to reference it by code - tag it if you have several texts
	}

	void Update () {
		corYText.text = "CorY: " + correlator.getCorY().ToString();  // make it a string to output to the Text object
	}
}