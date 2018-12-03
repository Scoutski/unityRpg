using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestObjectActivator : MonoBehaviour {

	public GameObject objectToActivate;
	public string questToCheck;
	public bool activeIfComplete;

	private bool initialCheckDone;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (!initialCheckDone) {
			initialCheckDone = true;
			CheckCompletion();
		}
	}

	public void CheckCompletion() {
		Debug.Log("Should the cave be open?");
		if (QuestManager.instance.IsQuestComplete(questToCheck)) {
			Debug.Log("Setting the object to... " + activeIfComplete);
			objectToActivate.SetActive(activeIfComplete);
		}
	}
}
