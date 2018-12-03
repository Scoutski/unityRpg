using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetHouseMenu : MonoBehaviour {

	private bool canBeOpened;
	private PetHouseMenu instance;

	// Use this for initialization
	void Start () {
		instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		if (canBeOpened && Input.GetButtonUp("Fire1") && !GameManager.instance.gameMenuOpen) {
			GameMenu.instance.OpenPetMenu();
		}
	}

	private void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Player") {
			canBeOpened = true;
		}
	}

	private void OnTriggerExit2D(Collider2D other) {
		if (other.tag == "Player") {
			canBeOpened = false;
		}
	}
}
