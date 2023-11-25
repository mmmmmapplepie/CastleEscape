using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour {
	public List<Sound> sounds;
	public bool UseClipNames = true;
	protected virtual void Awake() {
		foreach (Sound sound in sounds) {
			AudioSource src = sound.audioSource = gameObject.AddComponent<AudioSource>();
			if (sound.clip != null && UseClipNames) {
				sound.Name = sound.clip.name;
			}
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
	public void StopSoundsAndRoutines(string name, bool stopAll = false) {
		if (stopAll) {
			StopFadeRoutines("", true);
			foreach (Sound s in sounds) {
				s.audioSource.Stop();
			}
		} else {
			StopFadeRoutines(name);
			findSound(name).audioSource.Stop();
		}
	}
	public Sound findSound(string name) {
		return sounds.Find(x => x.Name == name);
	}
	public bool CheckPlaying(string name) {
		return findSound(name).audioSource.isPlaying;
	}
	public void ChangeLoop(string name, bool LoopIsTrue = true) {
		findSound(name).audioSource.loop = LoopIsTrue ? true : false;
	}
	List<FadedSounds> fadeSounds = new List<FadedSounds>();
	public void PlaySound(string name, float fadeInTime = 0f, bool stopAllRoutines = false, float volume = 1f, bool limitVolume = true) {
		Sound sound = findSound(name);
		if (sound == null) return;
		StopFadeRoutines(name, stopAllRoutines);
		float vol = sound.volume;
		if (limitVolume) {
			volume *= vol;
		}
		if (fadeInTime == 0) {
			sound.audioSource.volume = volume;
			sound.audioSource.Play();
		} else {
			FadedSounds fadeSound = new FadedSounds(StartCoroutine(FadeInRoutine(sound, fadeInTime, volume)), name);
			fadeSounds.Add(fadeSound);
		}
	}
	IEnumerator FadeInRoutine(Sound sound, float fadeInTime, float volumeFinal) {
		float InitialVol = sound.audioSource.volume;
		float VolDiff = volumeFinal - InitialVol;
		float StartTime = Time.time;
		sound.audioSource.Play();
		while (Time.time < StartTime + fadeInTime) {
			float ratio = ((Time.time - StartTime - fadeInTime) / fadeInTime) + 1f;
			sound.audioSource.volume = InitialVol + VolDiff * ratio;
			yield return null;
		}
		sound.audioSource.volume = volumeFinal;
	}
	public void StopSound(string name, float fadeOutTime = 0f, bool stopAllRoutines = false) {
		Sound sound = findSound(name);
		if (sound == null) return;
		StopFadeRoutines(name, stopAllRoutines);
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
	public void StopFadeRoutines(string name, bool stopAllRoutines = false) {
		List<FadedSounds> fadesounds = null;
		if (stopAllRoutines) {
			fadesounds = fadeSounds;
		} else {
			fadesounds = fadeSounds.FindAll(x => x.Name == name);
		}
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
	public void changeVolume(string name, float targetVolume = 1f, float changeTime = 0f, bool stopAllRoutines = false, bool limitVolume = true) {
		Sound sound = findSound(name);
		if (sound == null || !sound.audioSource.isPlaying) return;
		float volM = sound.volume;
		if (limitVolume) {
			targetVolume *= volM;
		}
		StopFadeRoutines(name, stopAllRoutines);
		if (changeTime == 0f) {
			sound.audioSource.volume = targetVolume;
		} else {
			FadedSounds fadeSound = new FadedSounds(StartCoroutine(VolumeRoutine(sound, targetVolume, changeTime)), name);
			fadeSounds.Add(fadeSound);
		}
	}
	IEnumerator VolumeRoutine(Sound sound, float FinalVol, float changeTime) {
		float InitialVol = sound.audioSource.volume;
		float VolDiff = FinalVol - InitialVol;
		float StartTime = Time.time;
		while (Time.time < StartTime + changeTime) {
			float ratio = ((Time.time - StartTime - changeTime) / changeTime) + 1f;
			sound.audioSource.volume = InitialVol + VolDiff * ratio;
			yield return null;
		}
		sound.audioSource.volume = FinalVol;
	}
	public void PauseOrResumeSound(string name, bool pause) {
		Sound sound = findSound(name);
		if (sound == null) return;
		if (pause) { sound.audioSource.Pause(); } else {
			sound.audioSource.UnPause();
		}
	}
}
