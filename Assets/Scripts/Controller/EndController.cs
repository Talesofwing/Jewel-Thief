using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndController : MonoBehaviour {
	[SerializeField] private AudioClip click;					// 點擊音效
	[SerializeField] private AudioClip enter;
	[SerializeField] private End scene;

	void Start () {
		SoundController.PlaySFX (enter);

		Setup ();
	}

	void Setup () {
		scene.Setup ();

		scene.StartEnterAnimation ();
	}

	// 按下繼續按鈕
	public void Continue () {
		SoundController.PlaySFX (click);	

		GameController.ResetGame ();
	}

	// 按下分享按鈕
	public void Shared () {

	}

	IEnumerator WaitCaptureScreenshotAndShare () {
		yield return null;
	}

	public static void Show () {
		SceneManager.LoadSceneAsync ("End", LoadSceneMode.Additive);
	}

	void OnDestroy () {
		Resources.UnloadUnusedAssets ();
	}

}
