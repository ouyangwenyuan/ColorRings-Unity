using System.Collections;
using System.Collections.Generic;
using SgLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
struct Line {
    public Vector2 Position { get; set; }

    public float Angle { get; set; }

    public Color Color { get; set; }
}

public class DotManager : MonoBehaviour {
    public int gameType = 0; // 0- level mode ,1 - mess mode , 2 - score mode
    public Text levelTx;
    public GameObject TargetContainer;
    TargetItem scoreItem;
    TargetItem comboItem;
    TargetItem[] ringItems;
    int currentLevel;
    int targetScore;
    int targetComboTotalCnt;
    public GameObject lineObject;
    [HideInInspector]
    public int combo;
    public ParticleSystem ringExplode;
    public ParticleSystem ps_comboExplode;
    public UIManager uIManager;
    public CameraController cameraControler;
    public GameObject[] dots;
    public AnimationClip bigRingScale;
    public AnimationClip comboText;
    [HideInInspector]
    public int dotIndex;
    [HideInInspector]
    public bool finishCheckAfterDestroy;

    private List<GameObject> listDestroyRing = new List<GameObject> ();
    private List<Line> listLine = new List<Line> ();
    private int previousCombo;
    private List<DotController> points;
    // Use this for initialization
    public int gridSize = 3;
    void Awake () {
        float unitSize = 3.2f / (gridSize - 1);
        dots = new GameObject[gridSize * gridSize];
        for (int i = 0; i < gridSize; i++) {
            for (int j = 0; j < gridSize; j++) {
                int index = i * gridSize + j;
                GameObject dot = Instantiate (uIManager.dotPoint);
                DotController dotc = dot.GetComponent<DotController> ();
                dotc.dotIndex = index;
                dotc.x = i;
                dotc.y = j;
                dotc.transform.position = new Vector3 (unitSize * i, unitSize * j, 0f);
                dotc.transform.localScale = Vector3.one;
                dotc.transform.parent = this.transform;
                dots[index] = dot;
                // dot.transform.Translate(new Vector3(0,0,-0.2f));
            }
        }
        this.transform.Translate (-unitSize * (gridSize - 1) / 2, -unitSize * (gridSize - 1) / 2+0.5f, -0.2f);
        points = new List<DotController> ();
        for (int i = 0; i < dots.Length; i++) {
            points.Add (dots[i].GetComponent<DotController> ());
        }
        // for (int i = 0; i < dots.Length; i++) {
        //     dots[i].GetComponent<DotController> ().dotIndex = i;
        //     dots[i].GetComponent<DotController> ().CheckRing ();
        // }
    }

    private void Start () {
        //
        if (gameType == 0) {
            // int currentLevel = PlayerPrefs.GetInt (CommonConst.PrefKeys.CURRENT_LEVEL, 1);
            currentLevel = GameState.levelindex;
            levelTx.text = GameState.levelindex + ">" + GameState.realLevel;
            GameLevelData levelData = CSVReader.gameLevelDatas[GameState.realLevel];
            targetScore = int.Parse (levelData.targetScore);

            scoreItem = Instantiate (UIManager.Instance.targetItem);
            scoreItem.setData ("score", levelData.targetScore);
            scoreItem.transform.parent = TargetContainer.transform;

            comboItem = Instantiate (UIManager.Instance.targetItem);
            comboItem.setData ("combo", levelData.targetCombo);
            comboItem.transform.parent = TargetContainer.transform;

            ringItems = new TargetItem[levelData.targetRingCounts.Length];
            for (int i = 0; i < levelData.targetRingCounts.Length; i++) {
                TargetItem ringItem = Instantiate (UIManager.Instance.targetItem);
                ringItem.setData ("", levelData.targetRingCounts[i], int.Parse (levelData.targetRingColors[i]));
                ringItem.transform.parent = TargetContainer.transform;
                ringItems[i] = ringItem;
            }

        } else if (gameType == 3) {

        }

    }
    // Update is called once per frame
    void Update () {

        if (GameManager.Instance.finishDrop) //Finish drop -> check all ring at that dot, caculate combo and destroy rings
        {
            GameManager.Instance.finishDrop = false;
            dots[dotIndex].GetComponent<DotController> ().CheckRing ();
            if (gameType == 3) {
                CheckAllDot ();
                return;
            }
            // CaculateCombo ();
            gameLogic (dotIndex);
            if (previousCombo > 1) {
                // == 2 ? targetComboTotalCnt + 2 : targetComboTotalCnt + 1;
                if (previousCombo == 2) {
                    targetComboTotalCnt += 2;
                } else {
                    targetComboTotalCnt += 1;
                }
                ShowCombo ();
            } else {
                HideCombo ();
            }
            StartCoroutine (DestroyRings ());
        }

        if (gameType == 0) {
            comboItem.setValue (targetComboTotalCnt);

        }

    }
    void ShowCombo () {
        UIManager.Instance.txtCombo.gameObject.SetActive (true);
        UIManager.Instance.txtCombo.text = "x" + previousCombo.ToString ();
    }

