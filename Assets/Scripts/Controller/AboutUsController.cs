using UnityEngine;
using UnityEngine.SceneManagement;

public class AboutUsController : MonoBehaviour {
	
	[SerializeField] private About about;

	private bool inHide;

	void Start () {
		Setup ();
	}
		
	void Setup () {
		about.Setup ();

		about.StartFadeInAnimation ();
	}

	void Update () {

		if (Input.GetMouseButton (0)) {

			if (! inHide)
				Hide ();
		}


	}

	public static void Show () {
		SceneManager.LoadScene ("about", LoadSceneMode.Additive);
	}

	private void Hide () {
		inHide = true;
		about.StartFadeOutAnimation ();
	}

	void OnDestroy () {
		Resources.UnloadUnusedAssets ();
	}

}
