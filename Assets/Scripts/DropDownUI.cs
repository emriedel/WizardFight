using UnityEngine;
using System.Collections;

public class DropDownUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void SetVisible(bool status) {
        var template = transform.Find("Template").gameObject;
        template.SetActive(status);
 
    }
}
