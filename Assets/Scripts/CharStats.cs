using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharStats : MonoBehaviour {

	public string charName;
	public int playerLevel = 1;
	public int currentEXP;
	public int[] expToNextLevel;
	public int maxLevel = 100;
	public int baseEXP = 1000;

	public int maxHP = 100;
	public int currentHP;
	public int maxMP = 30;
	public int[] mpLvlBonus;
	public int currentMP;
	public int strength;
	public int defence;
	public int weaponPower;
	public int armorPower;
	public string equippedWeapon = "None";
	public string equippedArmor = "None";
	// elements as per Avatar :D
	public int fireRes = 0;
	public int waterRes = 0;
	public int airRes = 0;
	public int earthres = 0;
	public Sprite charImage;


	// Use this for initialization
	void Start () {
		expToNextLevel = new int[maxLevel];
		expToNextLevel[1] = baseEXP;

		for(int i = 2; i < expToNextLevel.Length; i++) {
			expToNextLevel[i] = Mathf.FloorToInt(expToNextLevel[i - 1] * 1.05f);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void AddExp(int expToAdd) {
		currentEXP += expToAdd;

		if (playerLevel < maxLevel && currentEXP > expToNextLevel[playerLevel]) {
			// Level up!
			currentEXP -= expToNextLevel[playerLevel];
			playerLevel++;

			// determine whether to add to STR or DEF based on odd/even.
			if (playerLevel % 2 == 0) {
				strength++;
			} else {
				defence++;
			}

			maxHP = Mathf.FloorToInt(maxHP * 1.10f);
			maxMP += mpLvlBonus[playerLevel];
			
			// Recover health & MP fully on level up.
			currentHP = maxHP;
			currentMP = maxMP;
		}

		if (playerLevel >= maxLevel) {
			currentEXP = 0;
		}
	}
}
