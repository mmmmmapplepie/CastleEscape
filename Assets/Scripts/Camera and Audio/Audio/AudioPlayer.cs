using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour {
	[SerializeField] List<Sound> sounds;
	void Awake() {
		foreach (Sound sound in sounds) {
			AudioSource src = sound.audioSource = gameObject.AddComponent<AudioSource>();
			src.clip = sound.clip;
			src.priority = sound.priority;
			src.volume = sound.volume;
			src.pitch = sound.pitch;
			src.loop = sound.loop;
			src.playOnAwake = sound.playOnAwake;
			src.spatialBlend = sound.spatialBlend;
			if (src.playOnAwake == true) {
				src.Play();
			} else {
				src.Stop();
			}
		}
	}
	public Sound findSound(string name) {
		return sounds.Find(x => x.clip.name == name);
	}
	public bool CheckPlaying(string name) {
		return findSound(name).audioSource.isPlaying;
	}
	List<FadedSounds> fadeSounds = new List<FadedSounds>();
	public void PlaySound(string name, float fadeInTime = 0f) {
		Sound sound = findSound(name);
		if (sound == null) return;
		StopFadeRoutines(name);
		if (fadeInTime == 0) {
			sound.audioSource.volume = sound.volume;
			sound.audioSource.Play();
		} else {
			FadedSounds fadeSound = new FadedSounds(StartCoroutine(FadeInRoutine(sound, fadeInTime)), name);
			fadeSounds.Add(fadeSound);
		}
	}
	IEnumerator FadeInRoutine(Sound sound, float fadeInTime) {
		float InitialVol = sound.audioSource.volume;
		float FinalVol = sound.volume;
		float VolDiff = FinalVol - InitialVol;
		float StartTime = Time.time;
		sound.audioSource.Play();
		while (Time.time < StartTime + fadeInTime) {
			float ratio = ((Time.time - StartTime - fadeInTime) / fadeInTime) + 1f;
			sound.audioSource.volume = InitialVol + VolDiff * ratio;
			yield return null;
		}
		sound.audioSource.volume = FinalVol;
	}
	public void StopSound(string name, float fadeOutTime = 0f) {
		Sound sound = findSound(name);
		if (sound == null) return;
		StopFadeRoutines(name);
		if (fadeOutTime == 0) {
			sound.audioSource.volume = 0f;
			sound.audioSource.Stop();
		} else {
			FadedSounds fadeSound = new FadedSounds(StartCoroutine(FadeOutRoutine(sound, fadeOutTime)), name);
			fadeSounds.Add(fadeSound);
		}
	}
	IEnumerator FadeOutRoutine(Sound sound, float fadeOutTime) {
		float InitialVol = sound.audioSource.volume;
		float FinalVol = 0f;
		float VolDiff = FinalVol - InitialVol;
		float StartTime = Time.time;
		while (Time.time < StartTime + fadeOutTime) {
			float ratio = ((Time.time - StartTime - fadeOutTime) / fadeOutTime) + 1f;
			sound.audioSource.volume = InitialVol + VolDiff * ratio;
			yield return null;
		}
		sound.audioSource.volume = FinalVol;
		sound.audioSource.Stop();
	}
	void StopFadeRoutines(string name) {
		List<FadedSounds> fadesounds = fadeSounds.FindAll(x => x.Name == name);
		for (int i = 0; i < fadesounds.Count; i++) {
			if (fadesounds[i] != null) {
				StopCoroutine(fadesounds[i].Routine);
				fadesounds[i] = null;
			}
		}
		fadesounds.RemoveAll(x => x == null);
	}
	public class FadedSounds {
		public Coroutine Routine;
		public string Name;
		public FadedSounds(Coroutine routine, string name) {
			this.Routine = routine;
			this.Name = name;
		}
	}
	public void changeVolume(string name, float targetVolume = 1f) {
		Sound sound = findSound(name);
		if (sound == null || !sound.audioSource.isPlaying) return;
		StopFadeRoutines(name);
		sound.audioSource.volume = sound.volume * targetVolume;
	}
	public void PauseOrResumeSound(string name, bool pause) {
		Sound sound = findSound(name);
		if (sound == null) return;
		if (pause) { sound.audioSource.Pause(); } else {
			sound.audioSource.UnPause();
		}
	}
}
