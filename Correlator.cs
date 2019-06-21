using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Correlator : MonoBehaviour {
	public Headset headset;		// reference to the headset
	public int windowSize = 60; // number of readings to take
	public float sampleDuration;
	private float screenFrameRate = 60.0f;

	private ArrayList headTime;		// list of times of last head readings
	private ArrayList headX;		// list of last pitch axis readings
	private ArrayList headY;		// list of last yaw axis readings
	private ArrayList targetTime;	// list of times of last target readings
	private ArrayList orbitX;		// list of last target x positions
	private ArrayList orbitY;		// list of last target y positions
	private GameObject[] targets; 		// list of all targets
	private float corResultsX;		// results of the x correlation
	private float corResultsY;		// results of the y correlation
	private float corThreshold = 0.8f;
	private float closeCorThreshold = 0.6f;
	private int winner;

	private float lapse = 0.5f;			// time allowed for momentary lapses in correlation
	private float minTrackTime = 0.75f;	// minimum time user must be tracking target for
	private float stopTrackTime = 2.0f;	// time till system accepts that user is no longer tracking
	private float stopDisplayTime = 3.5f;	// time till system accepts that user is no longer tracking
	private float delaySelection = 0.3f;// time to wait after correlating before checking for confirmation
	private bool phaseShiftMode = false; //flag to make sure that unnecessary loops don't execute while angles are shifted
	private float phaseShiftTimer = 5.0f; //When angles are shifted, this extra time is added to recorrelate 
	private bool selectionComplete = false; 
	private int selectedTargets=0;
	private float whiteLightCountDown = 0.5f;
	// Returns the average of a list of numbers
	float Avg(ArrayList list)
	{
		float sum = 0.0f;
		foreach (var i in list) 
		{
			sum += (float) i;
		}
		return sum / list.Count;
	}

	// Part of the correlation calculation
	float ZipSum(ArrayList head, ArrayList target, float avg1, float avg2)
	{
		float sum = 0;
		for(int i = 0 ; i < head.Count; i++)
		{
			sum += ((float) head [i] - avg1) * ((float) target [i] - avg2);
		}
		return sum;
	}

	// Part of the correlation calculation
	float SqrSum(ArrayList list, float avg)
	{
		float sum = 0;
		for (int i = 0; i < list.Count; i++) {
			sum += Mathf.Pow (((float) list [i] - (float) avg), 2.0f);
		}
		return sum;
	}

	// Correlation calculation
	float Coeff(ArrayList head, ArrayList target)
	{
		if (head.Count != target.Count)
			return 0;
	
		float avg1 = Avg (head);
		float avg2 = Avg (target);

		float sum = ZipSum (head, target, avg1, avg2);

		float sqrSum1 = SqrSum (head, avg1);
		float sqrSum2 = SqrSum (target, avg2);
		float result = 0.0f;
		if (sqrSum1 != 0 && sqrSum2 != 0)
			result = sum / Mathf.Sqrt (sqrSum1 * sqrSum2);
		//if (result >= 1.0f || result <= -1.0f)
			//Debug.Log ("Coeff calc. ERROR");
		return result;
	}
		
	// Use this for initialization
	void Start () {
		headX = headset.getHeadX ();
		headY = headset.getHeadY ();
		sampleDuration = windowSize / screenFrameRate * 1000.0f;

		//setup targets
		targets = GameObject.FindGameObjectsWithTag("target");
	}

	// Update is called once per frame
	void Update () {
		//Application.targetFrameRate = 60;
		// Update the records
		headTime = headset.getTimes ();
		headX = headset.getHeadX ();
		headY = headset.getHeadY ();


		// Error checks
		for (int i = 0; i < headX.Count; i++) {
			if (headX [i] != headset.getHeadX () [i]) {
				Debug.Log ("ERROR");
			}
		}
		for (int i = 0; i < headY.Count; i++) {
			if (headY [i] != headset.getHeadY () [i]) {
				Debug.Log ("ERROR");
			}
		}

		// Control phase shift timer and flags
		// Reset speeds when selection is complete
		managePhaseShift ();

		//check each target for correlations
		runCorrelations ();

		if (!phaseShiftMode) {
			selectedTargets = countCorrelatingTargets ();
			// Enter phase shift mode if necessary and turn other targets off
			phaseShiftOperations ();
		}
	}

	// Control phase shift timer and flags
	// Reset speeds when selection is complete
	public void managePhaseShift() {
		//This if manages the Timer count down when phase shift is activated
		if (phaseShiftMode && phaseShiftTimer > 0.0f) 
		{//timer goes down
			phaseShiftTimer -= Time.deltaTime;
		}
		else if (phaseShiftMode && phaseShiftTimer < 0.0f)
		{ //when timer hits 0, go back to normal mode and reset timer
			phaseShiftMode = false;
			phaseShiftTimer = 5.0f;
			selectionComplete = true;
		}
		//if it's not phaseShift mode and selected targers >1 that means this is the first loop after ending phaseShift,
		//this loop ensures that all targets get reactivated
		if (selectionComplete) 
		{
			foreach(GameObject _target in targets)
			{
				if (_target.GetComponent<Target>().speedChanged) 
				{
					_target.GetComponent<Target>().resetSpeed ();
					_target.GetComponent<Target>().speedChanged = false;
				}
				// NEEDS TO BE FADE BACK IN
				_target.GetComponent<Target>().SetActive(true);
			}
			selectionComplete = false;
		}
	}

	// Run correlation calculations on all targets
	public void runCorrelations() {
		//time to be added to correlation check if we are in phase shift mode
		float extraTime = (phaseShiftMode) ? phaseShiftTimer : 0.0f;

		// Check each target for correlations
		foreach (GameObject _target in targets) 
		{
			if (!_target.GetComponent<Target>().IsActive())
				continue;
			
			orbitY =_target.GetComponent<Target>().getYs ();
			if (_target.transform.position.z >= 0.0f ||
				_target.transform.position.z <= -100.0f) {
				orbitX = _target.GetComponent<Target> ().getXs ();
			} else {
				orbitX = _target.GetComponent<Target>().getNegXs ();
			}
			// run the correlation 
			corResultsX = Coeff (headX, orbitX);
			corResultsY = Coeff (headY, orbitY);
			// Check that correlation is high enough

			if (corResultsX >= corThreshold && corResultsY <= -corThreshold) {
				//_target.GetComponent<Renderer>().material.color = Color.blue;	
				//Debug.Log("********************CORRELATING*********************");
				//Debug.Log("THRESHOLD" + _target.GetComponent<Target>().getID());
				// Record time that this correlation took place
				_target.GetComponent<Target>().currentTime = Time.time;
				// Account for momentary lapses in correlation
				if (_target.GetComponent<Target>().currentTime - _target.GetComponent<Target>().prevTime < lapse) {
					// Keep track of how long the user is tracking this target
					_target.GetComponent<Target>().correlateTime += _target.GetComponent<Target>().currentTime - _target.GetComponent<Target>().prevTime;
				} else {
					// User has stopped tracking, reset to zero
					_target.GetComponent<Target>().correlateTime = 0.0f;
				}
				// Set previous time to now for next run
				_target.GetComponent<Target>().prevTime = _target.GetComponent<Target>().currentTime;
				// If user has been tracking for long enough, turn the targets light on
				//Debug.Log("********************CORRELATE TIME*********************");
				//Debug.Log (_target.GetComponent<Target> ().correlateTime);
				//Debug.Log("********************MIN + EXTRA TIME*********************");
				//Debug.Log (minTrackTime + extraTime);
				if (_target.GetComponent<Target>().correlateTime > minTrackTime + extraTime) {
         			_target.GetComponent<Target>().selected = true;
					// _target.GetComponent<Target>().light.range = 10.0f;
					// _target.GetComponent<Target>().light.color = Color.white;
					// _target.GetComponent<Target>().light.enabled = true;
					_target.GetComponent<Target>().waitingForConf = true;
					_target.GetComponent<Renderer>().material.color = Color.blue;
					// Get a reading from the headset at correlation time for comparison later
					_target.GetComponent<Target>().setHeadsetFirstX ((float)headset.getHeadX () [59]);
					_target.GetComponent<Target>().setHeadsetFirstY ((float)headset.getHeadY () [59]);
					// Record winner and spin cube
					winner = _target.GetComponent<Target>().getID ();
					//Debug.Log("*******************WINNER********************** " + winner);
					GameObject cube = _target.GetComponent<Target>().getCube ();
					cube.transform.Rotate (new Vector3 (0, 0, -10));
				}
			}  

			if (Time.time - _target.GetComponent<Target> ().currentTime > stopDisplayTime)
			{
				_target.GetComponent<Target> ().endFunction ();
			} 
				
			// If this target was being followed...
			if (_target.GetComponent<Target>().waitingForConf) {
				// switch its light off if the user stops tracking for stopTrackTime
				if (Time.time - _target.GetComponent<Target>().currentTime > stopTrackTime) {
					_target.GetComponent<Target>().light.enabled = false;

					_target.GetComponent<Target>().selected = false;
					_target.GetComponent<Target>().waitingForConf = false;
				} 
				// wait for a delaySelection time
				else if (Time.time - _target.GetComponent<Target>().currentTime > delaySelection + extraTime) {
					// Get a reading from the headset
					_target.GetComponent<Target>().setHeadsetLastX ((float)headset.getHeadX () [59]);
					_target.GetComponent<Target>().setHeadsetLastY ((float)headset.getHeadY () [59]);
					// check if headset has been still
					if (headset.checkStill (_target.GetComponent<Target>().getHeadsetFirstX (), _target.GetComponent<Target>().getHeadsetLastX (), _target.GetComponent<Target>().getHeadsetFirstY(), _target.GetComponent<Target>().getHeadsetLastY()) == true) {
						_target.GetComponent<Target> ().performFunction ();
						_target.GetComponent<Renderer>().material.color = Color.green;

						phaseShiftMode = false;
						selectionComplete = true;
					}
				}
			}
		}//foreach targets ends
	}

	// Count the number of targets that the user has correlated with
	public int countCorrelatingTargets() {
		int total = 0;
		foreach (GameObject _target in targets) {
			if (_target.GetComponent<Target>().selected)
				total++;
		}
		return total;
	}

	// Check if we have to enter phase shift mode and perform necessary operations
	public void phaseShiftOperations() {
		int acc = 0;
		foreach (GameObject _target in targets) 
		{
			if (_target.GetComponent<Target>().selected && selectedTargets > 1) 
			{

				// If this is a correlating target and there is more than one
				// correlating target then enter phase shift mode.
				// Speed targets up by different amounts to spread them out
				if (!_target.GetComponent<Target>().speedChanged) 
				{
					phaseShiftMode = true;
					_target.GetComponent<Target>().light.enabled = false;
					_target.GetComponent<Target>().ANGLE_SPEED += (acc++ / (float)selectedTargets);
					_target.GetComponent<Target>().speedChanged = true;
				}
			}
			if (!_target.GetComponent<Target>().selected && selectedTargets > 1) 
			{
				// If this is not a correlating target and there is more than one
				// correlating target then turn this target off.
				_target.GetComponent<Target>().SetActive(false);
			} 
			// Reset target selection for next run
			_target.GetComponent<Target>().selected = false;					
		}
	}
		
	public float getCorX(){
		return corResultsX;
	}

	public float getCorY(){
		return corResultsY;
	}

	public int getWinner() {
		return winner;
	}
}