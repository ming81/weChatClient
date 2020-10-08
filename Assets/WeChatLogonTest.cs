using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeChatLogonTest : MonoBehaviour
{
    WeChatComponent weChatComponent;
    Text ResultText;
    void Start()
    {
        weChatComponent = GameObject.Find("WeChatComponent").GetComponent<WeChatComponent>();
        transform.Find("Panel/WeChatLogonBtn").GetComponent<Button>().onClick.AddListener(TestWechatLogon);
        weChatComponent.weChatLogonCallback += WeChatLogonCallback;
        ResultText = transform.Find("Panel/ResultText").GetComponent<Text>();
    }

    private void WeChatLogonCallback(WeChatUserData obj)
    {
        if (obj != null)
        {
           // Debug.Log(obj.nickname);

           ResultText.text = "微信登录结果:"+ JsonUtility.ToJson(obj);
        }
        else {
            Debug.Log("取消登录!");
        }
    }

    private void TestWechatLogon()
    {
        ResultText.text = "微信登录结果:";
        if (weChatComponent.IsWechatInstalled())
        {
            weChatComponent.WeChatLogon("mafeng");
        }
        else
        {
            Debug.Log("未安装微信!");
        }
    }

    
}
