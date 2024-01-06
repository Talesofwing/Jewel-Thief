using UnityEngine;
using System.Collections;

public class PlayerInfo {

	private float bgmVolume;
	private float sfxVolume;
	private int bestScore;

	public PlayerInfo (float bgmVolume, float sfxVolume, int bestScore) {
		BGMVolume = bgmVolume;
		SFXVolume = sfxVolume;
		BestScore = bestScore;
	}

	public float BGMVolume {
		set { bgmVolume = value; Save (); }
		get { return bgmVolume; }
	}

	public float SFXVolume {
		set {sfxVolume = value; Save (); }
		get { return sfxVolume; }
	}

	public int BestScore {
		set { bestScore = value; Save (); }
		get { return bestScore; }
	}

	private void Save () {
		PlayerPrefs.SetFloat (SaveController.SAVE_BGMVolumeKey, bgmVolume);
		PlayerPrefs.SetFloat (SaveController.SAVE_SFXVolumeKey, sfxVolume);
		PlayerPrefs.SetInt (SaveController.SAVE_BestScoreeKey, bestScore);
	}

	public override string ToString () {
		return string.Format ("PlayerInfo : BGM volume : {0}, SFX volume : {1} , BestScore : {2}", BGMVolume, SFXVolume, BestScore);
	}

}
