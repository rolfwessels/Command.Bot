using System.Collections.Generic;
using Newtonsoft.Json;
using SlackConnector.Models;

namespace Command.Bot.Core.Parser
{
    public static class MessageParser
    {
        public static RootObject GetFullMessage(this SlackMessage rawData)
        {
            return JsonConvert.DeserializeObject<RootObject>(rawData.RawData);
        }


        public class File
        {
            public string Id { get; set; }
            public int Created { get; set; }
            public int Timestamp { get; set; }
            public string Name { get; set; }
            public string Title { get; set; }
            public string Mimetype { get; set; }
            public string Filetype { get; set; }
            public string PrettyType { get; set; }
            public string User { get; set; }
            public bool Editable { get; set; }
            public int Size { get; set; }
            public string Mode { get; set; }
            public bool IsExternal { get; set; }
            public string ExternalType { get; set; }
            public bool IsPublic { get; set; }
            public bool PublicUrlShared { get; set; }
            public bool DisplayAsBot { get; set; }
            public string Username { get; set; }
            public string Url { get; set; }
            [JsonProperty(PropertyName = "url_download")]
            public string UrlDownload { get; set; }
            [JsonProperty(PropertyName = "url_private")]
            public string UrlPrivate { get; set; }
            public string UrlPrivateDownload { get; set; }
            public string Permalink { get; set; }
            public string PermalinkPublic { get; set; }
            public string EditLink { get; set; }
            public string Preview { get; set; }
            public string PreviewHighlight { get; set; }
            public int Lines { get; set; }
            public int LinesMore { get; set; }
            public List<object> Channels { get; set; }
            public List<object> Groups { get; set; }
            public List<object> Ims { get; set; }
            public int CommentsCount { get; set; }
        }

        public class RootObject
        {
            public string Type { get; set; }
            public string Subtype { get; set; }
            public string Text { get; set; }
            public File File { get; set; }
            public string User { get; set; }
            public bool Upload { get; set; }
            public bool DisplayAsBot { get; set; }
            public string Username { get; set; }
            public object BotId { get; set; }
            public string Channel { get; set; }
            public string Ts { get; set; }
        }

    }
}