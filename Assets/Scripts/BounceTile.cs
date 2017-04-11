using UnityEngine;
using System.Collections;

public enum TileDirection {
	UpToRight, UpToLeft, Vertical, Horizontal
}

public class BounceTile : MonoBehaviour {

	public bool destructible = true;
    public AudioClip twang;
    AudioSource audioS;

	int durability = 2;
	SpriteRenderer sprender;
	TileDirection tileDirection;


	// Use this for initialization
	void Awake() {
		sprender = gameObject.GetComponent<SpriteRenderer>();
		destructible = true;
        audioS = GetComponent<AudioSource>();
    }

	void OnTriggerEnter(Collider col) {
		if (col.tag == "Player") {
            audioS.PlayOneShot(twang, 1.0f);
			Player p = col.gameObject.GetComponent<Player>();
			Vector3 currentDirection = p.direction;
			Vector3 newDirection;
			if (tileDirection == TileDirection.UpToRight) {
				newDirection = new Vector3(currentDirection.y, currentDirection.x);
			} else if (tileDirection == TileDirection.UpToLeft) {
				newDirection = new Vector3(-currentDirection.y, -currentDirection.x);
			} else {
				newDirection = currentDirection * -1;
			}
			p.beBounced(newDirection);
		} else if (col.tag == "Grenade") {
            audioS.PlayOneShot(twang, 1.0f);
            Grenade g = col.gameObject.GetComponent<Grenade>();
			if (g.superCharged && destructible) {
				GameManager.S.bounceTiles.Remove(gameObject);
				Destroy(gameObject);
				return;
			}
			Vector3 currentDirection = g.direction;
			Vector3 newDirection;
			if (tileDirection == TileDirection.UpToRight) {
				newDirection = new Vector3(currentDirection.y, currentDirection.x);
			} else if (tileDirection == TileDirection.UpToLeft) {
				newDirection = new Vector3(-currentDirection.y, -currentDirection.x);
			} else {
				newDirection = currentDirection * -1;
			}
			g.beBounced(newDirection);
		}
	}

	void OnTriggerExit(Collider col) {
		if (col.tag == "Grenade") {
			if (destructible && !col.gameObject.GetComponent<Grenade>().bounceShot) {
				--durability;
				sprender.sprite = GameManager.S.bouncetileSprites[(int)tileDirection + 2]; // get the damaged version of the sprite
			}
			if (durability == 0) {
				GameManager.S.bounceTiles.Remove(gameObject);
				Destroy(gameObject);
			}
		}
	}

	public void setDirection(TileDirection dir) {
		tileDirection = dir;
		sprender.sprite = GameManager.S.bouncetileSprites[(int)dir];
	}
}
