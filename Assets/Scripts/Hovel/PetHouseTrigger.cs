using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetHouseTrigger : MonoBehaviour {

	public GameObject petHouseHightlight;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Player") {
			petHouseHightlight.SetActive(true);
		}
	}

	private void OnTriggerExit2D(Collider2D other) {
		if (other.tag == "Player") {
			petHouseHightlight.SetActive(false);
		}
	}
}
