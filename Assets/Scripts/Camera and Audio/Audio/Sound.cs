using UnityEngine;
[System.Serializable]
public class Sound {
	[HideInInspector]
	public AudioSource audioSource;
	public AudioClip clip;
	[Range(0, 255)]
	public int priority;
	[Range(0, 1)]
	public float volume;
	public float pitch;
	public bool loop;
	public bool playOnAwake;
	[Range(0, 1)]
	public float spatialBlend;

}