    void HideCombo () {
        UIManager.Instance.txtCombo.gameObject.SetActive (false);
    }

    public void ShowComboText (float waitTime) {
        StartCoroutine (ComboText (waitTime));
    }

    IEnumerator ComboText (float waitTime) {
        UIManager.Instance.txtComboText.text = "combo x" + previousCombo.ToString ();
        UIManager.Instance.txtComboText.gameObject.SetActive (true);
        yield return new WaitForSeconds (waitTime);
        UIManager.Instance.txtComboText.gameObject.SetActive (false);
    }
    //Find all destroyed ring and destroy it, caculate score base on number of destroyed ring
    IEnumerator DestroyRings () {
        //Find all destroyed ring add ad to the list
        for (int i = 0; i < dots.Length; i++) {
            for (int j = 0; j < dots[i].transform.childCount; j++) {
                if (dots[i].transform.GetChild (j).GetComponent<RingController> ().destroyed) {

                    listDestroyRing.Add (dots[i].transform.GetChild (j).gameObject);
                }
            }
        }
        if (gameType == 0 && listDestroyRing.Count > 0) {
            for (int j = 0; j < listDestroyRing.Count; j++) {
                for (int i = 0; i < ringItems.Length; i++) {
                    if (listDestroyRing[j].GetComponent<SpriteRenderer> ().color == ringItems[i].icon.color) {
                        ringItems[i].changeValue (1);
                    }
                }
            }
        }

        yield return null;

        //Caculate score and change ring time
        int scoreAdded = previousCombo * listDestroyRing.Count * 10;
        if (previousCombo > 1) {
            cameraControler.ShakeCamera ();
            ShowComboText (comboText.length);
            Vector2 comboExplodePosition = Camera.main.ScreenToWorldPoint (uIManager.txtCombo.transform.position);
            ParticleSystem comboExplode = Instantiate (ps_comboExplode, comboExplodePosition, Quaternion.identity) as ParticleSystem;
            comboExplode.Play ();
            Destroy (comboExplode.gameObject, 1f);
            StartCoroutine (PlayComboSound (combo));
        }

        if (scoreAdded > 0) {
            ScoreManager.Instance.AddScore (scoreAdded);

            SoundManager.Instance.PlaySound (SoundManager.Instance.lineDestroy);
            if (gameType == 0) {
                scoreItem.setValue (ScoreManager.Instance.Score);
                if (ScoreManager.Instance.Score >= targetScore) {
                    // GameManager.Instance.gameOver = true;
                    // 过关进入下一关
                    // GameRunState.currentLevel.ChangeValue (1);
                    int curLevel = PlayerPrefs.GetInt (CommonConst.PrefKeys.CURRENT_LEVEL, 1);
                    if (currentLevel == curLevel) {
                        PlayerPrefs.SetInt (CommonConst.PrefKeys.CURRENT_LEVEL, curLevel + 1);
                    }
                    UIManager.Instance.ShowSuccessUI ();
                    Debug.Log (",reallevel=" + curLevel + 1);

                }
            } else if (gameType == 2) {
            }
                UIManager.Instance.txtScore.text = ScoreManager.Instance.Score.ToString ();
        }

        if (listDestroyRing.Count >= 5) {
            float changeRingTimeAdded = Mathf.Ceil (listDestroyRing.Count / 5f);
            CoinManager.Instance.AddCoins ((int) changeRingTimeAdded);
        }

        CreateAndDestroyLine ();

        //Destroy rings
        for (int i = 0; i < listDestroyRing.Count; i++) {
            ParticleSystem particle = Instantiate (ringExplode, listDestroyRing[i].transform.position, Quaternion.identity) as ParticleSystem;
#if UNITY_5_5_OR_NEWER
            var tmp = particle.main;
            // tmp.startColor = UIManager.ringColors[listDestroyRing[i].GetComponent<RingController> ().colorIndex];
            tmp.startColor = Color.red;
#else
            // particle.startColor = UIManager.ringColors[listDestroyRing[i].GetComponent<RingController> ().colorIndex];
            particle.startColor = Color.red;
#endif
            particle.Play ();
            Destroy (particle.gameObject, 2f);
            listDestroyRing[i].GetComponent<Animator> ().enabled = true;
            Destroy (listDestroyRing[i], bigRingScale.length);
        }

        yield return (listDestroyRing.Count > 0) ? (new WaitForSeconds (bigRingScale.length)) : null;
        //float waitTime = (listDestroyRing.Count > 0) ? bigRingScale.length : 0.02f;
        listDestroyRing.Clear ();

        //Check all to update bool variable of ring
        //yield return new WaitForSeconds(waitTime);
        CheckAllDot ();
    }

