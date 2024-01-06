using UnityEngine;

public class SoundController : MonoBehaviour {

	private static GameObject audioGO;

	private static AudioSource bgmAudio;
	// 音樂播放器
	private static AudioSource sfxAudio;
	// 音效播放器

	public static void Setup () {

		if (audioGO != null) return;

		audioGO = new GameObject ("SoundController");
		DontDestroyOnLoad (audioGO);
		sfxAudio = audioGO.AddComponent <AudioSource> ();
		bgmAudio = audioGO.AddComponent <AudioSource> ();
		audioGO.AddComponent <AudioListener> ();
		bgmAudio.volume = SaveController.PLAYER_INFO.BGMVolume;
		sfxAudio.volume = SaveController.PLAYER_INFO.SFXVolume;
	}

	// 設置背景音量
	public static void SetBGMVolume () {
		bgmAudio.volume = bgmAudio.volume == 1 ? 0 : 1;
		SaveController.PLAYER_INFO.BGMVolume = bgmAudio.volume;
	}

	// 設置音效音量
	public static void SetSFXVolume () {
		sfxAudio.volume = sfxAudio.volume == 1 ? 0 : 1;
		SaveController.PLAYER_INFO.SFXVolume = sfxAudio.volume;
	}

	// 播放SFX
	public static void PlaySFX (AudioClip clip) {
		sfxAudio.PlayOneShot (clip, sfxAudio.volume);
	}

	public static void PlaySFX (AudioClip clip, float volume) {
		sfxAudio.PlayOneShot (clip, volume);
	}

	// 播放BGM
	public static void PlayBGM (AudioClip clip, bool loop) {

		if (bgmAudio.clip != clip) {
			bgmAudio.clip = clip;
			bgmAudio.loop = loop;
			bgmAudio.Play ();
		}

	}

}
