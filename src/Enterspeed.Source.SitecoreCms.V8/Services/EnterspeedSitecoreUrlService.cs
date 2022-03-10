﻿using Enterspeed.Source.SitecoreCms.V8.Models.Configuration;
using Sitecore.Abstractions;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Links;
using Sitecore.Resources.Media;
using Sitecore.Sites;
using System;

namespace Enterspeed.Source.SitecoreCms.V8.Services
{
    public class EnterspeedSitecoreUrlService : IEnterspeedUrlService
    {
        private readonly IEnterspeedConfigurationService _enterspeedConfigurationService;
        private readonly BaseSiteContextFactory _siteContextFactory;
        private readonly BaseLinkManager _linkManager;
        private readonly BaseMediaManager _mediaManager;

        public EnterspeedSitecoreUrlService(
            IEnterspeedConfigurationService enterspeedConfigurationService,
            BaseSiteContextFactory siteContextFactory,
            BaseLinkManager linkManager,
            BaseMediaManager mediaManager)
        {
            _enterspeedConfigurationService = enterspeedConfigurationService;
            _siteContextFactory = siteContextFactory;
            _linkManager = linkManager;
            _mediaManager = mediaManager;
        }

        public string GetItemUrl(Item item, EnterspeedSiteInfo siteInfo, bool enableLanguageEmbedding = false)
        {
            if (item == null)
            {
                return null;
            }

            var urlBuilderOptions = new UrlOptions()
            {
                SiteResolving = true,
                AlwaysIncludeServerUrl = true,
                LowercaseUrls = true,
                LanguageEmbedding = enableLanguageEmbedding ? LanguageEmbedding.Always : LanguageEmbedding.Never
            };

            if (siteInfo != null)
            {
                SiteContext siteContext = _siteContextFactory.GetSiteContext(siteInfo.Name);

                urlBuilderOptions.Site = siteContext;
                urlBuilderOptions.SiteResolving = string.IsNullOrEmpty(siteInfo.BaseUrl);
                urlBuilderOptions.AlwaysIncludeServerUrl = string.IsNullOrEmpty(siteInfo.BaseUrl);
            }

            var itemUrl = "";
            using (var siteContextSwitcher = new SiteContextSwitcher(urlBuilderOptions.Site))
            {

                itemUrl = LinkManager.GetItemUrl(item, urlBuilderOptions);
                if (!itemUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    itemUrl = "http" + itemUrl;
                }

                if (!string.IsNullOrEmpty(siteInfo.BaseUrl))
                {
                    itemUrl = siteInfo.BaseUrl + itemUrl.Replace(siteInfo.StartPathUrl,"/");
                }
                return itemUrl;
            }
 

        }

        public string GetMediaUrl(MediaItem mediaItem, EnterspeedSiteInfo siteInfo)
        {
            if (mediaItem == null)
            {
                return null;
            }

            var urlBuilderOptions = new MediaUrlOptions()
            {
                AbsolutePath = true,
                AlwaysIncludeServerUrl = string.IsNullOrEmpty(siteInfo.MediaBaseUrl),
                Language = Language.Invariant
            };

            string mediaUrl = _mediaManager.GetMediaUrl(mediaItem, urlBuilderOptions);
            if (string.IsNullOrEmpty(mediaUrl))
            {
                return null;
            }

            if (!string.IsNullOrEmpty(siteInfo.MediaBaseUrl))
            {
                mediaUrl = siteInfo.MediaBaseUrl + mediaUrl;
            }

            if (mediaUrl.EndsWith(".ashx") &&
                !string.IsNullOrEmpty(mediaItem.Extension))
            {
                mediaUrl = mediaUrl.Replace(".ashx", $".{mediaItem.Extension}");
            }

            return mediaUrl;
        }
    }
}