using System.Collections.Generic;
using UnityEngine;

public class ItemsController : MonoBehaviour {
	[SerializeField] List<GameObject> buffsPre, debuffsPre;
	static List<GameObject> buffPrefabs = new List<GameObject>();
	static List<GameObject> debuffPrefabs = new List<GameObject>();
	static List<GameObject> ItemsInRoom = new List<GameObject>();
	[SerializeField] Transform uiItemHolderRef;
	static Transform UIItemHolderRef;
	void Awake() {
		GameStateManager.GameEnd += ClearItems;
		GameStateManager.EnterMenu += ClearItems;
		GameStateManager.StartNewRoom += NewRoom;
		UIItemHolderRef = uiItemHolderRef;
		buffPrefabs = buffsPre;
		debuffPrefabs = debuffsPre;
		// setBonus(buffPrefabs[0], 1 * Vector3.one);
		// setBonus(buffPrefabs[1], 2 * Vector3.one);
		// setBonus(buffPrefabs[2], 3 * Vector3.one);
		// setBonus(buffPrefabs[3], 4 * Vector3.one);
		// setBonus(buffPrefabs[4], 5 * Vector3.one);
		// setBonus(buffPrefabs[5], 6 * Vector3.one);
		// setBonus(buffPrefabs[6], 7 * Vector3.one);
		// setBonus(debuffPrefabs[0], -5 * Vector3.one);
		// setBonus(debuffPrefabs[1], -6 * Vector3.one);
		// setBonus(debuffPrefabs[2], -1 * Vector3.one);
		// setBonus(debuffPrefabs[3], -2 * Vector3.one);
		// setBonus(debuffPrefabs[4], -3 * Vector3.one);
		// setBonus(debuffPrefabs[5], -4 * Vector3.one);
	}
	void ClearItems() {
		foreach (Transform tra in UIItemHolderRef) {
			if (tra != null) {
				Destroy(tra.gameObject);
			}
		}
		NewRoom();
	}
	void NewRoom() {
		foreach (GameObject g in ItemsInRoom) {
			if (g != null) Destroy(g);
		}
	}




	public static int luck = 0;

	//buff/debuff chances are out of hundred. pick random from -50 to 50. range from -50 to debuff gives debuff and 50 down to buffchance gives buff. affected by Luck stat.
	static int buffs = 0;
	static int debuffs = 0;
	public static void randomizeBuffAndDebuff(Vector3 pos) {
		int itemchanceluck = 2 * luck + 50;
		int itemChance = Random.Range(1, 1001);
		if (itemChance > itemchanceluck) return;

		int buffChance = 91;
		float gradient = 80f / 9f;
		buffChance = Mathf.RoundToInt(-gradient * luck + 91f + gradient);
		int buff_debuffInt = Random.Range(1, 101);
		GameObject bonus = null;
		if (buff_debuffInt >= buffChance) {
			buffs++;
			bonus = buffPrefabs[Random.Range(0, buffPrefabs.Count)];
		} else {
			debuffs++;
			bonus = debuffPrefabs[Random.Range(0, debuffPrefabs.Count)];
		}
		setBonus(bonus, pos);
	}
	public static void setBonus(GameObject prefab, Vector3 center) {
		if (prefab == null) return;
		Vector3 pos = new Vector3(Random.Range(-2f, 2f) + center.x, Random.Range(-3.5f, 3.5f) + center.y);
		GameObject item = Instantiate(prefab, pos, Quaternion.identity);
		ItemsInRoom.Add(item);
		item.GetComponent<Item>().UIItemHolder = UIItemHolderRef;
	}
}
