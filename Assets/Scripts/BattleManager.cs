using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour {

	public static BattleManager instance;

	private bool battleActive;
	public GameObject battleScene;

	[Header("Players & Enemies")]
	public Transform[] playerPositions;
	public Transform[] enemyPositions;

	public BattleChar[] playerPrefabs;
	public BattleChar[] enemyPrefabs;

	public List<BattleChar> activeBattlers = new List<BattleChar>();

	[Header("Battle Status")]
	public int currentTurn;
	public bool turnWaiting;

	public GameObject uiButtonsHolder;
	public GameObject statsPanel;

	public BattleMove[] movesList;
	public GameObject enemyAttackEffect;

	public DamageNumber theDamageNumber;

	[Header("Player UI Stats")]
	public Text[] playerName;
	public Text[] playerHP;
	public Text[] playerMP;

	public GameObject targetMenu;
	public BattleTargetButton[] targetButtons;

	public GameObject magicMenu;
	public BattleMagicSelect[] magicButtons;

	public BattleNotification battleNotice;

	public int chanceToFlee = 35; // Percent
	private bool isFleeing;

	public GameObject itemsMenu;
	public Item activeItem;
	public ItemButton[] itemButtons;
	public GameObject itemCharChoiceMenu;
	public Text[] itemCharChoiceNames;
	public Text itemName, itemDescription;

	public string gameOverScene;

	public int rewardExp;
	public string[] rewardItems;

	public bool cannotFlee;

	// Use this for initialization
	void Start () {
		instance = this;

		DontDestroyOnLoad(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.T)) {
			BattleStart(new string[] { "Eyeball", "Skeleton" }, false);
		}

		if (battleActive) {
			if (turnWaiting) {
				if (activeBattlers[currentTurn].isPlayer) {
					uiButtonsHolder.SetActive(true);
				} else {
					uiButtonsHolder.SetActive(false);
					
					// Enemy attack
					StartCoroutine(EnemyMoveCo());
				}
			}
		}

		if (Input.GetKeyDown(KeyCode.V)) {
			AdvanceTurn();
		}
	}

	public void BattleStart(string[] enemiesToSpawn, bool setCannotFlee) {
		if (!battleActive) {
			cannotFlee = setCannotFlee;

			battleActive = true;

			// Disable player movement & move the scene over the current position.
			GameManager.instance.battleActive = true;
			transform.position = new Vector3(
				Camera.main.transform.position.x,
				Camera.main.transform.position.y,
				transform.position.z);
			battleScene.SetActive(true);
			statsPanel.SetActive(true);

			// Play battle music
			AudioManager.instance.PlayBGM(0);

			// Spawn units & update UI.
			SpawnPlayers();
			SpawnEnemies(enemiesToSpawn);
			UpdateUIStats();

			// Set starting variables 
			turnWaiting = true;
			currentTurn = Random.Range(0, activeBattlers.Count);
		}
	}

	private void SpawnPlayers() {
		for (int i = 0; i < playerPositions.Length; i++) {
			// Only put them in if they are active.
			if (GameManager.instance.playerStats[i].gameObject.activeInHierarchy) {
				for (int j = 0; j < playerPrefabs.Length; j++) {
					if (playerPrefabs[j].charName == GameManager.instance.playerStats[i].charName) {
						BattleChar newPlayer = Instantiate(
							playerPrefabs[j],
							playerPositions[i].position,
							playerPositions[i].rotation);
						
						// Creates reference to the position of the instantiated player.
						newPlayer.transform.parent = playerPositions[i];
						activeBattlers.Add(newPlayer);

						// Set all the stats on the prefabs
						CharStats thePlayer = GameManager.instance.playerStats[i];
						activeBattlers[i].currentHP = thePlayer.currentHP;
						activeBattlers[i].maxHP = thePlayer.maxHP;
						activeBattlers[i].currentMP = thePlayer.currentMP;
						activeBattlers[i].maxMP = thePlayer.maxMP;
						activeBattlers[i].strength = thePlayer.strength;
						activeBattlers[i].defence = thePlayer.defence;
						activeBattlers[i].weaponPower = thePlayer.weaponPower;
						activeBattlers[i].armorPower = thePlayer.armorPower;
					}
				}
			}
		}
	}

	private void SpawnEnemies(string[] enemiesToSpawn) {
		for (int i = 0; i < enemiesToSpawn.Length; i++) {
			if (enemiesToSpawn[i] != "") {
				for (int j = 0; j < enemyPrefabs.Length; j++) {
					if (enemiesToSpawn[i] == enemyPrefabs[j].charName) {
						BattleChar newEnemy = Instantiate(
							enemyPrefabs[j],
							enemyPositions[i].position,
							enemyPositions[i].rotation);

							newEnemy.transform.parent = enemyPositions[i];
							activeBattlers.Add(newEnemy);
					}
				}
			}
		}
	}

	public void AdvanceTurn() {
		currentTurn++;
		if (currentTurn >= activeBattlers.Count) {
			currentTurn = 0;
		}

		turnWaiting = true;

		UpdateBattle();
		UpdateUIStats();
	}

	public void UpdateBattle() {
		bool allEnemiesDead = true;
		bool allPlayersDead = true;

		for (int i = 0; i < activeBattlers.Count; i++) {
			if (activeBattlers[i].currentHP < 0) {
				activeBattlers[i].currentHP = 0;
			}

			if (activeBattlers[i].currentHP == 0) {
				if (activeBattlers[i].isPlayer) {
					activeBattlers[i].theSprite.sprite = activeBattlers[i].deadSprite;
				} else {
					activeBattlers[i].EnemyFade();
				}
			} else {
				if (activeBattlers[i].isPlayer) {
					allPlayersDead = false;
					activeBattlers[i].theSprite.sprite = activeBattlers[i].aliveSprite;
				} else {
					allEnemiesDead = false;
				}
			}
		}

		if (allEnemiesDead || allPlayersDead) {
			if (allEnemiesDead) {
				StartCoroutine(EndBattleCo());
			} else {
				StartCoroutine(GameOverCo());
			}

			// battleScene.SetActive(false);
			// GameManager.instance.battleActive = false;
			// battleActive = false;
		} else {
			while (activeBattlers[currentTurn].currentHP <= 0) {
				currentTurn++;
				if (currentTurn >= activeBattlers.Count) {
					currentTurn = 0;
				}
			}
		}
	}

	public IEnumerator EnemyMoveCo() {
		turnWaiting = false;
		yield return new WaitForSeconds(1f);
		EnemyAttack();
		yield return new WaitForSeconds(1f);
		AdvanceTurn();
	}

	public void EnemyAttack() {
		List<int> players = GetActivePlayers();

		// Attack determination
		int selectedTarget = players[Random.Range(0, players.Count)];
		int selectedAttack = Random.Range(0, activeBattlers[currentTurn].movesAvailable.Length);
		string moveName = "";
		int movePower = 0;
		for (int i = 0; i < movesList.Length; i++) {
			if (movesList[i].moveName == activeBattlers[currentTurn].movesAvailable[selectedAttack]) {
				moveName = movesList[i].moveName;
				Instantiate(
					movesList[i].theEffect,
					activeBattlers[selectedTarget].transform.position,
					activeBattlers[selectedTarget].transform.rotation);

				movePower = movesList[i].movePower;

			}
		}

		BattleManager.instance.battleNotice.notificationText.text =
			activeBattlers[currentTurn].charName + " used " + moveName + " on " +
			activeBattlers[selectedTarget].charName + "!";
		BattleManager.instance.battleNotice.Activate();

		Instantiate(enemyAttackEffect, activeBattlers[currentTurn].transform.position, activeBattlers[currentTurn].transform.rotation);
		DealDamage(selectedTarget, movePower);
	}

	public void DealDamage(int targetPosition, int movePower) {
		float attackPower = activeBattlers[currentTurn].strength + activeBattlers[currentTurn].weaponPower;
		float defencePower = activeBattlers[targetPosition].defence + activeBattlers[targetPosition].armorPower;

		// Damage is a range of 10% +/- from the calculated power.
		float damageCalc = (attackPower / defencePower) * movePower * (Random.Range(0.9f, 1.1f));
		// Only attack in whole numbers:
		int damageToGive = Mathf.RoundToInt(damageCalc);

		// Debug.Log(activeBattlers[currentTurn].charName + " is dealing " + damageCalc.ToString() +
			// " (" + damageToGive.ToString() + ") damage to " + activeBattlers[targetPosition].charName);

		activeBattlers[targetPosition].currentHP -= damageToGive;
		Instantiate(theDamageNumber, activeBattlers[targetPosition].transform.position, activeBattlers[targetPosition].transform.rotation).SetDamage(damageToGive);

		UpdateUIStats();
	}

	public void UpdateUIStats() {
		for (int i = 0; i < playerName.Length; i++) {
			if (activeBattlers.Count > i) {
				if (activeBattlers[i].isPlayer) {
					BattleChar playerData = activeBattlers[i];
					playerName[i].text = playerData.charName;
					playerHP[i].text = Mathf.Clamp(playerData.currentHP, 0, int.MaxValue) + "/" + playerData.maxHP;
					playerMP[i].text = Mathf.Clamp(playerData.currentMP, 0, int.MaxValue) + "/" + playerData.maxMP;
				} else {
					playerName[i].gameObject.SetActive(false);
				}
			} else {
				playerName[i].gameObject.SetActive(false);
			}
		}
	}

	public void PlayerAttack(string moveName, int selectedTarget) {
		int movePower = 0;
		for (int i = 0; i < movesList.Length; i++) {
			if (movesList[i].moveName == moveName) {
				Instantiate(
					movesList[i].theEffect,
					activeBattlers[selectedTarget].transform.position,
					activeBattlers[selectedTarget].transform.rotation);

				movePower = movesList[i].movePower;

			}
		}

		BattleManager.instance.battleNotice.notificationText.text =
			activeBattlers[currentTurn].charName + " used " + moveName + " on " +
			activeBattlers[selectedTarget].charName + "!";
		BattleManager.instance.battleNotice.Activate();
		Instantiate(enemyAttackEffect, activeBattlers[currentTurn].transform.position, activeBattlers[currentTurn].transform.rotation);
		DealDamage(selectedTarget, movePower);

		uiButtonsHolder.SetActive(false);
		targetMenu.SetActive(false);

		AdvanceTurn();
	}

	public void OpenTargetMenu(string moveName) {
		targetMenu.SetActive(true);

		List<int> enemies = new List<int>();
		for (int i = 0; i < activeBattlers.Count; i++) {
			if (!activeBattlers[i].isPlayer && activeBattlers[i].currentHP > 0) {
				enemies.Add(i);
			}
		}

		for (int i = 0; i < targetButtons.Length; i++) {
			if (enemies.Count > i) {
				targetButtons[i].gameObject.SetActive(true);
				targetButtons[i].moveName = moveName;
				targetButtons[i].activeBattlerTarget = enemies[i];
				targetButtons[i].targetName.text = activeBattlers[enemies[i]].charName;
			} else {
				targetButtons[i].gameObject.SetActive(false);
			}
		}
	}

	public void OpenMagicMenu() {
		magicMenu.SetActive(true);

		for (int i = 0; i < magicButtons.Length; i++) {
			if (activeBattlers[currentTurn].movesAvailable.Length > i) {
				magicButtons[i].gameObject.SetActive(true);
				magicButtons[i].spellName = activeBattlers[currentTurn].movesAvailable[i];
				magicButtons[i].nameText.text = magicButtons[i].spellName;
				for (int j = 0; j < movesList.Length; j++) {
					if (movesList[j].moveName == magicButtons[i].spellName) {
						magicButtons[i].spellCost = movesList[j].moveCost;
						magicButtons[i].nameCost.text = movesList[j].moveCost.ToString();
					}
				}
			} else {
				magicButtons[i].gameObject.SetActive(false);
			}
		}
	}
	
	public void Flee() {
		if (cannotFlee) {
			BattleManager.instance.battleNotice.notificationText.text = "Can't flee this battle!";
			BattleManager.instance.battleNotice.Activate();
		} else {
			int fleeSuccess = Random.Range(0, 100);
			if (fleeSuccess < chanceToFlee) {
				// battleActive = false;
				// GameManager.instance.battleActive = false;
				// AudioManager.instance.PlayPreviousSong();
				// battleScene.SetActive(false);
				isFleeing = true;
				StartCoroutine(EndBattleCo());
			} else {
				// Can't escape!
				BattleManager.instance.battleNotice.notificationText.text = "Attempt to flee failed!";
				BattleManager.instance.battleNotice.Activate();
				AdvanceTurn();
			}
		}
	}

	public void OpenItemsMenu() {
		itemsMenu.SetActive(true);
		int buttonPosition = 0;
		for (int i = 0; i < GameManager.instance.itemsHeld.Length; i++) {
			itemButtons[i].buttonValue = buttonPosition;

			if (GameManager.instance.itemsHeld[i] != "") {
				for (int j = 0; j < GameManager.instance.referenceItems.Length; j++) {
					if (GameManager.instance.itemsHeld[i] == GameManager.instance.referenceItems[j].name) {
						itemButtons[i].buttonImage.gameObject.SetActive(false);
						itemButtons[i].amountText.text = "";

						if (GameManager.instance.referenceItems[j].isItem) {
							itemButtons[buttonPosition].buttonValue = i;
							itemButtons[buttonPosition].buttonImage.gameObject.SetActive(true);

							Item heldItem = GameManager.instance.referenceItems[j];
							itemButtons[buttonPosition].buttonImage.sprite = heldItem.itemSprite;
							itemButtons[buttonPosition].amountText.text = GameManager.instance.numberOfItems[i].ToString();
							buttonPosition++;
						}
					}
				}
			} else {
				itemButtons[i].buttonImage.gameObject.SetActive(false);
				itemButtons[i].amountText.text = "";
			}
		}
	}

	public void CloseItemsMenu() {
		itemsMenu.SetActive(false);
	}

	public void SelectItem(Item itemToUse) {
		activeItem = itemToUse;

		itemName.text = activeItem.itemName;
		itemDescription.text = activeItem.description;
	}

	public void OpenItemCharChoice() {
		itemCharChoiceMenu.SetActive(true);

		for (int i = 0; i < itemCharChoiceNames.Length; i++) {
			if (GameManager.instance.playerStats[i].currentHP > 0) {
				itemCharChoiceNames[i].text = GameManager.instance.playerStats[i].charName;
				itemCharChoiceNames[i].transform.parent.gameObject.SetActive(GameManager.instance.playerStats[i].gameObject.activeInHierarchy);
			}
		}
	}

	public void CloseItemCharChoice() {
		itemCharChoiceMenu.SetActive(false);
	}

	public void UseItem(int selectedChar) {
		activeItem.Use(selectedChar); // consume the item from the inventory.
		if (activeItem.affectHP) {
			activeBattlers[selectedChar].currentHP += activeItem.amountToChange;
			if (activeBattlers[selectedChar].currentHP > activeBattlers[selectedChar].maxHP) {
				activeBattlers[selectedChar].currentHP = activeBattlers[selectedChar].maxHP;
			}
		} if (activeItem.affectMP) {
			activeBattlers[selectedChar].currentMP += activeItem.amountToChange;

			if (activeBattlers[selectedChar].currentMP > activeBattlers[selectedChar].maxMP) {
				activeBattlers[selectedChar].currentMP = activeBattlers[selectedChar].maxMP;
			}
		}
		CloseItemCharChoice();
		CloseItemsMenu();
		BattleManager.instance.battleNotice.notificationText.text =
			"Used " + activeItem.name + " on " + GameManager.instance.playerStats[selectedChar].charName + "!";
		BattleManager.instance.battleNotice.Activate();
		activeItem = null;
		UpdateUIStats();
		AdvanceTurn();
	}

	private List<int> GetActivePlayers() {
		List<int> players = new List<int>();
		for (int i = 0; i < activeBattlers.Count; i++) {
			if (activeBattlers[i].isPlayer && activeBattlers[i].currentHP > 0) {
				players.Add(i);
			}
		}

		return players;
	}

	public IEnumerator EndBattleCo() {
		battleActive = false;
		uiButtonsHolder.SetActive(false);
		statsPanel.SetActive(false);
		targetMenu.SetActive(false);
		magicMenu.SetActive(false);
		itemsMenu.SetActive(false);
		

		yield return new WaitForSeconds(.5f);
		UIFade.instance.FadeToBlack();
		yield return new WaitForSeconds(1.5f);

		UpdatePlayerStatsInGameManager();

		UIFade.instance.FadeFromBlack();
		battleScene.SetActive(false);

		for (int i = 0; i < activeBattlers.Count; i++) {
			Destroy(activeBattlers[i].gameObject);
		}
		activeBattlers.Clear();
		currentTurn = 0;
		if (isFleeing) {
			isFleeing = false;
			GameManager.instance.battleActive = false;
		} else {
			// Open rewards screen
			BattleReward.instance.OpenRewardScreen(rewardItems, rewardExp);
		}

		AudioManager.instance.PlayBGM(FindObjectOfType<CameraController>().musicToPlay);
	}

	public IEnumerator GameOverCo() {
		battleActive = false;
		UIFade.instance.FadeToBlack();
		yield return new WaitForSeconds(1.5f);
		battleScene.SetActive(false);

		SceneManager.LoadScene(gameOverScene);
	}

	private void UpdatePlayerStatsInGameManager() {
		for (int i = 0; i < activeBattlers.Count; i++) {
			if (activeBattlers[i].isPlayer) {
				for (int j = 0; j < GameManager.instance.playerStats.Length; j++) {
					if (activeBattlers[i].charName == GameManager.instance.playerStats[j].charName) {
						GameManager.instance.playerStats[j].currentHP = activeBattlers[i].currentHP;
						GameManager.instance.playerStats[j].currentMP = activeBattlers[i].currentMP;
					}
				}
			}
		}
	}
}
