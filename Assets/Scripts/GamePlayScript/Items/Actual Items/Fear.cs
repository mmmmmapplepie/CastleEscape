public class Fear : Item {
	protected override void ItemEffect() {
		PlayerLife script = player.transform.root.gameObject.GetComponent<PlayerLife>();
		script.ChangeFear(80f);
	}
}
