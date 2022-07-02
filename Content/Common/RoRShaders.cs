using System;
using System.Collections.Generic;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using RoRMod.Utilities;

namespace RoRMod.Content.Common
{
    public class RoRShaders : ModSystem
    {
        public enum ShaderID
        {
            FissureEffect2x2,
        }

        private const string PATH = "RoRMod/Assets/Effects/";

        private static readonly Dictionary<ShaderID, Asset<Effect>> shaders = new Dictionary<ShaderID, Asset<Effect>>();

        public override void Load()
        {
            if (PlayerUtils.IsClient())
            {
                foreach (ShaderID id in Enum.GetValues(typeof(ShaderID)))
                {
                    AddShader(id);
                }
            }
        }

        public override void Unload()
        {
            if (PlayerUtils.IsClient())
            {
                shaders.Clear();
            }
        }

        private static void AddShader(ShaderID id)
        {
            shaders.Add(id, ModContent.Request<Effect>(PATH + Enum.GetName(id)));
        }

        public static Effect GetShader(ShaderID id)
        {
            return shaders[id].Value;
        }
    }
}
