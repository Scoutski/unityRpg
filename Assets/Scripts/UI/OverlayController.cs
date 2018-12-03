using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void PressMenuButton() {
		if (GameMenu.instance.theMenu.activeInHierarchy) {
			GameMenu.instance.CloseMenu();
		} else {
			GameMenu.instance.theMenu.SetActive(true);
			GameMenu.instance.UpdateMainStats();
			GameManager.instance.gameMenuOpen = true;
		}

		AudioManager.instance.PlaySFX(5);
	}
}
