using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class End : Scene {
	[SerializeField] private RectTransform continueButton;			// 繼續按鈕
	[SerializeField] private RectTransform sharedButton;			// 分享按鈕
	[SerializeField] private RectTransform background;				// 背景
	[SerializeField] private RectTransform body;					// 身體
	[SerializeField] private RectTransform scoreSpawn;
	[SerializeField] private RectTransform canvas;					// end scene ui幕布

	[SerializeField] private Text bestScoreText;					// 最高分文本

	[SerializeField] private Image bodyPic;

	[SerializeField] private Sprite[] digitSprites;				// 數字图片
	[SerializeField] private Sprite picNew;
	[SerializeField] private Sprite picLose;

	[SerializeField] private Color bestScoreColor;				// 破記錄時的分數顏色

	[SerializeField] private float scoreDistance;

	[SerializeField] AnimationInfo scene;

	[HideInInspector] public bool _inAnim;

	private float distance;

	private int bestScore;
	private bool isNewBestScore = false;

	public void Setup () {
		Setup (canvas.sizeDelta.x, canvas.sizeDelta.y);

		sharedButton.GetComponent <Button> ().enabled = false;
		distance = canvas.sizeDelta.x;

		BestScore ();
		Layout ();

		bestScoreText.text = "" + bestScore;
	}

	void BestScore () {
		bestScore = SaveController.PLAYER_INFO.BestScore;

		if (ScoreController.score > bestScore) {
			bestScore = ScoreController.score;
			isNewBestScore = true; 
			SaveController.PLAYER_INFO.BestScore = bestScore;
		}

	}

	void Layout () {
		// 分數
		CreateScore ();

		// 背景
		background.localPosition = new Vector3 (background.localPosition.x - distance, 0f, 0f);
		background.sizeDelta = canvas.sizeDelta;

		// 身體
		body.localPosition = new Vector3 (body.localPosition.x - distance, 0f, 0f);
		body.sizeDelta = new Vector2 (canvas.sizeDelta.x, body.sizeDelta.y * scalingH);

		if (isNewBestScore) {
			bodyPic.sprite = picNew;
		} else {
			bodyPic.sprite = picLose;
		}

		// 分數
		scoreSpawn.localPosition = new Vector3 (scoreSpawn.localPosition.x * scalingW, scoreSpawn.localPosition.y * scalingH, 0f);

		// 最高分
		RectTransform bestScoreTransform = bestScoreText.GetComponent <RectTransform> ();
		bestScoreTransform.localPosition = new Vector3 (bestScoreTransform.localPosition.x * scalingW, bestScoreTransform.localPosition.y * scalingH, 0f);

		// 分享按鈕
		sharedButton.localPosition = new Vector3 (sharedButton.localPosition.x * scalingW - distance, sharedButton.localPosition.y * scalingH, 0f);

		continueButton.localPosition = new Vector3 (continueButton.localPosition.x * scalingW - distance, continueButton.localPosition.y * scalingH, 0f);
	}

	void CreateScore () {
		int score = ScoreController.score;
		int[] digits = ScoreController.digits;
		int length = score.ToString ().Length;
		float firstX;
		if (length % 2 == 0) // 偶數
			firstX = scoreDistance * (int) (length / 2) - scoreDistance / 2;
		else
			firstX = scoreDistance * (int) (length / 2);

		/* 順序
		 * 個位 -> 十位 -> 百位
		 */
		for (int i = 0; i < length ; i ++) {
			GameObject go = new GameObject ("score_" + i);
			go.transform.SetParent (scoreSpawn);
			go.transform.localScale = new Vector3 (1f, 1f, 1f);
			go.transform.localPosition = new Vector3 (firstX - i * scoreDistance, 0f, 0f);
			Sprite digit = digitSprites [digits [i]];
			go.AddComponent <Image> ().sprite = digit;
			go.GetComponent <RectTransform> ().sizeDelta = digit.rect.size;
		}

	}

	public void StartEnterAnimation () {
		StartCoroutine (BeginningEnterAnimation ());
	}

	IEnumerator BeginningEnterAnimation () {
		_inAnim = true;

		float timer = 0f;
		float speed = scene.speed;
		float duration = scene.duration;

		// 顯示背景
		while (timer <= duration) {
			timer += Time.deltaTime * speed;
			background.localPosition = Vector3.Lerp (background.localPosition, new Vector3 (0f, 0f, 0f), timer);
			yield return null;
		}

		// 顯示圖片及分數
		timer = 0f;
		while (timer <= duration) {
			timer += Time.deltaTime * speed;
			body.localPosition = Vector3.Lerp (body.localPosition, new Vector3 (0f, 0f, 0f), timer);
			yield return null;
		}

		// 顯示按鈕
		Vector3 sharedEndPos = sharedButton.localPosition;
		Vector3 continueEndPos = continueButton.localPosition;
		continueEndPos.x += distance;
		sharedEndPos.x += distance;
		timer = 0f;
		while (timer <= duration) {
			timer += Time.deltaTime * speed;
			continueButton.localPosition = Vector3.Lerp (continueButton.localPosition, continueEndPos, timer);
			sharedButton.localPosition = Vector3.Lerp (sharedButton.localPosition, sharedEndPos, timer);
			yield return null;
		}

		_inAnim = false;
	}

}
