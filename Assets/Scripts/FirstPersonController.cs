using UnityEngine;
using System.Collections;

public class FirstPersonController : MonoBehaviour {
	public float walkSpeed = 5f;

	//Flight speed of a bald eagle. 'Murica.
	public float flightSpeed = 13.41112f; 

	public float flapUpSpeed = 2;
	public float flapDownSpeed = 2;
	public float flapRange = 30f;

	public float glideDurationMin = 5f;
	public float glideDurationMax = 15f;

	public float tiltZMax = 60f;
	public float tiltXMax = 85f;


	//AudioSource flapSound;
	//bool flapPlayedOnce = false;

	CharacterController characterController;

	bool inFlight = true;
	bool onGround = false;

	float currentAscent = 0f;
	float ascentTimer;

	bool flappingUp = true;
	float currentFlapPosition = 0f;

	bool gliding = false;
	float glideDuration = 0f;
	float glideDurationTimer = 0f;

	float currentTiltZ = 0f;
	float currentTiltX = 0f;

	Transform cameraTransform;

	// Use this for initialization
	void Start () {
		Screen.showCursor = false;
		Screen.lockCursor = true;

		characterController = GetComponent<CharacterController> ();
		Physics.gravity = new Vector3 (0, -9.8f, 0);

//		flapSound = GetComponent<AudioSource> ();

		float glideDuration = Random.Range (glideDurationMin, glideDurationMax);

		cameraTransform = transform.Find ("Main Camera");
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey ("left shift") && Input.GetKeyDown ("space")) {
			inFlight = !inFlight;
		}

		Move ();
	}

	void Move(){
		float rotationLeftRight = Input.GetAxis ("Mouse X");
		float rotationUpDown = -Input.GetAxis ("Mouse Y");

		if (cameraTransform != null) { 
			if (rotationUpDown != 0f) {
				currentTiltX += rotationUpDown;
				currentTiltX = Mathf.Clamp (currentTiltX, -tiltXMax, tiltXMax);
			}
			
			cameraTransform.eulerAngles = new Vector3 (currentTiltX, cameraTransform.eulerAngles.y, cameraTransform.eulerAngles.z);
		}

		transform.Rotate (0, rotationLeftRight, 0);

		if (inFlight) {
			FlyYouFools();
		} else {
			Walk();
		}	
	}

	void FlyYouFools(){
		float forwardSpeed = Input.GetAxis ("Vertical");

		Glide (forwardSpeed);
		if(!gliding){
			Flap();
		}

		Ascend ();
		
		Tilt(forwardSpeed);	

		Vector3 movement = new Vector3(0, currentAscent, forwardSpeed);
		movement = transform.rotation * movement * Time.deltaTime;
		
		characterController.Move(movement * flightSpeed);
		SetOnGround ();
		
		if(onGround){
			inFlight = false;
		}
	}

	void Ascend(){
		float forwardSpeed = Input.GetAxis ("Vertical");

		if (Input.GetKey ("space")) {
			currentAscent = 1f;
		} else if(Input.GetKey ("left shift") && currentTiltX != 0f){
			if( forwardSpeed > 0f ){
				currentAscent = -(forwardSpeed * Mathf.Tan(Mathf.Deg2Rad * currentTiltX));
			} else if ( forwardSpeed == 0f ){
				currentAscent = 1f;
			}
		}
	}

	void Glide(float forwardSpeed){
		if(forwardSpeed > 0f){
			glideDurationTimer += Time.deltaTime;

			if(glideDurationTimer >= glideDuration){
				glideDurationTimer = 0f;
				glideDuration = Random.Range (glideDurationMin, glideDurationMax);
				gliding = !gliding;
			}
		} else {
			gliding = false;
			glideDurationTimer = 0f;
			glideDuration = Random.Range (glideDurationMin, glideDurationMax);
		}
	}

	void Flap() {
		Vector3 movement;
		if(flappingUp){
			currentFlapPosition += flapUpSpeed;
			movement = new Vector3(0, flapUpSpeed, 0);
			
			if(currentFlapPosition >= flapRange){
				flappingUp = false;
			}
		} else {
			currentFlapPosition -= flapDownSpeed;
			movement = new Vector3(0, -flapDownSpeed, 0);
			
			if(currentFlapPosition <= 0f){
				flappingUp = true;
			}
		}
		
		characterController.Move (movement * Time.deltaTime);
	}

	void Tilt(float forwardSpeed){
		float mouseMovementX = -(Input.GetAxis ("Mouse X") / 2);	

		if (forwardSpeed > 0f && mouseMovementX != 0f) {
			currentTiltZ += mouseMovementX;
			currentTiltZ = Mathf.Clamp (currentTiltZ, -tiltZMax, tiltZMax);
		} else {
			currentTiltZ = Mathf.Lerp(currentTiltZ, 0, Time.deltaTime);
		}

		cameraTransform.eulerAngles = new Vector3 (cameraTransform.eulerAngles.x, cameraTransform.eulerAngles.y, currentTiltZ); 
	}

	void Walk(){
		float sideSpeed = Input.GetAxis ("Horizontal");
		float forwardSpeed = Input.GetAxis ("Vertical");
		
		Vector3 movement = new Vector3 (sideSpeed, 0, forwardSpeed);
		
		movement = transform.rotation * movement;
		characterController.SimpleMove (movement * walkSpeed);
		SetOnGround ();
	}

	void SetOnGround(){
		onGround = ((characterController.collisionFlags & CollisionFlags.Below) != 0) || ((characterController.collisionFlags & CollisionFlags.Sides) != 0);
	}
}
