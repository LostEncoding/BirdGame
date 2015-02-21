using UnityEngine;
using System.Collections;

public class isKeyObject : MonoBehaviour {
	private GameObject player;
	private bool playerIsCloseEnoughToPickUpThisObjectSoWeWillShowTheText_Press_E_to_pickup;//<---wtf?
	// Use this for initialization
	void Start () {
		player = GameObject.FindWithTag("Player");
		playerIsCloseEnoughToPickUpThisObjectSoWeWillShowTheText_Press_E_to_pickup = false;
		print(player.name);
	}
	
	// Update is called once per frame
	void Update () {
		if(Vector3.Distance(player.gameObject.transform.position,transform.position) <= 2){
			playerIsCloseEnoughToPickUpThisObjectSoWeWillShowTheText_Press_E_to_pickup = true;
			if(Input.GetKeyUp(KeyCode.E)){
				onKeyPickup();
			}
		}else{
			playerIsCloseEnoughToPickUpThisObjectSoWeWillShowTheText_Press_E_to_pickup = false;
		}
	}

	void OnGUI(){
		if(playerIsCloseEnoughToPickUpThisObjectSoWeWillShowTheText_Press_E_to_pickup){
			GUI.color = Color.yellow;
			GUI.Label(new Rect((Screen.width/2)-40,(Screen.height/2)+Screen.height/4,40,50), "<size=40>E</size>" );
		}
	}

	void onKeyPickup(){
		Destroy(this.gameObject);
		BirdController.keyCount++;
	}
}
