using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TerrorWhisper : MonsterBase {
	static PlayerLife lifeScript;
	protected override IEnumerator ChasingRoutine() {
		if (lifeScript == null) lifeScript = player.gameObject.GetComponent<PlayerLife>();
		fearCharges++;
		while (true) {
			if (!PreyInRange(senseRange * 1f)) {
				StopChase();
				yield break;
			}
			direction = player.position - transform.position;
			if (PreyInRange(1f)) { yield return null; continue; }
			setMovement(direction, 0.8f);
			yield return null;
		}
	}
	protected override void UpdateMethod() {
		if (lifeScript != null) {
			while (lifeScript.Fear < 100f && fearCharges > 0 && lifeScript.panic == false) {
				fearCharges--;
				InstillFear();
			}
		}
	}
	void InstillFear() {
		lifeScript.Fear += addedFear;
	}
	float addedFear = 50f;

	protected override void damageAmountAndTime(float dmg, float time = 2) {
		PlayerLife lifeS = player.root.gameObject.GetComponent<PlayerLife>();
		lifeS.changeHealth((int)dmg, time);
		if (lifeS.panic == false) lifeS.Fear += addedFear / 2f;
	}
	static int fearCharges = 0;
}
