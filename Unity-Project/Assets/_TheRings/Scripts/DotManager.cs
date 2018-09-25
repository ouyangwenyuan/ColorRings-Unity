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
    // Use this for initialization

    void Awake () {
        for (int i = 0; i < dots.Length; i++) {
            dots[i].GetComponent<DotController> ().dotIndex = i;
            dots[i].GetComponent<DotController> ().CheckRing ();
        }
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
            CaculateCombo ();
            if (combo > 1) {
                // == 2 ? targetComboTotalCnt + 2 : targetComboTotalCnt + 1;
                if (combo == 2) {
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
        UIManager.Instance.txtCombo.text = "x" + combo.ToString ();
    }

    void HideCombo () {
        UIManager.Instance.txtCombo.gameObject.SetActive (false);
    }

    public void ShowComboText (float waitTime) {
        StartCoroutine (ComboText (waitTime));
    }

    IEnumerator ComboText (float waitTime) {
        UIManager.Instance.txtComboText.text = "combo x" + combo.ToString ();
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
        int scoreAdded = combo * listDestroyRing.Count * 10;
        if (combo > 1) {
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
                UIManager.Instance.txtScore.text = ScoreManager.Instance.Score.ToString ();
            }
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
            tmp.startColor = UIManager.ringColors[listDestroyRing[i].GetComponent<RingController> ().colorIndex];
#else
            particle.startColor = UIManager.ringColors[listDestroyRing[i].GetComponent<RingController> ().colorIndex];
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

            combo += (ComboLine (0, 1, 2) + ComboLine (0, 4, 8) + ComboLine (0, 5, 6) + ComboDotIndex (dotIndex));

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

            combo += (ComboLine (0, 1, 2) + ComboLine (2, 3, 8) + ComboLine (2, 4, 6) + ComboDotIndex (dotIndex));

            if (combo == previousCombo) {
                combo = 0;
            }
        } else if (dotIndex == 3) //Check line 2_3_8, 3_4_5
        {
            previousCombo = combo;
            combo += (ComboLine (2, 3, 8) + ComboLine (3, 4, 5) + ComboDotIndex (dotIndex));

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
            combo += (ComboLine (0, 5, 6) + ComboLine (3, 4, 5) + ComboDotIndex (dotIndex));

            if (combo == previousCombo) {
                combo = 0;
            }
        } else if (dotIndex == 6) //Check line: 0_5_6, 2_4_6, 6_7_8
        {
            previousCombo = combo;
            combo += (ComboLine (0, 5, 6) + ComboLine (6, 7, 8) + ComboLine (2, 4, 6) + ComboDotIndex (dotIndex));

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
            combo += (ComboLine (2, 3, 8) + ComboLine (6, 7, 8) + ComboLine (0, 4, 8) + ComboDotIndex (dotIndex));

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
        newLine.Color = UIManager.ringColors[colorIndex];
        if (secondDot == 5 || secondDot == 3) {
            newLine.Angle = 0;
        } else if (secondDot == 1 || secondDot == 7) {
            newLine.Angle = 90;
        } else if (secondDot == 4) {
            if (firstDot == 1) {
                newLine.Angle = 0;
            } else if (firstDot == 3) {
                newLine.Angle = 90;
            } else if (firstDot == 2) {
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
}