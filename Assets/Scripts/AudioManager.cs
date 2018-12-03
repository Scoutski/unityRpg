using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

	public AudioSource[] sfx;
	public AudioSource[] bgm;

	public static AudioManager instance;

	// Use this for initialization
	void Start () {
		instance = this;

		DontDestroyOnLoad(this.gameObject);

		// Sets the volume of all noises at 0%
		AudioListener.volume = 0f;
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void PlaySFX(int soundToPlay) {
		if (soundToPlay < sfx.Length) {
			sfx[soundToPlay].Play();
		} else {
			Debug.LogError("Tried to play SFX outside of array at position: " + soundToPlay);
		}
	}

	public void PlayBGM(int musicToPlay) {
		if (!bgm[musicToPlay].isPlaying) {
			StopMusic();

			if (musicToPlay < bgm.Length) {
				bgm[musicToPlay].Play();
			} else {
				Debug.LogError("Tried to play BGM outside of array at position: " + musicToPlay);
			}
		}
	}

	public void StopMusic() {
		// Ensure no music is playing when starting a new song.
		for (int i = 0; i < bgm.Length; i++) {
			bgm[i].Stop();
		}
	}
}
