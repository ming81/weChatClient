using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeChatShareTest : MonoBehaviour
{
    WeChatComponent weChatComponent;
    Text ResultText;
    void Start()
    {
        weChatComponent= GameObject.Find("WeChatComponent").GetComponent<WeChatComponent>();
       
        transform.Find("Panel/ShareImageBtn").GetComponent<Button>().onClick.
            AddListener(ShareImageBtnOnClick);
        transform.Find("Panel/ShareWebPageBtn").GetComponent<Button>().onClick.
         AddListener(ShareWebPageBtnOnClick);
        ResultText = transform.Find("Panel/ResultText").GetComponent<Text>();

        //要记得 移除委托的注册
        weChatComponent.weChatShareWebPageCallback += WeChatShareWebPageCallback;
        weChatComponent.weChatShareImageCallback += WeChatShareImageCallback;
    }

    private void WeChatShareImageCallback(string obj)
    {
        ResultText.text = "分享结果:"+obj;
    }

    private void WeChatShareWebPageCallback(string obj)
    {
        ResultText.text = "分享结果:" + obj;
    }

    Texture2D miniTexture, mainTexture;
    byte[] thumbData, mainImageData;
    void LoadTexture()
    {
        if (miniTexture == null)
        {

            Debug.Log("分享图片");
            miniTexture = Resources.Load<Texture2D>("shareIcon");
            thumbData = miniTexture.EncodeToPNG();//GetRawTextureData(); //miniTexture.EncodeToPNG(); 
        }
        if (mainTexture == null)
        {
            mainTexture = Resources.Load<Texture2D>("shareImage");
            mainImageData = mainTexture.EncodeToJPG();// EncodeToJPG();
        }
    }

    /// <summary>
    /// 分享网页的点击事件
    /// </summary>
    private void ShareWebPageBtnOnClick()
    {
        ResultText.text = "分享结果:";
        LoadTexture();
        weChatComponent.WeChatShare_WebPage(0, "https://www.baidu.com/", "百度一下,你就知道了",
            "全球最大的中文搜索引擎、致力于让网民更便捷地获取信息.", thumbData);
    }

    /// <summary>
    /// 分享图片的按钮点击事件
    /// </summary>
    private void ShareImageBtnOnClick()
    {
        ResultText.text = "分享结果:";
        LoadTexture();
        weChatComponent.WeChatShare_Image(0, mainImageData, thumbData);
    }

    private void OnDestroy()
    {
        weChatComponent.weChatShareWebPageCallback -= WeChatShareWebPageCallback;
        weChatComponent.weChatShareImageCallback -= WeChatShareImageCallback;
    }

}
