using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {

	[SerializeField] private AudioClip menuBGM;			// 開始音效
	[SerializeField] private AudioClip clickSFX;		// 點擊音效

	[SerializeField] private Menu menu;

	void Start () {
		Time.timeScale = 1f;

		SoundController.Setup ();

		// 判斷是否為新玩家
		if (SaveController.IsNewPlayer) {
			SaveController.IsNewPlayer = false;
			// 顯示幫助界面
			HelpController.OnNotNextPage += Setup;
		} else {
			// 直接載入場景
			Setup ();
		}
			
	}

	void Setup () {
		// 繪制場景;
		menu.Setup ();
		// 播放音樂及動畫
		PlayMusicAndStartAnimation ();
	}

	void PlayMusicAndStartAnimation () {
		SoundController.PlayBGM (menuBGM, false);
		menu.StartMenuAnimation ();
	}

	// 開始游戲
	public void PlayGame () {

		if (! menu.inAnim) {
			// 播放音效
			SoundController.PlaySFX (clickSFX);
			menu.StartTransition ();
		}

	}

	// 關於我們
	public void ShowAboutUs () {
		AboutUsController.Show ();
	}

}
