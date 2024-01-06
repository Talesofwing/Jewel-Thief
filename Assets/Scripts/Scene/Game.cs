using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Game : Scene {
	[SerializeField] private GameObject treasurePref;

	[SerializeField] private RectTransform mask;
	[SerializeField] private RectTransform maskCanvas;
	[SerializeField] private AnimationInfo maskAnim;

	[SerializeField] private AnimationInfo ballAnim;
	[SerializeField] private GameObject[] ballPrefabs;

	public Transform rope;
	public Transform box;
	public Transform scores;
	public Transform ballfloor;
	public Transform step;
	public Transform background;
	public Transform backgroundParticles;
	public Transform spring;
	public Transform ground;
	public Transform ballSpawner;
	public Transform ballContainer;
	[HideInInspector] public Transform treasure;

	public static Vector3 scaling;

	public void Setup () {
		Setup (Screen.width, Screen.height);

		GetScreenRatioAndSetCamera ();

		Physics2D.gravity = GameController.gravity * Game.scaling.y;

		Layout ();
	}

	void Layout () {
		background.localScale = scaling;

		step.localScale = scaling;
		step.localPosition = new Vector3 (step.localPosition.x * scalingW, step.localPosition.y * scalingH, 0f);

		ballfloor.localScale = scaling;
		ballfloor.localPosition = new Vector3 (ballfloor.localPosition.x * scalingW, ballfloor.localPosition.y * scalingH, 0f);

		box.localScale = scaling;
		float ballfloorH = ballfloor.GetComponent <SpriteRenderer> ().bounds.size.y / 2;
		box.localPosition = new Vector3 (box.localPosition.x * scalingW, ballfloor.localPosition.y + ballfloorH, 0f);

		scores.localScale = scaling;
		scores.localPosition = new Vector3 (scores.localPosition.x * scalingW, scores.localPosition.y * scalingH, 0f);

		for (int i = 1; i < backgroundParticles.GetComponentsInChildren <Transform> ().Length; i++) {
			backgroundParticles.GetComponentsInChildren <Transform> () [i].localScale = new Vector3 (scalingW, scalingH, scalingW);
			backgroundParticles.GetComponentsInChildren <Transform> () [i].localPosition = new Vector3 (backgroundParticles.GetComponentsInChildren <Transform> () [i].localPosition.x * scalingW, 
				backgroundParticles.GetComponentsInChildren <Transform> () [i].localPosition.y * scalingH, 0f);
		}

		spring.localPosition = new Vector3 (0f, spring.localPosition.y * scalingH, 0f);

		ground.localPosition = new Vector3 (0f, ground.localPosition.y * scalingH, 0f);
		ground.localScale = scaling;

		ballSpawner.localPosition = new Vector3 (ballSpawner.localPosition.x * scalingW, ballSpawner.localPosition.y * scalingH, 0f);

		CreateBall ();
	}

	public void CreateTreasure (Sprite treasureSprite) {
		treasure = Instantiate (treasurePref).transform;
		treasure.GetComponent <SpriteRenderer> ().sprite = treasureSprite;
		treasure.localScale = scaling;
		float stepH = step.GetComponent <SpriteRenderer> ().bounds.size.y / 2;
		float treasureH = treasure.GetComponent <SpriteRenderer> ().bounds.size.y / 2;
		treasure.localPosition = new Vector3 (0f, step.localPosition.y + stepH + treasureH, 0f);
	}

	// 獲取螢幕比例
	void GetScreenRatioAndSetCamera () {
		// 設置Camera
		float orthographicSize = Screen.height / 2f / 100f;					// 公式 : 螢幕高度 / 2 / 100, 100為像素轉換到單元的單位, Unity預設的值為100
		Camera.main.orthographicSize = orthographicSize;

		scaling = new Vector3 (scalingW, scalingH, 0f);
	}

	public void StartMaskAnimation () {
		SetMaskPos ();

		StartCoroutine (BeginningMaskAnimation ());
	}

	void SetMaskPos () {
		mask.gameObject.SetActive (true);
		Vector3 maskPos = Camera.main.WorldToScreenPoint (treasure.localPosition);
		mask.position = maskPos;
//		mask.sizeDelta = new Vector2 (maskCanvas.sizeDelta.x, mask.sizeDelta.y + Mathf.Abs (mask.localPosition.y) * 2);
	}

	IEnumerator BeginningMaskAnimation () {
		float timer = 0f;
		float duration = maskAnim.duration;
		float speed = maskAnim.speed;
		Vector3 scale = new Vector3 (30f, 30f, 0f);
		Vector3 startScale = mask.localScale;
		while (timer <= duration) {
			timer += Time.deltaTime * speed;
			mask.localScale = Vector3.Lerp (startScale, scale, timer / duration);
			yield return null;
		}

		DestroyMask ();
	}

	void DestroyMask () {
		GameObject maskCanvasGO = maskCanvas.gameObject;
		GameObject maskGO = mask.gameObject;
		maskCanvas = null;
		mask = null;
		Destroy (maskCanvasGO);
		Destroy (maskGO);
		Resources.UnloadUnusedAssets ();
	}

	void CreateBall () {
		BrokenOrder (1);
		CreateFirstBall ();

		for (int i = 1; i < ballPrefabs.Length; i++) {
			GameObject ball = Instantiate (ballPrefabs [i], ballSpawner.position, ballSpawner.rotation) as GameObject;
			ball.GetComponent <Ball> ().Setup ();
			ball.transform.localScale = new Vector3 (0.5f * scalingW, 0.5f * scalingH, 0f);
			ball.transform.SetParent (ballSpawner);
		}
			
	}

	// 第一顆球比較特別
	void CreateFirstBall () {
		GameObject ball = Instantiate (ballPrefabs [0], spring.position, spring.rotation) as GameObject;
		ball.GetComponent <Ball> ().Setup ();
		ball.transform.localScale = new Vector3 (0.7f * scalingW, 0.7f * scalingH, 0f);

		SetRope (ball.transform);
		spring.GetComponent <SpringJoint2D> ().connectedBody = ball.GetComponent <Rigidbody2D> ();

		ball.transform.SetParent (ballContainer);
		ball.GetComponent <Ball> ().ShowHand ();
	}

	void SetRope (Transform ball) {
		rope.SetParent (ball.transform);
		rope.localScale = new Vector3 (1.5f, 30f, 0f);
		rope.localPosition = new Vector3 (0f, 0f, 0f);
	}

	// 打亂球的順序
	void BrokenOrder (int m) {

		for (int n = 0; n < m; n++) {

			for (int i = 0; i < ballPrefabs.Length; i++) {
				int random = Random.Range (0, ballPrefabs.Length);

				GameObject temp = ballPrefabs [i];
				ballPrefabs [i] = ballPrefabs [random];
				ballPrefabs [random] = temp;
			}

		}

	}

}