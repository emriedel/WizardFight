using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TurnCounterUI : MonoBehaviour {
    private GameObject[] actions = new GameObject[3];
    private Color[] targets = new Color[3];
    public Color player1Color;
    public Color player2Color;
    public Color EmptyColor;
    
	// Use this for initialization
	void Start () {
        actions[0] = transform.Find("Action1").gameObject;
        actions[1] = transform.Find("Action2").gameObject;
        actions[2] = transform.Find("Action3").gameObject;
        for(int i = 0; i < 3; i++) {
            targets[i] = actions[i].GetComponent<Image>().color;
        }
	}
	IEnumerator LerpColor(int pip, Color target) {
        float time = 0;

        var oldColor = targets[pip];
        if (oldColor == target) yield break; ;
        while(time < 1.0f) {
            actions[pip].GetComponent<Image>().color = Vector4.Lerp(oldColor, target, time);
            yield return null;
            time += Time.deltaTime;
        }
        actions[pip].GetComponent<Image>().color = target;
        targets[pip] = target;

    }
	// Update is called once per frame
	void Update () {
        Color c;
        if(GameManager.S.currentPlayer == 0) {
            c = player1Color;
        } else {
            c = player2Color;
        }
        int actionsLeft = GameManager.S.remainingMoves;
        for(int i = 0; i < actionsLeft; i++) {
            StartCoroutine(LerpColor(i, c));
            //actions[i].GetComponent<Image>().color = c;
        }
        for(int i = actionsLeft; i < 3; i++) {
            StartCoroutine(LerpColor(i, EmptyColor));
            //actions[i].GetComponent<Image>().color = EmptyColor;
        }
	}
}
