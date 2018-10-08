using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace com.ftdsdk.sdk
{
#if UNITY_ANDROID
    public class FTDAndroid
    {
        private static AndroidJavaClass cFTDSDK = new AndroidJavaClass("com.ftdsdk.www.FTDSDK");
        private static AndroidJavaClass ftdSDKLogical = new AndroidJavaClass("com.ftdsdk.www.logical.FTDSDKLogical");
        private static AndroidJavaObject ftdSdk;

        /**
          * SDK初始化
          * @param activity 当前的activtiy
          * @param appId 
          * @param appKey 
          * @param signWay 
          */
        public static void ftdSdkInit(AndroidJavaObject activity, String appId, String appKey, String signWay)
        {
            AndroidJavaObject application = activity.Call<AndroidJavaObject>("getApplication");

            ftdSDKLogical.CallStatic("init", application, appId, appKey, signWay);
            if (ftdSdk == null)
            {
                ftdSdk = cFTDSDK.CallStatic<AndroidJavaObject>("getApi");
            }
        }
        public static void setOnAttributeListener(FTDAttributeCallback mCallback)
        {
            ftdSdk.Call("setOnAttributeListener", mCallback);

        }
        public static void setHttpCallback(FTDHttpCallback mCallback)
        {
            ftdSdk.Call("setHttpCallback", mCallback);
        }
        public static void onPause()
        {
            ftdSdk.Call("onPause");
        }

        public static void onResume()
        {
            ftdSdk.Call("onResume");
        }

        /**
         * 上报渠道SDK归因
         * @param coversionData 渠道回调返回的归因
         */
        public static void sendAttributeData(String channel, String jsonAttr)
        {
            Debug.Log(">>>>>>>>>>>>>sendAttributeData");
            AndroidJavaObject json = new AndroidJavaObject("org.json.JSONObject", jsonAttr);
            //ftdSdk.Call("sendAttributeData",  channel, json);
            ftdSdk.Call("sendAttributeData", channel, json);
        }

        /**
        * 上报登录事件
        * @param loginChannel 登录渠道
        * @param customParams 自定义参数，根据需要传入，不需要传null即可
        */
        public static void logEventLogin(String loginChannel, Dictionary<String, String> customParams)
        {
            AndroidJavaObject map = dicToMap(customParams);
            ftdSdk.Call("logEventLogin", loginChannel, map);

            // ftdSdk.Invoke("logEventLogin", new object[] { loginChannel, map });
        }

        /**
         * 上报注册事件
         * @param registChannel 注册渠道
         * @param customParams 自定义参数，根据需要传入，不需要传null即可
         */
        public static void logEventRegist(String registChannel, Dictionary<String, String> customParams)
        {
            AndroidJavaObject map = dicToMap(customParams);
            ftdSdk.Call("logEventRegist", registChannel, map);

            //ftdSdk.Invoke("logEventRegist", new object[] { registChannel, map });

        }

        /**
        * 上报支付事件
        * @param channel 支付渠道
        * @param itemid 商品id
        * @param itemName 商品id
        * @param usdPrice 必须是美分价格，否则无法计算数据,必传数据
        * @param price 支付金额(必须以“分”为单位)
        * @param currency 币种（使用统一币种，推荐“USD”）
        * @param customParams 自定义参数，根据需要传入
        */
        public static void logEventPurchase(String channel, String itemid, String itemName, int usdPrice, int price, String currency, Dictionary<String, String> customParams)
        {
            AndroidJavaObject map = dicToMap(customParams);
            ftdSdk.Call("logEventPurchase", channel, itemid, itemName, usdPrice, price, currency, map);

            //ftdSdk.Invoke("logEventPurchase", new object[] { channel, itemid, itemName, price, currency, map });
        }

        /**
        * 完成新手引导
        * @param isSuccess 是否成功
        * @param customParams 自定义参数，根据需要传入，不需要传null即可
        */
        public static void logEventCompletedTutorial(bool isSuccess, Dictionary<String, String> customParams)
        {
            AndroidJavaObject map = dicToMap(customParams);
            ftdSdk.Call("logEventCompletedTutorial", isSuccess, map);

            //ftdSdk.Invoke("logEventCompletedTutorial", new object[] { isSuccess, map });

        }

        /**
         * 自定义事件
         * @param eventName 自定义事件名称
         * @param eventId 自定义事件id
         * @param params 自定义事件参数
         */
        public static void logCustomEvent(String eventName, String eventId, Dictionary<String, String> customParams)
        {
            AndroidJavaObject map = dicToMap(customParams);
            ftdSdk.Call("logCustomEvent", eventName, eventId, map);

            //ftdSdk.Invoke("logCustomEvent", new object[] { eventName, eventId, map });
        }
        /**
         * 设置tag接口，此TAG是标识在设备层级的tag，设置即会在tag数组中追加tag。新设置tag不会覆盖旧tag，且不可删除。
         * 一旦设置每次数据上报都会通过通传参数发送给SDK服务器，服务器可根据tag对数据进行归类计算等。
         *
         * @param tags
         */
        public static void setTags(String[] tags)
        {
            ftdSdk.Call("setTags", javaArrayFromCS(tags));
        }

        public static AndroidJavaObject dicToMap(Dictionary<string, string> dictionary)
        {
            if (dictionary == null)
            {
                return null;
            }
            AndroidJavaObject map = new AndroidJavaObject("java.util.HashMap");
            foreach (KeyValuePair<string, string> pair in dictionary)
            {
                map.Call<AndroidJavaObject>("put", pair.Key, pair.Value);
            }
            return map;
        }
        private static AndroidJavaObject javaArrayFromCS(string[] values)
        {   
           if(values == null)
            {
                values = new string[] { };
            }
            AndroidJavaClass arrayClass = new AndroidJavaClass("java.lang.reflect.Array");
            AndroidJavaObject arrayObject = arrayClass.CallStatic<AndroidJavaObject>("newInstance", new AndroidJavaClass("java.lang.String"), values.Count());
            for (int i = 0; i < values.Count(); ++i)
            {
                arrayClass.CallStatic("set", arrayObject, i, new AndroidJavaObject("java.lang.String", values[i]));
            }

            return arrayObject;

        }
    }
#endif
    }

