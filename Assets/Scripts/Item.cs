using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

	[Header("Item Type")]
	public bool isItem;
	public bool isWeapon;
	public bool isArmor;

	[Header("Item Details")]
	public string itemName;
	public string description;
	public int value;
	public Sprite itemSprite;

	// Restoration/weaponPower/armorPower;
	[Header("Item Effects")]
	public int amountToChange;
	public bool affectHP, affectMP, affectStr, affectDef;

	[Header("Weapon/Armor Details")]
	public int weaponPower;
	public int armorPower;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Use(int charToUseOn) {
		CharStats selectedChar = GameManager.instance.playerStats[charToUseOn];

		if (isItem) {
			// Use item sound;
			AudioManager.instance.PlaySFX(7);
			if (affectHP) {
				selectedChar.currentHP += amountToChange;

				if (selectedChar.currentHP > selectedChar.maxHP) {
					selectedChar.currentHP = selectedChar.maxHP;
				}
			}

			if (affectMP) {
				selectedChar.currentMP += amountToChange;

				if (selectedChar.currentMP > selectedChar.maxMP) {
					selectedChar.currentMP = selectedChar.maxMP;
				}
			}

			if (affectStr) {
				selectedChar.strength += amountToChange;
			}

			if (affectDef) {
				selectedChar.defence += amountToChange;
			}
		}

		if (isWeapon || isArmor) {
			// Equip sound
			AudioManager.instance.PlaySFX(1);
		}

		if (isWeapon) {
			if (selectedChar.equippedWeapon != "") {
				GameManager.instance.AddItem(selectedChar.equippedWeapon);
			}

			selectedChar.equippedWeapon = itemName;
			selectedChar.weaponPower = weaponPower;
		}

		if (isArmor) {
			if (selectedChar.equippedArmor != "") {
				GameManager.instance.AddItem(selectedChar.equippedArmor);
			}

			selectedChar.equippedArmor = itemName;
			selectedChar.armorPower = armorPower;
		}

		GameManager.instance.RemoveItem(itemName);
	}
}
