using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest {

	public enum questState { Undiscovered, Active, Completed, Failed }
	public questState questStatus = questState.Undiscovered;
	// public static int questIdCount = 0;
	// public int questId {get; private set;}
	public string name;
	public string objective;
	public string description;
	public int[] questRewardAmount;
	public string[] questRewardName;

	// public Quest() {
		// Give quests a unique ID int for the amount of quests that currently exist.
		// Not using GUIDs because I'm not sure if they're suitable yet
		// questId = questIdCount;
		// questIdCount++;
	// }

	public void UpdateQuestStatus(questState newStatus) {
		questStatus = newStatus;
	}
}
