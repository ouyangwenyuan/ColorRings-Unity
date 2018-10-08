using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FTDHttpCallback : AndroidJavaProxy
{
    public FTDHttpCallback() : base("com.ftdsdk.www.http.base.FTHttpCallBack")
    {
    }
    void onResponse(int code, AndroidJavaObject request, AndroidJavaObject mResponse, AndroidJavaObject tag)
    {
        if (FTDSdk.ftHttpCallback != null)
        {
            string mRequest = request.Call<string>("toString");
            string response = mResponse.Call<string>("toString");
            FTDSdk.ftHttpCallback.onReponse(code, mRequest, response);
        }
    }

}
