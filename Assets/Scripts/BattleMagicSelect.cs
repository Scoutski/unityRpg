using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleMagicSelect : MonoBehaviour {

	public string spellName;
	public int spellCost;
	public Text nameText;
	public Text nameCost;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Press() {
		int currentMP = BattleManager.instance.activeBattlers[BattleManager.instance.currentTurn].currentMP;

		Debug.Log("spellcost: " + spellCost);
		if (currentMP >= spellCost) {
			BattleManager.instance.magicMenu.SetActive(false);
			BattleManager.instance.OpenTargetMenu(spellName);
			BattleManager.instance.activeBattlers[BattleManager.instance.currentTurn].currentMP -= spellCost;
		} else {
			// Let player know there isn't enough MP.
			BattleManager.instance.battleNotice.notificationText.text = "Not Enough MP!";
			BattleManager.instance.battleNotice.Activate();
			BattleManager.instance.magicMenu.SetActive(false);
		}
	}
}
