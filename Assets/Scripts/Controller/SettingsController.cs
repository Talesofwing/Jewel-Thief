using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsController : MonoBehaviour {
	[SerializeField] private AudioClip click;					// 按鈕點擊音效
	[SerializeField] private Settings settings;

	void Start () {
		Setup ();
	}

	void Setup () {
		settings.Setup ();

		settings.StartEnterAnimation ();
	}

	public static void Show () {
		SceneManager.LoadScene ("Settings", LoadSceneMode.Additive);
	}

	private void Hide () {
		settings.StartCloseAnimation ();
	}

	void OnDestroy () {
		Resources.UnloadUnusedAssets ();
	}

	public void Continue () {
		if (settings._inAnim)
			return;
		
		SoundController.PlaySFX (click);
		settings.StartCloseAnimation ();
	}

	public void Help () {
		if (settings._inAnim)
			return;

		SoundController.PlaySFX (click);
		HelpController.Show ();
	}

	public void BGM () {
		if (settings._inAnim)
			return;
		
		SoundController.PlaySFX (click);
		SoundController.SetBGMVolume ();
		settings.SetAudioImage ();
	}

	public void SFX () {
		if (settings._inAnim)
			return;
		
		SoundController.SetSFXVolume ();
		SoundController.PlaySFX (click);
		settings.SetAudioImage ();
	}

	public void Home () {
		if (settings._inAnim)
			return;

		SoundController.PlaySFX (click);
		GameController.isFirstStart = true;
		SceneManager.LoadScene ("Menu");
	}

}
