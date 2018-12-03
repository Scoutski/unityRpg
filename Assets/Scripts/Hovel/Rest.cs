using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rest : MonoBehaviour {

	private bool canActivate;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (canActivate && Input.GetButtonDown("Fire1")) {
			Debug.Log("You got in bed!");
			RecoverStats();
		}
	}

	private void RecoverStats () {
		for (int i = 0; i < GameManager.instance.playerStats.Length-1; i++) {
			if (GameManager.instance.playerStats[i].isActiveAndEnabled) {
				GameManager.instance.playerStats[i].currentHP = GameManager.instance.playerStats[i].maxHP;
				GameManager.instance.playerStats[i].currentMP = GameManager.instance.playerStats[i].maxMP;
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Player") {
			canActivate = true;
		}
	}

	private void OnTriggerExit2D(Collider2D other) {
		if (other.tag == "Player") {
			canActivate = false;
		}
	}
}
