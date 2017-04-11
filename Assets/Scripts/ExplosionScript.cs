using UnityEngine;
using System.Collections;

public class ExplosionScript : MonoBehaviour {
	public float maxSize = 3;
	public float startTime = 0;
	public float totalTime = 5;
	public Vector3 startsize;
    public AudioClip boom;
    AudioSource audioS;
    // Use this for initialization
    void Start() {
		startTime = Time.time;
		startsize = transform.localScale;
		Destroy(gameObject, totalTime);
        audioS = GetComponent<AudioSource>();
        audioS.PlayOneShot(boom, 1.0f);
	}

	// Update is called once per frame
	void Update() {
		float e = Time.time - startTime;
		float t = e / totalTime;
		if (t < 0.5) {
			transform.localScale = Vector3.Lerp(startsize, new Vector3(maxSize, maxSize, maxSize), t * 2);
		} else {
			transform.localScale = Vector3.Lerp(new Vector3(maxSize, maxSize, maxSize), startsize, (t - 0.5f) * 2);
		}

	}

    void OnDestroy()
    {
        GameManager.S.outstandingExplosions--;
    }

	/*
	void OnTriggerEnter(Collider col)
	{
			//print("ontriggerenter");
			//print(col.gameObject);
			if (col.gameObject.tag == "Player")
			{
					//do raycast
					RaycastHit hit;
					Ray ray = new Ray(transform.position, col.transform.position);
					if (Physics.Raycast(ray, out hit)){
							//print(hit.transform.gameObject.tag);
							// kill player only if there's line of sight
							if (hit.transform.gameObject.tag == "Player")
							{
									print("ur ded");
									//TileGenerator.Tiles.killOpposingPlayer();
							}
					}
			}
	}
	*/
}
