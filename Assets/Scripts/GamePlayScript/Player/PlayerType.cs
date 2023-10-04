using UnityEngine;
[CreateAssetMenu(menuName = "PlayerType")]
public class PlayerType : ScriptableObject {
	[Range(1, 10)]
	public int MaxHealth, DashStamina, Regeneration, Luck, Speed, DashPower, DashRegeneration, TorchIntensity, TorchWidth, Aura;
}
