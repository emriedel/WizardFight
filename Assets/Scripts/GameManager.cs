using UnityEngine;
//using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	public GameObject[] players = new GameObject[2];
	public List<GameObject> grenades = new List<GameObject>();
	public List<GameObject> bounceTiles = new List<GameObject>();
	public int[] grenadeCount = new int[2];
	public int currentPlayer = 0;

	public int mapWidth = 11;
	public int mapHeight = 9;
	public Vector3[] startingPos;
	public Vector3[] bounceTilePos; // the z component tells us which type it is
	public GameObject tile, player, bounceTile, grenadePrefab;
	Grenade mostRecentGrenade;
	public Sprite defaultSprite, greenSprite, blueSprite, orangeSprite;
	public Sprite[] bouncetileSprites;
	GameObject[,] tileArray;
	SpriteRenderer[,] spriteArray;
	public GameObject gameOverCanvas;
    public GameObject turnChangeCanvas;
    public float turnChangeDuration = 1f;
    float turnChangeStartTime = 0;
    bool turnChange = true;
    bool gameOver = false;
    int loser = 0;

	public static GameManager S;


	public enum placeObject {
		forwardBand,
		backwardBand,
		grenade,
		move,
		bounceShot
	}
	public placeObject placeObj = placeObject.move;

	private int grenadesDone = 0;
	private bool playerMoving = false, canMove = true;
	int spawnedTerrains = 0;


	public float normalLerpTime = 0.35f;
	public float superchargedLerpTime = 0.2f;

	public bool actionDone = false;
	public bool grenadesMoving = false;
	public bool performingAction = false;
	public int maxActions = 3;
	public int maxGrenades = 2;
	public int maxBands = 2;
	public int remainingMoves;
	public int remainingGrenades;
	public int remainingBands;
    public int outstandingExplosions = 0;

	int tileLayerMask;

    LineRenderer lineRenderer;

    void Start() {
		// Position the camera
		float tempZ = Camera.main.transform.position.z;
		Camera.main.transform.position = new Vector3((mapWidth - 1) / 2.0f, (mapHeight) / 2.0f, tempZ);

		tileArray = new GameObject[mapWidth, mapHeight];
		spriteArray = new SpriteRenderer[mapWidth, mapHeight];

		tileLayerMask = LayerMask.GetMask("Tile");

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;

        turnChangeCanvas.SetActive(true);

		// Generate the map
		for (int i = 0; i < mapWidth; i++) {
			for (int j = 0; j < mapHeight; j++) {
				Vector3 pos = new Vector3(i, j, 0);
				tileArray[i, j] = (GameObject)Instantiate(tile, pos, Quaternion.identity);
				spriteArray[i, j] = tileArray[i, j].GetComponent<SpriteRenderer>();
			}
		}

		TileDirection tileDir = TileDirection.UpToLeft;
		// By default, make borders on the grid
		for (int i = 1; i < mapWidth - 1; i++) {
			GameObject go = (GameObject)Instantiate(bounceTile, new Vector3(i, 0), Quaternion.identity);
			go.GetComponent<BounceTile>().setDirection(tileDir);
			go.GetComponent<BounceTile>().destructible = false;
			bounceTiles.Add(go);

			tileDir = (TileDirection)(((int)tileDir + 1) % 2); // switch tiledir to get a zigzag

			go = (GameObject)Instantiate(bounceTile, new Vector3(i, mapHeight - 1), Quaternion.identity);
			go.GetComponent<BounceTile>().setDirection(tileDir);
			go.GetComponent<BounceTile>().destructible = false;
			bounceTiles.Add(go);
		}
		for (int i = 1; i < mapHeight - 1; i++) {
			GameObject go = (GameObject)Instantiate(bounceTile, new Vector3(0, i), Quaternion.identity);
			go.GetComponent<BounceTile>().setDirection(tileDir);
			go.GetComponent<BounceTile>().destructible = false;
			bounceTiles.Add(go);

			tileDir = (TileDirection)(((int)tileDir + 1) % 2); // switch tiledir to get a zigzag

			go = (GameObject)Instantiate(bounceTile, new Vector3(mapWidth - 1, i), Quaternion.identity);
			go.GetComponent<BounceTile>().setDirection(tileDir);
			go.GetComponent<BounceTile>().destructible = false;
			bounceTiles.Add(go);
		}

		// Corners
		GameObject go2 = (GameObject)Instantiate(bounceTile, new Vector3(0, 0), Quaternion.identity);
		go2.GetComponent<BounceTile>().setDirection(TileDirection.UpToLeft);
		go2.GetComponent<BounceTile>().destructible = false;
		bounceTiles.Add(go2);

		go2 = (GameObject)Instantiate(bounceTile, new Vector3(0, mapHeight - 1), Quaternion.identity);
		go2.GetComponent<BounceTile>().setDirection(TileDirection.UpToRight);
		go2.GetComponent<BounceTile>().destructible = false;
		bounceTiles.Add(go2);

		go2 = (GameObject)Instantiate(bounceTile, new Vector3(mapWidth - 1, 0), Quaternion.identity);
		go2.GetComponent<BounceTile>().setDirection(TileDirection.UpToRight);
		go2.GetComponent<BounceTile>().destructible = false;
		bounceTiles.Add(go2);

		go2 = (GameObject)Instantiate(bounceTile, new Vector3(mapWidth - 1, mapHeight - 1), Quaternion.identity);
		go2.GetComponent<BounceTile>().setDirection(TileDirection.UpToLeft);
		go2.GetComponent<BounceTile>().destructible = false;
		bounceTiles.Add(go2);


		// Place bounce tiles according to custom position array
		for (int i = 0; i < bounceTilePos.Length; i++) {
			// we use the z component to determine the type of bounce tile
			// janky, or efficient? who knows?
			TileDirection dir = (TileDirection)bounceTilePos[i].z;
			bounceTilePos[i].z = 0;
			buildBounceTile(bounceTilePos[i], dir);
			//tileArray[(int)bounceTilePos[i].x, (int)bounceTilePos[i].y] = go;
			spawnedTerrains++;
		}

        Invoke("NextTurn", turnChangeDuration);
	}

    public Player getCurrentPlayer()
    {
        return players[currentPlayer].GetComponent<Player>();
    }

    public Player getOtherPlayer()
    {
        return players[(currentPlayer + 1) % 2].GetComponent<Player>();
    }

	public bool PlayerCanMove(int playerNum) {
		if (playerNum != currentPlayer || !canMove) return false;
		return true;
	}
	public void PlayerDie(int player) {
        gameOver = true;
        loser = player;
        players[player].SetActive(false);
        Invoke("EndGame", 1.5f);
	}
    void EndGame()
    {
        string victory = (loser == 1) ? "Player 1 wins!" : "Player 2 wins!";
        gameOverCanvas.transform.GetComponentInChildren<UnityEngine.UI.Text>().text = victory;
        gameOverCanvas.SetActive(true);
        UIManager.S.enabled = false;
        // gameOverCanvas.GetComponent<TitleUI>().RestartFade();
    }
    public void DecrementActions() {
		remainingMoves -= 1;
		if (remainingMoves == 0 && !gameOver) {
            turnChangeCanvas.SetActive(true);
            turnChangeStartTime = Time.time;
            turnChange = true;
            Invoke("NextTurn", turnChangeDuration);
		}
		canMove = true;
	}
    void NextTurn()
    {
        if (gameOver) return;
        remainingMoves = maxActions;
        remainingGrenades = maxGrenades;
        remainingBands = maxBands;
        currentPlayer += 1;
        currentPlayer = currentPlayer % players.Length;
        turnChange = false;
        turnChangeCanvas.SetActive(false);
    }
	public void PlayerStartMoving() {
		playerMoving = true;
	}
	public void PlayerDoneMoving() {
		playerMoving = false;
        // knockback other player
        if (players[currentPlayer].transform.position == players[(currentPlayer + 1) % 2].transform.position)
        {
            Player p = players[currentPlayer].GetComponent<Player>();
            Player otherPlayer = players[(currentPlayer + 1) % 2].GetComponent<Player>();

            otherPlayer.Knockback(1, p.direction);
        }
	}

	public void AddGrenade(int player, GameObject grenade) {
		grenadeCount[player] += 1;
		grenades.Add(grenade);
		//grenadesDone++;
	}
	public void MoveGrenades() {
		grenadesMoving = true;
		grenadesDone = 0;
		foreach (var g in grenades) {
			var grenadeScript = g.GetComponent<Grenade>();
			grenadeScript.startMove();
		}
	}
	public void GrenadeDoneMoving() {
		grenadesDone += 1;
	}

	void Awake() {
		S = this;
	}

	void Update() {
        if (turnChange)
        {
            string turnText = (currentPlayer == 1) ? "P1's Turn!" : "P2's Turn!";
            turnChangeCanvas.transform.GetComponentInChildren<UnityEngine.UI.Text>().text = turnText;
            if (currentPlayer == 0)
            {
                turnChangeCanvas.transform.FindChild("RedColorBox").transform.localPosition = new Vector3(9999, 0, 0);
                turnChangeCanvas.transform.FindChild("BlueColorBox").transform.localPosition = Vector3.zero;
            }
            else
            {
                turnChangeCanvas.transform.FindChild("RedColorBox").transform.localPosition = Vector3.zero;
                turnChangeCanvas.transform.FindChild("BlueColorBox").transform.localPosition = new Vector3(9999, 0, 0);
            }
            return;
        }

		resetTiles();
        Player currPlayer = players[currentPlayer].GetComponent<Player>();
        Player otherPlayer = players[currentPlayer].GetComponent<Player>();

		if (canMove && !turnChange && grenadesDone >= grenades.Count && !playerMoving
            && !performingAction && !currPlayer.showText && outstandingExplosions == 0)
        {
			int moves = 1;
			if (placeObj == placeObject.forwardBand || placeObj == placeObject.backwardBand) {
				moves = 3;
			}
			changeToGreen(players[currentPlayer].transform.position, moves);

			if (currPlayer.wallLevel == 3 && (placeObj == placeObject.forwardBand || placeObj == placeObject.backwardBand)) {
				resetTiles ();
				changeAllToGreen ();
			}
		}

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (!performingAction && Physics.Raycast(ray, out hit, 99999, tileLayerMask)) {
			if (validAction(hit.transform.position)) {
                // draw dotted line arrow
                Vector3 startpoint = currPlayer.transform.position;
                int extraDist = 0;

                if (placeObj == placeObject.move && currPlayer.movementLevel == 3)
                {
                    extraDist = 2;
                    lineRenderer.enabled = true;
                }
                else if (placeObj == placeObject.grenade && currPlayer.fireballLevel == 3)
                {
                    extraDist = 3;
                    lineRenderer.enabled = true;
                }
				else if (placeObj == placeObject.bounceShot)
                {
					//extraDist = 4;
                    //lineRenderer.enabled = true;
                }

                Vector3 endpoint = hit.transform.position + (hit.transform.position - startpoint) * extraDist;

                lineRenderer.SetPositions(new Vector3[] { startpoint, endpoint });

                if (Input.GetMouseButtonUp(0))
                {
                    lineRenderer.enabled = false;
                    performAction(hit.transform.position);
				}
			}
            else
            {
                lineRenderer.enabled = false;
            }
		}
        else
        {
            lineRenderer.enabled = false;
        }

		if (actionDone) {
			if (!grenadesMoving) {
				MoveGrenades();
			}

            // wait for everything
			if (grenadesDone >= grenades.Count && !playerMoving
                && !currPlayer.showText && outstandingExplosions == 0)
            {
                DecrementActions();
                grenadesMoving = false;
				actionDone = false;
				performingAction = false;
				/*
if (mostRecentGrenade != null)
					AddGrenade(currentPlayer, mostRecentGrenade.gameObject);
						*/
				mostRecentGrenade = null;
			}
		}
	}


    public void changeToGreen(Vector3 pos, int movesLeft) {
		if (movesLeft <= 0) {
			return;
		}

		changeToGreenHelper(new Vector3(pos.x, pos.y + 1), movesLeft);
		changeToGreenHelper(new Vector3(pos.x, pos.y - 1), movesLeft);
		changeToGreenHelper(new Vector3(pos.x + 1, pos.y), movesLeft);
		changeToGreenHelper(new Vector3(pos.x - 1, pos.y), movesLeft);
	}

	void changeToGreenHelper(Vector3 pos, int movesLeft) {
		if (pos.x < 0 || pos.x >= mapWidth || pos.y < 0 || pos.y >= mapHeight) {
			return;
		}

		bool changeSprite = true;
		if (placeObj == placeObject.move && tileHasPlayer(pos)) {
			changeSprite = false;
		} else if ((placeObj == placeObject.backwardBand || placeObj == placeObject.forwardBand) &&
			(tileHasGrenade(pos) || tileHasPlayer(pos) || tileHasBand(pos))) {
			changeSprite = false;
		}

		// Change the sprite
		SpriteRenderer spRend = spriteArray[(int)pos.x, (int)pos.y];
		if (changeSprite) {
			if (placeObj == placeObject.move) {
				spRend.sprite = greenSprite;
			} else if (placeObj == placeObject.grenade) {
				spRend.sprite = orangeSprite;
			} else {
				spRend.sprite = blueSprite;
			}
		}
		changeToGreen(new Vector3(pos.x, pos.y), movesLeft - 1);
	}

	void changeAllToGreen() {
		for (int i = 0; i < mapWidth; i++) {
			for (int j = 0; j < mapHeight; j++) {
				Vector3 pos = new Vector3 (i, j);
				if (!tileHasBand (pos) && !tileHasPlayer (pos) && !tileHasGrenade (pos)) {
					spriteArray [i, j].sprite = blueSprite;
				}
			}
		}
	}

	// Reset the map, but not the terrain
	public void resetTiles() {
		for (int i = 0; i < mapWidth; i++) {
			for (int j = 0; j < mapHeight; j++) {
				spriteArray[i, j].sprite = defaultSprite;
			}
		}
	}


	bool tileHasGrenade(Vector3 pos) {
		foreach (GameObject g in grenades) {
			if (pos == g.transform.position) {
				return true;
			}
		}
		return false;
	}

	public bool tileHasPlayer(Vector3 pos) {
		for (int i = 0; i < players.Length; i++) {
			if (pos == players[i].transform.position) {
				return true;
			}
		}
		return false;
	}

	bool tileHasBand(Vector3 pos) {
		foreach (GameObject t in bounceTiles) {
			if (pos == t.transform.position) {
				return true;
			}
		}
		return false;
	}

	bool validAction(Vector3 actionLocation) {
		return (spriteArray[(int)actionLocation.x, (int)actionLocation.y].sprite.name != "DefaultTile");
	}

	void performAction(Vector3 pos) {
		Player p = players[currentPlayer].GetComponent<Player>();
		Vector3 dir = pos - p.transform.position;
		if (placeObj == placeObject.move && !tileHasPlayer(pos)) {
			performingAction = true;
			actionDone = true;
			p.direction = dir;
			p.startMove();
		} else if (placeObj == placeObject.grenade && remainingGrenades > 0) {
			performingAction = true;
			GameObject grenadeGO = (GameObject)Instantiate(grenadePrefab, p.transform.position, Quaternion.identity);
			mostRecentGrenade = grenadeGO.GetComponent<Grenade>();
			mostRecentGrenade.direction = dir;
			if (p.fireballLevel == 3) {
				mostRecentGrenade.initialFlightBonus = 3;
				mostRecentGrenade.superCharged = true;
				p.fireballLevel = 0;
			} else {
				p.fireballLevel++;
				if (p.fireballLevel == 3) {
                    p.setAbilityText("Super Fireball Charged!");
				}
			}
			mostRecentGrenade.mostRecentAction = true;
			// mostRecentGrenade.startMove();
			AddGrenade(currentPlayer, grenadeGO);
			actionDone = true;
			remainingGrenades--;
		} else if (placeObj == placeObject.bounceShot) {
			performingAction = true;
			GameObject grenadeGO = (GameObject)Instantiate(grenadePrefab, p.transform.position, Quaternion.identity);
			mostRecentGrenade = grenadeGO.GetComponent<Grenade>();
			mostRecentGrenade.direction = dir;
			mostRecentGrenade.mostRecentAction = true;
			mostRecentGrenade.bounceShot = true;
			// mostRecentGrenade.startMove();
			AddGrenade(currentPlayer, grenadeGO);
			actionDone = true;

			p.wallLevel = 0;
		} else if (remainingBands > 0) // assuming it's always a bounce tile in this case
			{
			performingAction = true;
			TileDirection tileDir;
			if (placeObj == placeObject.backwardBand) {
				tileDir = TileDirection.UpToLeft;
			} else {
				tileDir = TileDirection.UpToRight;
			}
			buildBounceTile(pos, tileDir);
			if (p.wallLevel == 3) {
				p.wallLevel = 0;
			} else {
				p.wallLevel++;
				if (p.wallLevel == 3) {
                    p.setAbilityText("Wall Placement Charged!");
				}
			}
			remainingBands--;
			actionDone = true;
		}
	}

	void buildBounceTile(Vector3 pos, TileDirection dir) {
		GameObject go = (GameObject)Instantiate(bounceTile, pos, Quaternion.identity);
		go.GetComponent<BounceTile>().setDirection(dir);
		bounceTiles.Add(go);
	}
}