    IEnumerator PlayComboSound (int combo) {
        for (int i = 0; i < combo; i++) {
            SoundManager.Instance.PlaySound (SoundManager.Instance.lineDestroy);
            yield return new WaitForSeconds (0.3f);
        }
    }

    //Check all dot
    public void CheckAllDot () {
        for (int i = 0; i < dots.Length; i++) {
            dots[i].GetComponent<DotController> ().CheckRing ();
        }
        finishCheckAfterDestroy = true;
    }

    //Caculate combo base on dot sequence
    void CaculateCombo () {
        if (dotIndex == 0) //Check line: 0_1_2, 0_4_8. 0_5_6
        {
            previousCombo = combo;

            combo += (ComboLine (0, 1, 2) + ComboLine (0, 4, 8) + ComboLine (0, 3, 6) + ComboDotIndex (dotIndex));

            if (combo == previousCombo) {
                combo = 0;
            }
        } else if (dotIndex == 1) //Check line: 0_1_2, 1_4_7
        {
            previousCombo = combo;

            combo += (ComboLine (0, 1, 2) + ComboLine (1, 4, 7) + ComboDotIndex (dotIndex));

            if (combo == previousCombo) {
                combo = 0;
            }
        } else if (dotIndex == 2) //Check line: 2_1_0, 2_3_8, 2_4_6
        {
            previousCombo = combo;

            combo += (ComboLine (0, 1, 2) + ComboLine (2, 5, 8) + ComboLine (2, 4, 6) + ComboDotIndex (dotIndex));

            if (combo == previousCombo) {
                combo = 0;
            }
        } else if (dotIndex == 3) //Check line 2_3_8, 3_4_5
        {
            previousCombo = combo;
            combo += (ComboLine (0, 3, 6) + ComboLine (3, 4, 5) + ComboDotIndex (dotIndex));

            if (combo == previousCombo) {
                combo = 0;
            }
        } else if (dotIndex == 4) //Check line: 1_4_7, 0_4_8, 2_4_6, 3_4_5
        {
            previousCombo = combo;
            combo += (ComboLine (1, 4, 7) + ComboLine (0, 4, 8) + ComboLine (2, 4, 6) + ComboLine (3, 4, 5) + ComboDotIndex (dotIndex));

            if (combo == previousCombo) {
                combo = 0;
            }
        } else if (dotIndex == 5) //Check line: 0_5_6, 3_4_5
        {
            previousCombo = combo;
            combo += (ComboLine (2, 5, 8) + ComboLine (3, 4, 5) + ComboDotIndex (dotIndex));

            if (combo == previousCombo) {
                combo = 0;
            }
        } else if (dotIndex == 6) //Check line: 0_5_6, 2_4_6, 6_7_8
        {
            previousCombo = combo;
            combo += (ComboLine (0, 3, 6) + ComboLine (6, 7, 8) + ComboLine (2, 4, 6) + ComboDotIndex (dotIndex));

            if (combo == previousCombo) {
                combo = 0;
            }
        } else if (dotIndex == 7) //Check line: 6_7_8, 1_4_7
        {
            previousCombo = combo;
            combo += (ComboLine (6, 7, 8) + ComboLine (1, 4, 7) + ComboDotIndex (dotIndex));

            if (combo == previousCombo) {
                combo = 0;
            }
        } else //Check line: 2_3_8, 6_7_8, 0_4_8
        {
            previousCombo = combo;
            combo += (ComboLine (2, 5, 8) + ComboLine (6, 7, 8) + ComboLine (0, 4, 8) + ComboDotIndex (dotIndex));

            if (combo == previousCombo) {
                combo = 0;
            }
        }
    }

