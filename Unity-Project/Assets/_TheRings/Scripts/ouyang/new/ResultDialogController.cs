using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultDialogController : MonoBehaviour {
	public int from;
	public bool success;
	// Use this for initialization
	void Start () {
		Transform panel = transform.GetChild (0);
		Button btn = panel.GetComponent<Button> ();
		btn.onClick.AddListener (() => {
			if (from == 0) {
				SceneManager.LoadScene (1);
			} else if (from == 1) {
				SceneManager.LoadScene (2);
			} else {
				SceneManager.LoadScene (0);
			}
		});
		// btn.onClick.AddListener (delegate () {
		// 	if (from == 0) {
		// 		SceneManager.LoadScene (1);
		// 	} else if (from == 1) {
		// 		SceneManager.LoadScene (2);
		// 	} else {
		// 		SceneManager.LoadScene (0);
		// 	}
		// });
		Text display = panel.GetChild (0).GetComponent<Text> ();
		if (success) {
			display.text = "Congratulations!";
		} else {
			display.text = "Failed!";
		}
	}

	// Update is called once per frame
	void Update () {

	}
}