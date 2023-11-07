using System.Collections;
using UnityEngine;

public class Totem : MonsterBase {
	protected override void AwakeMethod() {
		if (ItemsHolder == null) ItemsHolder = GameObject.Find("UIItemsHolder").transform;
	}
	Transform ItemsHolder;
	protected override IEnumerator ChasingRoutine() {
		RemoveBuffs();
		while (true) {
			if (!PreyInRange(senseRange)) {
				StopChase();
				yield break;
			}
			direction = player.position - transform.position;
			if (PreyInRange(1f)) { yield return null; continue; }
			setMovement(direction);
			yield return null;
		}
	}
	void RemoveBuffs() {
		foreach (Transform tra in ItemsHolder) {
			if (tra.gameObject.GetComponent<UIItem>().IsABuff) Destroy(tra.gameObject);
		}
	}
}