    int ComboLine (int firstDot, int secondDot, int thirdDot) {
        int combo = 0;
        bool hasDestroy = false;
        //Check first dot
        for (int i = 0; i < dots[firstDot].transform.childCount; i++) {
            int colorIndexOfChild = dots[firstDot].transform.GetChild (i).GetComponent<RingController> ().colorIndex;
            //Check second dot
            for (int j = 0; j < dots[secondDot].transform.childCount; j++) {
                RingController secondDotRingControlller = dots[secondDot].transform.GetChild (j).GetComponent<RingController> ();
                if (secondDotRingControlller.colorIndex == colorIndexOfChild) {
                    //Check third dot
                    for (int k = 0; k < dots[thirdDot].transform.childCount; k++) {
                        RingController thirdDotRingController = dots[thirdDot].transform.GetChild (k).GetComponent<RingController> ();
                        if (thirdDotRingController.colorIndex == colorIndexOfChild) {
                            dots[thirdDot].transform.GetChild (k).GetComponent<RingController> ().destroyed = true;
                            dots[thirdDot].GetComponent<DotController> ().ringTotal -= dots[thirdDot].transform.GetChild (k).GetComponent<RingController> ().ringType;
                            dots[secondDot].transform.GetChild (j).GetComponent<RingController> ().destroyed = true;
                            dots[secondDot].GetComponent<DotController> ().ringTotal -= dots[secondDot].transform.GetChild (j).GetComponent<RingController> ().ringType;
                            dots[firstDot].transform.GetChild (i).GetComponent<RingController> ().destroyed = true;
                            dots[firstDot].GetComponent<DotController> ().ringTotal -= dots[firstDot].transform.GetChild (i).GetComponent<RingController> ().ringType;
                            AddForListLine (firstDot, secondDot, colorIndexOfChild);
                            hasDestroy = true;
                        }
                    }
                }
            }
        }
        if (hasDestroy) {
            combo = 1;
        }
        return combo;
    }

    int ComboDotIndex (int dotIndex) {
        bool hasDestroy = false;
        int combo = 0;

        if (dots[dotIndex].GetComponent<DotController> ().isSameColor) {
            hasDestroy = true;
            for (int i = 0; i < dots[dotIndex].transform.childCount; i++) {
                dots[dotIndex].transform.GetChild (i).GetComponent<RingController> ().destroyed = true;
                dots[dotIndex].GetComponent<DotController> ().ringTotal -= dots[dotIndex].transform.GetChild (i).GetComponent<RingController> ().ringType;
            }
        }

        if (hasDestroy) {
            combo = 1;
        }

        return combo;
    }

