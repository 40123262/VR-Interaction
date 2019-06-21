using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateObjects : MonoBehaviour {
	private ArrayList targetPositions;

	public Headset headset;
	public Correlator corr;
	public int ROWS = 1, COLS = 1, DEPTH = 1; //number of rows and columns of objects
	public Vector3 startPos = new Vector3 (-20, 26, 15);	//position of first object

	private ArrayList cubes;	//some lists to store everything
	private ArrayList targets;
	private ArrayList targetInfo;
	private ArrayList popupMaterials;
	private Material teleportMaterial;
	private Material tutorialMaterial;

	// Use this for initialization
	void Start () {
		Cursor.visible = false;
		cubes = new ArrayList ();
		targets = new ArrayList ();
		targetPositions = new ArrayList();
		targetInfo = new ArrayList();
		targetInfo = new ArrayList();
		popupMaterials = new ArrayList ();

		initialiseTargetPositions();
		initialiseTargetInfo();
		initialisePopupMaterials ();


		CreateCubes ();
		CreateTargets ();
	}

	//creates cubes from prefab
	void CreateCubes() {
		//create game object
		int acc = 0;
		foreach (Vector3 currentPos in targetPositions) {
			GameObject clone = GameObject.Instantiate (Resources.Load<GameObject> ("Cube"), currentPos, Quaternion.identity) as GameObject;
			//add necessary components (line renderer in order to draw the orbit)
			clone.AddComponent<LineRenderer> ();
			clone.GetComponent<Renderer> ().enabled = false;

			//add offset so the popup apperas above the object
			var offset = new Vector3 (0, 9, 0);
			var offsetArrow = new Vector3 (0, 5, 0);

			//instantiate the prefab for the popup
			GameObject popup = GameObject.Instantiate (Resources.Load ("triangle"), currentPos + offset, Quaternion.identity) as GameObject;
			popup.GetComponent<Renderer> ().material = popupMaterials [acc % 6] as Material;
			popup.SetActive (false);
			//set its text
			popup.tag = "popup";

			//instantiate the text object
			var popupText = GameObject.Instantiate (Resources.Load ("text"), popup.transform.position, Quaternion.identity) as GameObject;
			var text = popupText.GetComponent<TextMesh> ();
			popupText.SetActive (false);
			popupText.tag = "popup";

			// add the arrow to the rock
			var popupArrow = GameObject.Instantiate (Resources.Load ("arrow"), currentPos + offsetArrow, Quaternion.identity) as GameObject;
			Material arrowMat = Resources.Load ("PopupArrow", typeof(Material)) as Material;
			popupArrow.GetComponent<Renderer> ().material = arrowMat;
			popupArrow.SetActive (false);
			popupArrow.tag = "popup";
			
			//add the popup component to the cube
			clone.AddComponent<Popup> ();
			var pComp = clone.GetComponent<Popup> ();
			//pass the popup and the text to it
			pComp.popup = popup;
			pComp.text = popupText;
			pComp.arrow = popupArrow;

			if (acc == 0) {
				GameObject tutorialInfo = GameObject.Instantiate (Resources.Load ("triangle"), currentPos + offset, Quaternion.identity) as GameObject;
				tutorialInfo.GetComponent<Renderer> ().material = tutorialMaterial as Material;
				tutorialInfo.SetActive (true);
				//set its text
				tutorialInfo.tag = "popup";
				tutorialInfo.transform.Rotate (new Vector3 (0.0f, 180.0f, 0.0f));
			}

			if (acc == targetPositions.Count - 1 || acc == targetPositions.Count - 2) {
				GameObject teleportInfo = GameObject.Instantiate (Resources.Load ("triangle"), currentPos + offset, Quaternion.identity) as GameObject;
				teleportInfo.GetComponent<Renderer> ().material = teleportMaterial as Material;
				teleportInfo.SetActive (true);
				//set its text
				teleportInfo.tag = "popup";
				if (acc == targetPositions.Count - 1) {
					teleportInfo.transform.Rotate (new Vector3 (0.0f, 180.0f, 0.0f));
				}
			}

			//add sound
			popup.AddComponent<AudioSource> ();
			var audioComp = popup.GetComponent<AudioSource> ();
			audioComp.clip = Resources.Load ("pop2", typeof(AudioClip)) as AudioClip;
			audioComp.playOnAwake = true;

			clone.tag = "findable";
			clone.layer = 8;
			cubes.Add (clone); //add to list	
			acc++;
		}
	}

	//creates a target for each object
	void CreateTargets() {
		var starting_angle = 0f;
		int id = 0;
		foreach(GameObject _cube in cubes) {
			var popup = _cube.GetComponent<Popup>();
			var text = popup.text.GetComponent<TextMesh>();
			text.text = targetInfo[id].ToString();
			text.fontSize = 10;

			//same as the cubes
			GameObject target = GameObject.Instantiate(Resources.Load<GameObject>("Sphere"), _cube.GetComponent<Transform>().position, Quaternion.identity) as GameObject;
			target.tag = "target";
			//add the target script
			target.AddComponent<Target> ();
			//target.AddComponent<Light>();

			popup.target = target;

			//set up the target
			var comp = target.GetComponent<Target> ();
			comp.cX = _cube.transform.position.x;
			comp.cY = _cube.transform.position.y;
			comp.starting_angle = starting_angle;
			comp.radius = 2.0f;
			comp.ANGLE_SPEED = 2.0f;
			comp.direction = 1;
			comp.cube = _cube;
			comp.correlator = corr;
			comp.setID (id);

			_cube.AddComponent<Cube>();
			var cubeComp = _cube.GetComponent<Cube> ();
			cubeComp.setTarget(target);
			cubeComp.Deactivate ();

			starting_angle += 45.0f;
			if(id % 3==0)
			{
				comp.direction*=-1;
			}

			//Set teleport targets
			if (id == 0) {
				target.AddComponent<Teleport> ();
				var teleComp = target.GetComponent<Teleport> ();
				teleComp.headset = headset;
				text.text = "Teleporting...";
				teleComp.destination = GameObject.Find ("SciRocksSphere");
			}
			if (id == targetPositions.Count - 2) {
				target.AddComponent<Teleport> ();
				var teleComp = target.GetComponent<Teleport> ();
				teleComp.headset = headset;
				text.text = "Teleporting...";
				teleComp.destination = GameObject.Find ("SciSextantsSphere");
			}
			if (id == targetPositions.Count - 1) {
				target.AddComponent<Teleport> ();
				var teleComp = target.GetComponent<Teleport> ();
				teleComp.headset = headset;
				text.text = "Teleporting...";
				teleComp.destination = GameObject.Find ("SciRocksSphere");
			}
			id++;
		}	
	}

	// Update is called once per frame
	void Update () {
		
	}

	//initalises the positions of the targets
	void initialiseTargetPositions() {
		// Practice target
		targetPositions.Add(new Vector3(0.0f, 0.0f, -370.0f));

        //top shelf (left to right)
		targetPositions.Add(new Vector3(-40.0f, -3.5f, 19.0f));
		targetPositions.Add(new Vector3(-32.0f, -3.5f, 19.0f));
		targetPositions.Add(new Vector3(-28.0f, -3.5f, 19.0f));
		targetPositions.Add(new Vector3(-24.0f, -4.5f, 19.0f));
        targetPositions.Add(new Vector3(-18.0f, -4.5f, 19.0f));
        targetPositions.Add(new Vector3(-13.0f, -5.5f, 19.5f));
        targetPositions.Add(new Vector3(-10.0f, -5.0f, 20.0f));
        targetPositions.Add(new Vector3(-5.0f, -5.0f, 20.0f));
		targetPositions.Add(new Vector3(1.0f, -5.0f, 19.0f));
        targetPositions.Add(new Vector3(10.0f, -5.0f, 20.0f));
        targetPositions.Add(new Vector3(15.0f, -5.0f, 20.0f));
        targetPositions.Add(new Vector3(22.0f, -6.0f, 20.0f));
        targetPositions.Add(new Vector3(28.0f, -6.5f, 18.5f));
        targetPositions.Add(new Vector3(31.5f, -5.5f, 14.0f));
        targetPositions.Add(new Vector3(32.5f, -3.5f, 9.0f));
		targetPositions.Add(new Vector3(39.0f, -3.0f, 8.0f));
		targetPositions.Add(new Vector3(46.0f, -3.0f, 7.0f));
		targetPositions.Add(new Vector3(43.0f, -1.5f, 3.0f));
		targetPositions.Add(new Vector3(48.0f, -1.5f, 2.0f));
        //middle shelf at the back (left to back)
		targetPositions.Add(new Vector3(-31.0f, -10.0f, 19.0f));
		targetPositions.Add(new Vector3(-26.0f, -12.0f, 19.0f));
        targetPositions.Add(new Vector3(-15.0f, -12.5f, 17.5f));
        targetPositions.Add(new Vector3(-10.0f, -13.5f, 19.5f));
        targetPositions.Add(new Vector3(-3.0f, -15.0f, 20.0f));
        targetPositions.Add(new Vector3(6.5f, -15.0f, 20.0f));
        targetPositions.Add(new Vector3(17.0f, -16.0f, 18.0f));
        targetPositions.Add(new Vector3(28.0f, -12.0f, 12.0f));
        targetPositions.Add(new Vector3(34.0f, -9.0f, 7.0f));
		targetPositions.Add(new Vector3(45.0f, -11.5f, 5.0f));
        //middle shelf at the front (left to right)
		targetPositions.Add(new Vector3(-28.0f, -8.0f, 10.0f));
		targetPositions.Add(new Vector3(-24.0f, -9.0f, 10.0f));
		targetPositions.Add(new Vector3(-16.0f, -10.0f, 10.0f));
		targetPositions.Add(new Vector3(-10.0f, -11.0f, 10.0f));
		targetPositions.Add(new Vector3(-4.0f, -12.0f, 10.0f));
		targetPositions.Add(new Vector3(3.0f, -13.0f, 10.0f));
		targetPositions.Add(new Vector3(13.0f, -15.0f, 10.0f));
		targetPositions.Add(new Vector3(24.0f, -15.0f, 8.0f));
		targetPositions.Add(new Vector3(34.0f, -13.0f, 5.0f));
		targetPositions.Add(new Vector3(43.0f, -13.0f, 3.0f));
		targetPositions.Add(new Vector3(48.0f, -11.5f, 1.0f));

        //bottom at the back (left to right)
        targetPositions.Add(new Vector3(0.5f, -22.5f, 14.0f));
        targetPositions.Add(new Vector3(11.0f, -22.0f, 12.0f));
        targetPositions.Add(new Vector3(16.5f, -22.0f, 11.0f));
        targetPositions.Add(new Vector3(21.0f, -20.0f, 9.0f));
        targetPositions.Add(new Vector3(26.0f, -18.0f, 6.5f));
		targetPositions.Add(new Vector3(31.0f, -18.0f, 6.0f));
		targetPositions.Add(new Vector3(38.0f, -20.0f, 6.0f));
		targetPositions.Add(new Vector3(49.0f, -21.0f, 4.0f));

        //bottom at the front (left to right)
        targetPositions.Add(new Vector3(-20.0f, -13.5f, 7.0f));
        targetPositions.Add(new Vector3(-16.0f, -16.0f, 8.0f));
        targetPositions.Add(new Vector3(-9.0f, -17.0f, 8.0f));
        targetPositions.Add(new Vector3(-6.0f, -18.5f, 8.0f));
        targetPositions.Add(new Vector3(-2.0f, -19.0f, 8.0f));
        targetPositions.Add(new Vector3(4.0f, -20.0f, 7.0f));
        targetPositions.Add(new Vector3(12.0f, -21.0f, 7.0f));
        targetPositions.Add(new Vector3(22.5f, -24.0f, 6.5f));
        targetPositions.Add(new Vector3(28.0f, -21.0f, 4.0f));
		targetPositions.Add(new Vector3(34.0f, -21.0f, 3.0f));
		targetPositions.Add(new Vector3(40.0f, -22.0f, 2.0f));
		targetPositions.Add(new Vector3(55.0f, -24.0f, 1.0f));
		targetPositions.Add(new Vector3(64.0f, -25.0f, 1.0f));


        //targetPositions.Add(new Vector3(35.0f, -16.0f, 10.0f));

        //behind cam
        //sample
        targetPositions.Add(new Vector3(8.0f, -1.0f, -40.0f));
        targetPositions.Add(new Vector3(11.0f, -0.5f, -38.0f));
        targetPositions.Add(new Vector3(16.0f, -0.5f, -38.0f));
        targetPositions.Add(new Vector3(21.0f, -1.0f, -38.0f));
		// Teleport to sextants
		targetPositions.Add(new Vector3(15.0f, 10.0f, -30.0f));
		// Teleport to rocks
		targetPositions.Add(new Vector3(-10.0f, 10.0f, -170.0f));


	}

	//initialises information about each target
		void initialiseTargetInfo() {
		targetInfo.Add("This is an example");
		targetInfo.Add("of adding information");
		targetInfo.Add("for targets.");
		targetInfo.Add("If the array list index");
		targetInfo.Add("matches the ID of the target");
		targetInfo.Add("information is assigned automatically!");
		targetInfo.Add("This is an example");
		targetInfo.Add("of adding information");
		targetInfo.Add("for targets.");
		targetInfo.Add("If the array list index");
		targetInfo.Add("matches the ID of the target");
		targetInfo.Add("information is assigned automatically!");
        targetInfo.Add("another 1");
        targetInfo.Add("another 1");
        targetInfo.Add("another 1");
        targetInfo.Add("another 1");
        targetInfo.Add("another 1");
        targetInfo.Add("another 1");
        targetInfo.Add("another 1");
        targetInfo.Add("another 1");
        targetInfo.Add("another 1");
		targetInfo.Add("another 1");
		targetInfo.Add("another 1");
		targetInfo.Add("another 1");
		targetInfo.Add("another 1");
		targetInfo.Add("another 1");
		targetInfo.Add("another 1");
		targetInfo.Add("another 1");
		targetInfo.Add("another 1");
		targetInfo.Add("another 1");
		targetInfo.Add("another 1");
		targetInfo.Add("another 1");
		targetInfo.Add("another 1");
		targetInfo.Add("another 1");
		targetInfo.Add("another 1");
		targetInfo.Add("another 1");
		targetInfo.Add("another 1");
        targetInfo.Add("another 1");
        targetInfo.Add("another 1");
        targetInfo.Add("another 1");
        targetInfo.Add("another 1");
        targetInfo.Add("another 1");
        targetInfo.Add("another 1");
        targetInfo.Add("another 1");
        targetInfo.Add("another 1");
		targetInfo.Add("another 1");
		targetInfo.Add("another 1");
		targetInfo.Add("another 1");
		targetInfo.Add("another 1");
		targetInfo.Add("another 1");
		targetInfo.Add("another 1");
		targetInfo.Add("another 1");
		targetInfo.Add("another 1");
		targetInfo.Add("another 1");
		targetInfo.Add("another 1");
		targetInfo.Add("another 1");
		targetInfo.Add("another 1");
		targetInfo.Add("another 1");
		targetInfo.Add("another 1");
		targetInfo.Add("another 1");
		targetInfo.Add("another 1");
		targetInfo.Add("another 1");
		targetInfo.Add("another 1");
		targetInfo.Add("another 1");
		targetInfo.Add("another 1");
		targetInfo.Add("another 1");
		targetInfo.Add("another 1");
		targetInfo.Add("another 1");
		targetInfo.Add("another 1");
		targetInfo.Add("another 1");
		targetInfo.Add("another 1");
		targetInfo.Add("another 1");
		targetInfo.Add("another 1");
		targetInfo.Add("another 1");
		targetInfo.Add("another 1");
		targetInfo.Add("another 1");
    }

	void initialisePopupMaterials () {
		Material popupMat1 = Resources.Load("PopupSphero", typeof(Material)) as Material;
		Material popupMat2 = Resources.Load("PopupConglomerate", typeof(Material)) as Material;
		Material popupMat3 = Resources.Load("PopupEruptive", typeof(Material)) as Material;
		Material popupMat4 = Resources.Load("PopupPitchstone", typeof(Material)) as Material;
		Material popupMat5 = Resources.Load("PopupSaltire", typeof(Material)) as Material;
		Material popupMat6 = Resources.Load("PopupSyenite", typeof(Material)) as Material;
		teleportMaterial = Resources.Load("Teleport", typeof(Material)) as Material;
		tutorialMaterial = Resources.Load("Tutorial", typeof(Material)) as Material;

		popupMaterials.Add (popupMat1);
		popupMaterials.Add (popupMat2);
		popupMaterials.Add (popupMat3);
		popupMaterials.Add (popupMat4);
		popupMaterials.Add (popupMat5);
		popupMaterials.Add (popupMat6);

	}

	public ArrayList getCubes() {
		return cubes;
	}

	//draw crosshair
	void OnGUI() {
		GUI.Box (new Rect (Screen.width / 2, Screen.height / 2, 10, 10), "");
	}
}