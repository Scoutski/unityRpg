using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssentialsLoader : MonoBehaviour {

	public GameObject UIScreen;
	public GameObject player;
	public GameObject gameManager;
	public GameObject audioManager;
	public GameObject battleManager;

	// Use this for initialization
	void Start () {
		if (UIFade.instance == null) {
			UIFade.instance = Instantiate(UIScreen).GetComponent<UIFade>();
		}

		if (PlayerController.instance == null) {
			PlayerController clone = Instantiate(player).GetComponent<PlayerController>();
			PlayerController.instance = clone;
		}

		if (!GameManager.instance) {
			Instantiate(gameManager);
		}

		if (!AudioManager.instance) {
			Instantiate(audioManager);
		}

		if (!BattleManager.instance) {
			Instantiate(battleManager);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
