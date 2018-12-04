using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour {

	public List<Quest> questsList;
	public List<Quest> archivedQuests;
	public string[] questMarkerNames;
	public bool[] questMarkersComplete;
	
	public static QuestManager instance;

	void Start () {
		instance = this;

		questMarkersComplete = new bool[questMarkerNames.Length];
	}

	void Update () {
		if (Input.GetKeyDown(KeyCode.J)) {
			Debug.Log("Pressed the J Key");
			// AddNewQuest("Smoke a joint", "Find and smoke a joint", "It will get you high");
			// ArchiveQuest("A world of pets!", Quest.questState.Failed);
		}
	}
	
	// public List<string> GetQuestList() {
	// 	List<string> questList = new List<string>();

	// 	// First quest in the list is blank
	// 	for (int i=1; i < questMarkerNames.Length; i++) {
	// 		questList.Add(questMarkerNames[i]);
	// 	}

	// 	return questList;
	// }

	public List<Quest> GetQuestList() {
		return questsList;
	}

	public int GetQuestPosition(string questToFind) {
		for (int i=0; i < questMarkerNames.Length; i++) {
			if (questMarkerNames[i] == questToFind) {
				return i;
			}
		}

		Debug.LogError("Unable to find quest with name: " + questToFind);
		return 0;
	}

	public bool IsQuestComplete(string questToCheck) {
		int questPosition = GetQuestPosition(questToCheck);
		return questMarkersComplete[questPosition];
	}

	public void MarkQuestComplete(string questToMark) {
		int questPosition = GetQuestPosition(questToMark);
		questMarkersComplete[questPosition] = true;

		UpdateLocalQuestObjects();
	}

	public void MarkQuestIncomplete(string questToMark) {
		int questPosition = GetQuestPosition(questToMark);
		questMarkersComplete[questPosition] = false;

		UpdateLocalQuestObjects();
	}

	public void UpdateLocalQuestObjects() {
		QuestObjectActivator[] questObjects = FindObjectsOfType<QuestObjectActivator>();

		if (questObjects.Length > 0) {
			for (int i = 0; i < questObjects.Length; i++) {
				questObjects[i].CheckCompletion();
			}
		}
	}

	public void AddNewQuest(string questName, string questObjective, string questDescription, int[] questRewardAmount, string[] questRewardName) {
		Quest quest = new Quest();
		quest.name = questName;
		quest.objective = questObjective;
		quest.description = questDescription;
		quest.questStatus = Quest.questState.Active;
		quest.questRewardAmount = questRewardAmount;
		quest.questRewardName = questRewardName;

		questsList.Add(quest);
	}

	public void ArchiveQuest(string questName, Quest.questState state) {
		for (int i = 0; i < questsList.Count; i++) {
			if (questsList[i].name == questName) {
				questsList[i].questStatus = state;

				if (questsList[i].questRewardAmount.Length > 0 && questsList[i].questStatus == Quest.questState.Completed) {
					GiveQuestRewards(questsList[i]);
				}

				archivedQuests.Add(questsList[i]);
				questsList.Remove(questsList[i]);
			}
		}
	}
	
	private void GiveQuestRewards(Quest quest) {
		for (int j = 0; j < quest.questRewardAmount.Length; j++) {
			if (quest.questRewardName[j] == "Gold") {
				GameManager.instance.currentGold += quest.questRewardAmount[j];
			} else {
				GameManager.instance.AddItem(quest.questRewardName[j], quest.questRewardAmount[j]);
			}
		}
	}

	public void SaveQuestData() {
		for (int i = 0; i < questMarkerNames.Length; i++) {
			if (questMarkersComplete[i]) {
				PlayerPrefs.SetInt("QuestMarker_" + questMarkerNames[i], 1);
			} else {
				PlayerPrefs.SetInt("QuestMarker_" + questMarkerNames[i], 0);
			}
		}
	}

	public void LoadQuestData() {
		for (int i = 0; i < questMarkerNames.Length; i++) {
			int valueToSet = 0;
			if (PlayerPrefs.HasKey("QuestMarker_" + questMarkerNames[i])) {
				valueToSet = PlayerPrefs.GetInt("QuestMarker_" + questMarkerNames[i]);
			}

			if (valueToSet == 0) {
				questMarkersComplete[i] = false;
			} else {
				questMarkersComplete[i] = true;
			}
		}
	}
}
