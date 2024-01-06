using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreController : MonoBehaviour {
	public static int score;
	// 分數
	public static int[] digits;

	[SerializeField] private Transform box;
	[SerializeField] private Transform scoreSpawn;

	[SerializeField] private AudioClip boxClip;

	[SerializeField] private float scoreDistance;

	[SerializeField] private BoxAnimationInfo boxAnim;
	[SerializeField] private ScoreAnimationInfo scoreAnim;

	[HideInInspector] public bool _inAnim;

	public static bool isNewScore;

	Color fadeColor;
	Color showColor;

	private GameObject[] digitsImage;
	private GameObject[] tempDigits;
	[SerializeField] private Sprite[] digitSprites;
	// 數字图片

	private int index;

	public void Setup () {
		digits = new int[] { 0, 0, 0 };
		digitsImage = new GameObject[3];
		tempDigits = new GameObject[3];

		CreateScore ();

		showColor = digitsImage [0].GetComponent <SpriteRenderer> ().color;
		fadeColor = showColor;
		fadeColor.a = 0f;

		if (isNewScore) {
			isNewScore = false;
			AddScore ();
			StartBoxAnimation ();
		}

	}

	void CreateScore () {

		if (index < 3) {
			digitsImage [index] = new GameObject ("Digit_" + (index + 1));
			digitsImage [index].transform.SetParent (scoreSpawn);
			digitsImage [index].transform.localScale = new Vector3 (1f, 1f, 1f);
			digitsImage [index].transform.localPosition = new Vector3 (0f, 0f, 0f);
			digitsImage [index].AddComponent <SpriteRenderer> ().sprite = digitSprites [digits [index]];

			tempDigits [index] = new GameObject ("Digit_Temp_" + (index + 1));
			tempDigits [index].transform.SetParent (scoreSpawn);
			tempDigits [index].transform.localScale = new Vector3 (1f, 1f, 1f);
			tempDigits [index].transform.localPosition = new Vector3 (0f, 0f, 0f);
			tempDigits [index].AddComponent <SpriteRenderer> ().sprite = digitSprites [digits [index]];
			Color fadeColor = tempDigits [index].GetComponent <SpriteRenderer> ().color;
			fadeColor.a = 0f;
			tempDigits [index].GetComponent <SpriteRenderer> ().color = fadeColor;

			index++;
			Layout ();
		}

	}

	void Layout () {
		int length = score.ToString ().Length;
		float firstX = scoreDistance * (int)(length / 2);
		;
		if (length % 2 == 0)
			firstX = scoreDistance * (int)(length / 2) - scoreDistance / 2;

		for (int i = 0; i < length; i++) {

			if (digitsImage [i] != null) {
				digitsImage [i].transform.localPosition = new Vector3 (firstX - i * scoreDistance, digitsImage [i].transform.localPosition.y, 0f);
				tempDigits [i].transform.localPosition = new Vector3 (firstX - i * scoreDistance, digitsImage [i].transform.localPosition.y - scoreAnim.distance, 0f);
			}

		}

	}

	private void CalculateScore () {
		int temp = score;
		// 取得百位數
		int hundred = temp / 100;
		temp -= hundred * 100;
		// 取得十位數
		int ten = temp / 10;
		// 取得個位數
		temp -= ten * 10;
		int unit = temp;

		digits = new int[] { unit, ten, hundred };
	}

	void AddScore () {
		score++;
		CalculateScore ();

		if (score >= 10) {
			CreateScore ();
		} else if (score >= 100) {
			CreateScore ();
		} else if (score == 999) {
			// 999分結束游戲
			GameController.GameOver ();
		}

		SetTempDigitImage ();
		StartScoreAnimation (0);

		if (score % 10 == 0) {
			digitsImage [1].GetComponent <SpriteRenderer> ().color = fadeColor;
			StartScoreAnimation (1);
		}

		if (score % 100 == 0) {
			digitsImage [2].GetComponent <SpriteRenderer> ().color = fadeColor;
			StartScoreAnimation (2);
		}

	}

	void SetTempDigitImage () {

		for (int i = 0; i < score.ToString ().Length; i++) {

			if (tempDigits [i].GetComponent <SpriteRenderer> () != null)
				tempDigits [i].GetComponent <SpriteRenderer> ().sprite = digitSprites [digits [i]];
		}

	}

	void StartScoreAnimation (int unit) {
		StartCoroutine (BeginningScoreAnimation (unit));
	}

	IEnumerator BeginningScoreAnimation (int unit) {
		float timer = 0f;
		float duration = scoreAnim.duration;
		float speed = scoreAnim.speed;
		float distance = scoreAnim.distance;
		float upY = digitsImage [unit].transform.localPosition.y + distance;

		timer = 0f;
		Vector3 endPos = new Vector3 (digitsImage [unit].transform.localPosition.x, upY, 0f);
		Vector3 tempPos = digitsImage [unit].transform.localPosition;
		while (timer <= duration) {
			timer += Time.deltaTime * speed;
			digitsImage [unit].transform.localPosition = Vector3.Lerp (digitsImage [unit].transform.localPosition, endPos, timer / duration);
			digitsImage [unit].GetComponent <SpriteRenderer> ().color = Color.Lerp (digitsImage [unit].GetComponent <SpriteRenderer> ().color, fadeColor, timer / duration);
			tempDigits [unit].transform.localPosition = Vector3.Lerp (tempDigits [unit].transform.localPosition, tempPos, timer / duration);
			tempDigits [unit].GetComponent <SpriteRenderer> ().color = Color.Lerp (tempDigits [unit].GetComponent <SpriteRenderer> ().color, showColor, timer / duration);
			yield return null;
		}
		digitsImage [unit].transform.localPosition = tempDigits [unit].transform.localPosition;
		digitsImage [unit].GetComponent <SpriteRenderer> ().sprite = digitSprites [digits [unit]];
		digitsImage [unit].GetComponent <SpriteRenderer> ().color = showColor;
		tempPos.y -= distance;
		tempDigits [unit].transform.localPosition = tempPos;
		tempDigits [unit].GetComponent <SpriteRenderer> ().color = fadeColor;
	}

	public void StartBoxAnimation () {
		// 播放音效
		SoundController.PlaySFX (boxClip);
		StartCoroutine (BeginningBoxAnimation ());
	}

	// 寶箱動畫
	IEnumerator BeginningBoxAnimation () {
		_inAnim = true;

		// 寶箱放大縮小
		float timer = 0f;
		float duration = boxAnim.duration;
		Vector2 scale = boxAnim.scale;	// 放大縮小比例
		float speed = boxAnim.speed;

		Vector3 startScale = box.localScale;
		// 拉高的大小
		Vector3 hScale = startScale;
		hScale.x -= scale.x * Game.scaling.x;
		hScale.y += scale.y * Game.scaling.y;
		// 拉伸的大小
		Vector3 wScale = startScale;
		wScale.y -= scale.y * Game.scaling.y;
		wScale.x += scale.x * Game.scaling.x;

		// 拉伸
		while (timer <= duration) {
			timer += Time.deltaTime * speed;
			box.localScale = Vector3.Lerp (box.localScale, wScale, timer / duration);
			yield return null;
		}
		// 拉高
		timer = 0f;
		while (timer <= duration) {
			timer += Time.deltaTime * speed;
			box.localScale = Vector3.Lerp (box.localScale, hScale, timer / duration);
			yield return null;
		}
		// 回復原樣
		timer = 0f;
		while (timer <= duration) {
			timer += Time.deltaTime * speed;
			box.localScale = Vector3.Lerp (box.localScale, startScale, timer / duration);
			yield return null;
		}

		_inAnim = false;
	}

}