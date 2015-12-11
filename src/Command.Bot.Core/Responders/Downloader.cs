using System;
using System.Net;

namespace Command.Bot.Core.Responders
{
    public static class Downloader
    {
        public static byte[] Download(this Uri resource)
        {
            using (var client = new WebClient())
            {
                return client.DownloadData(resource);
            }
        }
    }
}