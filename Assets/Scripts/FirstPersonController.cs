using UnityEngine;
using System.Collections;

public class FirstPersonController : MonoBehaviour {
	public float movementSpeed = 5f;

	CharacterController characterController;
	bool inFlight;
	bool onGround = false;
	float currentAscent;

	// Use this for initialization
	void Start () {
		characterController = GetComponent<CharacterController> ();
		inFlight = false;
		currentAscent = 0f;

		Physics.gravity = new Vector3 (0, -9.8f, 0);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey ("left shift") && Input.GetKeyDown ("space")) {
			inFlight = !inFlight;
		}

		Move ();
	}

	void Move(){
		Vector3 movement;
		float sideSpeed = Input.GetAxis ("Horizontal");
		float rotationLeftRight = Input.GetAxis ("Mouse X");
		transform.Rotate (0, rotationLeftRight, 0);

		if (inFlight) {
			if(Input.GetKey ("space")){
				currentAscent = 1f;
			} else {
				currentAscent = 0f;
			}
			
			movement = new Vector3(0, currentAscent, 1);
			movement = transform.rotation * movement;

			characterController.Move(movement);		
		} else {
			float forwardSpeed = Input.GetAxis ("Vertical");

			movement = new Vector3 (sideSpeed, 0, forwardSpeed);

			movement = transform.rotation * movement;
			onGround = characterController.SimpleMove (movement * movementSpeed);
		}
	}
}
