using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace RoRMod.Content.Buffs
{
    internal abstract class Buff
    {
		/// <summary>
		/// Shorthand Entity with the buff.
		/// </summary>
		protected Entity Entity { get; private set; }

		/// <summary>
		/// If the buff is applying its effect.
		/// </summary>
		public abstract bool Active { get; }

		/// <summary>
		/// The path to this buff's texture
		/// </summary>
		protected virtual string TextureName => "RoRMod/Content/Buffs/" + GetType().Name;

		/// <summary>
		/// The texture
		/// </summary>
		protected Asset<Texture2D> textureAsset;

		/// <summary>
		/// Static Dictionary to store textures
		/// </summary>
		protected static readonly Dictionary<string, Asset<Texture2D>> textures = new Dictionary<string, Asset<Texture2D>>();

		/// <summary>
		/// A Buff for an IBuffable.
		/// </summary>
		/// <param name="buffable"></param>
		public Buff(Entity buffedEntity)
		{
			Entity = buffedEntity;

			// Get texture
			if (!textures.TryGetValue(TextureName, out textureAsset))
            {
				textureAsset = ModContent.Request<Texture2D>(TextureName);
				textures[TextureName] = textureAsset;
			}
		}

		/// <summary>
		/// Updates the buff.
		/// </summary>
		public virtual void Update() { }

		/// <summary>
		/// Draws the buff icon centered at position.
		/// </summary>
		/// <param name="position"></param>
		public virtual void Draw(Vector2 position)
		{
			Texture2D texture = textureAsset.Value;
			position -= texture.Size() / 2;
			float scale = 0.75f;
			Main.EntitySpriteDraw(texture, position - Main.screenPosition, null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
		}

		/// <summary>
		/// Clears the buff.
		/// </summary>
		public virtual void Clear() { }
	}
}
