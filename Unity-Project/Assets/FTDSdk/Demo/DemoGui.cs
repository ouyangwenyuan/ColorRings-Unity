using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoGui : MonoBehaviour {
    // public static List<Action<string>> actionlist = new List<Action<string>>();

    [SerializeField]
    private string android_appId;
    [SerializeField]
    private string android_appKey;
    [SerializeField]
    private string android_signWay;

    [SerializeField]
    private string ios_appId;
    [SerializeField]
    private string ios_appKey;
    [SerializeField]
    private string ios_signWay;

    void Awake () {

#if UNITY_IOS
        FTDSdk.init (ios_appId, ios_appKey, ios_signWay);
#elif UNITY_ANDROID
        FTDSdk.init (android_appId, android_appKey, android_signWay);
#endif

        //以下设置回调接口，若不需要则可以忽略不设置
        FtOnattributeChangedListener myListener = new MyListener ();
        FtHttpCallback myCallback = new MyHttpCallback ();
        FtPayVerifyCallback payVerifyCallback = new MyPayVerifyCallback ();
        FTDSdk.getInstance ().setOnAttributeListener (myListener);
        // FTDSdk.getInstance ().setHttpCallback (myCallback);
        // FTDSdk.getInstance ().setPayVerifyCallback (payVerifyCallback);

    }
    void OnApplicationPause (bool pauseStatus) {
        if (FTDSdk.getInstance () == null) {
            return;
        }
        FTDSdk.getInstance ().OnFTApplicationPause (pauseStatus);
    }
    // Use this for initialization
    void Start () {

    }

    // Update is called once per frame
    void Update () {
        //遍历action 在Update中调用action自然是主线程调用  
        // for (int i = 0; i < actionlist.Count; i++)
        // {
        //     actionlist[i](Time.time + "");
        // }
        // actionlist.Clear();
    }
    // void OnGUI()
    // {
    //     int space = 20;
    //     int btnAreWidth = Screen.width / 6;
    //     int btnWidth = btnAreWidth - space;
    //     int btnHeight = 50;

    //     GUILayout.BeginVertical();
    //     GUILayout.Space(space);
    //     GUILayout.BeginHorizontal();
    //     GUILayout.Space(space);
    //     GUI.skin.button.fontSize = 30;
    //     if (GUILayout.Button("初始化接口", GUILayout.Width(btnWidth), GUILayout.Height(btnHeight)))
    //     {
    //         FTDSdk.initAndroid(android_appId, android_appKey, android_signWay);
    //     }
    //     GUILayout.Space(space);

    //     if (GUILayout.Button("adjust归因数据上报", GUILayout.Width(btnWidth), GUILayout.Height(btnHeight)))
    //     {
    //         string attrjson = @"{
    //                 ""media_source"": ""tapjoy_int"",
    // 	            ""agency"": ""starcomm"",
    // 	            ""site_id"": ""57"",
    // 	            ""af_status"": ""Non -organic"",
    // 	            ""af_siteid"": null,
    // 	            ""af_sub1"": null,
    // 	            ""campaign"": ""July4 -Campaign"",
    // 	            ""channel"": ""adjust"",
    // 	            ""af_sub2"": ""subtext1""
    //             }";
    //         if (FTDSdk.getInstance() == null)
    //         {
    //             return;
    //         }
    //         FTDSdk.getInstance().sendAttributeData("adjust", attrjson);
    //     }
    //     GUILayout.Space(space);

    //     if (GUILayout.Button("appsflyer归因数据上报", GUILayout.Width(btnWidth), GUILayout.Height(btnHeight)))
    //     {
    //         string attrjson = @"{
    //                 ""media_source"": ""tapjoy_int"",
    // 	            ""agency"": ""starcomm"",
    // 	            ""site_id"": ""57"",
    // 	            ""af_status"": ""Non -organic"",
    // 	            ""af_siteid"": null,
    // 	            ""af_sub1"": null,
    // 	            ""campaign"": ""July4 -Campaign"",
    // 	            ""channel"": ""adjust"",
    // 	            ""af_sub2"": ""subtext1""
    //             }";

    //         if (FTDSdk.getInstance() == null)
    //         {
    //             return;
    //         }
    //         FTDSdk.getInstance().sendAttributeData("appsflyer", attrjson);
    //     }
    //     GUILayout.Space(space);

    //     if (GUILayout.Button("登录事件", GUILayout.Width(btnWidth), GUILayout.Height(btnHeight)))
    //     {
    //         Dictionary<String, String> testCustomParams = new Dictionary<String, String>();
    //         testCustomParams.Add("userId", "5210045");
    //         if (FTDSdk.getInstance() == null)
    //         {
    //             return;
    //         }
    //         FTDSdk.getInstance().logEventLogin("google_game", testCustomParams);
    //     }
    //     GUILayout.Space(space);

    //     if (GUILayout.Button("设置TAG", GUILayout.Width(btnWidth), GUILayout.Height(btnHeight)))
    //     {

    //         if (FTDSdk.getInstance() == null)
    //         {
    //             return;
    //         }
    //         FTDSdk.getInstance().setTags(new string[] {"cc","ace","accd"});
    //     }
    //     GUILayout.Space(space);

    //     GUILayout.EndHorizontal();
    //     GUILayout.Space(space);

    //     GUILayout.BeginHorizontal();
    //     GUILayout.Space(space);

    //     if (GUILayout.Button("注册事件", GUILayout.Width(btnWidth), GUILayout.Height(btnHeight)))
    //     {

    //         Dictionary<String, String> testCustomParams = new Dictionary<String, String>();
    //         testCustomParams.Add("userId", "6665666");
    //         if (FTDSdk.getInstance() == null)
    //         {
    //             return;
    //         }
    //         FTDSdk.getInstance().logEventRegist("facebook", testCustomParams);
    //     }
    //     GUILayout.Space(space);

    //     if (GUILayout.Button("支付事件", GUILayout.Width(btnWidth), GUILayout.Height(btnHeight)))
    //     {

    //         Dictionary<String, String> testCustomParams = new Dictionary<String, String>();
    //         testCustomParams.Add("orderId", "54254122525455");
    //         if (FTDSdk.getInstance() == null)
    //         {
    //             return;
    //         }
    //         FTDSdk.getInstance().logEventPurchase("google", "cindm.xj.djx.1", "500钻石", 499, 499, "USD", testCustomParams);
    //     }
    //     GUILayout.Space(space);

    //     if (GUILayout.Button("完成新手引导", GUILayout.Width(btnWidth), GUILayout.Height(btnHeight)))
    //     {

    //         Dictionary<String, String> testCustomParams = new Dictionary<String, String>();
    //         testCustomParams.Add("time", "5000");
    //         if (FTDSdk.getInstance() == null)
    //         {
    //             return;
    //         }
    //         FTDSdk.getInstance().logEventCompletedTutorial(true, testCustomParams);
    //     }
    //     GUILayout.Space(space);

    //     if (GUILayout.Button("自定义事件", GUILayout.Width(btnWidth), GUILayout.Height(btnHeight)))
    //     {

    //         Dictionary<String, String> testCustomParams = new Dictionary<String, String>();
    //         testCustomParams.Add("level", "30");
    //         if (FTDSdk.getInstance() == null)
    //         {
    //             return;
    //         }
    //         FTDSdk.getInstance().logCustomEvent("Level升级", "xikdhf", testCustomParams);
    //     }
    //     GUILayout.Space(space);

    //     GUILayout.EndHorizontal();

    // }
    class MyListener : FtOnattributeChangedListener {
        public void onAttributionChanged (string jsonAttr) {
            FTDSdk.getInstance ().sendAttributeData ("appflyer", jsonAttr);
            //这里进行游戏逻辑
            // actionlist.Add(curname => {
            //     Debug.Log(" || attr=" + jsonAttr + " >> curname = " + curname);
            // }); 
        }
    }
    class MyHttpCallback : FtHttpCallback {
        public void onReponse (int code, string request, string mResponse) {
            //这里进行游戏逻辑
            // actionlist.Add(curname => {
            //     if(code == 200)
            //     {
            //         Debug.Log("HTTP SUCCESS.");
            //     }
            //     else
            //     {
            //         Debug.Log("HTTP FAILED.");
            //     }
            // });
        }
    }

    class MyPayVerifyCallback : FtPayVerifyCallback {
        public void callback (string callback) {
            //这里进行游戏逻辑
            // actionlist.Add(curname => {
            //     Debug.Log(" || callback=" + callback + " >> curname = " + curname);
            // });
        }
    }
}