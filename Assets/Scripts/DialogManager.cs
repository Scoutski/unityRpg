using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour {

	public Text dialogText;
	public Text nameText;
	public GameObject dialogBox;
	public GameObject nameBox;

	public string[] dialogLines;
	public int currentLine;
	private bool justStarted;

	private string questToMark;
	private bool markQuestComplete;
	private bool shouldMarkQuest;
	private Quest.questState questState;

	private bool shouldCreateQuest;
	private string questName;
	private string questObjective;
	private string questDescription;


	public static DialogManager instance;

	// Use this for initialization
	void Start () {
		instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		if (dialogBox.activeInHierarchy) {
			if (Input.GetButtonUp("Fire1")) {
				if (!justStarted) {
					currentLine++;

					if (currentLine >= dialogLines.Length) {
						dialogBox.SetActive(false);
						GameManager.instance.dialogActive = false;

						if (shouldMarkQuest) {
							shouldMarkQuest = false;
							QuestManager.instance.ArchiveQuest(questToMark, questState);
							questToMark = "";
							questState = Quest.questState.Undiscovered;
						}

						if (shouldCreateQuest) {
							shouldCreateQuest = false;
							QuestManager.instance.AddNewQuest(questName, questObjective, questDescription);
							questName = "";
							questObjective = "";
							questDescription = "";
						}
					} else {
						CheckIfName();
						dialogText.text = dialogLines[currentLine];
					}
				} else {
					justStarted = false;
				}
			}
		}
	}

	public void ShowDialog(string[] newLines, bool isPerson) {
		dialogLines = newLines;
		currentLine = 0;

		CheckIfName();

		dialogText.text = dialogLines[currentLine];
		dialogBox.SetActive(true);

		justStarted = true;

		nameBox.SetActive(isPerson);
		GameManager.instance.dialogActive = true;
	}

	public void CheckIfName() {
		if (dialogLines[currentLine].StartsWith("n-")) {
			nameText.text = dialogLines[currentLine].Replace("n-", "");
			currentLine++;
		}
	}

	public void ShouldActivateQuestAtEnd(string questName, bool markComplete) {
		questToMark = questName;
		markQuestComplete = markComplete;

		shouldMarkQuest = true;
	}

	public void ShouldCompleteQuestAtEnd(string questName, Quest.questState state) {
		questToMark = questName;
		questState = state;
		shouldMarkQuest = true;
	}

	public void ShouldCreateQuestAtEnd(string name, string objective, string description) {
		questName = name;
		questObjective = objective;
		questDescription = description;

		shouldCreateQuest = true;
	}
}
