using System.Collections.Generic;
using UnityEngine;

public class UIStaticAccess : MonoBehaviour {
	static GameObject thisg;
	void Awake() {
		thisg = gameObject;
	}
	public static void playClick() {
		thisg.GetComponent<AudioPlayer>().PlaySound("Click");
	}
}
