using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour {
	public new string name;
	public bool IsABuff = false;
	public GameObject consumeParticleEffect = null;

	//itemTime below is the time it takes for the item to wear off. 0 mean item effects are instantaneous and dont add to UI.
	public float itemTime = 0f;
	[SerializeField] AudioClip sfxClip;
	[SerializeField] GameObject UIItemPrefab, UIItemDummyPrefab;
	[HideInInspector] public Transform UIItemHolder;
	[HideInInspector] public GameObject player;
	Color playerColor;
	bool triggered = false;
	void OnTriggerEnter2D(Collider2D coll) {
		ItemTrigger(coll);
	}
	void OnTriggerStay2D(Collider2D coll) {
		ItemTrigger(coll);
	}
	bool itemConsumed = false;
	void ItemTrigger(Collider2D coll) {
		if ((UIItemHolder.childCount > 5 || triggered) && itemTime != 0f) return;
		if (coll.tag == "Player") {
			itemConsumed = true;
			playerColor = coll.gameObject.GetComponent<SpriteRenderer>().color;
			player = coll.transform.root.gameObject;
			triggered = true;
			CheckAddingItemToUI();
			ItemEffect();
			ItemVisuals();
			ItemSound();
			Destroy(gameObject);
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
	bool EndingCalled = false;
	void AddItemToUI() {
		UIItemPrefab.GetComponent<UIItem>().playerColor = playerColor;
		UIItemPrefab.GetComponent<UIItem>().remainingTime = itemTime;
		GameObject uIItem = Instantiate(UIItemPrefab, UIItemHolder);
		UIItem script = uIItem.GetComponent<UIItem>();
		script.itemName = name;
		script.IsABuff = IsABuff;
		uIItem.transform.Find("Image").gameObject.GetComponent<Image>().sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
		script.EndFunction = EndItemEffect;
		EndingCalled = true;
		GameObject Dummy = Instantiate(UIItemDummyPrefab, uIItem.transform.parent.parent.Find("DummyHolder"));
		Dummy.GetComponent<Image>().sprite = gameObject.GetComponent<SpriteRenderer>().sprite;
		Dummy.GetComponent<RectTransform>().anchoredPosition = new Vector2(300f, 80f);
		script.ItemDummy = Dummy.transform;
	}

	void ExtendItemTime(UIItem itemInUI) {
		itemInUI.remainingTime = itemTime;
	}
	protected virtual void ItemEffect() { }
	protected virtual void EndItemEffect() { }
	void ItemVisuals() {
		if (consumeParticleEffect != null) {
			Instantiate(consumeParticleEffect, transform.position, quaternion.identity);
		}
	}
	void ItemSound() {
		AudioPlayer audioS = UIItemHolder.gameObject.GetComponent<AudioPlayer>();
		audioS.PlaySound(sfxClip.name, 0f);
	}
	// void OnDestroy() {
	// 	if (EndingCalled || !GameStateManager.InGame || !itemConsumed) return;
	// 	print(1);
	// 	EndItemEffect();
	// }
}
