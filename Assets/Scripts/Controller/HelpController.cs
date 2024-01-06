using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class HelpController : MonoBehaviour {
	[SerializeField] private Transform dotSpawn;
	[SerializeField] private Transform pageSpawn;
	[SerializeField] private RectTransform canvas;
	[SerializeField] private GameObject[] helpsPref;
	[SerializeField] private Sprite dot;
	[SerializeField] private Sprite currentDot;
	[SerializeField] private float distance = 50;
	[SerializeField] private float ratio = 5;

	[SerializeField] private AnimationInfo nextPageAnimation;

	private Transform[] dots;
	private Transform[] pages;
	private GameObject hightlightDot;
	private GameObject[] helps;
	private int currentPage = 0;

	public static event Action OnNotNextPage;

	void Start () {
		Setup ();
	}

	void Setup () {
		dots = new Transform[helpsPref.Length];
		helps = new GameObject[helpsPref.Length];

		CreateTeachImages ();
		CreateDots ();
		CreateHightlightDot ();
	}

	// 創建图片
	void CreateTeachImages () {
		float sizeX = canvas.sizeDelta.x;

		for (int i = 0; i < helpsPref.Length; i++) {
			GameObject go = Instantiate(helpsPref [i]) as GameObject;
			go.transform.SetParent (pageSpawn);
			go.transform.localPosition = new Vector3 (i * sizeX, 0f, 0f);
			go.transform.localScale = new Vector3 (1f, 1f, 1f);
			helps [i] = go;
		}

	}

	// 創建點
	void CreateDots () {
		float firstX = 0;
		float offsetY = 0;
		if (helpsPref.Length % 2 == 0) {
			// 偶數頁
			firstX = - (distance * (int) (helpsPref.Length / 2) - distance / 2);
		} else {
			// 奇數頁
			firstX = - distance * (int) (helpsPref.Length / 2);
		}

		offsetY = canvas.sizeDelta.y / ratio;

		for (int i = 0; i < helpsPref.Length; i++) {
			GameObject dot = new GameObject ("Dot_" + (i + 1));
			dot.transform.SetParent (dotSpawn);
			dot.AddComponent <Image> ().sprite = this.dot;
			dot.transform.localPosition = new Vector3 (firstX + i * distance, -offsetY, 0f);
			dot.transform.localScale = new Vector3 (1f, 1f, 1f);
			dot.GetComponent <RectTransform> ().sizeDelta = this.dot.rect.size;
			dots [i] = dot.transform;
		}

	}

	// 創建高亮點
	void CreateHightlightDot () {
		hightlightDot = new GameObject ("HightlightDot");
		hightlightDot.transform.SetParent (dotSpawn);
		hightlightDot.AddComponent <Image> ().sprite = currentDot;
		hightlightDot.transform.localPosition = dots [0].localPosition;
		hightlightDot.transform.localScale = new Vector3 (1f, 1f, 1f);
		hightlightDot.GetComponent <RectTransform> ().sizeDelta = currentDot.rect.size;
	}

	// 下一頁
	void NextPage () {
		currentPage++;

		if (currentPage < helpsPref.Length) {
			StartCoroutine (DotMoveAnimation ());
			StartCoroutine (PageMoveAnimation ());
		} else {
			Exit ();
		}

	}

	// 開啟動畫
	void ActiveHelpAnimation () {
		helps [currentPage].GetComponent <Animator> ().enabled = true;
	}


	// 點移動動畫
	IEnumerator DotMoveAnimation () {
		float timer = 0f;
		Vector3 endPos = dots [currentPage].localPosition;
		Vector3 startPos = hightlightDot.transform.localPosition;
		float lastTime = Time.realtimeSinceStartup;
		while (timer <= nextPageAnimation.duration) {
			timer += (Time.realtimeSinceStartup - lastTime)  * nextPageAnimation.speed;
			hightlightDot.transform.localPosition = Vector3.Lerp (startPos, endPos, timer / nextPageAnimation.duration);
			lastTime = Time.realtimeSinceStartup;
			yield return null;
		}
	}

	// 頁面移動動畫
	IEnumerator PageMoveAnimation () {
		float timer = 0f;
		Vector3 endPos = pageSpawn.localPosition;
		endPos.x = currentPage * canvas.sizeDelta.x;
		Vector3 startPos = pageSpawn.localPosition;
		float lastTime = Time.realtimeSinceStartup;
		while (timer <= nextPageAnimation.duration) {
			timer += (Time.realtimeSinceStartup - lastTime) * nextPageAnimation.speed;
			pageSpawn.localPosition = Vector3.Lerp (startPos, -endPos, timer / nextPageAnimation.duration);
			lastTime = Time.realtimeSinceStartup;
			yield return null;
		}

		ActiveHelpAnimation ();
	}

	// 跳过
	public void Skip () {
		Exit ();	
	}

	// 離開
	void Exit () {
		
		if (OnNotNextPage != null) {
			OnNotNextPage ();
			OnNotNextPage = null;
		}

		SceneManager.UnloadScene ("Help");
	}

	// 點擊
	public void Click () {
		NextPage ();
	}

	// 顯示幫助場景
	public static void Show () {
		SceneManager.LoadScene ("Help", LoadSceneMode.Additive);
	}

}