using UnityEngine;
using System.Collections;


public class Target : MonoBehaviour {

	public float cX, cY;			// target (fixed) positions
	public float starting_angle;	// initial phase position
	public int direction;			// 1 (clockwise) or -1 (counterclockwise)
	public float radius;			// radius of orbit

	private float oX, oY;			// orbit (moving) positions
	private float angle;
	public float ANGLE_SPEED;	// orbit speed
	public float STARTING_ANGLE_SPEED = 2.0f;
	private int target_id;			// 0: corner; 1: edge; 2:centre

	public ArrayList targetXs;		// this target's previous x positions
	public ArrayList targetYs;		// this target's previous y positions
	private ArrayList targetTime;

	public Correlator correlator;	// reference to the correlator
	public GameObject cube;			//cube associated with target

	public float currentTime;		// most current time when correlation took place
	public float prevTime = 0.0f;	// previous time when correlation took place
	public float correlateTime;		// total time correlation has been detected for

	public Light light;				// reference to the targets light

	private float headsetFirstX;		// headset reading from moment of correlation detection
	private float headsetLastX;		// headset reading to compare
	private float headsetFirstY;		// headset reading from moment of correlation detection
	private float headsetLastY;		// headset reading to compare
	public bool waitingForConf = false;

	public bool selected;
	public bool active = true;
	public bool speedChanged = false;
	private float prev_angle;
	private LineRenderer lineRenderer;
	private int segments = 50;

	GameObject test;

	// Use this for initialization
	void Start () {
		angle = starting_angle;
		targetXs = new ArrayList ();
		targetYs = new ArrayList ();
		targetTime = new ArrayList ();
		gameObject.AddComponent<Light>();
		light = GetComponent<Light>();
	}
	
	// Update is called once per frame
	void Update () {
		moveOrbit ();
		checkActive ();
		transform.position = new Vector3 (oX, oY, transform.position.z);
		addTargetData (Time.time * 1000.0f, oX, oY);		
	}

	public void performFunction()
	{
		var popComp = cube.GetComponent<Popup>();
		if (popComp != null) {
			popComp.show ();
		}
		if (GetComponent<Teleport> () != null) {
			GetComponent<Teleport> ().teleport ();
		}
	}
	public void endFunction()
	{
		if (GetComponent<Popup> () != null) {
			GetComponent<Popup> ().hide();
		}
	}
	
	//draws the orbit
	public void DrawOrbit ( ) {
		Color c = Color.white;
		c.a = 0.5f;
		lineRenderer = cube.GetComponent<LineRenderer>();
		lineRenderer.material = new Material(Shader.Find("Mobile/Particles/Additive"));
		lineRenderer.startColor = c;
		lineRenderer.endColor = c;
		lineRenderer.startWidth = 0.3f;
		lineRenderer.endWidth = 0.3f;
		lineRenderer.positionCount = (segments + 1);
		lineRenderer.useWorldSpace = false;

		float deltaTheta = (float) (2.0 * Mathf.PI) / segments;
		float theta = 0f;
		
		for (int i = 0 ; i < segments + 1 ; i++) {
			float x = 2 * radius * Mathf.Cos(theta);
			float z = 2 * radius * Mathf.Sin(theta);
			Vector3 pos = new Vector3(x, z, 0);
			lineRenderer.SetPosition(i, pos);
			theta += deltaTheta;
		}
	}

	public void HideOrbit() {
		Color c = Color.white;
		c.a = 0;
		var comp = cube.GetComponent<LineRenderer> ();
		comp.startColor = c;
		comp.endColor = c;
	}

	// Move the orbit to its original position at the start of the trail
	public void resetOrbit () {
		angle = starting_angle;
	}

	public void resetSpeed () {
		ANGLE_SPEED = STARTING_ANGLE_SPEED;
	}

	// Move the orbit by ANGLE_SPEED
	public void moveOrbit() {	
		angle += direction * ANGLE_SPEED;
		oX = cX + Mathf.Sin (Mathf.Deg2Rad * angle) * radius;
		oY = cY + Mathf.Cos (Mathf.Deg2Rad * angle) * radius;		
	}

	// Fade in and out depending on 'active'
	public void checkActive() {
		if (active) {
			Color color = GetComponent<Renderer> ().material.color;
			color.a = Mathf.Min (1.0f, color.a + 0.01f);
			GetComponent<Renderer> ().material.color = color;
		} else {
			Color color = GetComponent<Renderer> ().material.color;
			color.a = Mathf.Max (0.0f, color.a - 0.01f);
			GetComponent<Renderer> ().material.color = color;
		}
	}

		
	// Add target data to the store
	void addTargetData(float now, float x, float y)
	{
		targetTime.Add(now); 

		targetXs.Add (x);
		targetYs.Add (y);

		// Remove old data if we have more than windowSize samples
		if (targetTime.Count > correlator.windowSize)    
		{
			while ((float) targetTime[1] < now-correlator.sampleDuration && targetTime.Count > 1)
			{
				targetTime.RemoveAt(0);
				targetXs.RemoveAt(0);
				targetYs.RemoveAt(0);
			}
		}
	}

	// Return the current orbit position (x)
	float getOrbitX () {
		return oX;
	}

	// Return the current orbit position (y)
	float getOrbitY () {
		return oY;
	}

	//	// 0: corner; 1: edge; 2:centre
	//	public int getTargetID () {
	//		return target_id;
	//	}

	// Return the target's X history
	public ArrayList getXs() {
		return targetXs;
	}

	// Return the target's Y history
	public ArrayList getYs() {
		return targetYs;
	}

	// Return the times readings
	public ArrayList getTimes () {
		return targetTime;
	}

	//setter for id
	public void setID(int id) {
		this.target_id = id;
	}

	//getter for id
	public int getID() {
		return target_id;
	}

	public GameObject getCube() {
		return cube;
	}

	//getter for first headset reading
	public float getHeadsetFirstX() {
		return headsetFirstX;
	}

	//setter for first headset reading
	public void setHeadsetFirstX(float first) {
		headsetFirstX = first;
	}

	//getter for last headset reading
	public float getHeadsetLastX() {
		return headsetLastX;
	}

	//setter for last headset reading
	public void setHeadsetLastX(float last) {
		headsetLastX = last;
	}

	//getter for first headset reading
	public float getHeadsetFirstY() {
		return headsetFirstY;
	}

	//setter for first headset reading
	public void setHeadsetFirstY(float first) {
		headsetFirstY = first;
	}

	//getter for last headset reading
	public float getHeadsetLastY() {
		return headsetLastY;
	}

	//setter for last headset reading
	public void setHeadsetLastY(float last) {
		headsetLastY = last;
	}

	public bool IsActive() {
		return active;
	}

	public void SetActive(bool b) {
		active = b;
	}

	public ArrayList getNegXs() {
		ArrayList negXs = new ArrayList ();
		for (int i = 0; i < getXs ().Count; i++) {
			negXs.Add (-1.0f * (float)getXs () [i]);
		}
		return negXs;
	}
}
