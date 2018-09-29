using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MapController : MonoBehaviour {

	public int pageIndex;
	public int pagePoints;
	// Use this for initialization
	public Sprite[] icon;
	void Start () {

		int curLevel = PlayerPrefs.GetInt (CommonConst.PrefKeys.CURRENT_LEVEL, 1);
		pagePoints = transform.childCount;
		for (int i = 0; i < transform.childCount; i++) {
			Transform pos = transform.GetChild (i);
			Button btn = pos.GetComponent<Button> ();
			int x = pageIndex * pagePoints + (i + 1);
			btn.onClick.AddListener (delegate () {
				clickPoint (x, curLevel);
			});
			pos.GetChild (0).GetComponent<Text> ().text = x + "";
			if (x > curLevel) {
				pos.GetComponent<Image> ().sprite = icon[0];
				pos.GetChild (1).gameObject.SetActive (false);
				pos.GetChild (2).gameObject.SetActive (false);
				pos.GetChild (3).gameObject.SetActive (false);
			} else {

				pos.GetComponent<Image> ().sprite = icon[1];
				pos.GetChild (0).gameObject.SetActive (true);
				int rate = PlayerPrefs.GetInt ("sp_level_" + x, 0);
				if(rate>3){rate =3;}
				for (int j = 0; j < rate; j++) {
					pos.GetChild (j + 1).gameObject.SetActive (true);
				}
				// pos.GetChild (1).gameObject.SetActive (true);
				// pos.GetChild (2).gameObject.SetActive (true);
				// pos.GetChild (3).gameObject.SetActive (true);
			}
		}
	}

	// Update is called once per frame
	void Update () {

	}

	public void clickPoint (int i, int curLevel) {
		if (i > curLevel) {
			return;
		}
		GameState.toGameScene (i);
	}
}