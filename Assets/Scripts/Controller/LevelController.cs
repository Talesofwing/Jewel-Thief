using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour {
	public static bool levelStart = false;
	public static LevelController Instance;

	void Awake () {
		Instance = this;
	}

	[HideInInspector] public bool gameOver = false;
	// 判斷游戲是否結束

	[SerializeField] private GameObject ballContainer;
	// 球容器
	[SerializeField] private AudioClip catchJewelClip;
	// 抓到寶石音效
	[SerializeField] private AudioClip impactLoseClip;
	// 碰撞失敗音效
	[SerializeField] private AudioClip motionlessLoseClip;
	// 靜止失敗音效
	[SerializeField] private AudioClip gameClip;

	[SerializeField] private BallController ballController;

	// 繩的長度範圍
	private static Vector2 ropeLengthRange = new Vector2 (5f, 6f);
	// 繩的阻尼值範圍
	private static Vector2 ropeDampingRatioRange = new Vector2 (0.6f, 0.6f);
	// 繩的阻尼值每關增加大小
	private float dampingUpPerLevel = 0.02f;
	// 繩的振動頻率範圍
	private static Vector2 ropeFrequencyRange = new Vector2 (3.5f, 3.5f);
	// 繩的振動頻率每關增加大小
	private float frequencyUpPerLevel = 0.1f;

	// 彈簧物件
	[SerializeField] private SpringJoint2D spring;

	[SerializeField] private AnimationInfo winAnimInfo;
	// 勝利動畫的速度曲線
	[SerializeField] private AnimationCurve winAnimCurve;
	// 游戲界面
	[SerializeField] private Game scene;

	private static List<Level> levels = new List<Level> ();

	public static int level = 1;
	// 當前關卡數

	[HideInInspector] public static bool running = false;

	public void Setup () {
		SoundController.PlayBGM (gameClip, true);
		running = true;

		if (level == 1)
			levels.Add (Resources.Load<Level> ("Levels/BlackRoom"));

		if (level == 6)
			levels.Add (Resources.Load<Level> ("Levels/Maya"));
		
		if (level == 11)
			levels.Add (Resources.Load<Level> ("Levels/Lab"));
		
		Layout ();
		SetupProperty ();
		SetupPhysics ();

		ballController.Setup ();
	}

	// 設置屬性
	void SetupProperty () {

		// 關卡6~10 才會產生變化
		if (level > 5 && level <= 10) {
			// 阻尼
			ropeDampingRatioRange.x -= dampingUpPerLevel;
			// 彈性
			ropeFrequencyRange.x -= frequencyUpPerLevel;
		}

	}

	void Layout () {
		int length = levels.Count;
		int randomLevel = Random.Range (0, length);

		if (level == 6)
			randomLevel = 1;
		else if (level == 11)
			randomLevel = 2;

		Level l = levels [randomLevel];
		scene.background.GetComponent <SpriteRenderer> ().sprite = l.background;

		scene.step.GetComponent <SpriteRenderer> ().sprite = l.step;

		RandomStepPos ();
		CreateTreasure (l);
	}

	// 隨機階梯位置
	void RandomStepPos () {
		float randomY = Random.Range (0f, scene.step.GetComponent <SpriteRenderer> ().bounds.size.y / 3);
		scene.step.localPosition = new Vector3 (scene.step.localPosition.x, scene.step.localPosition.y - randomY, 0f);
	}

	void CreateTreasure (Level level) {
		int randomTreasure = Random.Range (0, level.treasures.Length);
		scene.CreateTreasure (level.treasures [randomTreasure]);
	}

	void SetupPhysics () {
		SetSpring ();	
	}
		
	void SetSpring () {
		float distance = Random.Range (ropeLengthRange.x, ropeLengthRange.y);
		float damping = Random.Range (Mathf.Min (ropeDampingRatioRange.x, 1f), Mathf.Min (ropeDampingRatioRange.y, 1f));
		float frequency = Random.Range (ropeFrequencyRange.x, ropeFrequencyRange.y);

		spring.distance = distance * Game.scaling.y;
		spring.dampingRatio = damping;
		spring.frequency = frequency;
	}

	// 失去生命值
	void LevelLose () {
		GameController.life--;

		// 如果還有生命值,即重置關卡,否則游戲失敗
		if (GameController.life <= 0) {
			ResetLevel ();
			GameController.GameOver ();
		} else {
			ReLoadLevel ();
		}

	}

	// 重置關卡
	void ResetLevel () {
		levels.Clear ();
		level = 1;
	}

	// 重載關卡
	void ReLoadLevel () {
		SceneManager.LoadScene ("Game");
	}

	// 沒有球後靜止的失敗
	public void MotionlessLose () {
		running = false;
		// 游戲結束

		// 播放音效
		SoundController.PlaySFX (motionlessLoseClip, 0.7f);
		ballController.MotionlessLose ();
		// 動畫
		StartCoroutine (DelayLose ());
	}

	public void ImpactLose () {
		running = false;
		StartCoroutine (SlowAnimation ());
		SoundController.PlaySFX (impactLoseClip, 0.7f);
		spring.connectedBody = null;
		scene.rope.GetComponent <Rope> ().StartUpAnimation ();
		scene.treasure.GetComponent <Treasure> ().StartDropAnimation ();
		ballController.ImpactLose ();
		StartCoroutine (DelayLose ());
	}

	IEnumerator SlowAnimation () {
		Time.timeScale = 0.3f;
		yield return new WaitForSeconds (0.2f);
		Time.timeScale = 1f;
	}

	// 延遲失敗
	IEnumerator DelayLose () {
		// 等待X秒後,減掉生命值
		float duration = 1.0f;
		yield return new WaitForSeconds (duration);
		// 失去生命
		LevelLose ();
	}


	// 勝利
	public void LevelWin () {
		running = false;
		level++;
		// 加分
		ScoreController.isNewScore = true;
		ballController.LevelWin ();
		SoundController.PlaySFX (catchJewelClip);
		StartWinAnimation ();
	}

	void StartWinAnimation () {
		StartCoroutine (BeginningWinAnimation ());
	}

	// 勝利動畫
	IEnumerator BeginningWinAnimation () {
		float timer = 0f;								// 保存經过的時間
		float duration = winAnimInfo.duration;								// 結束時間
		float speed = winAnimInfo.speed;
		float treasureH = scene.treasure.GetComponent <SpriteRenderer> ().bounds.size.y;
		float endY = 7f * Game.scaling.y + spring.GetComponent <SpringJoint2D> ().distance + treasureH;
		Vector3 endPos = new Vector3 (ballContainer.transform.position.x, endY, 0f);
		Vector3 startPos = ballContainer.transform.position;
		while (timer <= duration) {
			timer += Time.deltaTime * speed;
			ballContainer.transform.position = Vector3.Lerp (startPos, endPos, timer * winAnimCurve.Evaluate (timer) / duration);
			SetJewel ();
			yield return null;
		}
			
		// 重載關卡
		ReLoadLevel ();
	}

	// 當球的手摸到寶石的上半身寶石才開始上去
	void SetJewel () {
		Transform treasure = scene.treasure;
		// 如果珠寶沒有父節點
		if (!treasure.transform.parent) {
			// 計算手的位置是否在寶石的上半身
			// 寶石的上半身位置
			float midJewelH = treasure.transform.position.y - treasure.GetComponent <SpriteRenderer> ().bounds.size.y / 20;
			// 手的位置
			Transform ball = ballController.ball.transform;
			Transform hand = ball.Find ("Off_Hand");
			float handY = hand.position.y;
			if (handY >= midJewelH) {
				// 計算寶石的比例
				Vector3 jewelScale = treasure.transform.localScale;
				jewelScale.x /= ball.localScale.x;
				jewelScale.y /= ball.localScale.y;
				jewelScale.z = 0f;
				// 寶石成為球的子物件
				treasure.SetParent (ballController.ball.transform);
				// 設置寶石的位置以及大小
				treasure.localScale = jewelScale;
				ballController.ChangeBallFaceToWin ();
			}
	
		}
	
	}

}
