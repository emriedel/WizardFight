using UnityEngine;
using System.Collections;
using System;

public class Player : MonoBehaviour {

	public Vector3 direction;
	public int playerNum;
	public int health = 3;
	public int dashBonus = 3;
	public int fireballLevel = 0;
	public int wallLevel = 0;
	public int movementLevel = 0;
    public float textShowDur = 1f;
    public int knockbackDist = 2;
    public bool showText = false;

    /****************************/

    bool moving = false;
	float lerpPosition = 0.0f;
	Vector3 startPos;
	Vector3 endPos;
	bool moveAgain = false;
	int timesMoved = 0;
    float textShowTime = 0;
    string abilityTextText;
    bool knockback = false;

    SpriteRenderer spRend;
    TextMesh abilityText;
	public Sprite redSprite, greenSprite;


	// Use this for initialization
	void Awake() {
		spRend = gameObject.GetComponent<SpriteRenderer>();
        abilityText = gameObject.GetComponentInChildren<TextMesh>();
    }
	// Update is called once per frame
	void Update() {
		if (moving) {
			move();
			return;
		}

		if (playerNum == 0) {
			spRend.sprite = redSprite;
		} else if (playerNum == 1) {
			spRend.sprite = greenSprite;
		}

        if (showText && Time.time - textShowTime < textShowDur)
        {
            // show text
            abilityText.text = abilityTextText;
        }
        else
        {
            showText = false;
            abilityText.GetComponent<Renderer>().enabled = false;
            abilityText.text = "done";
        }

		/*
		if (GameManager.S.PlayerCanMove(playerNum)) {
			if (Input.GetKey (KeyCode.A)) {
				direction = Vector3.left;
				startMove ();
			} else if (Input.GetKey (KeyCode.D)) {
				direction = Vector3.right;
				startMove ();
			} else if (Input.GetKey (KeyCode.W)) {
				direction = Vector3.up;
				startMove ();
			} else if (Input.GetKey (KeyCode.S)) {
				direction = Vector3.down;
				startMove ();
			}
		}
		*/
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player" && !moving)
        {
            Knockback(1, col.gameObject.GetComponent<Player>().direction);
        }
    }

	public void startMove() {
		startPos = transform.position;
		endPos = startPos + direction;
        GameManager.S.PlayerStartMoving();
        moving = true;
	}

	void move() {
		float lerpTime = (movementLevel == 3 || knockback) ? GameManager.S.superchargedLerpTime : GameManager.S.normalLerpTime;
		lerpPosition += Time.deltaTime / lerpTime;
		transform.position = Vector3.Lerp(startPos, endPos, lerpPosition);

		if (transform.position == endPos) {
			moving = false;
			lerpPosition = 0.0f;

			//Check if landed on rubber band tile
			if (moveAgain) {
				moveAgain = false;
				startMove();
				return;
			}

            timesMoved++;
            
            if (knockback)
            {
                if (timesMoved < knockbackDist)
                {
                    startMove();
                    return;
                }
                else
                {
                    knockback = false;
                    timesMoved = 0;
                }
            }
            else if (movementLevel == 3)
            {
                if (timesMoved < dashBonus)
                {
                    startMove();
                    return;
                }
                else
                {
                    movementLevel = 0;
                }
            }
            else {
                movementLevel++;
                if (movementLevel == 3)
                {
                    setAbilityText("Dash Charged!");
                }
            }

			GameManager.S.PlayerDoneMoving();
			timesMoved = 0;
		}
	}

    public void Damage()
    {
        health--;
        if (health <= 0)
        {
            GameManager.S.PlayerDie(playerNum);
        }
    }

    public void Knockback(int dist, Vector3 dir)
    {
        knockbackDist = dist;
        direction = dir;
        if (!moving)
        {
            startMove();
            knockback = true;
        }
    }

	public void beBounced(Vector3 dir) {
		direction = dir;
		moveAgain = true;
	}

    public void setAbilityText(string s)
    {
        showText = true;
        abilityTextText = s;
        abilityText.GetComponent<Renderer>().enabled = true;
        textShowTime = Time.time;
    }
}
