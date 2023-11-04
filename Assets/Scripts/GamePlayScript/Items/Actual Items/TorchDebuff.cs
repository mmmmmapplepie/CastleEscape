using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchDebuff : Item {
	protected override void ItemEffect() {
		GameBuffsManager.TorchModifierMultiplier *= 0.25f;
	}
	protected override void EndItemEffect() {
		GameBuffsManager.TorchModifierMultiplier /= 0.25f;
	}
}
