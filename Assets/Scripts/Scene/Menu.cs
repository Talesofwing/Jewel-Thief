using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Menu : Scene {
	[SerializeField] private RectTransform mask;				// 遮罩
	[SerializeField] private RectTransform canvas;				// ui幕布
	[SerializeField] private RectTransform background;			// 背景

	[SerializeField] private RectTransform aboutUs;				// 關於我們按鈕
	[SerializeField] private RectTransform play;				// 開始游戲按鈕

	[SerializeField] private RectTransform nameImage;			// 名字圖片
	[SerializeField] private RectTransform body;				// 身體圖片

	[SerializeField] private float distance;					// 動畫距離

	[SerializeField] private AnimationInfo animationInfo;		// 動畫資料

	public bool inAnim = false;

	public void Setup () {
		// 取消開始游戲按鈕功能
		play.GetComponent <Button> ().enabled = false;

		Setup (canvas.sizeDelta.x, canvas.sizeDelta.y);

		Layout ();
	}

	void Layout () {
		// background
		background.sizeDelta = canvas.sizeDelta;
		// about us
		aboutUs.localPosition = new Vector3(aboutUs.localPosition.x * scalingW, aboutUs.localPosition.y * scalingH, 0f);
		// fade color
		Color fadeColor = Color.white;
		fadeColor.a = 0f;
		// name
		nameImage.localPosition = new Vector3 (nameImage.localPosition.x - distance, nameImage.localPosition.y, 0f);
		nameImage.GetComponent <Image> ().color = fadeColor;
		// body
		body.localPosition = new Vector3 (body.localPosition.x + distance, body.localPosition.y, 0f);
		body.GetComponent <Image> ().color = fadeColor;
		// play
		play.localPosition = new Vector3 (play.localPosition.x, play.localPosition.y - distance, 0f);
		play.GetComponent <Image> ().color = fadeColor;
	}

	public void StartMenuAnimation () {
		StartCoroutine (BeginningMenuAnimation ());
	}

	IEnumerator BeginningMenuAnimation () {
		yield return new WaitForSeconds (0.1f);
		StartCoroutine (FadeInAndMoveAnimationWithGameObject (nameImage, new Vector2 (distance, 0f)));			
		yield return new WaitForSeconds (0.7f);
		StartCoroutine (FadeInAndMoveAnimationWithGameObject (body, new Vector2 (-distance, 0f)));
		yield return new WaitForSeconds (0.8f);
		StartCoroutine (FadeInAndMoveAnimationWithGameObject (play, new Vector2 (0f, distance)));
	}

	IEnumerator FadeInAndMoveAnimationWithGameObject (RectTransform go, Vector2 pos) {
		inAnim = true;
		float timer = 0f;

		Color fadeInColor = go.GetComponent <Image> ().color;
		fadeInColor.a = 1f;
		Vector3 movePos = go.localPosition;
		movePos.x += pos.x;
		movePos.y += pos.y;

		while (timer <= animationInfo.duration) {
			timer += Time.deltaTime * animationInfo.speed;
			go.localPosition = Vector3.Lerp (go.localPosition, movePos, timer / animationInfo.duration);
			go.GetComponent <Image> ().color = Color.Lerp (nameImage.GetComponent <Image> ().color, fadeInColor, timer / animationInfo.duration);

			yield return null;
		}

		inAnim = false;

		if (go.GetComponent <Button> () != null)
			// 開啟開始游戲按鈕功能
			play.GetComponent <Button> ().enabled = true;
	}

	public void StartTransition () {
		StartCoroutine (TransitionAnimation ());
	}

	IEnumerator TransitionAnimation () {
		inAnim = true;
		float timer = 0f;

		while (timer <= animationInfo.duration) {
			timer += Time.deltaTime * animationInfo.speed;


			mask.sizeDelta = Vector2.Lerp (mask.GetComponent <RectTransform> ().sizeDelta, new Vector2 (0f, 0f), timer);

			yield return null;
		}

		mask.sizeDelta = new Vector2 (0f, 0f);

		// 場景切換
		SceneManager.LoadScene ("Game");
	}

}
