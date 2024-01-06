using UnityEngine;

public class Treasure : MonoBehaviour {

	public BoxCollider2D boxCollider;
	public CircleCollider2D circleCollider;
	public Rigidbody2D rb;

	[SerializeField] private Sprite[] _jewelSprites;		// 珠寶圖片

	// 設置珠寶
	void SetTreasure () {
		boxCollider.enabled = false;
		circleCollider.enabled = true;
		rb.isKinematic = false;
	}

	// 開始掉落動畫
	public void StartDropAnimation () {
		SetTreasure ();	// 設置珠寶

		// 施加向左上方的力
		float randomVerticalForce = Random.Range (-150, -200) * Game.scaling.y;		// 隨機的垂直力
		float randomHorizontalForce = Random.Range (150f, 250f) * Game.scaling.x;		// 隨機的水平力
		float randomTorque = Random.Range (35f, 45) * Game.scaling.x;					// 隨機的扭力
		rb.AddForce (new Vector2 (randomVerticalForce, randomHorizontalForce));
		rb.AddTorque (randomTorque);
	}

}
