using System.Collections;
using UnityEngine;

public class Ghost : MonsterBase {
	protected override IEnumerator ChasingRoutine() {
		while (true) {
			if (!PreyInRange(senseRange * 1f)) {
				StopChase();
				yield break;
			}
			direction = player.position - transform.position;
			if (PreyInRange(2f)) { yield return null; continue; }
			setMovement(direction, 2f);
			yield return null;
		}
	}
}

