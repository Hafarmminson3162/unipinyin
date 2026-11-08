using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace UniPinyin
{
    public static class GooglePinyinService
    {
        private static readonly Dictionary<string, List<string>> Cache = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        private static readonly JavaScriptSerializer Serializer = new JavaScriptSerializer();
        private static readonly object CacheLock = new object();

        static GooglePinyinService()
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            }
            catch { }
        }

        public static List<string> Query(string pinyin, bool isSimplified, int count)
        {
            if (string.IsNullOrEmpty(pinyin)) return new List<string>();

            string cacheKey = string.Format("{0}_{1}_{2}", pinyin.ToLowerInvariant(), isSimplified ? "s" : "t", count);
            lock (CacheLock)
            {
                if (Cache.ContainsKey(cacheKey))
                {
                    return new List<string>(Cache[cacheKey]);
                }
            }

            string itc = isSimplified ? "zh-t-i0-pinyin" : "zh-hant-t-i0-pinyin";
            string url = string.Format("https://inputtools.google.com/request?text={0}&itc={1}&num={2}&cp=0&cs=1&ie=utf-8&oe=utf-8&app=unipinyin-en",
                Uri.EscapeDataString(pinyin),
                itc,
                count);

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.Timeout = 1500;
                request.ReadWriteTimeout = 1500;
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) UniPinyin-EN/1.0";

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string json = reader.ReadToEnd();
                    List<string> candidates = ParseGoogleResponse(json);

                    if (candidates != null && candidates.Count > 0)
                    {
                        lock (CacheLock)
                        {
                            if (Cache.Count > 500) Cache.Clear();
                            Cache[cacheKey] = candidates;
                        }
                        return candidates;
                    }
                }
            }
            catch
            {
                // Fallback to local dictionary when offline
            }

            return new List<string>();
        }

        private static List<string> ParseGoogleResponse(string json)
        {
            List<string> list = new List<string>();
            try
            {
                object[] root = Serializer.DeserializeObject(json) as object[];
                if (root != null && root.Length > 1 && "SUCCESS".Equals(root[0]))
                {
                    object[] queries = root[1] as object[];
                    if (queries != null && queries.Length > 0)
                    {
                        object[] firstQuery = queries[0] as object[];
                        if (firstQuery != null && firstQuery.Length > 1)
                        {
                            object[] rawCandidates = firstQuery[1] as object[];
                            if (rawCandidates != null)
                            {
                                foreach (var item in rawCandidates)
                                {
                                    if (item != null)
                                    {
                                        string cand = item.ToString();
                                        if (!string.IsNullOrEmpty(cand) && !list.Contains(cand))
                                        {
                                            list.Add(cand);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch { }
            return list;
        }
    }
}
