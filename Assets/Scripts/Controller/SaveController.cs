using UnityEngine;
using System.Collections;


public class SaveController {
	public readonly static string SAVE_BGMVolumeKey = "BGMVolume";			// 背景音量
	public readonly static string SAVE_SFXVolumeKey = "SFXVolume";			// 效果音量
	public readonly static string SAVE_BestScoreeKey = "BestScore";			// 最高分

	private static PlayerInfo playerInfo;
	private static bool isNewPlayer = true;

	public static PlayerInfo PLAYER_INFO {
		
		get {
			return playerInfo;
		}

	}
		
	public static bool IsNewPlayer {
		
		get {
			return isNewPlayer;
		}

		set {
			isNewPlayer = value;
		}

	}

	public static void Setup () {
		float bgmVolume = 1;
		float sfxVolume = 1;
		int bestScore = 0;
		isNewPlayer = true;

		if (PlayerPrefs.HasKey (SAVE_BGMVolumeKey)) {
			isNewPlayer = false;
			bgmVolume = PlayerPrefs.GetFloat (SAVE_BGMVolumeKey);
			sfxVolume = PlayerPrefs.GetFloat (SAVE_SFXVolumeKey);
			bestScore = PlayerPrefs.GetInt (SAVE_BestScoreeKey);
		}

		playerInfo = new PlayerInfo (bgmVolume, sfxVolume, bestScore);
	}

}
