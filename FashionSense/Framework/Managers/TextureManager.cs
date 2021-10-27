﻿using FashionSense.Framework.Models;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionSense.Framework.Managers
{
    class TextureManager
    {
        private IMonitor _monitor;
        private List<AppearanceModel> _appearanceTextures;

        public TextureManager(IMonitor monitor, IModHelper helper)
        {
            _monitor = monitor;
            _appearanceTextures = new List<AppearanceModel>();
        }

        public void Reset()
        {
            _appearanceTextures.Clear();
        }

        public void AddAppearanceModel(AppearanceModel model)
        {
            if (_appearanceTextures.Any(t => t.Id == model.Id))
            {
                var replacementIndex = _appearanceTextures.IndexOf(_appearanceTextures.First(t => t.Id == model.Id));
                _appearanceTextures[replacementIndex] = model;
            }
            else
            {
                _appearanceTextures.Add(model);
            }
        }

        public List<AppearanceModel> GetAllAppearanceModels()
        {
            return _appearanceTextures;
        }

        public AppearanceModel GetSpecificAppearanceModel(string appearanceId)
        {
            return _appearanceTextures.FirstOrDefault(t => String.Equals(t.Id, appearanceId, StringComparison.OrdinalIgnoreCase));
        }
    }
}