    void AddForListLine (int firstDot, int secondDot, int colorIndex) {
        Line newLine = new Line ();
        newLine.Position = dots[secondDot].transform.position;
        newLine.Color = Color.red; //UIManager.ringColors[colorIndex];
        if (secondDot == 3 || secondDot == 5) {
            newLine.Angle = 90;
        } else if (secondDot == 1 || secondDot == 7) {
            newLine.Angle = 0;
        } else if (secondDot == 4) {
            if (firstDot == 1) {
                newLine.Angle = 90;
            } else if (firstDot == 3) {
                newLine.Angle = 0;
            } else if (firstDot == 0) {
                newLine.Angle = 135;
            } else {
                newLine.Angle = 45;
            }
        }

        if (!listLine.Contains (newLine)) {
            listLine.Add (newLine);
        }
    }

    void CreateAndDestroyLine () {
        for (int i = 0; i < listLine.Count; i++) {
            GameObject line = Instantiate (lineObject, listLine[i].Position, Quaternion.Euler (0, 0, listLine[i].Angle)) as GameObject;
            line.GetComponent<SpriteRenderer> ().color = listLine[i].Color;
            line.GetComponent<Animator> ().enabled = true;
            Destroy (line, bigRingScale.length);
        }

        listLine.Clear ();
    }

    public void backMap () {
        SceneManager.LoadScene ("MapScene");
    }

    public void toNextLevel () {
        int curLevel = PlayerPrefs.GetInt (CommonConst.PrefKeys.CURRENT_LEVEL, 1);
        GameState.toGameScene (curLevel);
    }

    //ringchecker 

