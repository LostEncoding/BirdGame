using UnityEngine;
using System.Collections;

public class IsDoorObject : MonoBehaviour {
	private GameObject player;
	private bool playerIsCloseEnoughToPickUpThisObjectSoWeWillShowTheText_E;//<---wtf?

	void Start () {
		player = GameObject.FindWithTag("Player");
		playerIsCloseEnoughToPickUpThisObjectSoWeWillShowTheText_E = false;
		print(player.name);
	}
	
	// Update is called once per frame
	void Update () {
		if(Vector3.Distance(player.gameObject.transform.position,transform.position) <= 2 && BirdController.keyCount > 0){
			playerIsCloseEnoughToPickUpThisObjectSoWeWillShowTheText_E = true;
			if(Input.GetKeyUp(KeyCode.E)){
				openDoor();
			}
		}else{
			playerIsCloseEnoughToPickUpThisObjectSoWeWillShowTheText_E = false;
		}
	}
	
	void OnGUI(){
		if(playerIsCloseEnoughToPickUpThisObjectSoWeWillShowTheText_E){
			GUI.color = Color.yellow;
			GUI.Label(new Rect((Screen.width/2)-40,(Screen.height/2)+Screen.height/4,40,50), "<size=40>E</size>" );
		}
	}
	
	void openDoor(){
		Destroy(this.gameObject);
		BirdController.keyCount--;
	}
}
