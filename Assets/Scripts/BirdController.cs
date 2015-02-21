using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BirdController : MonoBehaviour {
	public float walkSpeed = 5f;
	
	//Flight and dive speed of a bald eagle. 'Murica.
	public float flightSpeed = 13.41112f; 
	public float diveSpeed = 44.704f; //Terminal velocity.
	public Slider meter;
	public static int keyCount = 0;

	public float flapUpSpeed = 8f;
	public float flapDownSpeed = 1.5f;
	public float flapRange = 20f;
	
	public float glideDurationMin = 4f;
	public float glideDurationMax = 8f;
	
	public float tiltZMax = 60f;
	public float tiltXMax = 85f;
	public float diveAfterTiltX = 60f;
	
	bool inFlight = true;
	
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
		
		float glideDuration = Random.Range (glideDurationMin, glideDurationMax);
		currentSpeed = flightSpeed;
		cameraTransform = transform.Find ("Main Camera");
	}

	void Update() {
		LookAround ();
	}

	void LookAround(){
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
	}

	void FixedUpdate () {
		FlyYouFools ();
	}
	
	void FlyYouFools(){
		if (Input.GetKeyDown (KeyCode.Space)) {
			meter.value += .11f;
			rigidbody.AddForce(new Vector3(0, flapUpSpeed, 0), ForceMode.Impulse);
		}else{
			meter.value -= .006f;
		}

		float forwardSpeed = Input.GetAxis ("Vertical");

		Tilt(forwardSpeed);	
		
		Vector3 movement = new Vector3(0, 0, forwardSpeed);
		movement = movement * Time.deltaTime;
		
		transform.Translate(movement * flightSpeed);	
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
}
