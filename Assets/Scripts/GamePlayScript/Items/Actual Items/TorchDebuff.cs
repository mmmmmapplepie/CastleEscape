public class TorchDebuff : Item {
	protected override void ItemEffect() {
		GameBuffsManager.TorchModifierMultiplier *= 0.5f;
		if (!GameStateManager.InGame) return;
		TorchMovement.ItemTorchSettings();
	}
	protected override void EndItemEffect() {
		GameBuffsManager.TorchModifierMultiplier /= 0.5f;
		if (!GameStateManager.InGame) return;
		TorchMovement.ItemTorchSettings();
	}
}
