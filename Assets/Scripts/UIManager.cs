using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {
	int playerGuiHeight = 175;
	int playerGuiWidth = 125;
	int textHeight = 25;
	int offset = 35;

	int selGridInt = 0;
	int tiletype = 0;
	bool tileTypeSelected = false;
	public static UIManager S;
	void Awake() {
		S = this;

	}
	void OnGUI() {
		/*
        //Player info GUI
        GUI.BeginGroup(new Rect(10, Screen.height - playerGuiHeight + 10, playerGuiWidth+10, Screen.height - 10));
        GUI.Box(new Rect(0, 0, playerGuiWidth, playerGuiHeight), "");
        GUI.Label(new Rect(10, 10, playerGuiWidth, textHeight), "Player " + getPlayer() + "'s Turn");
        GUI.Label(new Rect(10, offset+10, playerGuiWidth, textHeight), "Actions Left: " + getActions());
        GUI.Label(new Rect(10, offset*2+10, playerGuiWidth, textHeight), "Grenades Left: " + getGrenades());
        GUI.Label(new Rect(10, offset*3+10, playerGuiWidth, textHeight), "Bouncers Left: " + getWalls());
        GUI.EndGroup();
		*/

		string[] selStrings = new string[] {
						getMoveAction() + "\n" + getActions () + " Remaining\n" + getMovementLevel () + "/3 Charged",
						getFireballAction() + "\n" + getGrenades () + " Remaining\n" + getFireballLevel () + "/3 Charged",
						getWallAction() + "\n" + getWalls () + " Remaining\n" + getWallLevel () + "/3 Charged"
				};
        
		selGridInt = GUI.SelectionGrid(new Rect(10, 10, Screen.width - 20, 60), selGridInt, selStrings, 3);

		if (selGridInt == 0) {
			GameManager.S.placeObj = GameManager.placeObject.move;
		} else if (selGridInt == 1) {
			GameManager.S.placeObj = GameManager.placeObject.grenade;
		} else if (selGridInt == 2) {
			if (getWallAction() == "Bounce Shot!") {
				GameManager.S.placeObj = GameManager.placeObject.bounceShot;
			} else {
				string[] tiletypes = new string[]
				{
								"\\", "/"
				};
				tiletype = GUI.SelectionGrid(new Rect(Screen.width * (3f / 4f), 70, Screen.width / 5, 40), tiletype, tiletypes, 2);

				if (tiletype == 0) {
					GameManager.S.placeObj = GameManager.placeObject.backwardBand;
				} else if (tiletype == 1) {
					GameManager.S.placeObj = GameManager.placeObject.forwardBand;
				}
			}
		}
	}


	string getActions() {
		return GameManager.S.remainingMoves.ToString();
	}

	string getGrenades() {
		return GameManager.S.remainingGrenades.ToString();
	}

	string getWalls() {
		return GameManager.S.remainingBands.ToString();
	}

	string getPlayer() {
		if (GameManager.S.currentPlayer == 0) {
			return "Red";
		} else if (GameManager.S.currentPlayer == 1) {
			return "Blue";
		} else {
			return "Green";
		}
	}


	string getWallLevel() {
		return "" + GameManager.S.players[GameManager.S.currentPlayer].GetComponent<Player>().wallLevel;
	}

	string getFireballLevel() {
		return "" + GameManager.S.players[GameManager.S.currentPlayer].GetComponent<Player>().fireballLevel;
	}

	string getMovementLevel() {
		return "" + GameManager.S.players[GameManager.S.currentPlayer].GetComponent<Player>().movementLevel;
	}

	string getMoveAction() {
		if (getMovementLevel() == "3") {
			return "Dash!";
		} else return "Move";
	}

	string getFireballAction() {
		if (getFireballLevel() == "3") {
			return "Super Fireball!";
		} else return "Fireball";
	}

	string getWallAction() {
		if (getWallLevel() == "3") {
			return "Bounce Shot!";
		} else return "Bounce Tile";
	}
}
