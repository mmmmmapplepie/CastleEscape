using System.Collections.Generic;
using UnityEngine;

public class MonsterAudio : AudioPlayer {
	[SerializeField] List<AudioClip> IdleSounds, ChaseSounds;
	public void PlayIdleSound() {
		int i = Random.Range(0, 100);
		if (CheckPlaying("Idle") || i < 40) return;
		Sound s = findSound("Idle");
		s.audioSource.clip = IdleSounds[Random.Range(0, IdleSounds.Count)];
		s.audioSource.Play();
	}

	public void PlayChaseSound() {
		StopSound("Idle");
		Sound s = findSound("Chase");
		s.audioSource.PlayOneShot(ChaseSounds[Random.Range(0, ChaseSounds.Count)]);
	}
}
