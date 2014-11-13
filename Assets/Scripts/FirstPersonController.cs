using UnityEngine;
using System.Collections;

public class FirstPersonController : MonoBehaviour {
	public float walkSpeed = 5f;

	//Flight and dive speed of a bald eagle. 'Murica.
	public float flightSpeed = 13.41112f; 
	public float diveSpeed = 44.704f; //Terminal velocity.

	public float flapUpSpeed = 1f;
	public float flapDownSpeed = 1.5f;
	public float flapRange = 20f;

	public float glideDurationMin = 4f;
	public float glideDurationMax = 8f;

	public float tiltZMax = 60f;
	public float tiltXMax = 85f;
	public float diveAfterTiltX = 60f;

	CharacterController characterController;

	bool inFlight = true;
	bool onGround = false;

	float doubleTapSpaceTime = 0f;

	float currentAscent = 0f;

	bool flappingUp = true;
	float currentFlapPosition = 0f;

	bool gliding = false;
	float glideDuration = 0f;
	float glideDurationTimer = 0f;

	float currentTiltZ = 0f;
	float timeSinceLastTiltZ = 0f;

	float currentTiltX = 0f;
	float timeDiving = 0f;
	float currentSpeed;
	float diveStopSpeed;

	Transform cameraTransform;

	void Start () {
		diveStopSpeed = (flightSpeed + (diveSpeed) / 20);
		Screen.showCursor = false;
		Screen.lockCursor = true;

		characterController = GetComponent<CharacterController> ();
		Physics.gravity = new Vector3 (0, -9.8f, 0);

		float glideDuration = Random.Range (glideDurationMin, glideDurationMax);
		currentSpeed = flightSpeed;
		cameraTransform = transform.Find ("Main Camera");
	}

	void Update () {
		bool doubleTapSpace = false;
				
		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (Time.time < doubleTapSpaceTime + .3f)
			{
				doubleTapSpace = true;
			}
			doubleTapSpaceTime = Time.time;
		}

		if (doubleTapSpace) {
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

		Ascend ();
		Tilt(forwardSpeed);	
		Glide (forwardSpeed);

		if(!gliding){
			Flap();
		}

		if (currentTiltX > diveAfterTiltX && forwardSpeed > 0f && currentAscent < 0f) {
			if (currentSpeed < diveSpeed) {
				currentSpeed += (-Physics.gravity.y) * timeDiving;
				timeDiving += Time.deltaTime;
				if (currentSpeed > diveSpeed) {
					currentSpeed = diveSpeed;
				}
			}
		} else {
			timeDiving = 0f;
			currentSpeed = Mathf.Lerp (currentSpeed, flightSpeed, Time.deltaTime);

			if(currentSpeed > diveStopSpeed){
				forwardSpeed = 1f;
			}
		}

		Vector3 movement = new Vector3(0, currentAscent, forwardSpeed);
		movement.Normalize ();
		movement = transform.rotation * movement * Time.deltaTime;

		characterController.Move(movement * currentSpeed);

		SetOnGround ();
		
		if(onGround){
			inFlight = false;
		}
	}

	void Ascend(){
		float forwardSpeed = Input.GetAxis ("Vertical");

		if(currentSpeed > diveStopSpeed){
			forwardSpeed = 1f;
		}

		if (!Input.GetKey ("left shift") && currentTiltX != 0f && forwardSpeed > 0f) {
			currentAscent = -(forwardSpeed * Mathf.Tan (Mathf.Deg2Rad * currentTiltX));			
		} else {
			currentAscent = 0f;
		}
	}

	void Glide(float forwardSpeed){
		if(forwardSpeed > 0f && currentTiltX > -15f){
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
		float mouseMovementX = -(Input.GetAxis ("Mouse X") / 4);	
		timeSinceLastTiltZ += Time.deltaTime;
		if (forwardSpeed > 0f && mouseMovementX != 0f) {
			timeSinceLastTiltZ = 0f;
			currentTiltZ += mouseMovementX;
			currentTiltZ = Mathf.Clamp (currentTiltZ, -tiltZMax, tiltZMax);
		} else if(timeSinceLastTiltZ > .2f){
			currentTiltZ = Mathf.Lerp(currentTiltZ, 0, Time.deltaTime);
		}

		cameraTransform.eulerAngles = new Vector3 (cameraTransform.eulerAngles.x, cameraTransform.eulerAngles.y, currentTiltZ); 
	}

	void Walk(){
		if (currentTiltZ != 0f) {
			currentTiltZ = Mathf.Lerp (currentTiltZ, 0, Time.deltaTime*3);
			cameraTransform.eulerAngles = new Vector3 (cameraTransform.eulerAngles.x, cameraTransform.eulerAngles.y, currentTiltZ);
		}
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
