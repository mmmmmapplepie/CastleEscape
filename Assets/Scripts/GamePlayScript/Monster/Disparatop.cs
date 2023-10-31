using System.Collections;
public class Disparatop : MonsterBase {
	bool chasing = false;
	protected override IEnumerator ChasingRoutine() {
		chasing = true;
		GameBuffsManager.PlayerSpeedMultiplier = GameBuffsManager.PlayerSpeedMultiplier / 1.5f;
		while (true) {
			if (!PreyInRange(senseRange)) {
				GameBuffsManager.PlayerSpeedMultiplier = GameBuffsManager.PlayerSpeedMultiplier * 1.5f;
				chasing = false;
				StopChase();
				yield break;
			}
			direction = player.position - transform.position;
			if (PreyInRange(3f)) { yield return null; continue; }
			setMovement(direction);
			yield return null;
		}
	}
	void OnDestroy() {
		if (chasing) GameBuffsManager.PlayerSpeedMultiplier = GameBuffsManager.PlayerSpeedMultiplier * 1.5f;
	}
}