    public void gameLogic (int dotIndex) {
        combo = 0;
        DotController clickPoint = dots[dotIndex].GetComponent<DotController> ();
        clickPoint.CheckRing ();
        int ringTotal = clickPoint.ringTotal;
        // bool hassmall = clickPoint.hasSmallRing; //1 == (clickPoint.ringTotal & 1);
        // bool hasmid = clickPoint.hasNormalRing; //2 == (clickPoint.ringTotal & 2);
        // bool hasbig = clickPoint.hasBigRing; //4 == (clickPoint.ringTotal & 4);
        RingController smallring = null, midring = null, bigring = null;
        for (int i = 0; i < clickPoint.transform.childCount; i++) {
            RingController temp = clickPoint.transform.GetChild (i).GetComponent<RingController> ();
            if (temp.ringType == 1) {
                smallring = temp;
            } else if (temp.ringType == 2) {
                midring = temp;
            } else if (temp.ringType == 4) {
                bigring = temp;
            }
        }

        HashSet<DotController> smallColors = new HashSet<DotController> ();
        HashSet<DotController> midColors = new HashSet<DotController> ();
        HashSet<DotController> bigColors = new HashSet<DotController> ();
        foreach (DotController item in points) {
            if (item == clickPoint) { continue; }
            for (int i = 0; i < item.transform.childCount; i++) {
                RingController ring = item.transform.GetChild (i).GetComponent<RingController> ();
                if (smallring != null && smallring.colorIndex == ring.colorIndex) {
                    smallColors.Add (item);
                    // continue;
                }
                if (midring != null && midring.colorIndex == ring.colorIndex) {
                    midColors.Add (item);
                    // continue;
                }
                if (bigring != null && bigring.colorIndex == ring.colorIndex) {
                    bigColors.Add (item);
                }
            }
        }
        Debug.Log ("ringTotal =" + ringTotal + "small c=" + smallColors.Count + "mid c=" + midColors.Count + "big c=" + bigColors.Count);
        switch (ringTotal) {
            case 7:
                if (smallring.colorIndex == midring.colorIndex && midring.colorIndex == bigring.colorIndex) {

                    bool match = removeRings (smallColors, clickPoint, smallring);
                    clickPoint.ringTotal -= smallring.ringType;
                    smallring.destroyed = true;
                    smallring = null;
                    clickPoint.ringTotal -= bigring.ringType;
                    bigring.destroyed = true;
                    bigring = null;
                    clickPoint.ringTotal -= midring.ringType;
                    midring.destroyed = true;
                    midring = null;
                } else if (smallring.colorIndex == midring.colorIndex && midring.colorIndex != bigring.colorIndex) {
                    removeRings (bigColors, clickPoint, bigring);
                    bool match = removeRings (smallColors, clickPoint, smallring);
                    if (match) {
                        clickPoint.ringTotal -= midring.ringType;
                        midring.destroyed = true;
                        midring = null;
                    }

                } else if (midring.colorIndex == bigring.colorIndex && midring.colorIndex != smallring.colorIndex) {
                    removeRings (smallColors, clickPoint, smallring);
                    bool match = removeRings (midColors, clickPoint, midring);
                    if (match) {
                        clickPoint.ringTotal -= bigring.ringType;
                        bigring.destroyed = true;
                        bigring = null;
                    }
                } else if (smallring.colorIndex == bigring.colorIndex && smallring.colorIndex != midring.colorIndex) {
                    removeRings (midColors, clickPoint, midring);
                    bool match = removeRings (smallColors, clickPoint, smallring);
                    if (match) {
                        clickPoint.ringTotal -= bigring.ringType;
                        bigring.destroyed = true;
                        bigring = null;
                    }
                } else {
                    removeRings (midColors, clickPoint, midring);
                    removeRings (bigColors, clickPoint, bigring);
                    removeRings (smallColors, clickPoint, smallring);
                }
                break;
            case 6:
                if (midring.colorIndex == bigring.colorIndex) {
                    bool match = removeRings (midColors, clickPoint, midring);
                    if (match) {
                        clickPoint.ringTotal -= bigring.ringType;
                        bigring.destroyed = true;
                        bigring = null;
                    }
                } else {
                    removeRings (midColors, clickPoint, midring);
                    removeRings (bigColors, clickPoint, bigring);
                }
                break;
            case 5:
                if (bigring.colorIndex == smallring.colorIndex) {
                    bool match = removeRings (smallColors, clickPoint, smallring);
                    if (match) {
                        clickPoint.ringTotal -= bigring.ringType;
                        // Destroy (bigring.gameObject);
                        bigring.destroyed = true;
                        bigring = null;
                    }
                } else {
                    removeRings (smallColors, clickPoint, smallring);
                    removeRings (bigColors, clickPoint, bigring);
                }
                break;
            case 4:
                removeRings (bigColors, clickPoint, bigring);
                break;
            case 3:
                if (midring.colorIndex == smallring.colorIndex) {
                    bool match = removeRings (smallColors, clickPoint, smallring);
                    if (match) {
                        clickPoint.ringTotal -= midring.ringType;
                        midring.destroyed = true;
                        midring = null;
                    }
                } else {
                    removeRings (smallColors, clickPoint, smallring);
                    removeRings (midColors, clickPoint, midring);
                }
                break;
            case 2:
                removeRings (midColors, clickPoint, midring);
                break;
            case 1:
                removeRings (smallColors, clickPoint, smallring);
                break;
            default:
                break;
        }
        if (combo == 0) {
            previousCombo = combo;
        } else {
            previousCombo += combo;
        }

        Debug.Log ("smallColors count" + smallColors.Count + ",midColors count" + midColors.Count + ",bigColors count" + bigColors.Count);

        // }

        // foreach (DotController item in points) {
        // 	// if (item == clickPoint) { continue; }
        // 	for (int i = 0; i < item.transform.childCount; i++) {
        // 		RingController ring = item.transform.GetChild (i).GetComponent<RingController> ();
        // 		if (ring.destroyed) {
        // 			Destroy (ring.gameObject);
        // 		}
        // 	}
        // }
    }

