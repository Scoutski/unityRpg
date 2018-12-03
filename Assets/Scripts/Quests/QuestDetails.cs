using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestDetails : MonoBehaviour {

	public GameObject questDetailsWindow;

	public Text questNameText;
	public Text questObjectiveText;
	public Text questDescriptionText;

	public static QuestDetails instance;

	void Start() {
		instance = this;
	}

	public void OpenQuestDetailsWindow() {
		if (!questDetailsWindow.gameObject.activeInHierarchy) {
			questDetailsWindow.SetActive(true);
		}
	}

	public void CloseQuestDetailsWindow() {
		if (questDetailsWindow.gameObject.activeInHierarchy) {
			questDetailsWindow.SetActive(false);
		}
	}

	public void	PopulateQuestDetails(string questName, string questObjective, string questDescription) {
		questNameText.text = "";
		questObjectiveText.text = "";
		questDescriptionText.text = "";
		OpenQuestDetailsWindow();
	}
}
