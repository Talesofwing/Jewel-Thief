using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameController : MonoBehaviour {
	[SerializeField] private int maxLife;

	[SerializeField] private Game scene;
	[SerializeField] private ScoreController scoreController;
	[SerializeField] private LevelController levelController;

	public static Vector2 gravity = new Vector2 (0f, -40f);

	public static int life;		// 當前生命值

	public static bool isFirstStart = true;
	public static bool gameOver;

	public static void ResetGame () {
		isFirstStart = true;
		SceneManager.LoadScene ("Game");
	}

	void GameSetup () {
		ScoreController.score = 0;
		gameOver = false;
		life = maxLife;
		LevelController.level = 1;
	}

	void Start () {
		Setup ();
	}

	void Setup () {
		scene.Setup ();
		levelController.Setup ();
		scoreController.Setup ();

		if (isFirstStart) {
			isFirstStart = false;
			GameSetup ();
			scene.StartMaskAnimation ();
		}

	}

	// 當生命值為0時,即游戲結束
	public static void GameOver () {
		gameOver = true;
		// 顯示結束界面
		EndController.Show ();
	}

	public void Settings () {
		StopGame ();

		SettingsController.Show ();
	}

	void StopGame () {
		Time.timeScale = 0f;
	}

	public static void ContinueGame () {
		Time.timeScale = 1f;
	}

	void OnDestroy () {
		Resources.UnloadUnusedAssets ();
	}

}
