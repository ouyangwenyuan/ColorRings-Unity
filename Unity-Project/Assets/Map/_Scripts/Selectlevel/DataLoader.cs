using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataLoader : MonoBehaviour {
    public class LevelPoint {
        public float x;
        public float y;

        public LevelPoint (float x, float y) {
            this.x = x;
            this.y = y;
        }

    }
    public static DataLoader Data; // instance of  this class

    // public  List<Player> MyData ;     // list of Player object

    // const string KEY_DATA = "DATA";                             // key PlayerPrefs

    const string KEY_FRISTTIME = "sp_first_play_time"; // key check first app to play

    public static bool enableclick;

    public static bool fromMain;

    public const string KEY_MAPPOS = "mappos";

    public GameObject map;

    // private GameObject tmp;

    private GameObject mapParent;

    // public List<GameObject> listmap1;

    private GameObject[] listmap;

    private Vector2[] mappos;

    public UnityEngine.UI.Image processbar;

    // public GameObject fade;

    public Sprite[] MapSprite;

    bool hold;

    GameObject holdobj;

    const float STARMOVE_TIME = 0.3f;
    const int PageCount = 42;
    const int PointPerPage = 24;

    private GameObject Background;
    LevelPoint[] pagePoints; //每个page上的相对坐标 :与真实像素比：1:100
    void Awake () {
        Data = this;
    }
    void Start () {
        GameObject screen = new GameObject ("Screen"); //GameObject.Find ("Screen");
        screen.transform.position = Vector3.zero;
        screen.transform.parent = transform.parent;
        Background = new GameObject ("Background");
        Background.transform.position = Vector3.zero;
        Background.transform.parent = screen.transform; //GameObject.Find ("Background");
        mapParent = new GameObject ("Maps");
        mapParent.transform.position = new Vector3 (0, 0, -0.1f);
        mapParent.transform.parent = screen.transform;

        for (int i = 0; i < 42; i++) {
            GameObject page = Instantiate (MonoUtils.instance.pageBgPrefab);
            page.transform.SetParent (Background.transform);
            page.transform.position = new Vector3 (0, 8 * i, 0);
            page.GetComponent<SpriteRenderer> ().sprite = MonoUtils.instance.PageBg;
        }

        Time.timeScale = 1;
        listmap = new GameObject[CommonConst.default_total_levels];
        mappos = new Vector2[CommonConst.default_total_levels];

        pagePoints = new LevelPoint[PointPerPage];
        pagePoints[0] = new LevelPoint (3.06f, 0.435f);
        pagePoints[1] = new LevelPoint (3.38f, 1.015f);
        pagePoints[2] = new LevelPoint (2.95f, 1.77f);
        pagePoints[3] = new LevelPoint (2.07f, 1.77f);
        pagePoints[4] = new LevelPoint (1.62f, 2.53f);
        pagePoints[5] = new LevelPoint (2.07f, 3.30f);
        pagePoints[6] = new LevelPoint (2.94f, 3.30f);
        pagePoints[7] = new LevelPoint (3.39f, 2.54f);
        pagePoints[8] = new LevelPoint (3.72f, 3.11f);
        pagePoints[9] = new LevelPoint (4.05f, 3.68f);
        pagePoints[10] = new LevelPoint (3.62f, 4.41f);
        pagePoints[11] = new LevelPoint (2.72f, 4.41f);
        pagePoints[12] = new LevelPoint (1.84f, 4.41f);
        pagePoints[13] = new LevelPoint (1.00f, 4.41f);
        pagePoints[14] = new LevelPoint (1.42f, 5.18f);
        pagePoints[15] = new LevelPoint (1.85f, 5.93f);
        pagePoints[16] = new LevelPoint (2.30f, 5.18f);
        pagePoints[17] = new LevelPoint (3.18f, 5.18f);
        pagePoints[18] = new LevelPoint (3.64f, 5.94f);
        pagePoints[19] = new LevelPoint (4.06f, 6.70f);
        pagePoints[20] = new LevelPoint (3.39f, 6.70f);
        pagePoints[21] = new LevelPoint (2.73f, 6.70f);
        pagePoints[22] = new LevelPoint (2.40f, 7.20f);
        pagePoints[23] = new LevelPoint (2.77f, 7.84f);

        for (int i = 0; i < pagePoints.Length; i++) {
            pagePoints[i].x = pagePoints[i].x - 2.4f;
            pagePoints[i].y = pagePoints[i].y - 4 - 0.30f;
        }

        // pagePoints[24] = new LevelPoint(0.44f, 0.44f);

        //PlayerPrefs.DeleteKey(KEY_FRISTTIME);

        // if (PlayerPrefs.GetInt(KEY_FRISTTIME, 0) == 0)
        // {
        //     PlayerPrefs.SetString(KEY_DATA, datadefaut);
        //     PlayerPrefs.SetInt(KEY_FRISTTIME, 1);
        // }
        CameraMovement.mcamera.StarPoint.transform.GetChild (0).localPosition = new Vector3 (0, 0.619f, 0);
        StartCoroutine (MapButtonDrawer ());
    }

    GameObject moveobj (Vector3 mouseposition) {
        Vector3 wp = Camera.main.ScreenToWorldPoint (mouseposition);
        Vector2 touchPos = new Vector2 (wp.x, wp.y);
        if (Physics2D.OverlapPoint (touchPos)) {
            GameObject tmp = Physics2D.OverlapPoint (touchPos).gameObject;
            if (tmp != null && tmp.tag == "map") {
                return tmp;
            }
        }
        return null;
    }

    /// <summary>
    /// Draw buttons level on scene
    /// </summary>
    /// <returns></returns>
    IEnumerator MapButtonDrawer () {
        DataLoader.enableclick = false;
        MapPosD ();
        processbar.fillAmount = 0.1f;
        yield return new WaitForSeconds (0.1f);
        // Debug.Log("1");
        // PlayerUtils pu = new PlayerUtils();
        GameState.Load ();
        // CameraMovement.mcamera.MyData = MyData;
        int currentLevel = PlayerPrefs.GetInt (CommonConst.PrefKeys.CURRENT_LEVEL, 1) - 1;
        CameraMovement.StarPointMoveIndex = currentLevel;
        // GameState.levelDatas = MyData;
        // processbar.fillAmount = 0.2f;
        // Debug.Log("2");
        // yield return new WaitForSeconds(0.1f);

        int step1 = CommonConst.default_total_levels / 40;
        // int step2 = step1 * 2;
        for (int i = 0; i < step1; i++) {
            processbar.fillAmount += 1.0f / step1;
            fillPoint (i, step1);
            yield return null;
        }
        // Debug.Log("3");
        processbar.transform.parent.gameObject.SetActive (false);
        DataLoader.enableclick = true;

        if (CameraMovement.StarPointMoveIndex != -1 && CameraMovement.StarPointMoveIndex != 1000) {
            StarPointMove ();
            // yield return new WaitForSeconds (STARMOVE_TIME);

            CameraMovement.mcamera.PopUpShow (GameState.levelDatas[CameraMovement.StarPointMoveIndex]);

            PlayerPrefs.SetFloat ("LASTPOS", listmap[CameraMovement.StarPointMoveIndex].transform.position.y);
            PlayerPrefs.SetFloat ("LASTPOSX", listmap[CameraMovement.StarPointMoveIndex].transform.position.x);

        } else {
            // fade.GetComponent<CanvasGroup> ().blocksRaycasts = false;
            CameraMovement.mcamera.StarPoint.transform.GetChild (0).GetComponent<Animation> ().Play ("StarPoint");
        }
    }

    void fillPoint (int i, int step1) {
        for (int j = 0; j < 40; j++) {
            insmap (mappos[i + step1 * j], i + step1 * j);
        }
        // insmap(mappos[i + step1 * 0], i + step1 * 0);
        // insmap(mappos[i + step1 * 1], i + step1 * 1);
        // insmap(mappos[i + step1 * 2], i + step1 * 2);
        // insmap(mappos[i + step1 * 3], i + step1 * 3);
        // insmap(mappos[i + step1 * 4], i + step1 * 4);
        // insmap(mappos[i + step1 * 5], i + step1 * 5);
        // insmap(mappos[i + step1 * 6], i + step1 * 6);
        // insmap(mappos[i + step1 * 7], i + step1 * 7);
        // insmap(mappos[i + step1 * 8], i + step1 * 8);
        // insmap(mappos[i + step1 * 9], i + step1 * 9);
        // insmap(mappos[i + step1 * 10], i + step1 * 10);
        // insmap(mappos[i + step1 * 11], i + step1 * 11);
        // insmap(mappos[i + step1 * 12], i + step1 * 12);
        // insmap(mappos[i + step1 * 13], i + step1 * 13);
        // insmap(mappos[i + step1 * 14], i + step1 * 14);
        // insmap(mappos[i + step1 * 15], i + step1 * 15);
        // insmap(mappos[i + step1 * 16], i + step1 * 16);
    }

    Vector3 StringToVector3 (string s) {
        Vector3 vt = Vector3.zero;
        string[] p = s.Split (',');
        vt = new Vector3 (float.Parse (p[0]), float.Parse (p[1]));
        return vt;
    }

    void insmap (Vector3 pos, int index) {

        GameObject tmp = (GameObject) Instantiate (map);
        tmp.transform.position = new Vector3 (pos.x, pos.y);
        tmp.transform.SetParent (mapParent.transform, false);
        listmap[index] = tmp;
        tmp.transform.GetChild (1).GetComponent<TextMesh> ().text = (index + 1).ToString ();
        tmp.name = (index + 1).ToString ();
        Map m = tmp.GetComponent<Map> ();
        m.map = GameState.levelDatas[index];

        // TODO: 此时的currentLevel 如果 > 10 可能是累计跳关了.
        // 动态获取对应的data? 并且改变界面的显示 ?
        // 需要currentLevel - skip level num 然后 在点击跳转game的时候 在加回来
        // if (currentLevel > 10)
        // {
        //     int test = currentLevel - PlayerPrefs.GetInt(CommonConst.PREF_SKIP_TOTAL_EXP_LEVEL, 0);
        //     if (test > 10)
        //     {
        //         currentLevel = test;
        //     }
        // }

        m.SetMapInfo ();
    }

    void StarPointMove () {
        DataLoader.enableclick = false; //
        Vector3 newpos = listmap[CameraMovement.StarPointMoveIndex].transform.position + new Vector3 (0, 0, -0.3f);

        Ulti.MoveTo (CameraMovement.mcamera.StarPoint, newpos, 1f, newpos.z);
        StartCoroutine (stopanimation ());
    }
    IEnumerator stopanimation () {
        yield return new WaitForSeconds (STARMOVE_TIME);
        CameraMovement.mcamera.StarPoint.transform.GetChild (0).GetComponent<Animation> ().Play ("StarPoint");

    }

    #region
    // string datadefaut = "False,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,1,True,0,0,1,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,1,True,0,0,1,True,0,0,1,True,0,0,1,True,0,0,1,True,0,0,1,True,0,0,1,True,0,0,1,True,0,0,1,True,0,0,1,True,0,0,1,True,0,0,1,True,0,0,1,True,0,0,1,True,0,0,1,True,0,0,1,True,0,0,1,True,0,0,1,True,0,0,1,True,0,0,1,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,3,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,1,True,0,0,1,True,0,0,1,True,0,0,1,True,0,0,1,True,0,0,1,True,0,0,1,True,0,0,1,True,0,0,1,True,0,0,1,True,0,0,1,True,0,0,1,True,0,0,1,True,0,0,1,True,0,0,1,True,0,0,1,True,0,0,1,True,0,0,1,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,0,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,True,0,0,2,";
    /// <summary>
    /// position of buttons level is fixed
    /// </summary>
    // string level_pos = "-0.004228723,-3.587,0.695,-3.232,-0.09021276,-3.03,-0.925,-2.859967,-0.188883,-2.489,0.6173936,-2.367,1.342,-2,0.5240414,-1.590361,-0.2140727,-1.613735,-1.06,-1.636,-1.579,-1.265327,-1.274,-0.7695142,-0.8071734,0.4705882,-0.9679077,1,-1.731092,1.497479,-0.9021276,1.77,-0.05638298,1.736,0.7893617,1.752,1.557,1.98,1.307,2.653,0.439,2.86,-0.271578,2.98,-0.9669681,3.106,-1.39,3.66,-0.8239896,4.221,-1,4.758,-1.562,5.279,-0.996,5.581,-0.2349291,5.59,0.4792553,5.6,1.184042,5.72,1.631163,6.270145,1.090071,6.87,0.3523936,7.128,-0.3636702,7.196,-1.067,7.494,-0.963,8.204,-0.601,8.72,0.017,9.103,0.5887378,9.264387,1.207,9.45,1.583,9.921,0.9122202,10.28887,0.3170127,10.39903,-0.3274053,10.45336,-0.9444384,10.69456,-1.259,11.225,-1.013,11.708,-0.62,12.499,-0.173,12.927,0.2264377,13.25734,0.6922522,13.51896,1.184,13.786,1.528,14.277,1.381,14.842,0.818,15.127,0.2264377,15.20605,-0.329952,15.26113,-0.9251596,15.40536,-1.41,15.694,-1.674,16.34,-1.663,16.935,-1.254,17.392,-0.615,17.391,0.03234824,17.3409,0.7051916,17.40975,1.337,17.79,1.428,18.401,1.139,18.952,0.698,19.339,0.218,19.65,-0.3040734,19.95054,-0.595,20.795,-1.171,21.165,-1.759,21.565,-0.848,21.87,-0.1488019,21.88109,0.6146165,21.9224,1.257,22.087,1.636,22.677,1.002796,23.1341,0.2911341,23.24426,-0.4205271,23.28556,-1.067491,23.36818,-1.568,23.924,-0.769888,24.30333,0.006469648,24.34464,0.848,24.354,1.634,24.472,2.097,25.069,1.583,25.679,0.796,25.85,-0.05822683,25.80419,-0.8345845,25.68026,-1.657,25.697,-1.737,26.462,-1.155,27.123,-0.213,27.34,0.429,27.742,0.892,28.338,0.986,29.143,0.455,29.713,-0.1876198,30.10146,-0.8863418,30.37684,-1.542,30.722,-1.79,31.43,-1.293,31.832,-0.537,31.743,0.493,31.705,1.339217,31.8842,2.045,32.401,1.5,33.043,0.7569487,33.12344,-0.03234824,33.09591,-0.7569487,33.09591,-1.442731,33.26114,-1.326278,34.01845,-0.5887378,34.21122,0.09704469,34.23876,0.8216451,34.32138,1.563,34.803,1.054552,35.4792,0.2523163,35.56182,-0.55,35.563,-1.371,35.665,-1.954,36.421,-1.927,37.132,-1.365096,37.64099,-0.5757986,37.66853,0.2264377,37.29676,1.05,37.046,1.589,37.553,1.451,38.275,0.705,38.834,0.03234824,39.11264,-0.375,39.774,0.2134984,40.287,0.95,40.561,1.352,41.178,0.7957666,41.69,-0.07116612,41.865,-0.8345845,42.07,-1.391,42.64414,-0.8475238,43.2008,0.05822683,43.40195,0.874,43.678,1.15,44.36,0.54992,44.88013,-0.2134984,44.918,-0.9251596,45.02,-1.631,45.40336,-1.69,46.1,-1.132188,46.68477,-0.4205271,46.89131,0.2781948,47.08408,0.982,47.572,1.212,48.299,0.7828273,48.911,0.109984,49.03011,-0.6275558,49.04388,-1.379,49.142,-1.592,49.856,-0.8863418,50.31066,-0.1876198,50.442,0.6146165,50.44,1.264,50.744,0.866,51.208,0.2393769,51.482,-0.382,52.011,0.1229233,52.654,0.848,52.865,1.449,53.399,0.9122202,53.93117,0.1746805,54.11017,-0.6016772,54.31671,-1.346,54.684,-1.632,55.268,-0.731,55.509,0.05822683,55.50087,0.860463,55.45956,1.39,56.074,0.8345845,56.79091,0.1229233,57.0663,-0.6016772,57.159,-1.291,57.354,-1.478,57.954,-1.041613,58.51208,-0.3428913,58.7737,0.4334664,58.7737,1.095,58.771,1.654,59.268,1.698,60.006,0.2523163,61.01823,-0.4981629,61.01926,-1.18,61.044,-1.764,61.535,-1.27452,61.99689,-0.64,62.125,0.07116612,62.09327,0.8216451,62.07951,1.577,62.14,1.507428,62.802,0.860463,63.042,0.1876198,63.20859,-0.554,63.559,-0.679313,64.07061,-0.006469648,64.26888,0.679313,64.3515,1.331,64.598,1.733,65.177,1.416,65.765,0.7957666,66.01759,0.1617411,66.18282,-0.4852235,66.34806,-1.118,66.678,-1.479,67.221,-1.387,67.987,-1.241,68.757,-0.8345845,69.316,-0.1617411,69.34177,0.4722843,69.35554,1.09337,69.45193,1.652,69.92,1.46861,70.59,0.8345845,70.81509,0.1488019,70.98032,-0.534,71.172,-1.156,71.48979,-1.551,72.155,-1.028674,72.745,-0.2781948,72.78366,0.4464056,72.639,1.113,72.598,1.525,73.121,1.506,73.837,1.011,74.29829,0.3946485,74.577,-0.2264377,74.80776,-0.861,75.08315,-1.428,75.544,-1.464,76.365,-0.7569487,76.68608,-0.07116612,76.72739,0.6404951,76.762,1.371,76.925,1.544,77.566,0.951038,78.024,0.2781948,78.231,-0.446,78.376,-1.101,78.629,-1.63,79.149,-1.609,79.814,-1.025,80.375,-0.2393769,80.52411,0.3817092,80.60673,0.936,80.96473,1.248642,81.57059,1.171006,82.25905,0.5369807,82.43806,-0.1358626,82.43806,-0.7569487,82.45182,-1.329,82.676,-1.651,83.315,-1.723,83.929,-1.331,84.601,-0.7181308,84.98347,-0.07116612,84.88708,0.6146165,84.74938,1.261581,84.74938,1.38,85.375,0.941,85.829,0.2781948,86.16763,-0.4075879,86.27779,-1.052,86.4,-1.624,86.823,-1.746,87.543,-1.217,87.962,-0.413,87.571,0.3170127,87.69275,0.8087059,88.20222,1.653,88.344,1.891,88.914,1.64976,89.57915,1.080431,89.84077,0.3946485,89.68931,-0.200559,89.40015,-0.8216451,89.20738,-1.503,89.506,-1.736,90.223,-1.506,91.0685,-0.7440094,91.38519,0.122,91.242,0.93,91.686";
    void MapPosD () {
        // 17 张图， 每张图24 点，总共 17*24 = 408 个点
        for (int i = 0; i < PageCount; i++) {
            for (int j = 0; j < PointPerPage; j++) {
                mappos[j + i * PointPerPage] = new Vector2 (pagePoints[j].x, pagePoints[j].y + i * 8);
            }
        }
    }
    // void MapPosD()
    // {
    // string[] postions = level_pos.Split(',');
    // for(int i = 0; i < 293;i ++) {
    //     mappos[i] = new Vector2(float.Parse(postions[2*i]), float.Parse(postions[2*i+1]));
    // }
    // //{1.5,1.2,0.9,0.6,0.3,0,-0.3,-0.6,-0.9,-1.2,-1.5}
    // for (int i = 293; i < 393; i++)
    // {
    //     float start = (i - 293) * 0.32f;
    //     float x = 1.5f * Mathf.Sin(Mathf.PI / 2 * start + Mathf.PI / 2);
    //     mappos[i] = new Vector2(x, 92f + start);
    //     // float x = 1.5f - (i % 10) * 0.3f;
    //     // if (i % 20 >= 10){
    //     //     x = - x ;
    //     //     mappos[i] = new Vector2(x, 92f + (i - 293) * 0.3f);
    //     // }else {
    //     //     mappos[i] = new Vector2(x, 92f + (i - 293) * 0.3f);
    //     // }
    // }
    // mappos[393] = new Vector2(1.492f, 124.37659f);
    // mappos[394] = new Vector2(0.769888f, 124.56936f);
    // mappos[395] = new Vector2(-0.03234824f, 124.5969f);
    // mappos[396] = new Vector2(-0.956f, 124.74f);
    // mappos[397] = new Vector2(-0.84f, 125.2f);
    // mappos[398] = new Vector2(-0f, 126.1f);
    // mappos[399] = new Vector2(1f, 126.7f);
    // }
    #endregion

}