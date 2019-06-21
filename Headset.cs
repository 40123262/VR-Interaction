using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Headset : MonoBehaviour {

	private float pitch;		// pitch of headset
	private float yaw;			// yaw of headset

	private ArrayList headTime; // time of the readings
	private ArrayList headX;	// the headset's previous pitch readings
	private ArrayList headY;	// the headset's previous yaw readings

	public Correlator correlator; // reference to the correlator

	public Camera cam;

	// controls for mouse movement
	public float speedH = 2.0f;
	public float speedV = 2.0f;
	private float y = 0.0f;
	private float p = 0.0f;

	// Use this for initialization
	void Start () {
		headTime = new ArrayList ();
		headX = new ArrayList ();
		headY = new ArrayList ();
	}

	// Update is called once per frame
	void Update () {
		// get new pitch and yaw reading
		var pitch = getPitch();
		var yaw = getYaw();
	
		// Debug.Log(pitch);
		// Debug.Log(yaw);

		addHeadData (Time.time * 1000.0f, yaw, pitch);
		// move the headset if necessary
		move ();
	}

	void move() {
		y += speedH * Input.GetAxis("Mouse X");
		p -= speedV * Input.GetAxis("Mouse Y");

		transform.eulerAngles = new Vector3(p, y, 0.0f);
	}

	// Add head date to the store
	void addHeadData(float now, float yaw, float pitch)
	{
		headTime.Add(now); 
		headX.Add(yaw);
		headY.Add(pitch);

		// Remove old data if we have more than windowSize samples
		if (headTime.Count > correlator.windowSize)    
		{
			while ((float) headTime[1] < now-correlator.sampleDuration && headTime.Count > 1)
			{
				headTime.RemoveAt(0);
				headX.RemoveAt(0);
				headY.RemoveAt(0);
			}
		}
	}

	// Check if the two angles are close to each other withing a certain threshold
	public bool checkStill(float firstX, float lastX, float firstY, float lastY) {
		float closeThreshold = 2.0f;
		if (lastX > 180.0f)
			lastX -= 360.0f;
		if (firstX > 180.0f)
			firstX -= 360.0f;
		
		if (lastY > 180.0f)
			lastY -= 360.0f;
		if (firstY > 180.0f)
			firstY -= 360.0f;
		
		if (Mathf.Abs (lastX - firstX) < closeThreshold && Mathf.Abs (lastY - firstY) < closeThreshold)
			return true;
		else
			return false;
	}

	// Returns the yaw of the camera
	public float getYaw() {
		return cam.transform.rotation.eulerAngles.y;
	}

	// Returns the pitch of the camera
	public float getPitch() {
		return cam.transform.rotation.eulerAngles.x;
	}

	// Returns the previous pitch readings
	public ArrayList getHeadX() {
		return headX;
	}

	// Returns the previous yaw readings
	public ArrayList getHeadY() {
		return headY;
	}

	// Returns the times readings
	public ArrayList getTimes () {
		return headTime;
	}
}
