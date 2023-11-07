using System.Collections;

public class TerrorWhisper : MonsterBase {
	protected override void AwakeMethod() {
		lifeScript = player.root.gameObject.GetComponent<PlayerLife>();
	}
	static PlayerLife lifeScript;
	protected override IEnumerator ChasingRoutine() {
		fearCharges++;
		while (true) {
			if (!PreyInRange(senseRange)) {
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
		while (lifeScript.Fear < 100f && fearCharges > 0 && lifeScript.panic == false) {
			fearCharges--;
			InstillFear();
		}
	}
	void InstillFear() {
		lifeScript.Fear += addedFear;
	}
	float addedFear = 40f;

	protected override void damageAmountAndTime(float dmg, float time = 2) {
		bool changed = lifeScript.changeHealth((int)dmg, time);
		if (!lifeScript.panic && changed) lifeScript.Fear += addedFear / 4f;
	}
	static int fearCharges = 0;
	void OnDestroy() {
		if (fearCharges > 0) fearCharges--;
	}
}
