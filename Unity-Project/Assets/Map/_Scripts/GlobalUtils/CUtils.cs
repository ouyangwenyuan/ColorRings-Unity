using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditorInternal;
#endif
using System.Reflection;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CUtils {

    public static bool isRewardAdFirstInGameScene = true; // 这是个变量
#if UNITY_ANDROID && !UNITY_EDITOR
    private static AndroidJavaClass cls_UnityPlayer;
    private static AndroidJavaObject obj_Activity;
#endif

    static CUtils () {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJNI.AttachCurrentThread ();
        cls_UnityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
        obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject> ("currentActivity");
#endif
    }

    public static void ShowToast (string message) {
#if UNITY_ANDROID && !UNITY_EDITOR
        obj_Activity.Call ("showToast", message);
#endif
    }

    public static bool IsAppInstalled (string packageName) {
#if UNITY_ANDROID && !UNITY_EDITOR
        return obj_Activity.Call<bool> ("isAppInstalled", packageName);
#else
        return false;
#endif
    }

    public static void PushNotification (int id) {
#if UNITY_ANDROID && !UNITY_EDITOR
        obj_Activity.Call ("pushNotificaiton", id);
#endif
    }

    public static void RateGame () {
        // if (JobWorker.instance.onLink2Store != null) {
        //     JobWorker.instance.onLink2Store ();
        // }
        OpenStore ();
        SetRateGame ();
    }

    public static void OpenStore () {
#if UNITY_ANDROID || UNITY_EDITOR
        Application.OpenURL ("https://play.google.com/store/apps/details?id=" + CommonConst.androidPackageID);
#elif UNITY_IPHONE
        Application.OpenURL ("https://itunes.apple.com/app/id" + CommonConst.iosAppID);
#endif
    }

    public static void OpenStore (string id) {
#if UNITY_ANDROID || UNITY_EDITOR
        Application.OpenURL ("https://play.google.com/store/apps/details?id=" + id);
#elif UNITY_IPHONE
        Application.OpenURL ("https://itunes.apple.com/app/id" + id);
#endif
    }

    public static void LikeFacebookPage (string faceID) {
#if UNITY_ANDROID && !UNITY_EDITOR
        Application.OpenURL ("fb://page/" + faceID);
#else
        Application.OpenURL ("https://www.facebook.com/" + faceID);
#endif
        SetLikeFbPage (faceID);
    }

    public static string ReadFileContent (string path) {
        TextAsset file = Resources.Load (path) as TextAsset;
        return file == null ? null : file.text;
    }

    public static Vector3 CopyVector3 (Vector3 ori) {
        Vector3 des = new Vector3 (ori.x, ori.y, ori.z);
        return des;
    }

    public static bool EqualVector3 (Vector3 v1, Vector3 v2) {
        return Vector3.SqrMagnitude (v1 - v2) <= 0.0000001f;
    }

    public static float GetSign (Vector3 A, Vector3 B, Vector3 M) {
        return Mathf.Sign ((B.x - A.x) * (M.y - A.y) - (B.y - A.y) * (M.x - A.x));
    }

    public static Vector3 RotatePointAroundPivot (Vector3 point, Vector3 pivot, Vector3 angles) {
        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler (angles) * dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point; // return it
    }

    public static void Shuffle<T> (T[] array) {
        for (int i = 0; i < array.Length; i++) {
            var temp = array[i];
            var randomIndex = UnityEngine.Random.Range (0, array.Length);
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }

    public static string[] SeparateLines (string lines) {
        return lines.Replace ("\r\n", "\n").Replace ("\r", "\n").Split ("\n" [0]);
    }

    public static void ChangeSortingLayerRecursively (Transform root, string sortingLayerName, int offsetOrder = 0) {
        SpriteRenderer renderer = root.GetComponent<SpriteRenderer> ();
        if (renderer != null) {
            renderer.sortingLayerName = sortingLayerName;
            renderer.sortingOrder += offsetOrder;
        }

        foreach (Transform child in root) {
            ChangeSortingLayerRecursively (child, sortingLayerName, offsetOrder);
        }
    }

    public static void ChangeRendererColorRecursively (Transform root, Color color) {
        SpriteRenderer renderer = root.GetComponent<SpriteRenderer> ();
        if (renderer != null) {
            renderer.color = color;
        }

        foreach (Transform child in root) {
            ChangeRendererColorRecursively (child, color);
        }
    }

    public static void ChangeImageColorRecursively (Transform root, Color color) {
        Image image = root.GetComponent<Image> ();
        if (image != null) {
            image.color = color;
        }

        foreach (Transform child in root) {
            ChangeImageColorRecursively (child, color);
        }
    }

    public static string GetFacePictureURL (string facebookID, int? width = null, int? height = null, string type = null) {
        string url = string.Format ("/{0}/picture", facebookID);
        string query = width != null ? "&width=" + width.ToString () : "";
        query += height != null ? "&height=" + height.ToString () : "";
        query += type != null ? "&type=" + type : "";
        query += "&redirect=false";
        if (query != "")
            url += ("?g" + query);
        return url;
    }

    public static bool isInstallTimeOneweek () {
        // 用户分级别记录和统计逻辑
        // 用户分级(1,2,3,4)按金额来区分用户的购买能力
        // = 1 未支付 ,=2 (0,250],= 3（250，750] ,= 4> 750
        double currentTime = CUtils.GetCurrentTime ();
        double installTime = Double.Parse (PlayerPrefs.GetString (CommonConst.user_install_time, "0"));
        double weektime = 7 * 24 * 3600; // one week time seconds
        double timeSpan = currentTime - installTime;
        return (timeSpan >= weektime);
    }

    public static long getCurrentTimeSeconds () {
        DateTime time = System.DateTime.Now;
        TimeSpan timeSpan = new TimeSpan (time.DayOfYear, time.Hour, time.Minute, time.Second);
        long nowSeconds = long.Parse (timeSpan.TotalSeconds + "");
        return nowSeconds;
    }

    public static double GetCurrentTime () {
        TimeSpan span = DateTime.Now.Subtract (new DateTime (1970, 1, 1, 0, 0, 0));
        return span.TotalSeconds;
    }

    public static double GetCurrentTimeInDays () {
        TimeSpan span = DateTime.Now.Subtract (new DateTime (1970, 1, 1, 0, 0, 0));
        return span.TotalDays;
    }

    public static double GetCurrentTimeInMills () {
        TimeSpan span = DateTime.Now.Subtract (new DateTime (1970, 1, 1, 0, 0, 0));
        return span.TotalMilliseconds;
    }

    public static double GetCurrentTimeInMins () {
        TimeSpan span = DateTime.Now.Subtract (new DateTime (1970, 1, 1, 0, 0, 0));
        return span.TotalMinutes;
    }

    public static T GetRandom<T> (params T[] arr) {
        return arr[UnityEngine.Random.Range (0, arr.Length)];
    }

    public static bool IsActionAvailable (String action, int time, bool availableFirstTime = true) {
        if (!CPlayerPrefs.HasKey (action + "_time")) // First time.
        {
            if (availableFirstTime == false) {
                SetActionTime (action);
            }
            return availableFirstTime;
        }

        int delta = (int) (GetCurrentTime () - GetActionTime (action));
        return delta >= time;
    }

    public static double GetActionDeltaTime (String action) {
        if (GetActionTime (action) == 0)
            return 0;
        return GetCurrentTime () - GetActionTime (action);
    }

    public static void SetActionTime (String action) {
        CPlayerPrefs.SetDouble (action + "_time", GetCurrentTime ());
    }

    public static double GetActionTime (String action) {
        return CPlayerPrefs.GetDouble (action + "_time");
    }

    public static void SetLoggedInFb () {
        CPlayerPrefs.SetBool ("logged_in_fb", true);
    }

    public static bool IsLoggedInFb () {
        return CPlayerPrefs.GetBool ("logged_in_fb", false);
    }

    public static void SetBuyItem () {
        CPlayerPrefs.SetBool ("buy_item", true);
    }

    public static void SetRemoveAds (bool value) {
        CPlayerPrefs.SetBool ("remove_ads", value);
    }

    public static bool IsAdsRemoved () {
        return CPlayerPrefs.GetBool ("remove_ads");
    }

    public static bool IsBuyItem () {
        return CPlayerPrefs.GetBool ("buy_item", false);
    }

    public static void SetRateGame () {
        CPlayerPrefs.SetBool ("rate_game", true);
    }

    public static bool IsGameRated () {
        return CPlayerPrefs.GetBool ("rate_game", false);
    }

    public static void SetLikeFbPage (string id) {
        CPlayerPrefs.SetBool ("like_page_" + id, true);
    }

    public static bool IsLikedFbPage (string id) {
        return CPlayerPrefs.GetBool ("like_page_" + id, false);
    }

    public static void SetInitGame () {
        CPlayerPrefs.SetBool ("init_game", true);
    }

    public static bool IsGameInitialzied () {
        return CPlayerPrefs.GetBool ("init_game", false);
    }

    public static void SetAndroidVersion (string version) {
        CPlayerPrefs.SetString ("android_version", version);
    }

    public static void SetIOSVersion (string version) {
        CPlayerPrefs.SetString ("ios_version", version);
    }

    public static void SetWindowsPhoneVersion (string version) {
        CPlayerPrefs.SetString ("wp_version", version);
    }

    public static string GetAndroidVersion () {
        return CPlayerPrefs.GetString ("android_version", "1.0");
    }

    public static string GetIOSVersion () {
        return CPlayerPrefs.GetString ("ios_version", "1.0");
    }

    public static string GetWindowsPhoneVersion () {
        return CPlayerPrefs.GetString ("wp_version", "1.0");
    }

    public static void SetVersionCode (int versionCode) {
        CPlayerPrefs.SetInt ("game_version_code", versionCode);
    }

    public static int GetVersionCode () {
        return CPlayerPrefs.GetInt ("game_version_code");
    }

    public static string BuildStringFromCollection (ICollection values, char split = '|') {
        string results = "";
        int i = 0;
        foreach (var value in values) {
            results += value;
            if (i != values.Count - 1) {
                results += split;
            }
            i++;
        }
        return results;
    }

    public static List<T> BuildListFromString<T> (string values, char split = '|') {
        List<T> list = new List<T> ();
        if (string.IsNullOrEmpty (values))
            return list;

        string[] arr = values.Split (split);
        foreach (string value in arr) {
            if (string.IsNullOrEmpty (value)) continue;
            T val = (T) Convert.ChangeType (value, typeof (T));
            list.Add (val);
        }
        return list;
    }

#if UNITY_EDITOR
    public static string[] GetSortingLayerNames () {
        Type internalEditorUtilityType = typeof (InternalEditorUtility);
        PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty ("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
        return (string[]) sortingLayersProperty.GetValue (null, new object[0]);
    }

    public static int[] GetSortingLayerUniqueIDs () {
        Type internalEditorUtilityType = typeof (InternalEditorUtility);
        PropertyInfo sortingLayerUniqueIDsProperty = internalEditorUtilityType.GetProperty ("sortingLayerUniqueIDs", BindingFlags.Static | BindingFlags.NonPublic);
        return (int[]) sortingLayerUniqueIDsProperty.GetValue (null, new object[0]);
    }
#endif

    public static bool IsOutOfScreen (Vector3 pos, float padding = 0) {
        float width = UICamera.instance.GetWidth () + padding;
        float height = UICamera.instance.GetHeight () + padding;
        return (pos.x < -width || pos.x > width || pos.y < -height || pos.y > height);
    }

    public static void SetNumofEnterScene (string sceneName, int value) {
        CPlayerPrefs.SetInt ("numof_enter_scene_" + sceneName, value);
    }

    public static int GetNumofEnterScene (string sceneName) {
        return CPlayerPrefs.GetInt ("numof_enter_scene_" + sceneName, 0);
    }

    public static int IncreaseNumofEnterScene (string sceneName) {
        int current = GetNumofEnterScene (sceneName);
        SetNumofEnterScene (sceneName, ++current);
        return current;
    }

    public static List<T> GetObjectInRange<T> (Vector3 position, float radius, int layerMask = Physics2D.DefaultRaycastLayers) where T : class {
        List<T> list = new List<T> ();
        Collider2D[] colliders = Physics2D.OverlapCircleAll (position, radius, layerMask);

        foreach (Collider2D col in colliders) {
            list.Add (col.gameObject.GetComponent (typeof (T)) as T);
        }
        return list;
    }

    public static Sprite GetSprite (string textureName, string spriteName) {
        Sprite[] sprites = Resources.LoadAll<Sprite> (textureName);
        foreach (Sprite sprite in sprites) {
            if (sprite.name == spriteName) {
                return sprite;
            }
        }
        return null;
    }

    public static List<Transform> GetActiveChildren (Transform parent) {
        List<Transform> list = new List<Transform> ();
        foreach (Transform child in parent) {
            if (child.gameObject.activeSelf) list.Add (child);
        }
        return list;
    }

    public static List<Transform> GetChildren (Transform parent) {
        List<Transform> list = new List<Transform> ();
        foreach (Transform child in parent) {
            list.Add (child);
        }
        return list;
    }

    public static void LoadTurorialScene (int TutorialLevel) {
        GameState.tutorialLevel = TutorialLevel;
        LoadScene (4, false);
    }

    public static void LoadScene (int sceneIndex, bool useScreenFader = false) {
        // if (useScreenFader) {
        //     ScreenFader.instance.GotoScene (sceneIndex);
        // } else {
            SceneManager.LoadScene (sceneIndex);
        // }
    }

    public static void ReloadScene (bool useScreenFader = false) {
        int currentIndex;
        currentIndex = SceneManager.GetActiveScene ().buildIndex;
        LoadScene (currentIndex, useScreenFader);
    }

    public static void SetLeaderboardScore (int score) {
        CPlayerPrefs.SetInt ("leaderboard_score", score);
    }

    public static int GetLeaderboardScore () {
        return CPlayerPrefs.GetInt ("leaderboard_score");
    }

    public static void CheckConnection (MonoBehaviour behaviour, Action<int> connectionListener) {
        behaviour.StartCoroutine (ConnectUrl ("http://www.google.com", connectionListener));
    }

    private static IEnumerator ConnectUrl (string url, Action<int> connectionListener) {
        WWW www = new WWW (url);
        yield return www;
        if (www.error != null)
            connectionListener (1);
        else if (string.IsNullOrEmpty (www.text))
            connectionListener (2);
        else
            connectionListener (0);
    }

    public static void CheckDisconnection (MonoBehaviour behaviour, Action onDisconnected) {
        behaviour.StartCoroutine (ConnectUrl ("http://www.google.com", onDisconnected));
    }

    private static IEnumerator ConnectUrl (string url, Action onDisconnected) {
        WWW www = new WWW (url);
        yield return www;
        if (www.error != null) {
            yield return new WaitForSeconds (2f);
            www = new WWW (url);
            yield return www;

            if (www.error != null)
                onDisconnected ();
        }
    }

    public static void SetAutoSigninGPS (int value) {
        CPlayerPrefs.SetInt ("auto_sign_in_gps", value);
    }

    public static int GetAutoSigninGPS () {
        return CPlayerPrefs.GetInt ("auto_sign_in_gps");
    }

    public static IEnumerator LoadPicture (string url, Action<Texture2D> callback, int width, int height, bool useCached = true) {
#if !UNITY_WSA && !UNITY_WP8 && !UNITY_WEBPLAYER
        string localPath = GetLocalPath (url);
        bool loaded = false;

        if (useCached) {
            loaded = LoadFromLocal (callback, localPath, width, height);
        }

        if (!loaded) {
            WWW www = new WWW (url);
            yield return www;
            if (www.isDone && string.IsNullOrEmpty (www.error)) {
                callback (www.texture);
                System.IO.File.WriteAllBytes (localPath, www.bytes);
            } else {
                LoadFromLocal (callback, localPath, width, height);
            }
        }
#else
        yield return null;
#endif
    }

    private static string GetLocalPath (string url) {
#if !UNITY_WSA && !UNITY_WP8 && !UNITY_WEBPLAYER
        string justFilename = System.IO.Path.GetFileName (new Uri (url).LocalPath);
        return Application.persistentDataPath + "/" + justFilename;
#else
        return null;
#endif
    }

    public static IEnumerator CachePicture (string url, Action<bool> result) {
#if !UNITY_WSA && !UNITY_WP8 && !UNITY_WEBPLAYER
        string localPath = GetLocalPath (url);
        WWW www = new WWW (url);
        yield return www;
        if (www.isDone && string.IsNullOrEmpty (www.error)) {
            System.IO.File.WriteAllBytes (localPath, www.bytes);
            if (result != null) result (true);
        } else {
            if (result != null) result (false);
        }
#else
        yield return null;
#endif
    }

    public static bool IsCacheExists (string url) {
#if !UNITY_WSA && !UNITY_WP8
        return System.IO.File.Exists (GetLocalPath (url));
#else
        return false;
#endif
    }

    private static bool LoadFromLocal (Action<Texture2D> callback, string localPath, int width, int height) {
#if !UNITY_WSA && !UNITY_WP8 && !UNITY_WEBPLAYER
        if (System.IO.File.Exists (localPath)) {
            var bytes = System.IO.File.ReadAllBytes (localPath);
            var tex = new Texture2D (width, height, TextureFormat.RGB24, false);
            tex.LoadImage (bytes);
            if (tex != null) {
                callback (tex);
                return true;
            }
        }
        return false;
#else
        return false;
#endif
    }

    public static Sprite CreateSprite (Texture2D texture, int width, int height) {
        return Sprite.Create (texture, new Rect (0, 0, width, height), new Vector2 (0.5f, 0.5f), 100.0f);
    }

    public static List<List<T>> Split<T> (List<T> source, Predicate<T> split) {
        List<List<T>> result = new List<List<T>> ();
        bool begin = false;
        for (int i = 0; i < source.Count; i++) {
            T element = source[i];
            if (split (element)) {
                begin = false;
            } else {
                if (begin == false) {
                    begin = true;
                    result.Add (new List<T> ());
                }
                result[result.Count - 1].Add (element);
            }
        }
        return result;
    }

    public static List<List<T>> GetArrList<T> (List<T> source, Predicate<T> take) {
        List<List<T>> result = new List<List<T>> ();
        bool begin = false;
        foreach (T element in source) {
            if (take (element)) {
                if (begin == false) {
                    begin = true;
                    result.Add (new List<T> ());
                }
                result[result.Count - 1].Add (element);
            } else {
                begin = false;
            }
        }
        return result;
    }

    public static List<T> ToList<T> (T obj) {
        List<T> list = new List<T> ();
        list.Add (obj);
        return list;
    }

    public static int ChooseRandomWithProbs (float[] probs) {
        float total = 0;
        foreach (float elem in probs) {
            total += elem;
        }

        float randomPoint = UnityEngine.Random.value * total;
        for (int i = 0; i < probs.Length; i++) {
            if (randomPoint < probs[i]) {
                return i;
            } else {
                randomPoint -= probs[i];
            }
        }
        return probs.Length - 1;
    }

    public static bool IsObjectSeenByCamera (Camera camera, GameObject gameObj, float delta = 0) {
        Vector3 screenPoint = camera.WorldToViewportPoint (gameObj.transform.position);
        return (screenPoint.z > 0 && screenPoint.x > -delta && screenPoint.x < 1 + delta && screenPoint.y > -delta && screenPoint.y < 1 + delta);
    }

    public static Vector3 GetMiddlePoint (Vector3 begin, Vector3 end, float delta = 0) {
        Vector3 center = Vector3.Lerp (begin, end, 0.5f);
        Vector3 beginEnd = end - begin;
        Vector3 perpendicular = new Vector3 (-beginEnd.y, beginEnd.x, 0).normalized;
        Vector3 middle = center + perpendicular * delta;
        return middle;
    }

    public static AnimationClip GetAnimationClip (Animator anim, string name) {
        var ac = anim.runtimeAnimatorController;
        for (int i = 0; i < ac.animationClips.Length; i++) {
            if (ac.animationClips[i].name == name) return ac.animationClips[i];
        }
        return null;
    }

    public static void Swap<T> (ref T lhs, ref T rhs) {
        T temp = lhs;
        lhs = rhs;
        rhs = temp;
    }

    public static void Pause () {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPaused = true;
#endif
    }

    /// <summary>
    /// Path Rule : Levels/World_n/Level_n : All n from 1 to n
    /// </summary>
    /// <returns>path type of string</returns>
    public static string GetGameLevelPath (int gamelevel, int levelnum = 60) {
        int word_n = gamelevel / levelnum;
        int level_n = gamelevel % levelnum;
        string path = "";
        if (level_n == 0) {
            path = "Levels/World_" + (word_n + 1) + "/Level_60";
        } else {
            path = "Levels/World_" + (word_n + 1) + "/Level_" + (level_n + 1);

        }

        return path;
    }

    // 1 2 60 ...120......240

    // word_n
    // level_n

    public static int getWorldN (int gamelevel, int levelnum = CommonConst.MAX_LEVEL) {
        int word_n = gamelevel / levelnum;
        // return word_n + 1;
        if (word_n == 0) {
            return 1;
        } else {
            int temp = gamelevel % levelnum;
            if (temp == 0) {
                return word_n;
            } else {
                return word_n + 1;
            }
        }
    }

    public static int getLevelN (int gamelevel, int levelnum = CommonConst.MAX_LEVEL) {
        int level_n = gamelevel % levelnum;
        if (level_n == 0) return 60;
        // Debug.Log("=====>level_n" + level_n);
        return level_n;
    }

    /**
     * Include the Facebook namespace via the following code:
     * using Facebook.Unity;
     *
     * purchaseAmount is float.
     * currency is string from http://en.wikipedia.org/wiki/ISO_4217.
     * parameters is Dictionary<string, object>.
     */

    public static void LogPurchasedEvent (string contentId, string currency, float price, Dictionary<string, object> parameters) {

        // FB.LogPurchase(
        //     (float)price,
        //     currency,
        //     parameters
        // );
    }

    public static void analyticsEvent (string eventname, Dictionary<string, object> dict) {

        // if (CommonConst.IS_DEBUG) return;

        // Answers.LogCustom(
        //     eventname,
        //     customAttributes: dict

        // );

        // if (FB.IsInitialized)
        // {

        //     // Facebook
        //     FB.LogAppEvent(
        //                 eventname,
        //                 null,
        //                 dict);
        // }

        // Dictionary<string, string> sd = new Dictionary<string, string>();
        // foreach (KeyValuePair<string, object> keyValuePair in dict)
        // {
        //     sd.Add(keyValuePair.Key, keyValuePair.Value == null ? "" : keyValuePair.Value.ToString());
        // }

        // if (FotoableSdk.getInstance() != null)
        //     FotoableSdk.getInstance().logCustomEvent(eventname, eventname, sd);

        // Dictionary<string, string> sd = new Dictionary<string, string>();
        // foreach (KeyValuePair<string, object> keyValuePair in dict)
        // {
        //     sd.Add(keyValuePair.Key, keyValuePair.Value == null ? "" : keyValuePair.Value.ToString());
        // }
        // KHD.FlurryAnalytics.Instance.LogEventWithParameters(
        //            eventname,
        //            sd);

    }

    /// <summary>
    /// Facebook Fabric 统计封装
    /// </summary>
    public static void analyticsEvent (string eventname, string key, string value) {
        // if (CommonConst.IS_DEBUG) return;

        // Answers.LogCustom(
        //     eventname,
        //     customAttributes: new Dictionary<string, object>(){
        //         {key, value}
        //     }
        // );

        // if (FB.IsInitialized)
        // {

        //     // Facebook
        //     FB.LogAppEvent(
        //                 eventname,
        //                 null,
        //                 new Dictionary<string, object>()
        //                 {
        //                 {key, value}
        //                 });
        // }

        // if (FotoableSdk.getInstance() != null)
        //     FotoableSdk.getInstance().logCustomEvent(eventname, eventname, new Dictionary<string, string>()
        //                 {
        //                 {key, value}
        //                 });

        // KHD.FlurryAnalytics.Instance.LogEventWithParameters(
        //            eventname,
        //            new Dictionary<string, string>() {
        //                 { key, value }
        //        });

    }

    /// <summary>
    /// Facebook Fabric 统计封装
    /// </summary>
    public static void analyticsEvent (string eventname) {
        // if (CommonConst.IS_DEBUG) return;

        // Answers.LogCustom(
        //     eventname

        // );

        // if (FB.IsInitialized)
        // {

        //     // Facebook
        //     FB.LogAppEvent(
        //                 eventname);

        // }

        // KHD.FlurryAnalytics.Instance.LogEvent(eventname);

    }

    // 记录用户等级
    public static void userLevelRecord () {

        // 根据金币数目存储用户等级
        float totalPrice = PlayerPrefs.GetFloat (CommonConst.user_level_coin_count); // 总的购买金额
        int totalBuyNum = PlayerPrefs.GetInt (CommonConst.user_level_buy_count); // 总的购买次数
        int userLevel = 0;
        if (totalBuyNum == 0 && totalPrice == 0) {
            userLevel = 0;
        } else if ((totalBuyNum == 1) || (totalPrice > 0 && totalPrice < 4.99)) {
            userLevel = 1;
        } else if (totalBuyNum == 2 || (totalPrice >= 4.99 && totalPrice < 19.99)) {
            userLevel = 2;
        } else if (totalBuyNum > 2 || totalPrice >= 19.99) {
            userLevel = 3;
        }

        PlayerPrefs.SetInt (CommonConst.user_level_consumer_level, userLevel);
        CUtils.analyticsEvent (CommonConst.user_level, "user_level", (userLevel + ""));
        PlayerPrefs.Save ();
    }

    public static string analytisTimeSegmentation (double time) {
        if (time > 0 && time <= 2) {
            return "0 - 2";

        } else if (time > 2 && time <= 2) {
            return "2 - 4";

        } else if (time > 4 && time <= 6) {
            return "4 - 6";

        } else if (time > 6 && time <= 8) {
            return "6 - 8";

        } else if (time > 8 && time <= 10) {
            return "8 - 10";

        } else if (time > 10 && time <= 12) {
            return "10 - 12";

        } else if (time > 12) {
            return "12 +";
        }
        return "0 - 2";
    }

    ///<summery>
    /// 是否满足弹出插屏广告
    ///</summery>
    public static bool isShowInterstitial () {

        int adSwitch = PlayerPrefs.GetInt (CommonConst.PREF_AD_INTERSTITIAL_SWITCH, 0);
        int isPurchaseRemovedAd = PlayerPrefs.GetInt (CommonConst.purchaseRemoveAD, 0);

        if ((adSwitch == 0) || (isPurchaseRemovedAd == 1)) {
            Debug.Log ("isShowInterstitial 开关关闭");
            return false;
        } else {
            Debug.Log ("isShowInterstitial 开关打开");

            int limitMin = PlayerPrefs.GetInt (CommonConst.PREF_AD_INTERSTITIAL_INTERVAL, 5); // 时间间隔取不到默认5分钟
            double oldTime = double.Parse (PlayerPrefs.GetString (CommonConst.PREF_AD_INTERSTITIAL_TIME, "0"));
            double now = CUtils.GetCurrentTimeInMins (); //分钟

            if (oldTime != 0) {
                // 小于间隔弹出时间直接返回
                if (now - oldTime < limitMin) {
                    return false;
                }
            }
            PlayerPrefs.SetString (CommonConst.PREF_AD_INTERSTITIAL_TIME, now + "");
            PlayerPrefs.Save ();
            return true;
        }

    }

    public static bool isShowRewardVideo () {

        int adSwitch = PlayerPrefs.GetInt (CommonConst.PREF_AD_REWARD_SWITCH, 0);
        int isPurchaseRemovedAd = PlayerPrefs.GetInt (CommonConst.purchaseRemoveAD, 0);
        if ((adSwitch == 0) || (isPurchaseRemovedAd == 1)) {
            Debug.Log ("isShowRewardVideo 开关关闭");
            return false;
        } else {
            Debug.Log ("isShowRewardVideo 开关打开");

            // 判断是否24小时
            // 判断个数是否大于最大
            int hasShowCount = PlayerPrefs.GetInt (CommonConst.PREF_AD_REWARD_TEMP_COUNT, 0);
            int limitCount = PlayerPrefs.GetInt (CommonConst.PREF_AD_REWARD_COUNT_LIMIT, 5);
            long oldTime = long.Parse (PlayerPrefs.GetString (CommonConst.PREF_AD_REWARD_TIME));
            long now = CUtils.getCurrentTimeSeconds ();
            long oneDayTime = 24 * 3600;
            if ((now - oldTime) < oneDayTime) {
                if (hasShowCount >= limitCount) {
                    return false;
                }
            } else {
                // 重置广告每天计量初始时间
                PlayerPrefs.SetString (CommonConst.PREF_AD_REWARD_TIME, (CUtils.getCurrentTimeSeconds () + ""));
                PlayerPrefs.SetInt (CommonConst.PREF_AD_REWARD_TEMP_COUNT, 0);
            }
            PlayerPrefs.Save ();

            return true;
        }
    }

    ///<summery>
    ///Level----> 查找 RealLevel
    ///</summery>
    public static int findRealLevelByLevelIndex (int level) {
        // 1. 比如1-10 关肯定一一对应
        // 2. 比如用户第一次开始跳可能会 5,15,20 (真正跳关的含义是大于等于2关.)
        int firstSkipLevel = PlayerPrefs.GetInt (CommonConst.PREF_SKIP_FIRST_LEVEL, 0); // 需要在记录的全局pref里面获取第一次开始跳关的关卡值
        if (firstSkipLevel == 0 || level < 10) {
            return level;
        }

        int totalSkipLevel = PlayerPrefs.GetInt (CommonConst.PREF_SKIP_TOTAL_EXP_LEVEL);
        int realLevel = level + totalSkipLevel;
        return realLevel;
    }

    ///<summery>
    ///RealLevel----> 查找 level
    ///</summery>
    public static int findLevelIndexByRealLevel (int realLevel) {
        int fistSkipLevel = PlayerPrefs.GetInt (CommonConst.PREF_SKIP_FIRST_LEVEL, 1); // 默认为1 过滤条件判断
        // 1. 比如1-10 关肯定一一对应
        // 2. 比如用户第一次开始跳可能会 5,15,20 (真正跳关的含义是大于等于2关.)
        if (realLevel <= fistSkipLevel) {
            return realLevel;
        }
        //realLevel 10-->15 5
        //level 10 11 算 15 - 5 + 1 = 11
        int totalSkipLevel = PlayerPrefs.GetInt (CommonConst.PREF_SKIP_TOTAL_EXP_LEVEL);
        int level = realLevel - totalSkipLevel + 1;
        return level;
    }

    public static int calculateUserExpFor5Level (int totalPieceCount, int useHint) {
        int exp = caculateStepExp (totalPieceCount, 5, useHint);
        Debug.Log ("level 5 的用户exp=====   " + exp);
        return exp;
    }

    public static int caculateStepExp (int totalPieceCount, int mLevel, int useHint) {
        // 正常每个关的算法
        //1. 步数比率  exp = 片数/总的步数
        //2. 是否使用了hint 用了1 hint  exp - 0.1
        //3. 判断关卡范围,存储float 系数,判断关卡算出平均系数
        float exp = totalPieceCount * 1.0f / (GameState.tempGameLevelsSteps + useHint);

        if (exp > 1) exp = 1.0f; // 出现大雨1 的是因为有干扰块,所以按照1 算

        // 用户过关能力值正相关
        int userExpLevel = 1;
        if (exp > 0 && exp < 0.35) {
            userExpLevel = 1;
        } else if (exp >= 0.35 && exp < 0.51) {
            userExpLevel = 2;
        } else if (exp >= 0.51 && exp < 0.67) {
            userExpLevel = 3;
        } else if (exp >= 0.67 && exp < 0.83) {
            userExpLevel = 4;
        } else if (exp >= 0.83) {
            userExpLevel = 5;
        }

        int levelByhintcount; // userHint 消耗 1 个 减掉 一个exp等级,2个或以上-2
        if (useHint == 0) {
            levelByhintcount = 0;
        } else if (useHint == 1) {
            levelByhintcount = 1;
        } else {
            levelByhintcount = 2;
        }
        Debug.Log (string.Format ("每个关卡概率计算---->Level[{0}],exp=step/tstep[{1}],用户的过关能力->[{2}],总共用的步数->[{3}]",
            mLevel, exp, userExpLevel, GameState.tempGameLevelsSteps));
        //步数获取的exp - 使用hint 相应的降级
        int resultUserExp = userExpLevel - levelByhintcount;

        if (resultUserExp < 1) {
            resultUserExp = 1;
        }

        return resultUserExp;

    }

    ///<summery>
    ///跳关比例算法,计算跳关等级,关卡数
    ///</summery>
    public static int calculateUserExp (int totalPieceCount, int mLevel, int useHint) {
        // 前10关可能需要调整
        if (mLevel < CommonConst.skip_level_num) return 1;
        if (mLevel % 5 == 0) {
            int expStep = caculateStepExp (totalPieceCount, mLevel, useHint);
            Dictionary<string, object> sd = new Dictionary<string, object> ();
            sd.Add ("realLevelNum", GameState.realLevel + "");
            //  前十个关算法
            if (mLevel == 10) {
                int level5exp = PlayerPrefs.GetInt (CommonConst.PREF_LEVEL_5_EXP, 1);

                int hintExpLevel = (level5exp + expStep) / 2;
                sd.Add ("userExp", hintExpLevel + "");
                CUtils.analyticsEvent ("keyLevelExp", sd);

                return hintExpLevel;
            } else {
                sd.Add ("userExp", expStep + "");
                CUtils.analyticsEvent ("keyLevelExp", sd);
            }

            return expStep;
        } else {
            return 1;
        }
    }

    public static void SaveMapDatas (List<Player> Maps) {
        string data = "";
        foreach (var item in Maps) {
            data += item.ToSaveString ();
        }
        PlayerPrefs.SetString ("DATA", data);
    }

    //TODO: 1.0.9 暂时不上,不调用此方法
    // public static bool isShowSiginEdDialog()
    // {

    //     // 判断逻辑,判断是否是当天24点之前可以签到. 过了24点可以签到第二天,累计7天

    //     bool isTimeOk = true;

    //     System.DateTime nowT = System.DateTime.Now;
    //     string signinTimeText = PlayerPrefs.GetString(CommonConst.PREF_RECORD_SIGINEDIN_TIME);//获取签到保存的时间
    //     if (string.IsNullOrEmpty(signinTimeText))
    //     {
    //         // 第一次签到
    //         isTimeOk = true;
    //     }
    //     else
    //     {

    //         System.DateTime pauseT = System.Convert.ToDateTime(signinTimeText);
    //         int yearNow = nowT.Year;
    //         int year = pauseT.Year;
    //         int monthNow = nowT.Month;
    //         int month = pauseT.Month;
    //         int dayNow = nowT.Day;
    //         int day = pauseT.Day;

    //         if (yearNow - year > 0)
    //         {
    //             isTimeOk = true;// 年>1
    //         }
    //         else if (monthNow - month > 0)
    //         {
    //             isTimeOk = true;//月>1
    //         }
    //         else if (dayNow - day > 0)
    //         {
    //             isTimeOk = true;//天>1
    //         }
    //         else
    //         {
    //             // 不可能大于24小时,肯定是同一天(上次签到到现在)
    //             isTimeOk = false;
    //         }

    //     }

    //     int count = PlayerPrefs.GetInt(CommonConst.PREF_SIGINEDIN_DAY, 0);
    //     if (count < 7 && isTimeOk)
    //     {
    //         return true;
    //     }
    //     return false;
    // }

}