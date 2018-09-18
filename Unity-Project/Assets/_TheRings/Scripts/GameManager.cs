using System.Collections;
using System.Collections.Generic;
using SgLib;
using UnityEngine;

public enum GameUIState {
    Prepare,
    Playing,
    Paused,
    PreGameOver,
    GameOver
}

public enum RingType {
    BIG_RING,
    NORMAL_RING,
    SMALL_RING
}

public class GameManager : MonoBehaviour {
    public static event System.Action<GameUIState, GameUIState> GameStateChanged = delegate { };

    public GameUIState GameUIState {
        get {
            return _gameState;
        }
        private set {
            if (value != _gameState) {
                GameUIState oldState = _gameState;
                _gameState = value;

                GameStateChanged (_gameState, oldState);
            }
        }
    }

    private GameUIState _gameState = GameUIState.Prepare;

    [Header ("Check to enable premium features (require EasyMobile plugin)")]
    public bool enablePremiumFeatures = true;

    [Header ("Gameplay Reference")]
    public DotManager dotManager;
    public UIManager uIManager;
    public GameObject randomPoint;

    [HideInInspector]
    public bool finishDrop;
    [HideInInspector]
    public bool finishMoveRing;
    [HideInInspector]
    public bool gameOver;
    [HideInInspector]
    public bool isPaused;

    [Header ("Gameplay Config")]

    public float ringMovingTime = 0.3f;
    public int initialColorNumber = 4;
    public int scoreToAddNewRingColor = 30;

    private List<int> listCreateRings = new List<int> ();
    private GameObject currentRing;
    public Vector2 firstRandomPointPosition;
    //Save the first position of random point
    private Vector2 originalRingPostion;
    //This is the postion that the ring is created
    private Vector2 theLeftPosition;
    //This is end position of ring when the ring moved
    private int scoreCounter;
    public bool allowDrag;
    public bool finishMoveRandomPointBack;
    public static GameManager Instance;

    private void Awake () {
        Instance = this;
    }
    void Start () {

        GameUIState = GameUIState.Prepare;
        Application.targetFrameRate = 60;
        ScoreManager.Instance.Reset ();
        scoreCounter = scoreToAddNewRingColor;

        Vector2 theBottomPosition = Camera.main.ViewportToWorldPoint (new Vector2 (0.5f, 0));
        theLeftPosition = Camera.main.ViewportToWorldPoint (new Vector2 (1f, 0));

        originalRingPostion = theBottomPosition + new Vector2 (0, -1f);

        //Save the first position of random point
        firstRandomPointPosition = randomPoint.transform.position;

        finishMoveRandomPointBack = true;

        StartCoroutine (CRStartGame ());
    }

