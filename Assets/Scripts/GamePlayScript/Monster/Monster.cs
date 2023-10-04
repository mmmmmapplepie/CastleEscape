using UnityEngine;
[CreateAssetMenu(menuName = "Monster")]
public class Monster : ScriptableObject {
	public int Damage;
	public float Speed, Hunger, Range;
	public GameObject MonsterPrefab;
}
