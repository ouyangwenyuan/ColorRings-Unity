using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PauseDialogController : MonoBehaviour {

	public Button resume, restart, giveup, close;
	// Use this for initialization
	void Start () {
		close.onClick.AddListener (delegate () {
			btnEvent (0);
		});
		resume.onClick.AddListener (delegate () {
			btnEvent (0);
		});
		restart.onClick.AddListener (delegate () {
			btnEvent (1);
		});
		giveup.onClick.AddListener (delegate () {
			btnEvent (2);
		});

	}

	// Update is called once per frame
	void Update () {

	}

	private void btnEvent (int index) {
		this.gameObject.SetActive (false);
		if (index == 1) {
			//restart game
		} else if (index == 2) {
			//back home or back map
		}
	}
}