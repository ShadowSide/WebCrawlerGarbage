using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WebCrawler
{
    public class HtmlSelectorModel
    {
        [JsonProperty(Required = Required.Always)]
        public string HtmlSelectorName { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string FindingType { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string FinderRequest { get; set; }
    }

    public class ExtractedJoinerModel
    {
        [JsonProperty(Required = Required.Always)]
        public string JoinerName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Start { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Between { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public uint? End { get; set; }
    }

    public class ExtractorModel
    {
        [JsonProperty(Required = Required.Always)]
        public string ExtractorName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public LinkedList<string> HtmlSelectorNames { get; set; }//null for all content matches
        //then
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Regex { get; set; }//null for all content matches

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public uint? MatchLevel { get; set; }//multimatch if present

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string JoinerName { get; set; }
    }

    public class FormaterModel
    {
        [JsonProperty(Required = Required.Always)]
        public string FormaterName { get; set; }

        [JsonProperty(Required = Required.Always)]
        public LinkedList<string> ExtractorNames { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Formater { get; set; }
    }

    public class InPagePathModel
    {
        [JsonProperty(Required = Required.Always)]
        public string PathName { get; set; }

        [JsonProperty(Required = Required.Always)]
        public LinkedList<string> HtmlSelectorName { get; set; }
    }

    public class OverPagePathModel
    {
        [JsonProperty(Required = Required.Always)]
        public LinkedList<string> InPagePathNames { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        string NextHrefExtractorName { get; set; }
    }

    public class ItemModel
    {
        [JsonProperty(Required = Required.Always)]
        public string NameFormaterName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DescriptionFormaterName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        LinkedList<OverPagePathModel> OverPagePathsToDownloadHref { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DownloadHrefExtractorName { get; set; }
    }

    public class TargetsModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public uint? RepeatAfterAmountMinutes { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DefaultHost { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        LinkedList<ExtractorModel> Extractors { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        LinkedList<FormaterModel> Formaters{ get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        LinkedList<HtmlSelectorModel> Joiners { get; set; }

        [JsonProperty(Required = Required.Always)]
        LinkedList<HtmlSelectorModel> HtmlSelectors { get; set; }

        [JsonProperty(Required = Required.Always)]
        LinkedList<InPagePathModel> InPagePaths { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        LinkedList<OverPagePathModel> OverPagePaths { get; set; }

        [JsonProperty(Required = Required.Always)]
        string StarterPage { get; set; }

        [JsonProperty(Required = Required.Always)]
        ItemModel Item { get; set; }
    }
}
