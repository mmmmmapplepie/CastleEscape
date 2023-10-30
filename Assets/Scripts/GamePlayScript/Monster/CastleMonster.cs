using System.Collections;
public class CastleMonster : MonsterBase {
	protected override IEnumerator ChasingRoutine() {
		while (true) {
			if (!PreyInRange(senseRange * 1.5f)) {
				StopChase();
				yield break;
			}
			direction = player.position - transform.position;
			if (PreyInRange(2f)) { yield return null; continue; }
			setMovement(direction);
			yield return null;
		}
	}
	protected override void damagePlayer() {

	}
}
