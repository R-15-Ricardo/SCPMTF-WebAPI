using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using OpenAI_API;
using scpmtf_webapi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace scpmtf_webapi.Scrapper
{
    public class gpt3generator
    {
        public gpt3generator()
        {
        }
        public async Task<SourceSCP> getRawModelFromUrl(string iNo)
        {
            var url = $"http://lafundacionscp.wikidot.com/scp-{iNo}";

            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;

            var html = response.Content.ReadAsStringAsync().Result;
            var scpSource = new HtmlDocument();
            scpSource.LoadHtml(html);

            var objClassMatches = scpSource.DocumentNode.Descendants("p")
                .Where(node => node.InnerText.Contains(" Objeto:")).ToList();

            var objDescriptionMateches = scpSource.DocumentNode.Descendants("p")
                .Where(node => node.InnerText.Contains("Descripción:")).ToList();

            var objImgMatches = scpSource.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "").Equals("scp-image-block block-right")).ToList();

            if (objClassMatches.Count < 1 || objDescriptionMateches.Count < 1)
                return null;

            var newraw = new SourceSCP {
                ObjectClass = objClassMatches.First().InnerText.Trim(),
                Description = objDescriptionMateches.First().InnerText.Substring(13).Normalize(),
                ObjectImage = (objImgMatches.Count > 0) ? objImgMatches.First().Descendants("img").FirstOrDefault().ChildAttributes("src").FirstOrDefault().Value : ""
            };

            return newraw;
        }

        public async Task<SCPObject> autoGetSCP(int itNo)
        {
            var itemId = (itNo < 100) ? itNo.ToString("D3") : itNo.ToString();
            SourceSCP rawSource = await getRawModelFromUrl(itemId);

            if (rawSource == null) return null;

            rawSource.FieldsNormalize();
            
            var newscp = new SCPObject
            {
                ItemNo = itemId,
                ObjectClass = rawSource.ObjectClass,
                Description = rawSource.Description,
                ImageAttachment = rawSource.ObjectImage
            };

            return newscp;
        }

        public async Task<ScpGPT3Summary> autoGetSummary(SCPObject scp)
        {
            var api = new OpenAI_API.OpenAIAPI(engine: Engine.Davinci);
            var prompt = $"Resume brevemente el siguiente texto:\n\"{scp.Description}\"\nResumen:\nSCP-{scp.ItemNo} es";

            var result = await api.Completions.CreateCompletionAsync(prompt, temperature: 0.72, max_tokens: 64, top_p: 1, numOutputs: 5, stopSequences: ". ");

            var newSummary = new ScpGPT3Summary
            {
                ItemNo = int.Parse(scp.ItemNo),
                CombatSummary = Regex.Replace($"SCP-{scp.ItemNo} es{result.ToString()}.", "[^a-zA-Z\u00C0-\u017F0-9_. ,-]+", "", RegexOptions.Compiled).Trim(),
            };

            return newSummary;
        }
    }

    
}

//[^a-zA-Z0-9_. ,-]+
