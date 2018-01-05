using Comm;
using System;
using System.Collections.Generic;
using System.Text;

namespace WXGame
{
    class Program
    {
        static void Main(string[] args)
        {
            var session_id = "user_session_id";
            var user_score = 999;
            var user_times = 74;

            Dictionary<string, string> header = new Dictionary<string, string>
            {
                { "charset", "utf-8" },
                { "Accept-Encoding", "gzip" },
                { "referer", " https://servicewechat.com/wx7c8d593b2c3a7703/5/page-frame.html" },
                { "content-type", "application/json" },
                { "User-Agent", "MicroMessenger/6.6.1.1220(0x26060133) NetType/WIFI Language/zh_CN" },
                { "Host", "mp.weixin.qq.com" }
            };
            var aes_key = session_id.Substring(0, 16);
            AESComm.CipherMode = System.Security.Cryptography.CipherMode.CBC;
            AESComm.Key = aes_key;
            AESComm.IV = aes_key;

            //其中game_data、musicList、touchList可以根据分数进行程序模拟生成
            var action_data = Newtonsoft.Json.JsonConvert.SerializeObject(
                  new
                  {
                      score = user_score,
                      times = user_times,
                      game_data = Newtonsoft.Json.JsonConvert.SerializeObject(
                      new
                      {
                          seed = DateTime.Now.Ticks,
                          action = new object[][] {
                           new object[] { 0.8,1.16,false},
                           new object[] {0.592, 1.56, false },
                           new object[] {0.816, 1.12, false },
                           new object[] {0.852, 1.06, false },
                           new object[] {0.512, 1.73, false },
                           new object[] {0.626, 1.5, false },
                           new object[] {0.101, 2.55, false },
                           new object[] {0.083, 2.58, false },
                           new object[] { 0.8,1.16,false},
                           new object[] {0.592, 1.56, false },
                           new object[] {0.816, 1.12, false },
                           new object[] {0.852, 1.06, false },
                           new object[] {0.512, 1.73, false },
                           new object[] {0.626, 1.5, false },
                           new object[] {0.101, 2.55, false },
                           new object[] {0.083, 2.58, false }
                          },
                          musicList = new bool[] { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false },
                          touchList = new object[][] {
                           new object[]{ 105.88172, 571.77454 },
                           new object[]{ 106.875, 572.5244 },
                           new object[]{ 108.28125, 573.9697 },
                           new object[]{ 110.390625, 575.95703 },
                           new object[]{ 111.18164, 578.125 },
                           new object[]{ 110.74219, 577.7637 },
                           new object[]{ 114.87305, 585.35156 },
                           new object[]{ 112.67578, 585.89355 },
                           new object[]{ 105.88172, 571.77454 },
                           new object[]{ 106.875, 572.5244 },
                           new object[]{ 108.28125, 573.9697 },
                           new object[]{ 110.390625, 575.95703 },
                           new object[]{ 111.18164, 578.125 },
                           new object[]{ 110.74219, 577.7637 },
                           new object[]{ 114.87305, 585.35156 },
                           new object[]{ 112.67578, 585.89355 }
                          },
                          version = "1"
                      })
                  });            
            var encryptstr = AESComm.Encrypt(action_data);
            var pa = "{\"base_req\":{\"session_id\":\""
                + session_id + "\",\"fast\":1},\"action_data\":\""
                + encryptstr + "\"}";
            var http = new HttpComm
            {
                Url = "https://mp.weixin.qq.com/wxagame/wxagame_settlement",
                Method = "post",
                Headers = header,
                Params = pa
            };
            http.Request();
            var res = Encoding.UTF8.GetString(http.ResponseBts);
            Console.WriteLine(res);
            Console.ReadKey();
        }
    }
}
