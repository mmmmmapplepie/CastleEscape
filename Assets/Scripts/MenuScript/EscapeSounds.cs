using System;
using UnityEngine;

public class EscapeSounds : MonoBehaviour {
	[SerializeField] AudioClip Running, Clear;
	AudioSource aud;
	void Awake() {
		aud = GetComponent<AudioSource>();
	}


	public static event Action EscapeBGMTime;


	public void RunningSound() {
		aud.PlayOneShot(Running, 0.8f);
	}
	public void EscapeSound() {
		aud.PlayOneShot(Clear);
		EscapeBGMTime?.Invoke();
	}
}
