using UnityEngine;
using System.Collections;



public class BallController : MonoBehaviour {
	private static BallController instance;

	public static BallController Instance {
		get { return instance; }
	}

	[SerializeField] private Transform ballSpawner;
	[SerializeField] private Transform ballContainer;

	[SerializeField] private Vector2 ballAnimDelayTime = new Vector2 (3f, 5f);

	[SerializeField] private SpringAnimationInfo springAnimInfo;

	private BoxCollider2D winArea;

	[SerializeField] private LevelController levelController;

	// 球的質量範圍
	public static Vector2 ballMassRange = new Vector2 (5f, 15f);
	// 球旳質量每關增加大小
	[HideInInspector] public float ballMassUpPerLevel = 1f;
	// 球的變化速度
	public static Vector2 ballChangedSpeedRange = new Vector2 (15f, 80f);
	// 球的變化速度每關卡提升
	// 一至十關的
	[HideInInspector] public float ballChangedSpeedUpPerLevel = 0.5f;
	// 十關後的
	[HideInInspector] public float ballChangedSpeedUpPerTenLevelAfter = 2f;

	public static Ball fallingBall;
	// 正在下降的球
	private Ball currentBall;
	// 準備下降的球
	public Rigidbody2D ball;
	// 第一個球
	private int ballCount;
	// 剩餘球數

	void Awake () {
		instance = this;
	}

	public void Setup () {
		winArea = GameObject.FindWithTag ("Treasure").GetComponent <BoxCollider2D> ();
		fallingBall = null;
		ball = ballContainer.GetChild (0).GetComponent <Rigidbody2D> ();
		ball.GetComponent <Ball> ().BallDown ();
		StartBallMoveAnimation ();
		ballCount = 3;
		animDelay = Random.Range (ballAnimDelayTime.x, ballAnimDelayTime.y);
		StartBallAnimation ();
		StartCheckFallingBallVelocity ();
	}

	void SetupProperty () {
		// 質量

		// 速度在球上設置
	}

	void StartBallAnimation () {
		StartCoroutine (BeginningBallAnimation ());
	}

	float timer;
	float animDelay;

	IEnumerator BeginningBallAnimation () {

		while (true) {
			// 還有後備球
			if (ballSpawner.childCount > 1) {
				timer += Time.deltaTime;

				if (timer >= animDelay) {
					timer = 0f;
					int randomBallIndex = Random.Range (1, ballSpawner.childCount);
					int randomAnimationIndex = Random.Range (0, (int)BallAnimationType.length);
					BallAnimationType randomAnimationType = (BallAnimationType)randomAnimationIndex;
					Ball b = ballSpawner.GetChild (randomBallIndex).GetComponent <Ball> ();
					b.BallAnimation (randomAnimationType);
					// 隨機間隔
					float delay = Random.Range (ballAnimDelayTime.x, ballAnimDelayTime.y);
					animDelay = delay;
				}
					
			}

			yield return null;
		}

	}

	public void BallDown () {

		if (currentBall == null)
			return;

		fallingBall = currentBall;
		currentBall = null;
		fallingBall.SetNormalAnimation ();
		fallingBall.transform.SetParent (ballContainer);
		StartBallMoveAnimation ();
		fallingBall.BallDown ();
		ballCount--;
	}

	// 開始球移動動畫
	public void StartBallMoveAnimation () {
		StartCoroutine (DelayBeginningBallMoveAnimation ());
	}

	// 延遲球移動動畫
	IEnumerator DelayBeginningBallMoveAnimation () {
		
		for (int i = 0; i < ballSpawner.childCount; i++) {
			Transform ball = ballSpawner.GetChild (i);
			StartCoroutine (BeginningBallMoveAnimation (ball, i));
			yield return new WaitForSeconds (0.1f);
		}


	}

	// 球移動動畫
	IEnumerator BeginningBallMoveAnimation (Transform ball, int i) {
		float distance = 0f;							
		float springDistance = springAnimInfo.springDistance.x;				
		float speed = springAnimInfo.speed;		
		float timer = 0f;
		float duration = springAnimInfo.duration;

		Vector3 endPosition;							// 結束位置
		ball.GetComponent <Ball> ().SetNormalAnimation ();

		float x = ball.GetComponentInChildren <SpriteRenderer> ().bounds.size.x;
		if (i != 0)
			distance = 0.35f * Game.scaling.x;

		endPosition = new Vector3 (distance + i * x, ball.transform.position.y, 0f);

		// 彈簧效果距離
		Vector3 springEndPosition = endPosition;
		springEndPosition.x -= springDistance * Game.scaling.x;

		// 彈簧效果
		timer = 0f;
		while (timer <= duration) {
			timer += Time.deltaTime * speed;
			ball.transform.position = Vector3.Lerp (ball.transform.position, springEndPosition, timer);
			yield return null;
		}

		// 回到原來的位置
		timer = 0f;
		while (timer <= duration) {
			timer += Time.deltaTime * speed * 2;
			ball.transform.position = Vector3.Lerp (ball.transform.position, endPosition, timer);
			yield return null;
		}

		if (i == 0) {
			currentBall = ball.GetComponent <Ball> ();
			currentBall.StartBallChange ();
		}

	}

	void StartCheckFallingBallVelocity () {
		StartCoroutine (CheckFallingBallVelocity ());
	}

	bool down = false;
	bool down2 = false;
	bool up = false;
	IEnumerator CheckFallingBallVelocity () {
		
		while (LevelController.running) {
			yield return new WaitForSeconds (0.1f);

			if (ball.velocity.y > 0) {
				// 上升

				winArea.enabled = true;
				// 顯示勝利區域
			} else if (ball.velocity.y < 0) {
				// 下降

				// 隱藏失敗區域
				winArea.enabled = false;
			} else {
				// 靜止
			}

			if (!LevelController.running)
				yield return false;

			if (fallingBall == null) continue;

			// 判斷靜止失敗
			if (fallingBall.isImpacted && ballCount <= 0) {

				if (ball.velocity.y > 0) {
					if (down)
						up = true;

					if (down2 && up)
					{
						levelController.MotionlessLose ();
						yield return false;
					}

				} else if (ball.velocity.y < 0) {
					down = true;

					if (up)
						down2 = true;



				}

			}

		}

	}
		
	public void LevelWin () {
		ball.isKinematic = true;
		ball.GetComponent <Ball> ().CatchJewel ();

		if (currentBall)
			currentBall.StopBallChange ();
	}

	public void ImpactLose () {

		if (currentBall)
			currentBall.StopBallChange ();

		for (int i = 0; i < ballContainer.transform.childCount; i++) {
			ballContainer.transform.GetChild (i).GetComponent <Ball> ().ImpactLose ();
		}

	}

	public void MotionlessLose () {
		
		// 球的靜止失敗
		for (int i = 0; i < ballContainer.transform.childCount; i++) {
			ballContainer.transform.GetChild (i).GetComponent <Ball> ().MotionLessLose ();
		}

	}

	public void ChangeBallFaceToWin () {

		for (int i = 0; i < ballContainer.childCount; i++) {
			Ball b = ballContainer.GetChild (i).GetComponent <Ball> ();
			b.Win ();
		}

	}

}
