using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDirector : MonoBehaviour {

	//随机生成颜色环
	public GameObject randomPoint;

	//地图固定点
	private List<NailPoint> points = new List<NailPoint> ();
	private Dictionary<int, NailPoint> pointDict = new Dictionary<int, NailPoint> ();
	private Color[] ringColors = { Color.red, Color.magenta, Color.cyan, Color.green, Color.yellow, Color.blue, Color.gray, Color.black, Color.white };
	//最开始几种颜色
	private int initialColorNumber = 4;
	//允许拖拽状态
	private bool allowDrag;
	//需要随机一个环
	private bool needRandRomRing;
	//是否完成随机环生成
	private bool finishMoveRandomPointBack;
	//是否完成移动
	public bool finishDrop;
	//Save the first position of random point
	private Vector2 firstRandomPointPosition;
	//This is the postion that the ring is created
	private Vector2 originalRingPostion;
	//This is end position of ring when the ring moved
	private Vector2 theLeftPosition;
	private RandomPoint pointGenerator;
	int w = 5, h = 5;

	int ringcoins = 0;
	// Use this for initialization
	void Start () {
		// generateRandomRing ();

		Vector2 theBottomPosition = Camera.main.ViewportToWorldPoint (new Vector2 (0.5f, 0));
		theLeftPosition = Camera.main.ViewportToWorldPoint (new Vector2 (1f, 0));

		originalRingPostion = theBottomPosition + new Vector2 (0, -1f);

		//Save the first position of random point
		firstRandomPointPosition = randomPoint.transform.position;

		finishMoveRandomPointBack = true;

	}

	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) //First touch
		{
			Vector2 touchPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition); //Tranform mouse position to world position
			print ("touchpos =" + touchPosition + ",randpos=" + firstRandomPointPosition);
			//If the mouse position too far away the rearest random point -> not allow drag the rings
			if ((touchPosition - firstRandomPointPosition).magnitude < 1f) {
				allowDrag = true;
			}
		} else if (Input.GetMouseButton (0)) //Touch stay
		{
			//If allow drag -> move all rings by move the random point (the rings is child of random point)
			if (allowDrag) {
				float x = Input.mousePosition.x;
				float y = Input.mousePosition.y;
				randomPoint.transform.position = Camera.main.ScreenToWorldPoint (new Vector3 (x, y, 10f)) + new Vector3 (0, 0.3f, 0); // lift the ring a bit for not being covered by finger.
			}
		}

		if (Input.GetMouseButtonUp (0)) {
			if (allowDrag) {
				allowDrag = false;
				Vector2 mouseUpPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition); //Get mouse position
				NailPoint theNearestDot = TheNearestDot (mouseUpPosition); //Find the nearest dot (place where the ring stay)

				Vector2 theNearestDotPosition = theNearestDot.transform.position;
				print ("touch up pos =" + mouseUpPosition + ",theNearestDot=" + theNearestDotPosition);
				if ((mouseUpPosition - theNearestDotPosition).magnitude < 1f) //The ring not far away
				{
					//Check if allow drop - > drop all ring to the dot
					if (AllowDrop (theNearestDot)) {
						// SoundManager.Instance.PlaySound (SoundManager.Instance.dropRing);
						// dotManager.dotIndex = theNearestDot.GetComponent<DotController> ().dotIndex;
						// while (randomPoint.transform.childCount > 0) {
						// 	StartCoroutine (MoveRingToTheDot (randomPoint.transform.GetChild (0).gameObject, theNearestDot));
						// 	// randomPoint.transform.GetChild (0).transform.parent = theNearestDot.transform;
						// }
						StartCoroutine (testMove (theNearestDot));
					}
				}

				//Move the ranrom point back to the first position of it
				StartCoroutine (MoveRandomPointBack (randomPoint, firstRandomPointPosition));

			}
		}

		//After check -> generate rings
		if (finishMoveRandomPointBack) {
			finishMoveRandomPointBack = false;
			generateRandomRing ();
		}
		// if (dotManager.finishCheckAfterDestroy && finishMoveRandomPointBack) {
		// 	finishMoveRandomPointBack = false;
		// 	dotManager.finishCheckAfterDestroy = false;
		// 	UpdateColorNumber ();
		// 	GenerateRings ();
		// 	uIManager.CheckAndShowWatchAdOption ();
		// }
	}

	public void playClicked () {
		initGame ();
		GameObject.Find ("Canvas").SetActive (false);
	}

	public void initGame () {
		initMap ();
		putRings ();
		putBlock ();
	}

	private void initMap () {

		for (int i = 0; i < w; i++) {
			for (int j = 0; j < h; j++) {
				NailPoint point = Instantiate (SceneObject.instance.nailPoint);
				point.Position = new Vector2 (i, j);
				point.x = i;
				point.y = j;
				point.ringTotal = 0;
				point.transform.position = new Vector3 (i, j, 0);
				point.transform.localScale = Vector3.one;
				point.transform.SetParent (this.transform);
				points.Add (point);
				pointDict.Add (i * 10 + j, point);
			}
		}
		this.transform.Translate (-w / 2, -h / 3, 0);
	}
	private void putRings () { }
	private void putBlock () { }

	private void moveRings (RandomPoint randomPoint) {

	}

	private void gameLogic (NailPoint clickPoint) {
		if (finishDrop) {
			finishDrop = false;
			int ringTotal = clickPoint.ringTotal;
			bool hassmall = 1 == (clickPoint.ringTotal & 1);
			bool hasmid = 2 == (clickPoint.ringTotal & 2);
			bool hasbig = 4 == (clickPoint.ringTotal & 4);
			HashSet<NailPoint> smallColors = new HashSet<NailPoint> ();
			HashSet<NailPoint> midColors = new HashSet<NailPoint> ();
			HashSet<NailPoint> bigColors = new HashSet<NailPoint> ();
			foreach (NailPoint item in points) {
				if (item == clickPoint) { continue; }
				for (int i = 0; i < item.transform.childCount; i++) {
					RingController ring = item.transform.GetChild (i).GetComponent<RingController> ();
					if (hassmall && clickPoint.smallRing.colorIndex == ring.colorIndex) {
						smallColors.Add (item);
						continue;
					}
					if (hasmid && clickPoint.mediumRing.colorIndex == ring.colorIndex) {
						midColors.Add (item);
						continue;
					}
					if (hasbig && clickPoint.bigRing.colorIndex == ring.colorIndex) {
						bigColors.Add (item);
					}
				}
			}
			switch (ringTotal) {
				case 7:
					if (clickPoint.mediumRing.colorIndex == clickPoint.smallRing.colorIndex && clickPoint.mediumRing.colorIndex == clickPoint.bigRing.colorIndex) {
						// remove self
						//remove small
						bool match = removeRings (smallColors, clickPoint, clickPoint.smallRing);
						if (!match) {
							clickPoint.ringTotal -= clickPoint.smallRing.ringType;
							Destroy (clickPoint.smallRing.gameObject);
							clickPoint.smallRing = null;
						}
						clickPoint.ringTotal -= clickPoint.bigRing.ringType;
						Destroy (clickPoint.bigRing.gameObject);
						clickPoint.bigRing = null;
						clickPoint.ringTotal -= clickPoint.mediumRing.ringType;
						Destroy (clickPoint.mediumRing.gameObject);
						clickPoint.mediumRing = null;
					} else if (clickPoint.mediumRing.colorIndex == clickPoint.smallRing.colorIndex && clickPoint.mediumRing.colorIndex != clickPoint.bigRing.colorIndex) {
						removeRings (midColors, clickPoint, clickPoint.bigRing);
						bool match = removeRings (smallColors, clickPoint, clickPoint.smallRing);
						if (match) {
							clickPoint.ringTotal -= clickPoint.mediumRing.ringType;
							Destroy (clickPoint.mediumRing.gameObject);
							clickPoint.mediumRing = null;
						}

						//remove big
					} else if (clickPoint.mediumRing.colorIndex == clickPoint.bigRing.colorIndex && clickPoint.mediumRing.colorIndex != clickPoint.smallRing.colorIndex) {
						removeRings (smallColors, clickPoint, clickPoint.smallRing);
						bool match = removeRings (midColors, clickPoint, clickPoint.mediumRing); {
							clickPoint.ringTotal -= clickPoint.bigRing.ringType;
							Destroy (clickPoint.bigRing.gameObject);
							clickPoint.bigRing = null;
						}
						//remove big
					} else if (clickPoint.bigRing.colorIndex == clickPoint.smallRing.colorIndex && clickPoint.mediumRing.colorIndex != clickPoint.smallRing.colorIndex) {
						removeRings (midColors, clickPoint, clickPoint.mediumRing);
						bool match = removeRings (smallColors, clickPoint, clickPoint.smallRing);
						if (match) {
							clickPoint.ringTotal -= clickPoint.bigRing.ringType;
							Destroy (clickPoint.bigRing.gameObject);
							clickPoint.bigRing = null;
						}
					} else {
						removeRings (midColors, clickPoint, clickPoint.mediumRing);
						removeRings (bigColors, clickPoint, clickPoint.bigRing);
						removeRings (smallColors, clickPoint, clickPoint.smallRing);
					}
					break;
				case 6:
					if (clickPoint.mediumRing.colorIndex == clickPoint.bigRing.colorIndex) {
						bool match = removeRings (midColors, clickPoint, clickPoint.mediumRing);
						if (match) {
							clickPoint.ringTotal -= clickPoint.bigRing.ringType;
							Destroy (clickPoint.bigRing.gameObject);
							clickPoint.bigRing = null;
						}
					} else {
						removeRings (midColors, clickPoint, clickPoint.mediumRing);
						removeRings (bigColors, clickPoint, clickPoint.bigRing);
					}
					break;
				case 5:
					if (clickPoint.bigRing.colorIndex == clickPoint.smallRing.colorIndex) {
						bool match = removeRings (smallColors, clickPoint, clickPoint.smallRing);
						if (match) {
							clickPoint.ringTotal -= clickPoint.bigRing.ringType;
							Destroy (clickPoint.bigRing.gameObject);
							clickPoint.bigRing = null;
						}
					} else {
						removeRings (smallColors, clickPoint, clickPoint.smallRing);
						removeRings (bigColors, clickPoint, clickPoint.bigRing);
					}
					break;
				case 4:
					removeRings (bigColors, clickPoint, clickPoint.bigRing);
					break;
				case 3:
					if (clickPoint.mediumRing.colorIndex == clickPoint.smallRing.colorIndex) {
						bool match = removeRings (smallColors, clickPoint, clickPoint.smallRing);
						if (match) {
							clickPoint.ringTotal -= clickPoint.mediumRing.ringType;
							Destroy (clickPoint.mediumRing.gameObject);
							clickPoint.mediumRing = null;
						}
					} else {
						removeRings (smallColors, clickPoint, clickPoint.smallRing);
						removeRings (midColors, clickPoint, clickPoint.mediumRing);
					}
					break;
				case 2:
					removeRings (midColors, clickPoint, clickPoint.mediumRing);
					break;
				case 1:
					removeRings (smallColors, clickPoint, clickPoint.smallRing);
					break;
				default:
					break;
			}
			print ("smallColors count" + smallColors.Count + ",midColors count" + midColors.Count + ",bigColors count" + bigColors.Count);

		}
	}

	private bool removeRings (HashSet<NailPoint> sameColorRings, NailPoint clickPoint, RingController whichRing) {
		int x = clickPoint.x;
		int y = clickPoint.y;
		List<NailPoint> columnLine = new List<NailPoint> ();
		List<NailPoint> rowLine = new List<NailPoint> ();
		List<NailPoint> upLine = new List<NailPoint> ();
		List<NailPoint> downLine = new List<NailPoint> ();
		foreach (NailPoint item in sameColorRings) {
			if (item.x == x && (item.y == y - 1 || item.y == y + 1 || item.y == y - 2 || item.y == y + 2)) {
				columnLine.Add (item);
			} else if (item.y == y && (item.x == x - 1 || item.x == x + 1 || item.x == x - 2 || item.x == x + 2)) {
				rowLine.Add (item);
			} else if ((item.x == x - 1 && item.y == y + 1) ||
				(item.x == x + 1 && item.y == y - 1) ||
				(item.x == x + 2 && item.y == y - 2) ||
				(item.x == x - 2 && item.y == y + 2)
			) {
				downLine.Add (item);
			} else if ((item.x == x - 1 && item.y == y - 1) ||
				(item.x == x + 1 && item.y == y + 1) ||
				(item.x == x - 2 && item.y == y - 2) ||
				(item.x == x + 2 && item.y == y + 2)
			) {
				upLine.Add (item);
			}
		}
		print ("column count" + columnLine.Count + ",row count=" + rowLine.Count + ",up count=" + upLine.Count + ",down count=" + downLine.Count);

		bool match = false;
		match |= checkedMatch (columnLine, 0, whichRing);
		match |= checkedMatch (rowLine, 1, whichRing);
		match |= checkedMatch (downLine, 2, whichRing);
		match |= checkedMatch (upLine, 3, whichRing);
		if (match) {
			clickPoint.ringTotal -= whichRing.ringType;
			if (whichRing.ringType == 1) {
				clickPoint.smallRing = null;
			} else if (whichRing.ringType == 2) {
				clickPoint.mediumRing = null;
			} else if (whichRing.ringType == 4) {
				clickPoint.bigRing = null;
			}
			Destroy (whichRing.gameObject);
		}
		return match;

	}

	/// <summary>
	/// matchtype :  0- column ,1 -row ,2 -down, 3-up
	/// </summary>
	/// <param name="columnLine"></param>
	/// <param name="matchType"></param>
	public bool checkedMatch (List<NailPoint> columnLine, int matchType, RingController whichRing) {
		bool hasMatch = false;
		print ("whichRing total=" + whichRing.ringType);
		if (columnLine.Count == 2) {
			int sum = 1;
			foreach (NailPoint item in columnLine) {
				if (matchType == 0) {
					sum *= (item.y - whichRing.y);
				} else {
					sum *= (item.x - whichRing.x);
				}
			}
			print ("sum=" + sum);
			if (sum == -1 || sum == 2) {
				destoryRing (columnLine, whichRing.colorIndex);
				hasMatch = true;
			}
		} else if (columnLine.Count == 3) {
			int sum = 1;
			foreach (NailPoint item in columnLine) {
				if (matchType == 0) {
					sum *= (item.y - whichRing.y);
				} else {
					sum *= (item.x - whichRing.x);
				}
			}
			print ("sum=" + sum);
			if (sum == -2 || sum == 2) {
				destoryRing (columnLine, whichRing.colorIndex);
				hasMatch = true;
			}
		} else if (columnLine.Count == 4) {
			destoryRing (columnLine, whichRing.colorIndex);
			hasMatch = true;
		};

		return hasMatch;
	}

	public void destoryRing (List<NailPoint> columnLine, int colorIndex) {
		foreach (NailPoint item in columnLine) {
			for (int i = 0; i < item.transform.childCount; i++) {
				RingController ring = item.transform.GetChild (i).GetComponent<RingController> ();
				if (ring.colorIndex == colorIndex) {
					item.ringTotal -= ring.ringType;
					if (ring.ringType == 1) {
						Destroy (item.smallRing.gameObject);
						item.smallRing = null;
					} else if (ring.ringType == 2) {
						Destroy (item.mediumRing.gameObject);
						item.mediumRing = null;
					} else if (ring.ringType == 4) {
						Destroy (item.bigRing.gameObject);
						item.bigRing = null;
					}
				}
			}
		}
	}

	private bool checkedDragEnd () {
		return false;
	}

	public bool checkLevelComplete () {
		return false;
	}

	private bool AllowDrop (NailPoint point) {
		RandomPoint pointGenerator = randomPoint.GetComponent<RandomPoint> ();
		Debug.Log ("point.total = " + point.ringTotal + ",rand total =" + pointGenerator.ringTotal);
		if (point.ringTotal == 0) {
			return true;
		}
		if (point.ringTotal == pointGenerator.ringTotal) {
			return false;
		} else if (((point.ringTotal & 1) == 1 && (pointGenerator.ringTotal & 1) == 1) ||
			((point.ringTotal & 2) == 2 && (pointGenerator.ringTotal & 2) == 2) ||
			((point.ringTotal & 4) == 4 && (pointGenerator.ringTotal & 4) == 4)) {
			return false;
		} else {
			return true;
		}

	}
	private NailPoint TheNearestDot (Vector2 mouseUpPosition) {
		NailPoint theNearestDot = points[0];
		Vector2 dotPosition = theNearestDot.transform.position;
		float minDistance = (mouseUpPosition - dotPosition).magnitude;

		for (int i = 0; i < points.Count; i++) {
			dotPosition = points[i].transform.position;
			if ((mouseUpPosition - dotPosition).magnitude < minDistance) {
				minDistance = (mouseUpPosition - dotPosition).magnitude;
				theNearestDot = points[i];
			}
		}
		return theNearestDot;
	}

	private void generateRandomRing () {
		int seed = Random.Range (0, 7) + 1;
		RandomPoint pointGenerator = randomPoint.GetComponent<RandomPoint> ();
		pointGenerator.ringTotal = seed;
		if (1 == (seed & 1)) {
			createRing (1, pointGenerator);
		}
		if (2 == (seed & 2)) {
			createRing (2, pointGenerator);
		}
		if (4 == (seed & 4)) {
			createRing (4, pointGenerator);
		}
	}
	private void createRing (int ringsize, RandomPoint pointGenerator) {
		RingController colorring = null;
		if (ringsize == 1) {
			colorring = Instantiate (SceneObject.instance.smallRing);
			pointGenerator.smallRing = colorring;
		} else if (ringsize == 2) {
			colorring = Instantiate (SceneObject.instance.midRing);
			pointGenerator.mediumRing = colorring;
		} else if (ringsize == 4) {
			colorring = Instantiate (SceneObject.instance.bigRing);
			pointGenerator.bigRing = colorring;
		}

		colorring.transform.position = new Vector3 (0, -3f, 0);
		colorring.transform.transform.localScale = Vector3.one;
		colorring.transform.parent = randomPoint.transform;
		int colorIndex = Random.Range (0, initialColorNumber);
		colorring.GetComponent<SpriteRenderer> ().color = ringColors[colorIndex];
		colorring.colorIndex = colorIndex;
		colorring.ringType = ringsize;
		colorring.color = ringColors[colorIndex];

	}

	IEnumerator MoveRingToTheDot (GameObject theRing, NailPoint point) {

		Vector2 startPos = theRing.transform.position;
		Vector2 endPos = point.transform.position;
		float t = 0;
		while (t < 0.1f) {
			t += Time.deltaTime;
			float fraction = t / 0.1f;
			theRing.transform.position = Vector2.Lerp (startPos, endPos, fraction);
			yield return null;
		}

		finishDrop = true;
		finishMoveRandomPointBack = true;
	}

	IEnumerator MoveRandomPointBack (GameObject randomPoint, Vector2 endPos) {
		Vector2 startPos = randomPoint.transform.position;
		float t = 0;
		while (t < 0.1f) {
			t += Time.deltaTime;
			float fraction = t / 0.1f;
			randomPoint.transform.position = Vector2.Lerp (startPos, endPos, fraction);
			yield return null;
		}

	}
	private IEnumerator testMove (NailPoint point) {
		Debug.Log ("test move");
		RandomPoint pointGenerator = randomPoint.GetComponent<RandomPoint> ();
		bool hasbig = false, hasmid = false, hassmall = false;
		if (points.Count > 0 && randomPoint.transform.childCount > 0) {
			point.ringTotal += pointGenerator.ringTotal;
			Debug.Log ("rand child =" + randomPoint.transform.childCount + "point count=" + points.Count);

			if (4 == (pointGenerator.ringTotal & 4)) {
				point.bigRing = pointGenerator.bigRing;
				point.bigRing.transform.SetParent (point.transform);
				point.bigRing.x = point.x;
				point.bigRing.y = point.y;
				hasbig = true;
			}
			if (2 == (pointGenerator.ringTotal & 2)) {
				point.mediumRing = pointGenerator.mediumRing;
				point.mediumRing.transform.SetParent (point.transform);
				point.mediumRing.x = point.x;
				point.mediumRing.y = point.y;
				hasmid = true;
			}
			if (1 == (pointGenerator.ringTotal & 1)) {
				point.smallRing = pointGenerator.smallRing;
				point.smallRing.transform.SetParent (point.transform);
				point.smallRing.x = point.x;
				point.smallRing.y = point.y;
				hassmall = true;
			}

			Vector2 startPos = randomPoint.transform.position;
			Vector2 endPos = point.transform.position;
			float t = 0;
			while (t < 0.1f) {
				t += Time.deltaTime;
				float fraction = t / 0.1f;
				if (hassmall)
					point.smallRing.transform.position = Vector2.Lerp (startPos, endPos, fraction);
				if (hasmid)
					point.mediumRing.transform.position = Vector2.Lerp (startPos, endPos, fraction);
				if (hasbig)
					point.bigRing.transform.position = Vector2.Lerp (startPos, endPos, fraction);
				yield return null;
			}

			finishDrop = true;
			finishMoveRandomPointBack = true;

			gameLogic (point);
		}
	}

	private int moveIndex = 0;
	private void OnGUI () {

		if (GUILayout.Button ("initmap")) {
			initGame ();
		}

		if (GUILayout.Button ("randring")) {
			generateRandomRing ();
		}

		if (GUILayout.Button ("move")) {
			NailPoint point = points[moveIndex];
			StartCoroutine (testMove (point));
			this.moveIndex++;
		}

		if (GUILayout.Button ("clear")) {
			for (int i = 0; i < transform.childCount; i++) {
				Destroy (this.transform.GetChild (i).gameObject);
			}

		}

		if (GUILayout.Button ("restart")) {
			initMap ();
			generateRandomRing ();
		}

		if (GUILayout.Button ("autolayer")) {

		}

	}

}