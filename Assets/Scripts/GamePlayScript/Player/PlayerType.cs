using UnityEngine;
[CreateAssetMenu(menuName = "PlayerType")]
public class PlayerType : ScriptableObject {
	[Range(1, 10)]
	public int MaxHealth, Regeneration, DashPower, DashRegeneration, DashStamina, Luck, Speed, TorchIntensity, TorchWidth, Aura;

	public Color color;
}
