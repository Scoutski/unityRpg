using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public static GameManager instance;
	public CharStats[] playerStats;

	[Header("Movement constraints")]
	public bool gameMenuOpen;
	public bool dialogActive;
	public bool fadingBetweenAreas;
	public bool shopActive;
	public bool battleActive;

	[Header("Held Items")]
	public string[] itemsHeld;
	public int[] numberOfItems;
	public Item[] referenceItems;

	public int currentGold = 0;

	// Use this for initialization
	void Start () {
		instance = this;

		DontDestroyOnLoad(gameObject);

		SortItems();
	}
	
	// Update is called once per frame
	void Update () {
		if (gameMenuOpen || dialogActive || fadingBetweenAreas || shopActive || battleActive) {
			PlayerController.instance.canMove = false;
		} else {
			PlayerController.instance.canMove = true;
		}

		if (Input.GetKeyDown(KeyCode.I)) {
			SaveData();
		}

		if (Input.GetKeyDown(KeyCode.O)) {
			LoadData();
		}
	}

	public Item GetItemDetails(string itemToGrab) {
		for (int i = 0; i < referenceItems.Length; i++) {
			if (referenceItems[i].itemName == itemToGrab) {
				return referenceItems[i];
			}
		}

		return null;
	}

	public void SortItems() {
		bool itemAfterSpace = true;

		while(itemAfterSpace) {
			itemAfterSpace = false;
			for (int i = 0; i < itemsHeld.Length - 1; i++) {
				if (itemsHeld[i] == "") {
					itemsHeld[i] = itemsHeld[i + 1];
					itemsHeld[i + 1] = "";

					numberOfItems[i] = numberOfItems[i + 1];
					numberOfItems[i + 1] = 0;

					if (itemsHeld[i] != "") {
						itemAfterSpace = true;
					}
				}
			}
		}
	}

	public void AddItem(string itemToAdd) {
		int itemPosition = 0;
		bool foundSpace = false;

		for (int i = 0; i < itemsHeld.Length; i++) {
			// Will only work assuming items are already sorted;
			if (itemsHeld[i] == "" || itemsHeld[i] == itemToAdd) {
				itemPosition = i;
				i = itemsHeld.Length;
				foundSpace = true;
			}
		}

		if (foundSpace) {
			bool itemExists = false;
			for (int i = 0; i < referenceItems.Length; i++) {
				if (referenceItems[i].itemName == itemToAdd) {
					itemExists = true;
					i = referenceItems.Length;
				}
			}

			if (itemExists) {
				itemsHeld[itemPosition] = itemToAdd;
				numberOfItems[itemPosition]++;
			} else {
				Debug.LogError("Tried to add item that doesn't exist: " + itemToAdd);
			}
		}

		GameMenu.instance.ShowItems();
		SortItems();
	}

	public void RemoveItem(string itemToRemove) {
		bool foundItem = false;
		int itemPosition = 0;

		for (int i = 0; i < itemsHeld.Length; i++) {
			if (itemsHeld[i] == itemToRemove) {
				foundItem = true;
				itemPosition = i;

				i = itemsHeld.Length;
			}
		}

		if (foundItem) {
			numberOfItems[itemPosition]--;
			if (numberOfItems[itemPosition] <= 0) {
				itemsHeld[itemPosition] = "";
			}

			GameMenu.instance.ShowItems();

		} else {
			Debug.LogError("Unable to find item to remove: " + itemToRemove);
		}
	}

	public void SaveData() {
		// Current Scene + Player Location
		PlayerPrefs.SetString("Current_Scene", SceneManager.GetActiveScene().name);
		PlayerPrefs.SetFloat("Player_Position_x", PlayerController.instance.transform.position.x);
		PlayerPrefs.SetFloat("Player_Position_y", PlayerController.instance.transform.position.y);
		PlayerPrefs.SetFloat("Player_Position_z", PlayerController.instance.transform.position.z);

		// Character Info (stats)
		for (int i = 0; i < playerStats.Length; i++) {
			if (playerStats[i].gameObject.activeInHierarchy) {
				PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_active", 1);
			} else {
				PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_active", 0);
			}

			PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_level", playerStats[i].playerLevel);
			PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_currentExp", playerStats[i].currentEXP);
			PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_currentHp", playerStats[i].currentHP);
			PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_maxHp", playerStats[i].maxHP);
			PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_currentMp", playerStats[i].currentMP);
			PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_maxMp", playerStats[i].maxMP);
			PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_strength", playerStats[i].strength);
			PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_defence", playerStats[i].defence);
			PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_weaponPower", playerStats[i].weaponPower);
			PlayerPrefs.SetInt("Player_" + playerStats[i].charName + "_armorPower", playerStats[i].armorPower);
			PlayerPrefs.SetString("Player_" + playerStats[i].charName + "_equippedWeapon", playerStats[i].equippedWeapon);
			PlayerPrefs.SetString("Player_" + playerStats[i].charName + "_equippedArmor", playerStats[i].equippedArmor);
		}

		// Inventory
		for (int i = 0; i < itemsHeld.Length; i++) {
			PlayerPrefs.SetString("ItemInInventory_" + i, itemsHeld[i]);
			PlayerPrefs.SetInt("ItemAmount_" + i, numberOfItems[i]);
		}
	}

	public void LoadData() {
		// Set the player position to the last position
		PlayerController.instance.transform.position = new Vector3(
			PlayerPrefs.GetFloat("Player_Position_x"),
			PlayerPrefs.GetFloat("Player_Position_y"),
			PlayerPrefs.GetFloat("Player_Position_z")
		);

		for (int i = 0; i < playerStats.Length; i++) {
			if (PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_active") == 0) {
				playerStats[i].gameObject.SetActive(false);
			} else {
				playerStats[i].gameObject.SetActive(true);
			}

			playerStats[i].playerLevel = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_level");
			playerStats[i].currentEXP = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_currentExp");
			playerStats[i].currentHP = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_currentHp");
			playerStats[i].maxHP = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_maxHp");
			playerStats[i].currentMP = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_currentMp");
			playerStats[i].maxMP = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_maxMp");
			playerStats[i].strength = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_strength");
			playerStats[i].defence = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_defence");
			playerStats[i].weaponPower = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_weaponPower");
			playerStats[i].armorPower = PlayerPrefs.GetInt("Player_" + playerStats[i].charName + "_armorPower");
			playerStats[i].equippedWeapon = PlayerPrefs.GetString("Player_" + playerStats[i].charName + "_equippedWeapon");
			playerStats[i].equippedArmor = PlayerPrefs.GetString("Player_" + playerStats[i].charName + "_equippedArmor");
		}

		for (int i = 0; i < itemsHeld.Length; i++) {
			itemsHeld[i] = PlayerPrefs.GetString("ItemInInventory_" + i);
			numberOfItems[i] = PlayerPrefs.GetInt("ItemAmount_" + i);
		}
	}
	
}
