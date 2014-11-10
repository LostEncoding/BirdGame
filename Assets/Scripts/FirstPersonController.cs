using UnityEngine;
using System.Collections;

public class FirstPersonController : MonoBehaviour {
	public float walkSpeed = 5f;

	//Flight and dive speed of a bald eagle. 'Murica.
	public float flightSpeed = 13.41112f; 
	public float diveSpeed = 44.704f;

	public float startAscentFlap = .2f;
	public float endAscentFlap = .3f;
	public float minAscent = 1f;
	public float maxAscent = 3f;

	public float flapUpSpeed = 2;
	public float flapDownSpeed = 2;
	public float flapRange = 30f;

	public float glideDurationMin = 5f;
	public float glideDurationMax = 15f;


	//AudioSource flapSound;
	//bool flapPlayedOnce = false;

	CharacterController characterController;

	bool inFlight = false;
	bool onGround = false;
	bool diving = false;

	float currentAscent = 0f;
	float ascentTimer;

	bool flappingUp = true;
	float currentFlapPosition = 0f;

	bool gliding = false;
	float glideDuration = 0f;
	float glideDurationTimer = 0f;

	// Use this for initialization
	void Start () {
		characterController = GetComponent<CharacterController> ();
		Physics.gravity = new Vector3 (0, -9.8f, 0);

//		flapSound = GetComponent<AudioSource> ();

		float glideDuration = Random.Range (glideDurationMin, glideDurationMax);

		ascentTimer = startAscentFlap;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey ("left shift") && Input.GetKeyDown ("space")) {
			inFlight = !inFlight;
		}

		Move ();
	}

	void Move(){
		if (!onGround && !inFlight) {
			diving = true;
		} else {
			diving = false;
		}

		Transform cameraTransform = transform.Find ("Main Camera");

		float rotationLeftRight = Input.GetAxis ("Mouse X");
		float rotationUpDown = -Input.GetAxis ("Mouse Y");

		if (cameraTransform != null) { 
			cameraTransform.Rotate (rotationUpDown, 0, 0);
		}

		transform.Rotate (0, rotationLeftRight, 0);

		if (inFlight) {
			FlyYouFools();
		} else if(diving){
			Dive();
		} else {
			Walk();
		}	
	}

	void FlyYouFools(){
		float forwardSpeed = Input.GetAxis ("Vertical");

		Ascend ();

		if(currentAscent == 0f){
			Glide (forwardSpeed);
			if(!gliding){
				Flap();
			}
		}

		Vector3 movement = new Vector3(0, currentAscent, forwardSpeed);
		movement = transform.rotation * movement * Time.deltaTime;
		
		characterController.Move(movement * flightSpeed);
		SetOnGround ();
		
		if(onGround){
			inFlight = false;
		}

	}

	void Ascend(){
		if(Input.GetKey ("space")){
			if( ascentTimer >= startAscentFlap ){
				if( ascentTimer >= endAscentFlap ){
					ascentTimer = 0f;
				}

				ascentTimer += Time.deltaTime;
				currentAscent = maxAscent;

				/*if(!flapPlayedOnce){
					flapSound.Play ();
					flapPlayedOnce = true;
				}*/
			} else {
				ascentTimer += Time.deltaTime;
				currentAscent = minAscent;
				//flapPlayedOnce = false;
			}
		} else {
//			flapPlayedOnce = false;
			ascentTimer = startAscentFlap;
			currentAscent = 0f;
		}
	}

	void Glide(float forwardSpeed){
		if(forwardSpeed != 0f){
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

	void Dive(){
		float movementX = 0;
		float movementY = -1;
		float movementZ = 1;

		Vector3 movement = new Vector3 (movementX, movementY, movementZ);
		movement = transform.rotation * movement * Time.deltaTime;

		characterController.Move(movement * diveSpeed);
		SetOnGround ();
		
		if(onGround){
			inFlight = false;
		}
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
