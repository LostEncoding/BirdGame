using UnityEngine;
using System.Collections;

public class openDoor : MonoBehaviour {
	private GameObject player;
	private bool playerIsCloseEnoughToPickUpThisObjectSoWeWillShowTheText_E;//<---wtf?
	public GameObject top,right,left,bottom;
	private bool doorUnlocked;
	
	void Start () {
		doorUnlocked = false;
		player = GameObject.FindWithTag("Player");
		playerIsCloseEnoughToPickUpThisObjectSoWeWillShowTheText_E = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(Vector3.Distance(player.gameObject.transform.position,transform.position) <= 7 && BirdController.keyCount > 0){
			playerIsCloseEnoughToPickUpThisObjectSoWeWillShowTheText_E = true;
			if(Input.GetKeyUp(KeyCode.E)){
				doorUnlocked = true;
			}
		}else{
			playerIsCloseEnoughToPickUpThisObjectSoWeWillShowTheText_E = false;
		}
		if(doorUnlocked)
			openDoorAnimation();
	}
	
	void OnGUI(){
		if(playerIsCloseEnoughToPickUpThisObjectSoWeWillShowTheText_E){
			GUI.color = Color.yellow;
			GUI.Label(new Rect((Screen.width/2)-40,(Screen.height/2)+Screen.height/4,40,50), "<size=40>E</size>" );
		}
	}
	float i = 0;
	void openDoorAnimation(){
		if(i <= .07){
			top.transform.Translate(i,i,0);
			right.transform.Translate(-i,i,0);
			bottom.transform.Translate(-i,-i,0);
			left.transform.Translate(i,-i,0);
			i+=.001f;
		}else{
			BirdController.keyCount--;
		}
	}
}
