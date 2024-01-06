using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LaunchController : MonoBehaviour {

	private Launch launch;

	void Start () {
		// 設置幀率
		Application.targetFrameRate = 60;

		launch = GetComponent <Launch> ();

		SaveController.Setup ();

		Setup ();
	}

	void Setup () {
		launch.Layout ();

		#if UNITY_IPHONE
			StartCoroutine (DetectSplashScreen ());
		#else
			launch.StartLaunchAnimation ();
		#endif
	}

	// 檢測Splash Screen是否已經顯示完畢, 如果是才繼續執行動畫
	IEnumerator DetectSplashScreen () {

		while (Application.isShowingSplashScreen) {
			yield return null;
		}

		launch.StartLaunchAnimation ();
	}

}
