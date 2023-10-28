using System.Collections;
using UnityEngine;
public class CastleMonster : MonsterBase {
	protected override IEnumerator ChasingRoutine() {
		while (true) {
			if (!PreyInRange(senseRange * 1.5f)) {
				StopChase();
				yield break;
			}
			direction = player.position - transform.position;
			setMovement(direction);
			yield return null;
		}
	}
}
