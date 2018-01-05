#region copyright
// <copyright file="HttpComm"  company="CIS"> 
// Copyright (c) CIS. All Right Reserved
// </copyright>
// <author>ddfm</author>
// <datecreated>2017/11/17 9:16:44</datecreated>
#endregion
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
namespace Comm
{
    public class HttpComm
    {
        /// <summary>
        /// 请求地址
        /// </summary>
        public string Url { get; set; } = string.Empty;
        /// <summary>
        /// 请求方式：GET、POST...
        /// </summary>
        public string Method { get; set; } = string.Empty;
        public bool KeepAlive { get; set; } = true;
        /// <summary>
        /// 参数
        /// </summary>
        public string Params { get; set; } = string.Empty;

        public Dictionary<string, string> ParamDict { get; set; }
        /// <summary>
        /// 超时时间
        /// </summary>
        public int Timeout { get; set; } = 1000 * 5;
        public CookieContainer Cookies { get; set; } = new CookieContainer();
        private HttpWebResponse _webResponse { get; set; }
        /// <summary>
        /// 请求头
        /// </summary>
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// 响应流
        /// </summary>
        //public MemoryStream ResponseStream { get; set; } = new MemoryStream();
        /// <summary>
        /// 字节
        /// </summary>
        public byte[] ResponseBts { get; set; }

        public Func<HttpWebResponse, bool> BeforeResponse;

        private HttpWebRequest _httpWebRequest;
        public void Request(string url)
        {
            Url = url;
            Request();
        }
        public void Request()
        {
            Init();
            GetHttpResponse();
            if (BeforeResponse != null && !BeforeResponse.Invoke(_webResponse))
            {

            }
            else
            {
                GetResponseStream();
            }
            // return ResponseStream;
        }

        private void Init()
        {
            _httpWebRequest = WebRequest.Create(Url) as HttpWebRequest;
            _httpWebRequest.KeepAlive = KeepAlive;
            _httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2272.89 Safari/537.36";
            foreach (var item in Headers)
            {
                switch (item.Key.ToLower())
                {
                    case "host":
                        {
                            _httpWebRequest.Host = item.Value;
                            break;
                        }
                    case "content-type":
                        {
                            _httpWebRequest.ContentType = item.Value;
                            break;
                        }
                    case "user-agent":
                        {
                            _httpWebRequest.UserAgent = item.Value;
                            break;
                        }
                    case "referer":
                        {
                            _httpWebRequest.Referer = item.Value;
                            break;
                        }
                    case "connection":
                        {
                          //  SetHeaderValue(_httpWebRequest.Headers, "Connection", item.Value);
                            break;
                        }
                    default:
                        {
                            _httpWebRequest.Headers.Add(item.Key, item.Value);
                            break;
                        }
                }
            }

            _httpWebRequest.Timeout = Timeout;

            _httpWebRequest.CookieContainer = Cookies;
            switch (Method.ToLower())
            {
                case "get":
                    {
                        Get();
                    }
                    break;
                case "post":
                    {
                        Post();
                    }
                    break;
                default:
                    {
                        Get();
                    }
                    break;
            }
        }
        private void Get()
        {
            if (!string.IsNullOrEmpty(Params))
                if (!Params.StartsWith("?"))
                    Params += "?" + Params;
                else
                    Params += Params;
            _httpWebRequest.Method = "GET";
            //request.ContentType = "application/x-www-form-urlencoded";

        }

        private void Post()
        {
            byte[] bufferBytes = Encoding.UTF8.GetBytes(Params);
            _httpWebRequest.Method = "POST";
            _httpWebRequest.ContentLength = bufferBytes.Length;
            //request.ContentType = "application/x-www-form-urlencoded";


            using (var requestStream = _httpWebRequest.GetRequestStream())
            {
                requestStream.Write(bufferBytes, 0, bufferBytes.Length);
            }

        }
        private void GetHttpResponse()
        {

            _webResponse = _httpWebRequest.GetResponse() as HttpWebResponse;
        }
        private void GetResponseStream()
        {
            Stream resStream = null;
            MemoryStream ResponseStream = new MemoryStream();
            resStream = _webResponse.GetResponseStream();
            byte[] buffer = new byte[1024 * 1024];
            int i;
            while ((i = resStream.Read(buffer, 0, buffer.Length)) > 0)
                ResponseStream.Write(buffer, 0, i);
            byte[] arraryByte = ResponseStream.ToArray();
            ResponseStream.Seek(0, SeekOrigin.Current);
            ResponseStream.Position = 0;

            ResponseBts = ResponseStream.ToArray();
            _webResponse.Close();
            _httpWebRequest.Abort();

            resStream.Dispose();
            ResponseStream.Dispose();
        }
        private void SetHeaderValue(WebHeaderCollection header, string name, string value)
        {
            var property = typeof(WebHeaderCollection).GetProperty("InnerCollection",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (property != null)
            {
                var collection = property.GetValue(header, null) as NameValueCollection;
                collection[name] = value;
            }
        }
    }
}
