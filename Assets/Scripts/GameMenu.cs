using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameMenu : MonoBehaviour {

	public GameObject theMenu;
	public GameObject[] windows;

	private CharStats[] playerStats;
	public Text[] nameText, hpText, mpText, lvlText, expText;
	public Slider[] expSlider;
	public Image[] charImage;
	public GameObject[] charStatHolder;

	[Header("Stats info")]
	public GameObject[] statsPlayerButton;
	public Text statsNameText, statsHpText, statsMpText, statsStrengthText, statsDefenceText,
		statsEquippedWeaponText, statsWeaponPowerText, statsEquippedArmorText, statsArmorPowerText,
		statsExpToNextLevel;
	public Image statsImage;

	[Header("Items info")]
	public ItemButton[] itemButtons;
	public string selectedItem;
	public Item activeItem;
	public Text itemName, itemDescription, useButtonText;
	
	public GameObject itemCharChoiceMenu;
	public Text[] itemCharChoiceNames;
	public Text goldText;
	public string mainMenuName;

	public static GameMenu instance;

	[Header("Pet Menu")]
	public GameObject petMenu;


	// Use this for initialization
	void Start () {
		instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		// if (Input.GetButtonDown("Fire2")) {
		// 	if (theMenu.activeInHierarchy) {
		// 		CloseMenu();
		// 	} else {
		// 		theMenu.SetActive(true);
		// 		UpdateMainStats();
		// 		GameManager.instance.gameMenuOpen = true;
		// 	}

		// 	AudioManager.instance.PlaySFX(5);
		// }
	}

	public void UpdateMainStats() {
		playerStats = GameManager.instance.playerStats;

		for (int i = 0; i < playerStats.Length; i++) {
			if (playerStats[i].gameObject.activeInHierarchy) {
				charStatHolder[i].SetActive(true);

				nameText[i].text = playerStats[i].charName;
				hpText[i].text = "HP: " + playerStats[i].currentHP + "/" + playerStats[i].maxHP;
				mpText[i].text = "MP: " + playerStats[i].currentMP + "/" + playerStats[i].maxMP;
				lvlText[i].text = "Lvl: " + playerStats[i].playerLevel;
				expText[i].text = "" + playerStats[i].currentEXP + "/" + playerStats[i].expToNextLevel[playerStats[i].playerLevel];
				expSlider[i].maxValue = playerStats[i].expToNextLevel[playerStats[i].playerLevel];
				expSlider[i].value = playerStats[i].currentEXP;

				charImage[i].sprite = playerStats[i].charImage;

				statsPlayerButton[i].GetComponentInChildren<Text>().text = playerStats[i].charName;

			} else {
				charStatHolder[i].SetActive(false);
				statsPlayerButton[i].SetActive(false);
			}
		}

		goldText.text = "" + GameManager.instance.currentGold + "g";
	}

	public void ToggleWindow(int windowNumber) {
		UpdateMainStats();

		for (int i = 0; i < windows.Length; i++) {
			if (i == windowNumber) {
				windows[i].SetActive(!windows[i].activeInHierarchy);
			} else {
				windows[i].SetActive(false);
			}
		}

		itemCharChoiceMenu.SetActive(false);
	}

	public void CloseMenu() {
		for (int i = 0; i < windows.Length; i++) {
			windows[i].SetActive(false);
		}

		theMenu.SetActive(false);
		GameManager.instance.gameMenuOpen = false;

		itemCharChoiceMenu.SetActive(false);
	}

	public void SetDetailedStats(int playerNumber) {
		statsImage.sprite = playerStats[playerNumber].charImage;
		statsNameText.text = playerStats[playerNumber].charName;
		statsHpText.text = "" + playerStats[playerNumber].currentHP + "/" + playerStats[playerNumber].maxHP;
		statsMpText.text = "" + playerStats[playerNumber].currentMP + "/" + playerStats[playerNumber].maxMP;;
		statsStrengthText.text = playerStats[playerNumber].strength.ToString();
		statsDefenceText.text = playerStats[playerNumber].defence.ToString();
		if (playerStats[playerNumber].equippedWeapon != "") {
			statsEquippedWeaponText.text = playerStats[playerNumber].equippedWeapon;
		} else {
			statsEquippedWeaponText.text = "None";
		}
		statsWeaponPowerText.text = playerStats[playerNumber].weaponPower.ToString();
		if (playerStats[playerNumber].equippedArmor != "") {
			statsEquippedArmorText.text = playerStats[playerNumber].equippedArmor;
		} else {
			statsEquippedArmorText.text = "None";
		}
		statsArmorPowerText.text = playerStats[playerNumber].armorPower.ToString();
		statsExpToNextLevel.text = (playerStats[playerNumber].expToNextLevel[playerStats[playerNumber].playerLevel] - playerStats[playerNumber].currentEXP).ToString();
	}

	public void ShowItems() {
		GameManager.instance.SortItems();

		for (int i = 0; i < itemButtons.Length; i++) {
			itemButtons[i].buttonValue = i;

			if (GameManager.instance.itemsHeld[i] != "") {
				itemButtons[i].buttonImage.gameObject.SetActive(true);
				
				Item heldItem = GameManager.instance.GetItemDetails(GameManager.instance.itemsHeld[i]);
				itemButtons[i].buttonImage.sprite = heldItem.itemSprite;
				itemButtons[i].amountText.text = GameManager.instance.numberOfItems[i].ToString();
			} else {
				itemButtons[i].buttonImage.gameObject.SetActive(false);
				itemButtons[i].amountText.text = "";
			}
		}
	}

	public void SelectItem(Item newItem) {
		activeItem = newItem;
		if (activeItem.isItem) {
			useButtonText.text = "Use";
		}

		if (activeItem.isWeapon || activeItem.isArmor) {
			useButtonText.text = "Equip";
		}

		itemName.text = activeItem.itemName;
		itemDescription.text = activeItem.description;
	}

	public void DiscardItem() {
		if (activeItem != null) {
			GameManager.instance.RemoveItem(activeItem.itemName);
		}
	}

	public void OpenItemCharChoice() {
		itemCharChoiceMenu.SetActive(true);

		for (int i = 0; i < itemCharChoiceNames.Length; i++) {
			itemCharChoiceNames[i].text = GameManager.instance.playerStats[i].charName;
			itemCharChoiceNames[i].transform.parent.gameObject.SetActive(GameManager.instance.playerStats[i].gameObject.activeInHierarchy);
		}
	}

	public void CloseItemCharChoice() {
		itemCharChoiceMenu.SetActive(false);
	}

	public void UseItem(int selectedChar) {
		activeItem.Use(selectedChar);
		CloseItemCharChoice();
	}

	public void SaveGame() {
		GameManager.instance.SaveData();
		QuestManager.instance.SaveQuestData();
	}

	public void PlayButtonSound() {
		AudioManager.instance.PlaySFX(4);
	}

	public void QuitGame() {
		Destroy(GameManager.instance.gameObject);
		Destroy(PlayerController.instance.gameObject);
		Destroy(AudioManager.instance.gameObject);
		Destroy(gameObject);
		SceneManager.LoadScene(mainMenuName);
	}

	// --------
	// PET MENU
	// --------
	public void OpenPetMenu() {
		Debug.Log("Opening pet menu");
		GameManager.instance.gameMenuOpen = true;
		petMenu.SetActive(true);
	}

	public void ClosePetMenu() {
		Debug.Log("Closing pet menu");
		petMenu.SetActive(false);
		GameManager.instance.gameMenuOpen = false;
	}
}