    void Update () {
        if (isPaused) {
            return;
        }

        if (Input.GetMouseButtonDown (0)) //First touch
        {
            Vector2 touchPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition); //Tranform mouse position to world position

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
                randomPoint.transform.position = Camera.main.ScreenToWorldPoint (new Vector3 (x, y, 1f)) + new Vector3 (0, 0.3f, 0); // lift the ring a bit for not being covered by finger.
            }
        }

        if (Input.GetMouseButtonUp (0)) {
            if (allowDrag) {
                allowDrag = false;
                Vector2 mouseUpPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition); //Get mouse position
                GameObject theNearestDot = TheNearestDot (mouseUpPosition); //Find the nearest dot (place where the ring stay)

                Vector2 theNearestDotPosition = theNearestDot.transform.position;

                if ((mouseUpPosition - theNearestDotPosition).magnitude < 1f) //The ring not far away
                {
                    //Check if allow drop - > drop all ring to the dot
                    if (AllowDrop (theNearestDot)) {
                        SoundManager.Instance.PlaySound (SoundManager.Instance.dropRing);
                        dotManager.dotIndex = theNearestDot.GetComponent<DotController> ().dotIndex;
                        while (randomPoint.transform.childCount > 0) {
                            StartCoroutine (MoveRingToTheDot (randomPoint.transform.GetChild (0).gameObject, theNearestDot.transform.position));
                            randomPoint.transform.GetChild (0).transform.parent = theNearestDot.transform;
                        }
                    }
                }

                //Move the ranrom point back to the first position of it
                StartCoroutine (MoveRandomPointBack (randomPoint, firstRandomPointPosition));
            }
        }

        //After check -> generate rings
        if (dotManager.finishCheckAfterDestroy && finishMoveRandomPointBack) {
            finishMoveRandomPointBack = false;
            dotManager.finishCheckAfterDestroy = false;
            // UpdateColorNumber ();
            GenerateRings ();
            uIManager.CheckAndShowWatchAdOption ();
        }
    }

    IEnumerator CRStartGame () {
        while (UIManager.firstLoad) {
            yield return null;
        }

        GenerateRings ();

        GameUIState = GameUIState.Playing;
    }

    void GameOver () {
        GameUIState = GameUIState.GameOver;
    }

    void UpdateColorNumber () {
        if (ScoreManager.Instance.Score > scoreCounter) {
            if (initialColorNumber < UIManager.ringColors.Length - 1) {
                scoreCounter += scoreToAddNewRingColor;
                initialColorNumber += 1;
            }
        }
    }

    //Find the nearest dot when the mouse up
    public GameObject TheNearestDot (Vector2 mouseUpPosition) {
        GameObject theNearestDot = dotManager.dots[0];
        Vector2 dotPosition = dotManager.dots[0].transform.position;
        float minDistance = (mouseUpPosition - dotPosition).magnitude;

        for (int i = 0; i < dotManager.dots.Length; i++) {
            dotPosition = dotManager.dots[i].transform.position;
            if ((mouseUpPosition - dotPosition).magnitude < minDistance) {
                minDistance = (mouseUpPosition - dotPosition).magnitude;
                theNearestDot = dotManager.dots[i];
            }
        }
        return theNearestDot;
    }

    public IEnumerator ChangeRing () {
        GameObject theDestroyOb = new GameObject ();
        theDestroyOb.transform.position = randomPoint.transform.position;

        while (randomPoint.transform.childCount > 0) {
            randomPoint.transform.GetChild (0).transform.parent = theDestroyOb.transform;
        }

        Vector2 startPos = theDestroyOb.transform.position;
        Vector2 endPos = startPos + new Vector2 (theLeftPosition.x + 1f, 0);

        float t = 0;
        while (t < ringMovingTime) {
            t += Time.deltaTime;
            float fraction = t / ringMovingTime;
            theDestroyOb.transform.position = Vector2.Lerp (startPos, endPos, fraction);
            yield return null;
        }

        GenerateRings ();
        Destroy (theDestroyOb);
    }

    //Generate Rings
    public void GenerateRings () {
        int createBigRing = 1;
        int createNormalRing = 2;
        int createSmallRing = 3;
        int createBigAndNormalRing = 4;
        int createBigAndSmallRing = 5;
        int createNormalAndSmallRing = 6;
        for (int i = 0; i < dotManager.dots.Length; i++) {
            DotController dotController = dotManager.dots[i].GetComponent<DotController> ();

            if (dotController.isEmptyRing) //Has no ring
            {
                AddValueForList (1);
                AddValueForList (2);
                AddValueForList (2);
                AddValueForList (4);
                AddValueForList (5);
                AddValueForList (6);
                break;
            }
            //Has big ring
            else if (dotController.hasBigRing && !dotController.hasNormalRing && !dotController.hasSmallRing) {
                //Has big ring -> create normal ring or small ring or normal and small ring
                AddValueForList (createNormalRing);
                AddValueForList (createSmallRing);
                AddValueForList (createNormalAndSmallRing);
            }
            //Has normal ring
            else if (dotController.hasNormalRing && !dotController.hasBigRing && !dotController.hasSmallRing) {
                //Has normal ring -> create big ring or small ring or big and small ring
                AddValueForList (createBigRing);
                AddValueForList (createSmallRing);
                AddValueForList (createBigAndSmallRing);
            }
            //Has small ring
            else if (dotController.hasSmallRing && !dotController.hasBigRing && !dotController.hasNormalRing) {
                //Has small ring -> create big ring or normal ring or big and normal ring
                AddValueForList (createBigRing);
                AddValueForList (createNormalRing);
                AddValueForList (createBigAndNormalRing);
            }
            //Has big and normal ring
            else if (dotController.hasBigRing && dotController.hasNormalRing && !dotController.hasSmallRing) {
                //Has big and normal ring -> create small ring
                AddValueForList (createSmallRing);
            }
            //Has big and small ring
            else if (dotController.hasBigRing && dotController.hasSmallRing && !dotController.hasNormalRing) {
                //Has big and small ring -> create normal ring
                AddValueForList (createNormalRing);
            }
            //Has normal and small ring
            else if (dotController.hasNormalRing && dotController.hasSmallRing && !dotController.hasBigRing) {
                //Has normal and small ring -> create big ring
                AddValueForList (createBigRing);
            }
        }
        if (listCreateRings.Count == 0) {
            SoundManager.Instance.PlaySound (SoundManager.Instance.gameOver);
            gameOver = true;
            GameOver ();
        } else {
            int listIndex = Random.Range (0, listCreateRings.Count);
            if (listCreateRings[listIndex] == 1) {
                //Create big ring
                StartCoroutine (CreateAndMoveRing (UIManager.Instance.bigRing));
            } else if (listCreateRings[listIndex] == 2) {
                //Create normal ring
                StartCoroutine (CreateAndMoveRing (UIManager.Instance.normalRing));
            } else if (listCreateRings[listIndex] == 3) {
                //create small ring
                StartCoroutine (CreateAndMoveRing (UIManager.Instance.smallRing));
            } else if (listCreateRings[listIndex] == 4) {
                //Create big and normal ring
                StartCoroutine (CreateAndMoveRing (UIManager.Instance.bigRing));
                StartCoroutine (CreateAndMoveRing (UIManager.Instance.normalRing));
            } else if (listCreateRings[listIndex] == 5) {
                //Create big and small ring
                StartCoroutine (CreateAndMoveRing (UIManager.Instance.bigRing));
                StartCoroutine (CreateAndMoveRing (UIManager.Instance.smallRing));
            } else {
                //Create normal and small ring
                StartCoroutine (CreateAndMoveRing (UIManager.Instance.normalRing));
                StartCoroutine (CreateAndMoveRing (UIManager.Instance.smallRing));
            }
        }

        listCreateRings.Clear ();
    }

    void AddValueForList (int value) {
        if (!listCreateRings.Contains (value)) {
            listCreateRings.Add (value);
        }
    }

    IEnumerator CreateAndMoveRing (GameObject ring) {
        finishMoveRing = false;

        GameObject currentRing = Instantiate (ring, originalRingPostion, Quaternion.identity) as GameObject;
        int colorIndex = Random.Range (0, initialColorNumber);
        currentRing.GetComponent<SpriteRenderer> ().color = UIManager.ringColors[colorIndex];
        currentRing.GetComponent<RingController> ().colorIndex = colorIndex;

        float t = 0;
        Vector2 startPos = currentRing.transform.position;
        Vector2 endPos = randomPoint.transform.position;
        while (t < ringMovingTime) {
            t += Time.deltaTime;
            float fraction = t / ringMovingTime;
            currentRing.transform.position = Vector2.Lerp (startPos, endPos, fraction);
            yield return null;
        }
        currentRing.transform.parent = randomPoint.transform;

        finishMoveRing = true;
    }

    //Check to allow drop ring to the dot or not
    public bool AllowDrop (GameObject theNearestDot) {
        DotController dotController = theNearestDot.GetComponent<DotController> ();
        if (dotController.isFullRing) //Has 3 rings
        {
            return false;
        }
        //--------------------Has 2 rings
        else if (dotController.hasBigRing && dotController.hasNormalRing && !dotController.hasSmallRing) //Has big and normal ring
        {
            //Loop all child of random point and check, if it has big ring or normal ring -> return false
            for (int i = 0; i < randomPoint.transform.childCount; i++) {
                RingController ringController = randomPoint.transform.GetChild (i).GetComponent<RingController> ();
                if (ringController.ringType == RingType.BIG_RING || ringController.ringType == RingType.NORMAL_RING) {
                    return false;
                }
            }
            return true;
        } else if (dotController.hasBigRing && dotController.hasSmallRing && !dotController.hasNormalRing) //Has big and small ring
        {
            //Loop all child of random point and check, if it has big ring or small ring -> return false
            for (int i = 0; i < randomPoint.transform.childCount; i++) {
                RingController ringController = randomPoint.transform.GetChild (i).GetComponent<RingController> ();
                if (ringController.ringType == RingType.BIG_RING || ringController.ringType == RingType.SMALL_RING) {
                    return false;
                }
            }
            return true;
        } else if (dotController.hasNormalRing && dotController.hasSmallRing && !dotController.hasBigRing) //Has normal and small ring
        {
            //Loop all child of random point and check, if it has normal ring or small ring -> return false
            for (int i = 0; i < randomPoint.transform.childCount; i++) {
                RingController ringController = randomPoint.transform.GetChild (i).GetComponent<RingController> ();
                if (ringController.ringType == RingType.NORMAL_RING || ringController.ringType == RingType.SMALL_RING) {
                    return false;
                }
            }
            return true;
        }
        //-------------------Has 1 ring
        else if (dotController.hasBigRing && !dotController.hasNormalRing && !dotController.hasSmallRing) //Has big ring
        {
            //Loop all child of random point and check, if it has big ring -> return false
            for (int i = 0; i < randomPoint.transform.childCount; i++) {
                RingController ringController = randomPoint.transform.GetChild (i).GetComponent<RingController> ();
                if (ringController.ringType == RingType.BIG_RING) {
                    return false;
                }
            }
            return true;
        } else if (dotController.hasNormalRing && !dotController.hasBigRing && !dotController.hasSmallRing) //Has normal ring
        {
            //Loop all child of random point and check, if it has normal ring -> return false
            for (int i = 0; i < randomPoint.transform.childCount; i++) {
                RingController ringController = randomPoint.transform.GetChild (i).GetComponent<RingController> ();
                if (ringController.ringType == RingType.NORMAL_RING) {
                    return false;
                }
            }
            return true;
        } else if (dotController.hasSmallRing && !dotController.hasBigRing && !dotController.hasNormalRing) //Has small ring
        {
            //Loop all child of random point and check, if it has small ring -> return false
            for (int i = 0; i < randomPoint.transform.childCount; i++) {
                RingController ringController = randomPoint.transform.GetChild (i).GetComponent<RingController> ();
                if (ringController.ringType == RingType.SMALL_RING) {
                    return false;
                }
            }
            return true;
        } else //Has no ring
        {
            return true;
        }
    }

    public IEnumerator MoveRingToTheDot (GameObject theRing, Vector2 endPos) {
        Vector2 startPos = theRing.transform.position;
        float t = 0;
        while (t < 0.1f) {
            t += Time.deltaTime;
            float fraction = t / 0.1f;
            theRing.transform.position = Vector2.Lerp (startPos, endPos, fraction);
            yield return null;
        }

        finishDrop = true;
    }

    public IEnumerator MoveRandomPointBack (GameObject randomPoint, Vector2 endPos) {
        Vector2 startPos = randomPoint.transform.position;
        float t = 0;
        while (t < 0.1f) {
            t += Time.deltaTime;
            float fraction = t / 0.1f;
            randomPoint.transform.position = Vector2.Lerp (startPos, endPos, fraction);
            yield return null;
        }

        finishMoveRandomPointBack = true;
    }
}