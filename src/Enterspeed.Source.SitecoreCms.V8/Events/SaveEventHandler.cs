﻿using System;
using System.Diagnostics;
using System.Linq;
using Enterspeed.Source.Sdk.Api.Services;
using Enterspeed.Source.Sdk.Domain.Connection;
using Enterspeed.Source.Sdk.Domain.Services;
using Enterspeed.Source.SitecoreCms.V8.Extensions;
using Enterspeed.Source.SitecoreCms.V8.Models;
using Enterspeed.Source.SitecoreCms.V8.Models.Configuration;
using Enterspeed.Source.SitecoreCms.V8.Models.Mappers;
using Enterspeed.Source.SitecoreCms.V8.Providers;
using Enterspeed.Source.SitecoreCms.V8.Services;
using Enterspeed.Source.SitecoreCms.V8.Services.Serializers;
using Sitecore.Abstractions;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Events;
using Sitecore.Globalization;
using Sitecore.Links;
using Sitecore.Pipelines.Save;
using Sitecore.Publishing;
using Sitecore.Publishing.Pipelines.PublishItem;
using Version = Sitecore.Data.Version;

namespace Enterspeed.Source.SitecoreCms.V8.Events
{
    public class SaveEventHandler
    {
        private readonly BaseItemManager _itemManager;
        private readonly IEnterspeedConfigurationService _enterspeedConfigurationService;
        private readonly IEnterspeedSitecoreIngestService _enterspeedSitecoreIngestService;

        public SaveEventHandler(
            BaseItemManager itemManager,
            IEnterspeedConfigurationService enterspeedConfigurationService,
            IEnterspeedSitecoreIngestService enterspeedSitecoreIngestService)
        {
            _itemManager = itemManager;
            _enterspeedConfigurationService = enterspeedConfigurationService;
            _enterspeedSitecoreIngestService = enterspeedSitecoreIngestService;
        }

        public void OnItemSaved(object sender, EventArgs args)
            {
            SitecoreEventArgs eventArgs = args as SitecoreEventArgs;

            Item sourceItem = eventArgs.Parameters[0] as Item;

            if (sourceItem == null)
            {
                return;
            }

            var siteConfigurations = _enterspeedConfigurationService.GetConfiguration();
            foreach (EnterspeedSitecoreConfiguration configuration in siteConfigurations)
            {
                if (!configuration.IsEnabled)
                {
                    continue;
                }

                if (!configuration.IsPreview)
                {
                    continue;
                }

                EnterspeedIngestService enterspeedIngestService = new EnterspeedIngestService(new SitecoreEnterspeedConnection(configuration), new NewtonsoftJsonSerializer(), new EnterspeedSitecoreConfigurationProvider(_enterspeedConfigurationService));
                Language language = sourceItem.Language;

                // Getting the source item first
                if (sourceItem == null)
                {
                    continue;
                }

                if (!_enterspeedSitecoreIngestService.HasAllowedPath(sourceItem))
                {
                    continue;
                }

                //// Handling if the item was deleted or unpublished
                //bool itemIsDeleted = context.Action == PublishAction.DeleteTargetItem;

                //if (itemIsDeleted)
                //{
                //    _enterspeedSitecoreIngestService.HandleContentItem(sourceItem, enterspeedIngestService, configuration, true, false, true);
                //    _enterspeedSitecoreIngestService.HandleRendering(sourceItem, enterspeedIngestService, configuration, true, false,true);
                //    _enterspeedSitecoreIngestService.HandleDictionary(sourceItem, enterspeedIngestService, configuration, true, false,true);

                //    continue;
                //}

                // Handling if the item was published
                if (sourceItem == null || sourceItem.Versions.Count == 0)
                {
                    continue;
                }

                if (!_enterspeedSitecoreIngestService.HasAllowedPath(sourceItem))
                {
                    continue;
                }

                _enterspeedSitecoreIngestService.HandleContentItem(sourceItem, enterspeedIngestService, configuration, false, true, true);
                _enterspeedSitecoreIngestService.HandleRendering(sourceItem, enterspeedIngestService, configuration, false, true, true);
                _enterspeedSitecoreIngestService.HandleDictionary(sourceItem, enterspeedIngestService, configuration, false, true, true);
            }
        }
    }
}