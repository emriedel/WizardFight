using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour {
	private float ts;
	public float totalTime = 5.0f;
	public GameObject background;
	// Use this for initialization
	void Start() {
		ts = Time.time;
	}
	public void RestartFade() {
		ts = Time.time;
	}
    /*
	void Update() {
		try {
			var image = background.GetComponent<RawImage>();
			image.color = new Vector4(0,0,0,0.0f + 1.0f * ((Time.time - ts) / totalTime));
		} catch {
		}
	}
    */
	public void StartGame() {
		SceneManager.LoadScene("_main_scene");
	}
	public void QuitGame() {
		Application.Quit();
	}
}
