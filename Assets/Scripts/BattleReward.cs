using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleReward : MonoBehaviour {

	public static BattleReward instance;
	public Text expText, itemText;
	public GameObject rewardScreen;

	public string[] rewardItems;
	public int rewardExp;

	public bool markQuestComplete;
	public string questToMark;

	// Use this for initialization
	void Start () {
		instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Y)) {
			OpenRewardScreen(new string[] { "Iron sword", "Health Potion" }, 50);
		}
	}

	public void OpenRewardScreen(string[] items, int exp) {
		rewardItems = items;
		rewardExp = exp;

		expText.text = "Everyone earned " + rewardExp + " xp!";
		itemText.text = "";

		for (int i = 0; i < rewardItems.Length; i++) {
			itemText.text += items[i] + "\n";
		}

		rewardScreen.SetActive(true);
	}

	public void CloseRewardScreen() {
		for (int i = 0; i < GameManager.instance.playerStats.Length; i++) {
			if (GameManager.instance.playerStats[i].gameObject.activeInHierarchy) {
				GameManager.instance.playerStats[i].AddExp(rewardExp);
			}
		}

		for (int i = 0; i < rewardItems.Length; i++) {
			GameManager.instance.AddItem(rewardItems[i]);
		}

		rewardScreen.SetActive(false);
		GameManager.instance.battleActive = false;

		if (markQuestComplete) {
			QuestManager.instance.MarkQuestComplete(questToMark);
			markQuestComplete = false;
			questToMark = "";
		}
	}
}
