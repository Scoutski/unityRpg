using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour {

	public Image buttonImage;
	public Text amountText;
	public int buttonValue;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Press() {
		string itemName = GameManager.instance.itemsHeld[buttonValue];
		if (GameMenu.instance.theMenu.activeInHierarchy) {
			if (itemName != "") {
				GameMenu.instance.SelectItem(GameManager.instance.GetItemDetails(itemName));
			}
		} else if (Shop.instance.shopMenu.activeInHierarchy) {
			if (Shop.instance.buyMenu.activeInHierarchy) {
				Shop.instance.SelectBuyItem(GameManager.instance.GetItemDetails(Shop.instance.itemsForSale[buttonValue]));
			}

			if (Shop.instance.sellMenu.activeInHierarchy) {
				Shop.instance.SelectSellItem(GameManager.instance.GetItemDetails(itemName));
			}
		} else if (BattleManager.instance.itemsMenu.activeInHierarchy) {
			if (itemName != "") {
				BattleManager.instance.SelectItem(GameManager.instance.GetItemDetails(itemName));
			}
		}
	}
}
