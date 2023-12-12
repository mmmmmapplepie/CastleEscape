using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchBuff : Item {
	protected override void ItemEffect() {
		GameBuffsManager.TorchModifierMultiplier *= 1.5f;
		if (!GameStateManager.InGame) return;
		TorchMovement.ItemTorchSettings();
	}
	protected override void EndItemEffect() {
		GameBuffsManager.TorchModifierMultiplier /= 1.5f;
		if (!GameStateManager.InGame) return;
		TorchMovement.ItemTorchSettings();
	}
}
