using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBarUI : MonoBehaviour {
    public GameObject player;
    private GameObject healthBar;
    private float curLength;
    private Color lowHealthColor;
    private Color highHealthColor;
    private Color medHealthColor;
	// Use this for initialization
	void Start () {
        healthBar = transform.Find("Health").gameObject;
        curLength = GetComponent<RectTransform>().rect.height;
	}
	IEnumerator LerpHealthBar(float size) {
        if (size == curLength) yield break;
        var oldColor = healthBar.GetComponent<Image>().color;
        float time = 0;
        while (time < 1.0f) {
            float newSize = curLength + (size - curLength) * StaticUtils.easeOutElastic(time);
            //Color newColor = oldColor + (color - oldColor) 
            healthBar.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newSize);
            yield return null;
            time += Time.deltaTime;
        }
        curLength = size;
        healthBar.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
    }
	// Update is called once per frame
	void Update () {
        var totalSize = GetComponent<RectTransform>().rect.height;
        var health = player.GetComponent<Player>().health;
        float newSize = health * (totalSize / 3);
        StartCoroutine(LerpHealthBar(newSize));
        //healthBar.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newSize);
	}
}
