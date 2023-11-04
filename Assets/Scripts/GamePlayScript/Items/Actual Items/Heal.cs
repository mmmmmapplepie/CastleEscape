public class Heal : Item {
	protected override void ItemEffect() {
		PlayerLife script = player.transform.root.gameObject.GetComponent<PlayerLife>();
		script.changeHealth(script.maxHealth, 0f);
	}
	protected override void EndItemEffect() {

	}
}
