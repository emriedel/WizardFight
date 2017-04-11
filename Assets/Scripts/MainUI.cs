using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainUI : MonoBehaviour {
    public GameObject moveButton;
    public GameObject bounceButton;
    public GameObject fireballButton;
    public GameObject otherMoveBar;
    public GameObject otherBounceBar;
    public GameObject otherFireballBar;
    public Color ChargingColor;
    public Color ChargedColor;
    private GameObject moveProgress;
    private GameObject bounceProgress;
    private GameObject fireballProgress;
    private GameObject otherMoveProgress;
    private GameObject otherFireballProgress;
    private GameObject otherBounceProgress;
    private Text moveText;
    private Text fireballText;
    private Text bounceText;
	// Use this for initialization
	void Start () {
        moveProgress = moveButton.transform.Find("Progress").gameObject;
        bounceProgress = bounceButton.transform.Find("Progress").gameObject;
        fireballProgress = fireballButton.transform.Find("Progress").gameObject;
        moveText = moveButton.transform.Find("Text").gameObject.GetComponent<Text>();
        fireballText = fireballButton.transform.Find("Text").gameObject.GetComponent<Text>();
        bounceText = bounceButton.transform.Find("Text").gameObject.GetComponent<Text>();
        otherMoveProgress = otherMoveBar.transform.Find("Progress").gameObject;
        otherFireballProgress = otherFireballBar.transform.Find("Progress").gameObject;
        otherBounceProgress = otherBounceBar.transform.Find("Progress").gameObject;

		moveButton.GetComponent<Toggle> ().isOn = true;
	}
	
	// Update is called once per frame
	void Update () {
	    float totalWidth = moveButton.GetComponent<RectTransform>().rect.width;
        float wallWidth = getWallLevel() * (totalWidth / 3f);
        float fireballWidth = getFireballLevel() * (totalWidth / 3f);
        float movementWidth = getMovementLevel() * (totalWidth / 3f);
        float otherWallWidth = getOtherWallLevel() * (totalWidth / 3f);
        float otherFireballWidth = getOtherFireballLevel() * (totalWidth / 3f);
        float otherMovementWidth = getOtherMovementLevel() * (totalWidth / 3f);
        moveProgress.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, movementWidth);
        fireballProgress.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, fireballWidth);
        bounceProgress.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, wallWidth);
        otherMoveProgress.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, otherMovementWidth);
        otherFireballProgress.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, otherFireballWidth);
        otherBounceProgress.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, otherWallWidth);
        var moveImage = moveProgress.GetComponent<Image>();
        var bounceImage = bounceProgress.GetComponent<Image>();
        var fireballImage = fireballProgress.GetComponent<Image>();
        var otherMoveImage = otherMoveProgress.GetComponent<Image>();
        var otherBounceImage = otherBounceProgress.GetComponent<Image>();
        var otherFireballImage = otherFireballProgress.GetComponent<Image>();
        otherFireballImage.color = ChargingColor;
        otherBounceImage.color = ChargingColor;
        otherMoveImage.color = ChargingColor;
        bounceImage.color = ChargingColor;
        moveImage.color = ChargingColor;
        fireballImage.color = ChargingColor;
        bounceText.text = "Bounce";
        moveText.text = "Move";
        fireballText.text = "Fireball";
        if (getWallLevel() == 3) {
            bounceImage.color = ChargedColor;
            bounceText.text = "Wall Placement";
        }
        if (getMovementLevel() == 3) {
            moveImage.color = ChargedColor;
            moveText.text = "Dash";
        }
        if (getFireballLevel() == 3) {
            fireballImage.color = ChargedColor;
            fireballText.text = "Super Fireball";
        }
        if(getOtherFireballLevel() == 3) {
            otherFireballImage.color = ChargedColor;
        }
        if(getOtherMovementLevel() == 3) {
            otherMoveImage.color = ChargedColor;
        }
        if(getOtherWallLevel() == 3) {
            otherBounceImage.color = ChargedColor;
        }

	}
    public void OnBackwardTileSelected(bool enabled) {
        GameManager.S.placeObj = GameManager.placeObject.backwardBand;
    }
    public void OnForwardTileSelected(bool enabled) {
        GameManager.S.placeObj = GameManager.placeObject.forwardBand;
    }
    public void OnBounceSelected(bool enabled) {
        var template = bounceButton.transform.Find("Template").gameObject;
        if (getWallLevel() == 3) {
            GameManager.S.placeObj = GameManager.placeObject.bounceShot;
            bounceButton.GetComponent<Toggle>().isOn = false;
        } else {
            template.SetActive(enabled);
        }
    }
    public void OnFireballSelected(bool enabled) {
        GameManager.S.placeObj = GameManager.placeObject.grenade;
    }
    public void OnMoveSelected(bool enabled) {
        GameManager.S.placeObj = GameManager.placeObject.move;
    }
    static int getWallLevel() {
        return GameManager.S.players[GameManager.S.currentPlayer].GetComponent<Player>().wallLevel;
    }

    static int getFireballLevel() {
        return GameManager.S.players[GameManager.S.currentPlayer].GetComponent<Player>().fireballLevel;
    }

    static int getMovementLevel() {
        return GameManager.S.players[GameManager.S.currentPlayer].GetComponent<Player>().movementLevel;
    }
    static int getOtherWallLevel() {
        var mgr = GameManager.S;
        return mgr.players[(mgr.currentPlayer + 1) % 2].GetComponent<Player>().wallLevel;
    }

    static int getOtherFireballLevel() {
        var mgr = GameManager.S;
        return mgr.players[(mgr.currentPlayer + 1) % 2].GetComponent<Player>().fireballLevel;
    }

    static int getOtherMovementLevel() {
        var mgr = GameManager.S;
        return mgr.players[(mgr.currentPlayer + 1) % 2].GetComponent<Player>().movementLevel;
    }
}
