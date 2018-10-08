using System;
using System.Collections.Generic;
using UnityEngine;
using com.ftdsdk.sdk;
using System.Collections;
using System.Runtime.InteropServices;

public class FTDSdk : MonoBehaviour
    {
        private static FTDSdk instance;
        private const string errorMsgEditor = "FTDSdk: SDK can not be used in Editor.";
        private const string errorMsgStart = "FTDSdk: SDK not started. Start it manually using the 'start' method.";
        private const string errorMsgPlatform = "FTDSdk: SDK can only be used in Android, iOS .";
        private const string errorMsgNotInit = "FTDSdk: SDK not init .";
        public static FtOnattributeChangedListener changedListener;
        public static FtPayVerifyCallback payVerifyCallback;
        public static FtHttpCallback ftHttpCallback;
        private static bool isInit = false;
#if UNITY_IOS

        [DllImport("__Internal")]
        private static extern void fInitAppVestAndStartIt(string appid, string appkey, string way);

        [DllImport("__Internal")]
        private static extern void fRegisteredSuccese(string registChannel, string customParams);

        [DllImport("__Internal")]
        private static extern void fLoginedSuccese(string loginChannel, string customParams);

        [DllImport("__Internal")]
        private static extern void fNewbieGuideSuccese(string isSuccess, string customParams);

        [DllImport("__Internal")]
        private static extern void fTrackRevenueSuccese(string itemid, string itemName, string usdprice, string price, string currency, string channel, string customParams);

        [DllImport("__Internal")]
        private static extern void fGetCustomEvent(string eventName, string eventId, string customParams);

        [DllImport("__Internal")]
        private static extern void fGetAttributionReturnFromChannel(string jsonAttr, string channel);

        [DllImport("__Internal")]
        private static extern void fGetUserInAppWithWay(string way);

        [DllImport("__Internal")]
        private static extern void fGetUserWithTags(string tag);

        [DllImport("__Internal")]
        private static extern void fValidateAndTrackInAppPurchase(string productIdentifier
                                    ,string price,string currency,string transactionId,string customParams);
#endif
    private FTDSdk()
        {

        }
        /**
         * 获取api
         * */
        public static FTDSdk getInstance()
        {
            if(instance == null)
            {
                    GameObject ftGameObject = GameObject.Find("FTDSdk");
                    if(ftGameObject == null)
                    {
                          ftGameObject = new GameObject("FTDSdk");
                          ftGameObject.AddComponent<FTDSdk>();
                    }
                    instance = ftGameObject.GetComponent<FTDSdk>();
            }
      
        return instance;
        }

        /**
         * SDK初始化
         * */
       public static void  init(string appId, string appKey, string signWay)
        {
           if (IsEditor())
            {
                return;
            }
            if (!checkInitParams(appId, appKey, signWay))
            {
                return;
            }
#if UNITY_IOS
            initIos(appId, appKey, signWay);
#elif UNITY_ANDROID
            initAndroid(appId, appKey, signWay);
#else
                        Debug.Log(errorMsgPlatform);
#endif
        }

        /**
         * 初始化android api
         * */
        public static void initAndroid(string appId, string appKey, string signWay)
        {
            Debug.Log("initAndroid.");

            if (IsEditor())
            {
                return;
            }
#if UNITY_ANDROID
            if (!checkInitParams(appId, appKey, signWay))
            {
                return;
            }
            AndroidJavaObject currentActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            FTDAndroid.ftdSdkInit(currentActivity, appId, appKey, signWay);
            isInit = true;
#endif
        }

        /**
         * 初始化IOS api
         * */
        public static void initIos(string appId, string appKey, string signWay)
        {
            Debug.Log("initIos.");

            if (IsEditor())
            {
                return;
            }
#if UNITY_IOS
            if (!checkInitParams(appId, appKey, signWay))
            {
                Debug.Log("Invalid parameters");
                return;
            }
             fInitAppVestAndStartIt(appId, appKey, signWay);
            isInit = true;
#endif
        }
        public void OnFTApplicationPause(bool pauseStatus)
        {
            Debug.Log("OnApplicationPause  = " + pauseStatus);

            if (!checkAvailable())
            {
                return;
            }

#if UNITY_IOS
                      if (pauseStatus)
                        {
                        fGetUserInAppWithWay("out");
                        }
                        else
                        {
                        fGetUserInAppWithWay("in");
                        }
#elif UNITY_ANDROID
        if (pauseStatus)
                        {
                            FTDAndroid.onPause();
                        }
                        else
                        {
                            FTDAndroid.onResume();
                        }
#else
            Debug.Log(errorMsgPlatform);
#endif
        }

        /**
         * 设置归因回调
         * */
        public void setOnAttributeListener(FtOnattributeChangedListener changedListener)
        {
            Debug.Log("setOnAttributeListener success.");
            if (!checkAvailable())
            {
                return;
            }
        FTDSdk.changedListener = changedListener;

#if UNITY_IOS
                        // No action, iOS SDK is subscribed to iOS lifecycle notifications.
#elif UNITY_ANDROID
        FTDAndroid.setOnAttributeListener(new FTDAttributeCallback());
#else
                        Debug.Log(errorMsgPlatform);
#endif

        }
    /**
     * 设置ios支付验证回调
     * */

    public void setPayVerifyCallback(FtPayVerifyCallback payVerifyCallback)
    {
        Debug.Log("setOnAttributeListener success.");
        if (!checkAvailable())
        {
            return;
        }
        FTDSdk.payVerifyCallback = payVerifyCallback;
    }

    /**
     * 设置Http回调
     * */
    public void setHttpCallback(FtHttpCallback ftHttpCallback)
        {
            Debug.Log("setHttpCallback success.");

            if (!checkAvailable())
            {
                return;
            }
#if UNITY_IOS
                        // No action, iOS SDK is subscribed to iOS lifecycle notifications.
#elif UNITY_ANDROID
            FTDSdk.ftHttpCallback = ftHttpCallback;
            FTDAndroid.setHttpCallback(new FTDHttpCallback());
#else
                        Debug.Log(errorMsgPlatform);
#endif

        }
        /**
         * 上报渠道SDK归因
         * @param coversionData 渠道回调返回的归因
         */
        public void sendAttributeData(String channel, String jsonAttr)
        {
            Debug.Log("sendAttributeData . >>>>>" + "channel=" + channel + " || json=" + jsonAttr);

            if (!checkAvailable())
            {
                return;
            }
#if UNITY_IOS
            fGetAttributionReturnFromChannel(jsonAttr,channel);
#elif UNITY_ANDROID
            FTDAndroid.sendAttributeData(channel, jsonAttr);
#else
            Debug.Log(errorMsgPlatform);
#endif
           
        }

        /**
           * 上报登录事件
           * @param loginChannel 登录渠道
           * @param customParams 自定义参数，根据需要传入，不需要传null即可
           */
        public void logEventLogin(String loginChannel, Dictionary<String, String> customParams)
        {
            Debug.Log("logEventLogin . >>>>>" + "loginChannel=" + loginChannel + " || customParams=" + customParams);

            if (!checkAvailable())
            {
                return;
            }
#if UNITY_IOS
            fLoginedSuccese(loginChannel,dic2json(customParams));
#elif UNITY_ANDROID
            FTDAndroid.logEventLogin(loginChannel, customParams);

#else
            Debug.Log(errorMsgPlatform);
#endif
        }
        /**
         * 上报注册事件
         * @param registChannel 注册渠道
         * @param customParams 自定义参数，根据需要传入，不需要传null即可
         */
        public void logEventRegist(String registChannel, Dictionary<String, String> customParams)
        {
            Debug.Log("logEventRegist . >>>>>" + "registChannel=" + registChannel + " || customParams=" + customParams);

            if (!checkAvailable())
            {
                return;
            }
#if UNITY_IOS
           fRegisteredSuccese(registChannel, dic2json(customParams));
#elif UNITY_ANDROID
            FTDAndroid.logEventRegist(registChannel, customParams);
#else
            Debug.Log(errorMsgPlatform);
#endif
        }

    /**
      * 上报支付事件
      * @param channel 支付渠道（注意这里并不是传广告渠道，而是支付渠道，比如google支付就可以是“google”，自己定义）
      * @param itemid 商品id
      * @param itemName 商品id
      * @param usdPrice 必须是美分价格，否则无法计算数据,必传数据
      * @param price 支付金额(必须以“分”为单位)
      * @param currency 币种
      * @param customParams 自定义参数，根据需要传入
      */
    public void logEventPurchase(String channel, String itemid, String itemName, int usdPrice, int price, String currency, Dictionary<String, String> customParams)
        {
            Debug.Log("logEventPurchase . >>>>>" + "channel=" + channel + " || itemid=" + itemid + " || itemName=" + itemName + " || usdPrice=" + usdPrice + " || price=" + price + " || currency=" + currency + " || customParams=" + customParams);

            if (!checkAvailable())
            {
                return;
            }
#if UNITY_IOS
            fTrackRevenueSuccese(itemid,itemName,usdPrice+"",price+"",currency,channel, dic2json(customParams));
#elif UNITY_ANDROID
            FTDAndroid.logEventPurchase(channel, itemid, itemName, usdPrice,price, currency, customParams);
#else
            Debug.Log(errorMsgPlatform);
#endif
        }

        /**
           * 完成新手引导
           * @param isSuccess 是否成功
           * @param customParams 自定义参数，根据需要传入，不需要传null即可
           */
        public void logEventCompletedTutorial(bool isSuccess, Dictionary<String, String> customParams)
        {
            Debug.Log("logEventCompletedTutorial . >>>>>" + "isSuccess=" + isSuccess + " || customParams=" + customParams);

            if (!checkAvailable())
            {
                return;
            }
#if UNITY_IOS
            fNewbieGuideSuccese(isSuccess+"", dic2json(customParams));
#elif UNITY_ANDROID
            FTDAndroid.logEventCompletedTutorial(isSuccess, customParams);
#else
            Debug.Log(errorMsgPlatform);
#endif
        }

    //ios支付验证接口
    public void validateAndTrackInAppPurchase(string productIdentifier , string price, string currency, string transactionId, string customParams)
    {
        Debug.Log("logEventCompletedTutorial . >>>>>" + "productIdentifier=" + productIdentifier + "price=" + price + "currency=" + currency + "transactionId=" + transactionId + " || customParams=" + customParams);

        if (!checkAvailable())
        {
            return;
        }
        #if UNITY_IOS
        fValidateAndTrackInAppPurchase( productIdentifier,  price,  currency,  transactionId,  customParams);
        #endif
    }

    /**
     * 自定义事件
     * @param eventName 自定义事件名称
     * @param eventId 自定义事件id
     * @param params 自定义事件参数
     */
    public void logCustomEvent(String eventName, String eventId, Dictionary<String, String> customParams)
        {
            Debug.Log("logCustomEvent . >>>>>" + "eventName=" + eventName + " || eventId=" + eventId + " || customParams=" + dic2json(customParams));

            if (!checkAvailable())
            {
                return;
            }
#if UNITY_IOS
            fGetCustomEvent(eventName,eventId, dic2json(customParams));
#elif UNITY_ANDROID
            FTDAndroid.logCustomEvent(eventName, eventId, customParams);
#else
            Debug.Log(errorMsgPlatform);
#endif
        }
   /**
    * 设置tag接口，此TAG是标识在设备层级的tag，设置即会在tag数组中追加tag。新设置tag不会覆盖旧tag，且不可删除。
    * 一旦设置每次数据上报都会通过通传参数发送给SDK服务器，服务器可根据tag对数据进行归类计算等。
    *
    * @param tags
    */
    public void setTags(String[] tags)
    {
        if (tags == null || tags.Length < 1)
        {
            return;
        }
        Debug.Log("setTags . >>>>>" + "tags=" + "[\"" + string.Join("\",\"", tags) + "\"]");
        if (!checkAvailable())
        {
            return;
        }
#if UNITY_IOS

            string tagStr =  "[\"" + string.Join("\",\"", tags) + "\"]";
            fGetUserWithTags(tagStr);
    
#elif UNITY_ANDROID
        FTDAndroid.setTags(tags);
#else
            Debug.Log(errorMsgPlatform);
#endif
    }
    //判断当前环境是否是editor
    private static bool IsEditor()
        {
#if UNITY_EDITOR
                        Debug.Log(errorMsgEditor);
                        return true;
#else
                            return false;
#endif
        }

        //检查SDK是否可用
        private static bool checkAvailable()
        {
            if (IsEditor())
            {
                return false;
            }
            if (!isInit)
            {
                Debug.Log(errorMsgNotInit);
                return false;
            }
            return true;
        }
        public void onAttributeCallback(string attrJson)
        {
            if (FTDSdk.changedListener != null)
            {
                FTDSdk.changedListener.onAttributionChanged(attrJson);
            }
        }

    /**
     * ios支付验证回调
     * */
#if UNITY_IOS
        public void onPayResultCallback(string response)
        {
            if (FTDSdk.payVerifyCallback != null)
            {
                FTDSdk.payVerifyCallback.callback(response);
            }
        }
#endif
    //检查初始化参数是否有效
    private static bool checkInitParams(string appid,string appkey,string signWay)
        {
            if(appid == null || appkey == null || signWay == null)
            {
                Debug.Log("Invalid parameters");

                return false;
            }
            if (appid.Trim().Contains(" ") || appkey.Trim().Contains(" ") || signWay.Trim().Contains(" "))
            {
                Debug.Log("Invalid parameters");

                return false;
            }
            return true;
        }

        private static string dic2json(Dictionary<string , string> dictionary)
        {
            if (dictionary == null)
            {
                return null;
            }
            ArrayList list = new ArrayList();
            
            foreach (KeyValuePair<string, string> pair in dictionary)
            {
                list.Add(String.Format("\"{0}\":\"{1}\"", pair.Key, pair.Value));
            }

            return  "{" + string.Join(",", (string[])list.ToArray(typeof(string))) + "}";
        }
        
    }

