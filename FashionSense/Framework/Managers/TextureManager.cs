﻿using FashionSense.Framework.Interfaces.API;
using FashionSense.Framework.Models.Appearances;
using FashionSense.Framework.Models.Appearances.Generic;
using FashionSense.Framework.Models.Appearances.Hat;
using FashionSense.Framework.Models.Appearances.Shirt;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FashionSense.Framework.Managers
{
    internal class TextureManager
    {
        private IMonitor _monitor;
        private List<AppearanceContentPack> _appearanceTextures;

        private Dictionary<string, AppearanceContentPack> _idToModels;

        public TextureManager(IMonitor monitor)
        {
            _monitor = monitor;
            _appearanceTextures = new List<AppearanceContentPack>();

            _idToModels = new Dictionary<string, AppearanceContentPack>();
        }

        public void Reset(string packId = null)
        {
            if (String.IsNullOrEmpty(packId) is true)
            {
                _appearanceTextures.Clear();
                _idToModels.Clear();
            }
            else
            {
                _appearanceTextures = _appearanceTextures.Where(a => a.Owner.Equals(packId, StringComparison.OrdinalIgnoreCase) is false).ToList();
            }
        }

        public void AddAppearanceModel(AppearanceContentPack model)
        {
            if (string.IsNullOrEmpty(model.Id) is false && _appearanceTextures.Any(t => t.Id == model.Id && t.PackType == model.PackType))
            {
                var replacementIndex = _appearanceTextures.IndexOf(_appearanceTextures.First(t => t.Id == model.Id && t.PackType == model.PackType));
                _appearanceTextures[replacementIndex] = model;
            }
            else
            {
                if (model.IsLocalPack is false)
                {
                    if (AttemptHandleExternalPack(model) is false)
                    {
                        return;
                    }
                }

                _appearanceTextures.Add(model);
            }

            _idToModels[model.Id] = model;
        }

        private bool AttemptHandleExternalPack(AppearanceContentPack appearanceContentPack)
        {
            // Handle any appearances that may have been added externally
            if (string.IsNullOrEmpty(appearanceContentPack.PackId))
            {
                _monitor.Log($"Unable to add appearance from {appearanceContentPack.PackName}: Missing required field PackId", LogLevel.Warn);
                return false;
            }
            // Load in any missing properties (FS appearances added via Content Patcher)
            if (string.IsNullOrEmpty(appearanceContentPack.Owner))
            {
                appearanceContentPack.Owner = appearanceContentPack.PackId;
            }
            if (string.IsNullOrEmpty(appearanceContentPack.Id))
            {
                appearanceContentPack.Id = String.Concat(appearanceContentPack.Owner, "/", appearanceContentPack.PackType, "/", appearanceContentPack.Name);
            }
            if (appearanceContentPack.Texture is null && string.IsNullOrEmpty(appearanceContentPack.TexturePath) is false)
            {
                appearanceContentPack.Texture = FashionSense.modHelper.GameContent.Load<Texture2D>(appearanceContentPack.TexturePath);
            }
            appearanceContentPack.LinkId();

            return true;
        }

        public void Sync<T>(Dictionary<string, T> models, IApi.Type packType = IApi.Type.Unknown) where T : AppearanceContentPack
        {
            foreach (var model in models.Values)
            {
                if (model.IsLocalPack is false && model.PackType is IApi.Type.Unknown)
                {
                    model.PackType = packType;
                }

                AddAppearanceModel(model);
            }
        }

        public Dictionary<string, AppearanceContentPack> GetIdToAppearanceModels()
        {
            return _idToModels;
        }

        public Dictionary<string, T> GetIdToAppearanceModels<T>() where T : AppearanceContentPack
        {
            return _idToModels.Where(i => i.Value is T).ToDictionary(i => i.Key, i => (T)i.Value);
        }

        public List<AppearanceContentPack> GetAllAppearanceModels()
        {
            return _appearanceTextures.Where(t => t.IsLocked is false).ToList();
        }

        public List<T> GetAllAppearanceModels<T>() where T : AppearanceContentPack
        {
            return _appearanceTextures.OfType<T>().ToList();
        }

        public T GetSpecificAppearanceModel<T>(string appearanceId) where T : AppearanceContentPack
        {
            return (T)_appearanceTextures.FirstOrDefault(t => String.Equals(t.Id, appearanceId, StringComparison.OrdinalIgnoreCase) && t is T);
        }

        public AppearanceContentPack GetAppearanceContentPackByItemId(string itemId)
        {
            return _appearanceTextures.FirstOrDefault(t => t.Item is not null && t.Item.Id == itemId);
        }

        public AppearanceContentPack GetRandomAppearanceModel<T>()
        {
            var typedAppearanceModels = GetAllAppearanceModels().Where(m => m is T).ToList();
            if (typedAppearanceModels.Count() == 0)
            {
                return null;
            }

            var randomModelIndex = Game1.random.Next(typedAppearanceModels.Count());
            return typedAppearanceModels[randomModelIndex];
        }
    }
}
