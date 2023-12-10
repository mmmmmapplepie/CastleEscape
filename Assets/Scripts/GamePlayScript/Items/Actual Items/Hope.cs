using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hope : Item {
	protected override void ItemEffect() {
		PlayerLife script = player.transform.root.gameObject.GetComponent<PlayerLife>();
		script.ChangeFear(-80f, true);
	}
}
