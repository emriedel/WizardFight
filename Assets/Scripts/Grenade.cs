using UnityEngine;
using System.Collections;
using System;

public class Grenade : MonoBehaviour {

	public Vector3 direction;
	public bool mostRecentAction;
	public int initialFlightBonus = 0;
	public int movesPerAction = 2;
	public bool superCharged = false;
	public bool bounceShot = false;
	public int bounceShotRange = 3;
    public Player owner;
	public GameObject explosionPrefab;
    public AudioClip fwoosh;

    /****************************/

    bool moving = false;
	float lerpPosition = 0.0f;
	Vector3 startPos;
	Vector3 endPos;
	bool moveAgain = false;
	int numMoves = 0;
    AudioSource audioS;
    // int bounces = 0;

    GameObject sprite;

    void Start()
    {
        audioS.PlayOneShot(fwoosh, 1.0f);
    }

    // Use this for initialization
    void Awake() {
		sprite = transform.Find("Sprite").gameObject;
        owner = GameManager.S.getCurrentPlayer();
        audioS = GetComponent<AudioSource>();
    }

	// Update is called once per frame
	void Update() {
		if (moving) {
			move();
			return;
		}
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {

            Player p = col.gameObject.GetComponent<Player>();
            if (p != owner)
            {
                int dist = (superCharged) ? 2 : 1;
                p.Knockback(dist, direction);
            }
        }
        else if (col.tag == "Grenade")
        {
            Grenade g = col.gameObject.GetComponent<Grenade>();
            if (!superCharged && !bounceShot)
            {
                explode();
            }
        }
    }

    void OnTriggerStay(Collider col)
    {
        if (col.tag == "Player")
        {
            Player p = col.gameObject.GetComponent<Player>();
            if (p != owner)
            {
                p.Damage();
                explode();
            }
        }
    }

	public void startMove() {
		updateSprite();
		startPos = transform.position;
		endPos = startPos + direction;
		moving = true;
	}

	void move() {
		float lerpTime = (superCharged || bounceShot) ? GameManager.S.superchargedLerpTime : GameManager.S.normalLerpTime;
		lerpPosition += Time.deltaTime / lerpTime;
		transform.position = Vector3.Lerp(startPos, endPos, lerpPosition);

		if (transform.position == endPos) {
			moving = false;
			lerpPosition = 0.0f;

			if (bounceShot) {
                /*
				if (numMoves++ < bounceShotRange) {
                    print(numMoves);
					startMove();
				}
                else
                {
                    print("STOP");
                    bounceShot = false;
                }
                */
                movesPerAction = 3;
			}

			//Check if landed on rubber band tile
			if (moveAgain) {
				moveAgain = false;
				startMove();
				return;
			}

			if (initialFlightBonus > 0) {
				initialFlightBonus--;
				startMove();
				return;
			}

			if (++numMoves < movesPerAction) {
				startMove();
				return;
			}
            else
            {
                bounceShot = false;
                movesPerAction = 1;
            }

			if (mostRecentAction) {
				GameManager.S.actionDone = true;
				mostRecentAction = false;
				superCharged = false;
			}

			GameManager.S.GrenadeDoneMoving();
            owner = null;
			numMoves = 0;
		}
	}

	void updateSprite() {
		if (direction == Vector3.up) {
			sprite.transform.rotation = Quaternion.Euler(0, 0, 90);
		} else if (direction == Vector3.left) {
			sprite.transform.rotation = Quaternion.Euler(0, 0, 180);
		} else if (direction == Vector3.down) {
			sprite.transform.rotation = Quaternion.Euler(0, 0, 270);
		} else if (direction == Vector3.right) {
			sprite.transform.rotation = Quaternion.Euler(0, 0, 0);
		}
	}

	public void beBounced(Vector3 dir) {
		// bounces++;
        if (bounceShot)
        {
            numMoves = 0;
        }
		direction = dir;
		moveAgain = true;
	}

	void explode() {
        GameObject explosionGO = (GameObject)Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        GameManager.S.outstandingExplosions++;
        GameManager.S.GrenadeDoneMoving();
        // GameManager.S.grenadeCount[GameManager.S.currentPlayer]--;
		GameManager.S.grenades.Remove(gameObject);
		Destroy(gameObject);
	}
}
