using UnityEngine;
using System.Collections;

public enum BallAnimationType {
	BallAnimationBlink,
	BallAniamtionWatch,
	length}
;

public class Ball : MonoBehaviour {
	public float midMass;

	[SerializeField] private Body body;
	// 身體
	[SerializeField] private Hand hand;
	// 手
	// 撞擊粒子
	[SerializeField] private GameObject impactParticle;
	// 撞擊音效
	[SerializeField] private AudioClip impactClip;

	[HideInInspector] public bool isImpacted = false;

	private Vector2 massRange;
	// 質量範圍
	[HideInInspector] public float ballSpeed;
	// 球改變的速度

	private Rigidbody2D rb;
	// 球的刚體

	private bool zoomIn = true;
	// 此bool判斷球是否變大
	private bool stopChange = true;
	// 此bool判斷球是否要停止變化

	public void Setup () {
		rb = GetComponent <Rigidbody2D> ();
		SetupColliderArea ();
		CalculateBallSpeed ();
		CalculateBallMass ();
	}

	// 計算球的速度
	void CalculateBallSpeed () {

		float UpPerLevel = BallController.Instance.ballChangedSpeedUpPerLevel;
		float UpPerTenLevelAfter = BallController.Instance.ballChangedSpeedUpPerTenLevelAfter;

		ballSpeed = BallController.ballChangedSpeedRange.x + UpPerLevel * LevelController.level;
		if (LevelController.level > 10) {
			// 11關開始
			ballSpeed += UpPerTenLevelAfter * (LevelController.level - 10);
		}

		if (ballSpeed > BallController.ballChangedSpeedRange.y)
			ballSpeed = BallController.ballChangedSpeedRange.y;

	}

	// 計算球的質量
	void CalculateBallMass () {
		massRange = BallController.ballMassRange;

		if (LevelController.level > 5 && LevelController.level <= 10) {
			massRange.y += BallController.Instance.ballMassUpPerLevel;
		}

		midMass = (massRange.y + massRange.x) / 2;
	}

	void SetupColliderArea () {
		float scaling;

		if (Game.scaling.x < Game.scaling.y)
			scaling = Game.scaling.x / Game.scaling.y;
		else
			scaling = Game.scaling.y / Game.scaling.x;

		body.col.radius *= scaling;
	}

	public void BallDown () {
		StopBallChange ();
		body.col.enabled = true;
		rb.isKinematic = false;
	}

	public void BallAnimation (BallAnimationType type) {

		if (type == BallAnimationType.BallAniamtionWatch) {
			Blink ();
		} else if (type == BallAnimationType.BallAnimationBlink) {
			Watch ();
		}

	}

	// 球-眨眼
	void Blink () {
		body.animator.SetTrigger ("blink");
	}
	
	// 球-周圍望
	void Watch () {
		body.animator.SetTrigger ("watch");
	}

	public void ShowHand () {
		hand.ShowHand ();
	}

	// 球-普通
	public void SetNormalAnimation () {
		body.animator.SetTrigger ("normal");
	}

	void OnCollisionEnter2D (Collision2D other) {

		if (!LevelController.running) return;
		/* 如果碰撞到的是球
		 * 則產出粒子效果
		 * 並且播放撞擊音效
		 */

		if (other.gameObject.CompareTag ("Ball")) {
			// 粒子效果
			ShowImpactParticle (other.transform.position, Quaternion.Euler (0f, 90f, 0f));	// 右邊
			ShowImpactParticle (other.transform.position, Quaternion.Euler (0f, -90f, 0f));	// 左邊

			if (!isImpacted)
				isImpacted = true;

			// 播放撞擊音效
			SoundController.PlaySFX (impactClip, 0.3f);
		}

	}

	// 顯示撞擊粒子
	void ShowImpactParticle (Vector3 position, Quaternion rotation) {
		GameObject particle = Instantiate (impactParticle, position, rotation) as GameObject;

		particle.GetComponent <ParticleSystem> ().startSize *= Game.scaling.y;
		Destroy (particle, 1f);	// 1秒後銷毀
	}

	public void StartBallChange () {
		stopChange = false;
		rb.mass = midMass;
		StartCoroutine (BeginningBallChange ());
	}

	public void StopBallChange () {
		stopChange = true;
	}

	// 球的大小以及質量變化
	// 球的變化
	IEnumerator BeginningBallChange () {

		while (!stopChange) {
			ChangeBall ();
			yield return null;
		}

	}

	// 改變球的質量以及大小
	void ChangeBall () {
		/* 判斷是否放大
			 *   如果是,增大質量,直到上限,並且把比例*上scale
			 * 	 如果不是,減少質量,直到下限,並且把比例*上scale
			 */

		if (zoomIn) {
			// 放大
			if (rb.mass < massRange.y) {

				// 如果球的質量小於最大質量
				float mass = Time.deltaTime * ballSpeed;
				rb.mass += mass;										// 增加質量

				// 與中值相除,獲得比例
				float massRatio = rb.mass / midMass;

				// 把scale 乘上比例
				transform.localScale = new Vector3 (0.5f * Game.scaling.x * massRatio, 0.5f * Game.scaling.y * massRatio, 0f);
			} else {
				// 如果球的質量大於最大質量
				rb.mass = massRange.y;								// 修正質量為最大質量
				zoomIn = false;										// 縮小
			}

		} else {

			if (rb.mass > massRange.x) {
				float mass = Time.deltaTime * ballSpeed;
				rb.mass -= mass;

				// 與中值相除,獲得比例
				float massRatio = rb.mass / midMass;
				// 把scale 乘上比例
				transform.localScale = new Vector3 (0.5f * Game.scaling.x * massRatio, 0.5f * Game.scaling.y * massRatio, 0f);
			} else {
				rb.mass = massRange.x;
				zoomIn = true;
			}

		}

	}

	// 靜止失敗
	public void MotionLessLose () {
		body.animator.SetTrigger ("lose_motionless");					// 改變身體的圖片
	}

	// 抓住珠寶
	public void CatchJewel () {
		hand.CatchJewel ();		// 改變手
	}

	// 勝利
	public void Win () {
		body.animator.SetTrigger ("win");	// 改變身體
		SetBall (true, true);
	}

	// 設置球的物理屬性
	void SetBall (bool kinematic, bool b) {
		rb.mass = 1f;
		rb.isKinematic = kinematic;
		body.col.enabled = b;
	}
		
	private static int direction = 1;

	// 球碰撞失敗的掉落動畫
	void DropAnimation (int direction) {
		/* direction = 方向
			 * +1 : 右
			 * -1 : 左
			 */
	
		SetBall (false, false);
	
		// 施加向上方的力
		float randomVerticalForce = Random.Range (150f, 350f) * Game.scaling.y;		// 隨機的垂直力
		float randomHorizontalForce = Random.Range (200f, 300f) * Game.scaling.x;		// 隨機的水平力
		float randomTorque = Random.Range (150f, 250f);				// 隨機的扭力
		rb.AddForce (new Vector2 (randomHorizontalForce * direction, randomVerticalForce));
		rb.AddTorque (randomTorque);
	}

	// 撞擊失敗
	public void ImpactLose () {
		SetBall (true, false);
		direction *= -1;
		DropAnimation (direction);			// 播放跌落動畫
		body.animator.SetTrigger ("lose_impact");
	}

}