using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitController : MonoBehaviour
{
	public bool triggered;
	
	void Start(){
		triggered = false;
	}
	
    private void OnTriggerEnter(Collider collider){
		
		if(collider.gameObject.tag == "Player" && (!triggered)){
			GameController.mazeCompleted();
			triggered = true;
		}
	}
}
