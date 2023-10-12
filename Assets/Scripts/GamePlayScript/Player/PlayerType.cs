using UnityEngine;
using System.Reflection;
[CreateAssetMenu(menuName = "PlayerType")]
public class PlayerType : ScriptableObject {
	[Range(1, 10)]
	public int MaxHealth, Regeneration, DashPower, DashRegeneration, DashStamina, Luck, Speed, TorchIntensity, TorchWidth, Aura;

	public Color color;
	public int GetStat(string statname) {
		System.Type type = GetType();
		FieldInfo info = type.GetField(statname);
		if (info == null) {
			throw new System.Exception("The field " + statname + " does not exist");
		}
		return (int)info.GetValue(this);
	}
	public void SetStat(string statname, int value) {
		System.Type type = GetType();
		FieldInfo info = type.GetField(statname);
		if (info == null || value.GetType() != 5.GetType() || value > 10 || value < 1) {
			throw new System.Exception("The field " + statname + " does not exist or the input value is not applicable");
		}
		info.SetValue(this, value);
	}
}
