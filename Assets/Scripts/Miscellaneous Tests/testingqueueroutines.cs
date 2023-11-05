using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testingqueueroutines : MonoBehaviour {
	void Awake() {
		keys key = new keys();
		Coroutine newRoutine = StartCoroutine(wait(key));
		thequeue.Enqueue(new CoroutineQueueItem(newRoutine, key));
	}

	static bool running = false;
	void Update() {
		if (thequeue.Count > 0 && running == false) {
			print(running);
			running = true;
			thequeue.Dequeue().runKey.runKey = 1;
		}
	}
	static int i = 0;
	IEnumerator wait(keys key) {
		while (key.runKey == 0) {
			yield return null;
		}
		yield return new WaitForSeconds(1f);
		print("Calldis" + i);
		i++;
		running = false;
	}

	static Queue<CoroutineQueueItem> thequeue = new Queue<CoroutineQueueItem>();
	public class CoroutineQueueItem {
		Coroutine coroutine;
		//0 is dont run: 1 is for run;
		public keys runKey;
		public CoroutineQueueItem(Coroutine cr, keys runkey) {
			this.coroutine = cr;
			this.runKey = runkey;
		}
	}
	public class keys {
		public int runKey;
		public keys(int runkey = 0) {
			this.runKey = runkey;
		}
	}
}