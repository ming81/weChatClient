using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class WeChatPayTest : MonoBehaviour
{
    TcpClient client;
    WeChatComponent weChatComponent;
    Text PayResult1, PayResult2;
    Button WechatPayBtn;
    void Start()
    {
        WechatPayBtn = transform.Find("Panel/WechatPayBtn").GetComponent<Button>();
        PayResult1 = transform.Find("Panel/PayResult1").GetComponent<Text>();
        PayResult2 = transform.Find("Panel/PayResult2").GetComponent<Text>();
        WechatPayBtn.onClick.AddListener(WechatPayBtnOnClick);

        weChatComponent = GameObject.Find("WeChatComponent").GetComponent<WeChatComponent>();

        client = new TcpClient();
        Connect();

        weChatComponent.weChatPayCallback += WeChatPayCallback;
    }

    string sendStr = "pay";
    private async void WechatPayBtnOnClick()
    {
        try
        {
            if (client.Connected)
            {
                PayResult1.text = "同步通知:";
                PayResult2.text = "异步通知结果:";

                byte[] data = Encoding.UTF8.GetBytes(sendStr);
                await client.GetStream().WriteAsync(data, 0, data.Length);
                Debug.Log("发送成功,请求支付参数");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"发送异常:{e.Message}");
        }
    }

    private void WeChatPayCallback(int state)
    {
        //参考链接:https://pay.weixin.qq.com/wiki/doc/api/app/app.php?chapter=8_5
        string result = "";
        switch (state)
        {
            case 0:
                result = "成功";
                break;
            case -1:
                result = "失败";
                break;
            case -2:
                result = "取消";
                break;
            default:
                break;
        }
        PayResult1.text = $"同步通知结果:{result}";
    }

    private async void Connect()
    {
        try
        {
            await client.ConnectAsync("193.112.44.199", 7577);
            Receive();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"连接异常:{e.Message}");
        }
    }

    private async void Receive()
    {
        try
        {
            //连接
            if (client.Connected) {
                byte[] buffer = new byte[1024];
                
                int length = await client.GetStream().ReadAsync(buffer, 0, buffer.Length);
                string s = Encoding.UTF8.GetString(buffer, 0, length);
                if (s!= "error")
                {
                    if (s != "支付成功" && s != "支付失败")
                    {
                        //调用微信的支付接口 反序列化
                        WeChatPayModel payModel =JsonUtility.FromJson<WeChatPayModel>(s);
                        weChatComponent.WeChatPay(payModel.appid, payModel.mch_id, payModel.prepayid,
                            payModel.noncestr, payModel.timestamp, payModel.sign);
                    }
                    else
                    {
                        PayResult2.text = $"异步通知结果:{s}";
                    }
                }
                else
                {
                    Debug.LogError("Receive:error");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"接收异常:{e.Message}");
            return;
        }
        Receive();

    }

    [System.Serializable]
    public class WeChatPayModel
    {
        public string appid;
        public string mch_id;
        public string prepayid;
        public string noncestr;
        public string timestamp;
        public string packageValue;
        public string sign;
    }
}
