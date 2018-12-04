using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogActivator : MonoBehaviour {

	// public string[] lines;
	private bool canActivate;
	public bool isPerson = true;
	public Sprite personSprite;

	[Header("Creating new Quest")]
	public bool shouldCreateQuest;
	public string newQuestName;
	public string questObjective;
	public string questDescription;
	public int[] questRewardAmount;
	public string[] questRewardName;

	[Header("Complete quests")]
	public bool shouldCompleteQuest;
	public string completeQuestName;
	public bool successfulCompletion;

	/*
	states:
	- Player does not have specific quest
	- Player has specific active quest
	- Player has finished specific quest

	Each state should have a different set of dialog associated and should 
	have different effects on quests based on the state
	*/
	[Header("State based dialog")]
	public string questName;
	public string[] noQuestLines;
	public string[] activeQuestLines;
	public string[] finishedQuestLines;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (canActivate && Input.GetButtonDown("Fire1") && !DialogManager.instance.dialogBox.activeInHierarchy) {
			bool hasActiveQuest = CheckForActiveQuest();
			bool hasFinishedQuest = CheckForFinishedQuest();
			bool noActiveQuest = !hasActiveQuest && !hasFinishedQuest;

			if (isPerson && personSprite) {
				DialogManager.instance.npcSprite = personSprite;
			}

			if (noActiveQuest) {
				DialogManager.instance.ShowDialog(noQuestLines, isPerson);
			} else if (hasActiveQuest) {
				DialogManager.instance.ShowDialog(activeQuestLines, isPerson);
			} else {
				DialogManager.instance.ShowDialog(finishedQuestLines, isPerson);
			}

			if (shouldCompleteQuest & hasActiveQuest) {
				// shouldCompleteQuest = false;
				Quest.questState state = Quest.questState.Failed;
				if (successfulCompletion) {
					state = Quest.questState.Completed;
				}
				DialogManager.instance.ShouldCompleteQuestAtEnd(completeQuestName, state);
			}

			if (shouldCreateQuest && noActiveQuest) {
				// shouldCreateQuest = false;
				DialogManager.instance.ShouldCreateQuestAtEnd(
					newQuestName,
					questObjective,
					questDescription,
					questRewardAmount,
					questRewardName
				);				
			}
		}
	}

	private bool CheckForActiveQuest() {
		bool flag = false;
		for (int i = 0; i < QuestManager.instance.questsList.Count; i++) {
			Debug.Log("QuestManager.instance.questsList[i].name: " + QuestManager.instance.questsList[i].name);

			if (QuestManager.instance.questsList[i].name == questName) {
				flag = true;
				i = QuestManager.instance.questsList.Count;
			}
		}

		return flag;
	}

	private bool CheckForFinishedQuest() {
		bool flag = false;
		for (int i = 0; i < QuestManager.instance.archivedQuests.Count; i++) {
			if (QuestManager.instance.archivedQuests[i].name == questName) {
				flag = true;
				i = QuestManager.instance.archivedQuests.Count;
			}
		}

		return flag;
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