    private bool removeRings (HashSet<DotController> sameColorRings, DotController clickPoint, RingController whichRing) {
        int x = clickPoint.x;
        int y = clickPoint.y;
        List<DotController> columnLine = new List<DotController> ();
        List<DotController> rowLine = new List<DotController> ();
        List<DotController> upLine = new List<DotController> ();
        List<DotController> downLine = new List<DotController> ();
        foreach (DotController item in sameColorRings) {
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
        Debug.Log ("column count" + columnLine.Count + ",row count=" + rowLine.Count + ",up count=" + upLine.Count + ",down count=" + downLine.Count);

        bool match = false, match1 = false, match2 = false, match3 = false, match4 = false;
        match1 = checkedMatch (columnLine, 0, clickPoint, whichRing);
        match2 = checkedMatch (rowLine, 1, clickPoint, whichRing);
        match3 = checkedMatch (downLine, 2, clickPoint, whichRing);
        match4 = checkedMatch (upLine, 3, clickPoint, whichRing);
        match = match1 | match2 | match3 | match4;
        if (match) {
            clickPoint.ringTotal -= whichRing.ringType;
            if (whichRing.ringType == 1) {
                clickPoint.smallRing = null;
            } else if (whichRing.ringType == 2) {
                clickPoint.mediumRing = null;
            } else if (whichRing.ringType == 4) {
                clickPoint.bigRing = null;
            }
            whichRing.destroyed = true;
        }
        return match;

    }

    void AddEffectLine (int type, Transform clickPoint, int colorIndex) {
        Line newLine = new Line ();
        newLine.Position = clickPoint.transform.position;
        newLine.Color = Color.red; //UIManager.ringColors[colorIndex];
        if (type == 0) {
            newLine.Angle = 0;
        } else if (type == 1) {
            newLine.Angle = 90;
        } else if (type == 2) {
            newLine.Angle = 45;
        } else {
            newLine.Angle = 135;
        }

        if (!listLine.Contains (newLine)) {
            listLine.Add (newLine);
        }
    }
    /// <summary>
    /// matchtype :  0- column ,1 -row ,2 -down, 3-up
    /// </summary>
    /// <param name="columnLine"></param>
    /// <param name="matchType"></param>
    public bool checkedMatch (List<DotController> columnLine, int matchType, DotController clickPoint, RingController whichRing) {
        bool hasMatch = false;
        Debug.Log ("whichRing total=" + whichRing.ringType);
        if (columnLine.Count == 2) {
            int sum = 1;
            foreach (DotController item in columnLine) {
                if (matchType == 0) {
                    sum *= (item.y - clickPoint.y);
                } else {
                    sum *= (item.x - clickPoint.x);
                }
            }
            Debug.Log ("sum=" + sum);
            if (sum == -1 || sum == 2) {
                destoryRing (columnLine, whichRing.colorIndex);
                AddEffectLine (matchType, whichRing.transform, whichRing.colorIndex);
                combo++;
                hasMatch = true;
            }
        } else if (columnLine.Count == 3) {
            int sum = 1;
            foreach (DotController item in columnLine) {
                if (matchType == 0) {
                    sum *= (item.y - clickPoint.y);
                } else {
                    sum *= (item.x - clickPoint.x);
                }
            }
            Debug.Log ("sum=" + sum);
            if (sum == -2 || sum == 2) {
                destoryRing (columnLine, whichRing.colorIndex);
                AddEffectLine (matchType, whichRing.transform, whichRing.colorIndex);
                combo++;
                hasMatch = true;
            }
        } else if (columnLine.Count == 4) {
            destoryRing (columnLine, whichRing.colorIndex);
            AddEffectLine (matchType, whichRing.transform, whichRing.colorIndex);
            combo++;
            hasMatch = true;
        };

        return hasMatch;
    }

    public void destoryRing (List<DotController> columnLine, int colorIndex) {
        foreach (DotController item in columnLine) {
            for (int i = 0; i < item.transform.childCount; i++) {
                RingController ring = item.transform.GetChild (i).GetComponent<RingController> ();
                if (ring.colorIndex == colorIndex) {
                    item.ringTotal -= ring.ringType;
                    if (ring.ringType == 1) {
                        ring.destroyed = true;
                        item.smallRing = null;
                    } else if (ring.ringType == 2) {
                        ring.destroyed = true;
                        item.mediumRing = null;
                    } else if (ring.ringType == 4) {
                        ring.destroyed = true;
                        item.bigRing = null;
                    }
                }
            }
        }
    }

}