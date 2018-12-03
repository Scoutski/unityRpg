using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour {

	// public Text dialogText;
	public bool playerSpeaking;
	public Text npcText;
	public Text playerText;
	public Image playerImage;
	public Sprite playerSprite;
	public Image npcImage;
	public Sprite npcSprite;

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
						npcSprite = null;
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
						RenderDialog();
					}
				} else {
					justStarted = false;
				}
			}
		}
	}

	private void RenderDialog() {
		if (playerSpeaking) {
			playerImage.gameObject.SetActive(true);
			npcImage.gameObject.SetActive(false);
			playerImage.sprite = playerSprite;
			npcImage.sprite = null;
			playerText.text = dialogLines[currentLine];
			npcText.text = "";
		} else {
			npcImage.gameObject.SetActive(true);
			playerImage.gameObject.SetActive(false);
			if (npcSprite) {
				npcImage.sprite = npcSprite;
			}
			playerImage.sprite = null;
			npcText.text = dialogLines[currentLine];
			playerText.text = "";
		}
	}

	public void ShowDialog(string[] newLines, bool isPerson) {
		dialogLines = newLines;
		currentLine = 0;

		CheckIfName();

		RenderDialog();
		dialogBox.SetActive(true);

		justStarted = true;

		nameBox.SetActive(isPerson);
		GameManager.instance.dialogActive = true;
	}

	public void CheckIfName() {
		if (dialogLines[currentLine].StartsWith("n-")) {
			if (dialogLines[currentLine] == "n-Player") {
				playerSpeaking = true;
			} else {
				playerSpeaking = false;
			}
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
