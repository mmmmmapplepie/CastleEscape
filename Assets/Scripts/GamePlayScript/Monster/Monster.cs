using UnityEngine;
[CreateAssetMenu(menuName = "Monster")]
public class Monster : ScriptableObject {
	public int Damage;
	public float Speed, Senses, Hunger;
	public string Description;
	public GameObject MonsterPrefab;
	public Color Color;
}
