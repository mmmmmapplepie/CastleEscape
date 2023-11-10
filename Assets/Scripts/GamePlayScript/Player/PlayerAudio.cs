using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : AudioPlayer {
	protected override void Awake() {
		base.Awake();
		GameStateManager.GameEnd += stopAtDeathAudio;
		GameStateManager.EnterMenu += stopAllSounds;
	}
	void stopAtDeathAudio() {
		StopSoundsAndRoutines("", true);
		dashS.Stop();
	}
	void stopAllSounds() {
		stopAtDeathAudio();
		walkS.Stop();
		runS.Stop();
		dmgS.Stop();
		landS.Stop();
	}

	[SerializeField] AudioSource walkS;
	[SerializeField] AudioSource runS;
	[SerializeField] AudioSource dmgS;
	[SerializeField] AudioSource landS;
	[SerializeField] AudioSource dashS;
	public enum audioType { walk, run, dmg, land, dash }


	[SerializeField] List<AudioClip> walkingSounds, runningSounds;

	// Update is called once per frame
	public void playOneShotAudio(audioType type, bool soundFootsteps) {
		switch (type) {
			case audioType.walk:
				playWalkSound(soundFootsteps);
				break;
			case audioType.run:
				playRunSound(soundFootsteps);
				break;
			case audioType.dash:
				dashS.PlayOneShot(dashS.clip, dashS.volume);
				break;
			case audioType.land:
				landS.PlayOneShot(landS.clip, landS.volume);
				break;
			case audioType.dmg:
				dmgS.PlayOneShot(dmgS.clip, dmgS.volume);
				break;
		}
	}
	void playWalkSound(bool soundFootsteps) {
		if (!soundFootsteps) return;
		walkS.PlayOneShot(walkingSounds[Random.Range(0, walkingSounds.Count - 1)], walkS.volume);
	}
	void playRunSound(bool soundFootsteps) {
		if (!soundFootsteps) return;
		runS.PlayOneShot(runningSounds[Random.Range(0, runningSounds.Count - 1)], runS.volume);
	}



}
