using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PowerBarLabelUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        var player = (GameManager.S.currentPlayer + 1) % 2;
        player += 1;
        GetComponent<Text>().text = "P" + player.ToString();
	}
}
