using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestMenu : MonoBehaviour {

	public GameObject questMenu;
	public Text questListingText;
	public GameObject questListContainer;
	public GameObject questDetails;

	public Text questNameText;
	public Text questObjectiveText;
	public Text questDescriptionText;

	private List<Quest> questList;
	private List<Text> questListText = new List<Text>();


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Q) && questMenu.activeInHierarchy) {
			CloseQuestMenu();
		} else if (Input.GetKeyDown(KeyCode.Q) && !questMenu.activeInHierarchy){
			OpenQuestMenu();
		}
	}

	public void OpenQuestMenu() {
		PopulateQuestList();
		GameManager.instance.gameMenuOpen = true;
		questMenu.SetActive(true);
	}

	public void CloseQuestMenu() {
		ClearQuestList();
		GameManager.instance.gameMenuOpen = false;
		questMenu.SetActive(false);
		CloseQuestDetailsWindow();
	}

	public void PopulateQuestList() {
		questList = QuestManager.instance.GetQuestList();
		for (int i=0; i < questList.Count; i++) {
			Text newQuest = Instantiate(questListingText, transform.position, transform.rotation);
			newQuest.text = "> " + questList[i].name;
			newQuest.transform.SetParent(questListContainer.transform, false);
			Button newButton = newQuest.GetComponent<Button>();
			newButton.onClick.AddListener(() => {
				if (questDetails.gameObject.activeInHierarchy) {
					CloseQuestDetailsWindow();
				} else {
					PopulateQuestDetails(
						questList[i - 1].name,
						questList[i - 1].objective,
						questList[i - 1].description
					);
					OpenQuestDetailsWindow();
				}
			});

			questListText.Add(newQuest);
		}
	}

	public void OpenQuestDetailsWindow() {
		questDetails.SetActive(true);
	}

	public void CloseQuestDetailsWindow() {
		if (questDetails.activeInHierarchy) {
			questDetails.SetActive(false);
		}
	}

	void PopulateQuestDetails(string questName, string questObjective, string questDescription) {
		questNameText.text = questName;
		questObjectiveText.text = questObjective;
		questDescriptionText.text = questDescription;
	}

	public void ClearQuestList() {
		for (int i=0; i < questListText.Count; i++) {
			Destroy(questListText[i].gameObject);
		}
		questListText.Clear();
	}
}
