using UnityEngine;

public class EventSystemDontDestroy : MonoBehaviour {

	void Awake () {
		DontDestroyOnLoad (this);
	}

}
