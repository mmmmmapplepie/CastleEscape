using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Item : MonoBehaviour {
	public new string name;
	public GameObject consumeParticleEffect = null;


	//itemTime below is the time it takes for the item to wear off. 0 mean item effects are instantaneous and dont add to UI.
	public float itemTime = 0f;

	[SerializeField] GameObject UIItemPrefab, UIItemDummyPrefab;
	[HideInInspector] public Transform UIItemHolder;
	[HideInInspector] public GameObject player;
	void OnTriggerEnter2D(Collider2D coll) {
		if (coll.tag == "Player") {
			CheckAddingItemToUI();
			ItemEffect();
			ItemVisuals();
		}
	}
	void CheckAddingItemToUI() {
		if (itemTime == 0) return;
		UIItem itemPresent = FindForItemInUI();
		if (itemPresent != null) {
			ExtendItemTime(itemPresent); return;
		} else {
			AddItemToUI();
		}
	}
	UIItem FindForItemInUI() {
		foreach (Transform tra in UIItemHolder) {
			if (tra.gameObject.GetComponent<UIItem>().itemName == name) {
				return tra.gameObject.GetComponent<UIItem>();
			}
		}
		return null;
	}
	void AddItemToUI() {
		GameObject uIItem = Instantiate(UIItemPrefab);
		UIItem script = uIItem.GetComponent<UIItem>();
		script.remainingTime = itemTime;
		script.EndFunction = EndItemEffect;
		GameObject Dummy = Instantiate(UIItemDummyPrefab, uIItem.transform.root);
		Dummy.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -270f);
		Dummy.transform.parent = uIItem.transform;
		script.ItemDummy = Dummy.transform;
	}
	void ExtendItemTime(UIItem itemInUI) {
		itemInUI.remainingTime = itemTime;
	}
	protected virtual void ItemEffect() { }
	protected virtual void EndItemEffect() { }
	void ItemVisuals() {
		Instantiate(consumeParticleEffect, transform.position, quaternion.identity);
	}
}
