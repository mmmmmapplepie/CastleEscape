using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class Husher : MonsterBase {
	protected override IEnumerator ChasingRoutine() {
		if (playerlife == null) playerlife = player.gameObject.GetComponent<PlayerLife>();
		while (true) {
			if (!PreyInRange(senseRange)) {
				OutOfRange();
				StopChase();
				yield break;
			}
			direction = player.position - transform.position;
			if (PreyInRange(2f)) { yield return null; continue; }
			setMovement(direction);
			yield return null;
		}
	}
	void AddRoutineToList() {
		keys key = new keys();
		Coroutine newRoutine = StartCoroutine(TurnOfftorch(key));
		theList.Add(new CoroutineListItem(newRoutine, gameObject, key));
	}
	static bool coroutineRunning = false;
	static CoroutineListItem currentItem;
	static PlayerLife playerlife = null;
	void Update() {
		checkToStopForPanic();
		if (theList.Count > 0 && coroutineRunning == false && !playerlife.panic) {
			coroutineRunning = true;
			currentItem = theList[0];
			theList.RemoveAt(0);
			currentItem.runKey.runKey = 1;
		}
	}
	void checkToStopForPanic() {
		if (playerlife.panic) {
			if (currentItem != null) StopRunningRoutine(ref currentItem);
		}
	}

	void StopRunningRoutine(ref CoroutineListItem item) {
		StopCoroutine(item.coroutine);
		TorchMovement.SetTorchIntensity(1f);
	}
	void OutOfRange() {
		int i = theList.FindIndex(x => x.GO == gameObject);
		if (i != -1) {
			theList.RemoveAt(i);
		} else {
			if (currentItem.GO == gameObject) StopRunningRoutine(ref currentItem);
		}
		TorchMovement.SetTorchIntensity(1f);
	}
	static List<CoroutineListItem> theList = new List<CoroutineListItem>();
	public class CoroutineListItem {
		public Coroutine coroutine;
		public GameObject GO;

		//0 is dont run: 1 is for run;
		public keys runKey;
		public CoroutineListItem(Coroutine cr, GameObject go, keys runkey) {
			this.coroutine = cr;
			this.GO = go;
			this.runKey = runkey;
		}
	}
	public class keys {
		public int runKey;
		public keys(int runkey = 0) {
			this.runKey = runkey;
		}
	}

	float turnoffTime = 0.2f;
	float offWaitTime = 2f;
	IEnumerator TurnOfftorch(keys key) {
		while (key.runKey == 0) {
			yield return null;
		}
		float time = turnoffTime;
		while (time > 0f) {
			TorchMovement.SetTorchIntensity(time / turnoffTime);
			time -= Time.deltaTime;
			yield return null;
		}
		yield return new WaitForSeconds(offWaitTime);
		TorchMovement.SetTorchIntensity(1f);
		coroutineRunning = false;
	}
	void returnTorchToNormal() {
	}
	void OnDestroy() {
		if (theList.Count > 0) OutOfRange();
	}
}
