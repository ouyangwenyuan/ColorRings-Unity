using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayoutRingMgr : GameManager {
	public GameObject BottomdotPoint;
	public Text levelTx;
	private GameObject clickRing;
	private List<GameObject> bottomRings = new List<GameObject> ();
	MessGameLevel gamelevel;
	public int level;
	public RectTransform container;
	private float distance = 0.01f; // 世界坐标每单位像素为100，转换规则 屏幕坐标*0.01 = 世界坐标。
	void Start () {

		// gamelevel = Resources.Load<MessGameLevel> ("Levels/Mess/level_" + level);
		// string[] boardRingStr = gamelevel.boardRings.Split (';');
		// string[] bottomRingStr = gamelevel.bottomRings.Split (';');
		// // RingList = this.gameObject;
		// for (int i = 0; i < bottomRingStr.Length; i++) {
		// 	generateRandomRing (bottomRingStr[i], i);
		// }

		// for (int i = 0; i < boardRingStr.Length; i++) {
		// 	string[] ringStrs = boardRingStr[i].Split (',');
		// 	int dotIndex = int.Parse (ringStrs[ringStrs.Length - 1]);
		// 	GameObject wraper = dotManager.dots[dotIndex];
		// 	for (int j = 0; j < (ringStrs.Length) / 2; j++) {
		// 		int ringType = int.Parse (ringStrs[2 * j]);
		// 		int colorIndex = int.Parse (ringStrs[2 * j + 1]);
		// 		GameObject ring = null;
		// 		if (ringType == 1) {
		// 			ring = Instantiate (UIManager.Instance.smallRing);
		// 		} else if (ringType == 2) {
		// 			ring = Instantiate (UIManager.Instance.normalRing);
		// 		} else if (ringType == 4) {
		// 			ring = Instantiate (UIManager.Instance.bigRing);
		// 		}
		// 		ring.transform.parent = wraper.transform;
		// 		ring.transform.position = wraper.transform.position; //new Vector3 (transform.position.x + i * 1.5f, transform.position.y, transform.position.z);
		// 		ring.transform.localScale = Vector3.one;

		// 		// ring.layer = 9;

		// 		ring.GetComponent<SpriteRenderer> ().color = UIManager.ringColors[colorIndex];
		// 		ring.GetComponent<RingController> ().colorIndex = colorIndex;
		// 	}
		// }
		firstRandomPointPosition = randomPoint.transform.position;
		level = GameState.messLevel - 1;
		// currentLevel = GameState.levelindex;
		levelTx.text = GameState.messLevel + ">" + GameState.levelindex;
		loadLevelData ();
		transform.position += new Vector3 (0, 0);
		// container.anchoredPosition = new Vector2 (0, container.anchoredPosition.y);
		moveToRandomPoint ();

	}
	public void CameraPosUpdate () {
		// if (container.anchoredPosition.x >= 0 && container.anchoredPosition.x <= 3600) {
		transform.position = new Vector3 ((container.anchoredPosition.x + 320) * distance, transform.position.y, transform.position.z);
		// }
	}
	private void moveToRandomPoint () {
		if (bottomRings.Count > 0) {
			bottomRings[0].transform.parent = randomPoint.transform;
			// bottomRings[0].transform.position = randomPoint.transform.position;
			iTween.MoveTo (bottomRings[0].gameObject, randomPoint.transform.position, 1);
			// this.transform.Translate(-1.5f,0,0);
			iTween.MoveTo (this.gameObject, this.transform.position + new Vector3 (-1.5f, 0, 0), 1);
		}
	}
	public void resetGame () {
		clearRings ();
		loadLevelData ();
	}
	public void clearRings () {

		// for (int i = 0; i < bottomRings.Count; i++) {
		// 	if (bottomRings[i].transform.childCount > 0) {
		// 		for (int j = 0; j < bottomRings[i].transform.childCount; j++) {
		// 			Destroy (bottomRings[i].transform.GetChild (j).gameObject);
		// 		}
		// 	}
		// }
		// bottomRings.Clear ();
		for (int i = 0; i < dotManager.dots.Length; i++) {
			if (dotManager.dots[i].transform.childCount > 0) {
				for (int j = 0; j < dotManager.dots[i].transform.childCount; j++) {
					Destroy (dotManager.dots[i].transform.GetChild (j).gameObject);
				}
			}
		}

		// if (randomPoint.transform.childCount > 0) {
		// 	for (int j = 0; j < randomPoint.transform.childCount; j++) {
		// 		Destroy (randomPoint.transform.GetChild (j).gameObject);
		// 	}
		// }
	}
	public void loadLevelData () {
		gamelevel = Resources.Load<MessGameLevel> ("Levels/Mess/level_" + level);
		string[] boardRingStr = gamelevel.boardRings.Split (';');
		string[] bottomRingStr = gamelevel.bottomRings.Split (';');
		// RingList = this.gameObject;
		for (int i = 0; i < bottomRingStr.Length; i++) {
			generateRandomRing (bottomRingStr[i], i);
		}

		for (int i = 0; i < boardRingStr.Length; i++) {
			string[] ringStrs = boardRingStr[i].Split (',');
			int dotIndex = int.Parse (ringStrs[ringStrs.Length - 1]);
			GameObject wraper = dotManager.dots[dotIndex];
			for (int j = 0; j < (ringStrs.Length) / 2; j++) {
				int ringType = int.Parse (ringStrs[2 * j]);
				int colorIndex = int.Parse (ringStrs[2 * j + 1]);
				GameObject ring = null;
				if (ringType == 1) {
					ring = Instantiate (UIManager.Instance.smallRing);
				} else if (ringType == 2) {
					ring = Instantiate (UIManager.Instance.normalRing);
				} else if (ringType == 4) {
					ring = Instantiate (UIManager.Instance.bigRing);
				}
				ring.transform.parent = wraper.transform;
				ring.transform.position = wraper.transform.position; //new Vector3 (transform.position.x + i * 1.5f, transform.position.y, transform.position.z);
				ring.transform.localScale = Vector3.one;

				// ring.layer = 9;
				string spirtefile = "rings/" + colorIndex + "-" + ring.GetComponent<RingController> ().ringType;
				ring.GetComponent<SpriteRenderer> ().sprite = Resources.Load<Sprite> (spirtefile);
				// ring.GetComponent<SpriteRenderer> ().color = UIManager.ringColors[colorIndex];
				ring.GetComponent<RingController> ().colorIndex = colorIndex;
				float z = 0;
				// int ringType = theRing.GetComponent<RingController> ().ringType;
				if (ringType == 1) {
					z = -0.5f;
				} else if (ringType == 2) {
					z = -0.4f;
				} else {
					z = -0.3f;
				}
				ring.transform.Translate (new Vector3 (0, 0, z));
			}
		}
	}

	private void generateRandomRing (string bottomRingStr, int i) {
		string[] ringStrs = bottomRingStr.Split (',');
		GameObject wraper = Instantiate (BottomdotPoint);
		wraper.transform.parent = transform;
		wraper.transform.position = new Vector3 (transform.position.x + i * 1.5f, transform.position.y, transform.position.z);
		wraper.transform.localScale = Vector3.one;
		wraper.layer = 9;
		// wraper.GetComponent<DotController> ().dotIndex = i;
		for (int j = 0; j < (ringStrs.Length) / 2; j++) {
			int ringType = int.Parse (ringStrs[2 * j]);
			int colorIndex = int.Parse (ringStrs[2 * j + 1]);
			GameObject ring = null;
			if (ringType == 1) {
				ring = Instantiate (UIManager.Instance.smallRing);
			} else if (ringType == 2) {
				ring = Instantiate (UIManager.Instance.normalRing);
			} else if (ringType == 4) {
				ring = Instantiate (UIManager.Instance.bigRing);
			}
			ring.transform.parent = wraper.transform;
			ring.transform.position = wraper.transform.position; //new Vector3 (transform.position.x + i * 1.5f, transform.position.y, transform.position.z);
			ring.transform.localScale = Vector3.one;

			// ring.layer = 9;
			string spirtefile = "rings/" + colorIndex + "-" + ring.GetComponent<RingController> ().ringType;
			ring.GetComponent<SpriteRenderer> ().sprite = Resources.Load<Sprite> (spirtefile);
			// ring.GetComponent<SpriteRenderer> ().color = UIManager.ringColors[colorIndex];
			ring.GetComponent<RingController> ().colorIndex = colorIndex;
			float z = 0;
			// int ringType = theRing.GetComponent<RingController> ().ringType;
			if (ringType == 1) {
				z = -0.5f;
			} else if (ringType == 2) {
				z = -0.4f;
			} else {
				z = -0.3f;
			}
			ring.transform.Translate (new Vector3 (0, 0, z));
		}
		bottomRings.Add (wraper);

	}

	// Update is called once per frame
	void Update () {
		if (isPaused) {
			return;
		}
		moveRing ();
	}

	private void FixedUpdate () {

	}

	private void moveRing () {
		if (Input.GetMouseButtonDown (0)) //First touch
		{
			Vector2 touchPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition); //Tranform mouse position to world position
			// float distance = float.MaxValue;
			// int nearestIndex = 0;
			// for (int i = 0; i < transform.childCount; i++) {
			// 	Vector2 p = transform.GetChild (i).position;
			// 	if ((touchPosition - p).magnitude < distance) {
			// 		distance = (touchPosition - p).magnitude;
			// 		nearestIndex = i;
			// 	}
			// }
			Transform trans = bottomRings[0].transform;
			Vector2 pos = trans.position;
			if ((touchPosition - pos).magnitude < 1f) {
				allowDrag = true;
				firstRandomPointPosition = pos;
				clickRing = trans.gameObject;
			}
			//If the mouse position too far away the rearest random point -> not allow drag the rings
			// if ((touchPosition - firstRandomPointPosition).magnitude < 1f) {
			// 	allowDrag = true;
			// }
		} else if (Input.GetMouseButton (0)) //Touch stay
		{
			//If allow drag -> move all rings by move the random point (the rings is child of random point)
			if (allowDrag) {
				float x = Input.mousePosition.x;
				float y = Input.mousePosition.y;
				clickRing.transform.position = Camera.main.ScreenToWorldPoint (new Vector3 (x, y, 1f)) + new Vector3 (0, 0.3f, 0); // lift the ring a bit for not being covered by finger.
			}
		} else

		if (Input.GetMouseButtonUp (0)) {
			if (allowDrag) {
				allowDrag = false;
				Vector2 mouseUpPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition); //Get mouse position
				GameObject theNearestDot = TheNearestDot (mouseUpPosition); //Find the nearest dot (place where the ring stay)

				Vector2 theNearestDotPosition = theNearestDot.transform.position;
				Debug.Log (mouseUpPosition + " ,theNearestDotPosition = " + theNearestDotPosition);
				if ((mouseUpPosition - theNearestDotPosition).magnitude < 1f) //The ring not far away
				{
					theNearestDot.GetComponent<DotController> ().CheckRing ();
					//Check if allow drop - > drop all ring to the dot
					if (AllowDrop1 (theNearestDot)) {
						// SoundManager.Instance.PlaySound (SoundManager.Instance.dropRing);
						dotManager.dotIndex = theNearestDot.GetComponent<DotController> ().dotIndex;
						while (clickRing.transform.childCount > 0) {
							StartCoroutine (MoveRingToTheDot (clickRing.transform.GetChild (0).gameObject, theNearestDot.transform.position));
							clickRing.transform.GetChild (0).transform.parent = theNearestDot.transform;
						}
						bottomRings.Remove (clickRing);
						Destroy (clickRing);
						finishMoveRandomPointBack = true;

					} else {
						StartCoroutine (MoveRandomPointBack (clickRing, firstRandomPointPosition));
					}
				} else {
					StartCoroutine (MoveRandomPointBack (clickRing, firstRandomPointPosition));
				}

				//Move the ranrom point back to the first position of it

			}
		}

		//After check -> generate rings
		// 
		if (dotManager.finishCheckAfterDestroy && finishMoveRandomPointBack) {
			finishMoveRandomPointBack = false;
			dotManager.finishCheckAfterDestroy = false;
			// 	// UpdateColorNumber ();
			// 	GenerateRings ();
			// 	uIManager.CheckAndShowWatchAdOption ();

			if (bottomRings.Count == 0) {
				bool success = true;
				for (int i = 0; i < dotManager.dots.Length; i++) {
					if (dotManager.dots[i].transform.childCount > 0) {
						success = false;
					}
				}
				if (success) {
					// int curLevel = PlayerPrefs.GetInt (CommonConst.PrefKeys.CURRENT_LEVEL, 1);
					// if (currentLevel == curLevel) {
					// 	PlayerPrefs.SetInt (CommonConst.PrefKeys.CURRENT_LEVEL, curLevel + 1);
					// }
					UIManager.Instance.ShowSuccessUI ();
				} else {
					UIManager.Instance.ShowFailUI ();
				}
				isPaused = true;
				Debug.Log ("游戏结束" + success);
			} else {
				moveToRandomPoint ();
			}
		}

	}

	public bool AllowDrop1 (GameObject theNearestDot) {

		for (int i = 0; i < bottomRings[0].transform.childCount; i++) {
			int ringType = bottomRings[0].transform.GetChild (i).GetComponent<RingController> ().ringType;
			int ringTotal = theNearestDot.GetComponent<DotController> ().ringTotal;
			if ((ringType & ringTotal) == ringType) {
				return false;
			}
		}

		return true;
	}

}