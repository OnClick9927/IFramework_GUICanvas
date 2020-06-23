/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-08-06
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace IFramework.Net
{
    public class FileDownLoader_WebRequest : FileDownLoader
    {
        public override float progress
        {
            get
            {
                if (webRequest != null)
                    return webRequest.downloadProgress;
                return 0;
            }
        }
        public override long currentLength
        {
            get
            {
                if (webRequest != null)
                {
                    return (long)webRequest.downloadedBytes;
                }
                return 0;
            }
        }
        private UnityWebRequest webRequest;

        public FileDownLoader_WebRequest(string url, string SaveDir) : base(url, SaveDir) { }

        public override void DownLoad()
        {

            webRequest = UnityWebRequest.Get(url);

            downLoading = true;
            // webRequest.timeout = 30;//���ó�ʱ����webRequest.SendWebRequest()���ӳ�ʱ�᷵�أ���isNetworkErrorΪtrue

            var op = webRequest.SendWebRequest();
            while (!op.isDone)
            {

            }
            downLoading = false;
            if (webRequest.isNetworkError)
                Debug.Log("Download Error:" + webRequest.error);
            else
                File.WriteAllBytes(saveFilePath, webRequest.downloadHandler.data);
            Compelete();
        }

        public override void Dispose()
        {
            base.Dispose();
            if (webRequest != null)
            {
                webRequest.Dispose();
                webRequest = null;
            }
        }

    }
}