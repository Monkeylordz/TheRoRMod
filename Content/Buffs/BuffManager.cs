using Microsoft.Xna.Framework;
using RoRMod.Content.Common;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace RoRMod.Content.Buffs
{
    internal class BuffManager
    {
		private const float IconSpacing = 32;

		private readonly Dictionary<Type, Buff> buffDict = new Dictionary<Type, Buff>();

		private RoRClientConfig clientConfig;

		public BuffManager()
		{
			clientConfig = ModContent.GetInstance<RoRClientConfig>();
		}

		/// <summary>
		/// Adds a Buff.
		/// </summary>
		/// <typeparam name="T">The Buff type.</typeparam>
		/// <param name="buff">The Buff to add.</param>
        public void AddBuff<T>(T buff) where T : Buff
        {
			Type t = typeof(T);

			if (buffDict.ContainsKey(t))
            {
                buffDict[t] = buff;
            }
            else
            {
                buffDict.Add(t, buff);
            }
        }

		/// <summary>
		/// Determines if the Buff of the associated type exists.
		/// </summary>
		/// <typeparam name="T">The Buff type.</typeparam>
		/// <returns>True if the Buff exists; otherwise false.</returns>
        public bool HasBuff<T>() where T : Buff
		{
			return buffDict.ContainsKey(typeof(T));
		}

		/// <summary>
		/// Get the Buff
		/// </summary>
		/// <typeparam name="T">The Buff type.</typeparam>
		/// <returns>The Buff of the associated type.</returns>
		public T GetBuff<T>() where T : Buff
		{
			return buffDict[typeof(T)] as T;
		}

		/// <summary>
		/// Gets the Buff of the associated type.
		/// </summary>
		/// <typeparam name="T">The Buff type.</typeparam>
		/// <param name="buff">When this method returns, if the Buff of the associated type exists, contains that Buff, otherwise contains null.</param>
		/// <returns>True if the Buff of the associated type exists; otherwise false.</returns>
		public bool TryGetBuff<T>(out T buff) where T : Buff
		{
			if (HasBuff<T>())
			{
				buff = GetBuff<T>();
				return true;
			}
			buff = null;
			return false;
		}

		/// <summary>
		/// Clear a single buff.
		/// </summary>
		/// <typeparam name="T">The Buff type.</typeparam>
		public void ClearBuff<T>() where T : Buff
		{
			if (HasBuff<T>())
			{
				buffDict[typeof(T)].Clear();
				buffDict.Remove(typeof(T));
			}
		}

		/// <summary>
		/// Update all buffs.
		/// </summary>
		public void Update()
		{
			foreach (Buff buff in buffDict.Values)
			{
				buff.Update();
				if (!buff.Active)
				{
					buffDict[buff.GetType()].Clear();
					buffDict.Remove(buff.GetType());
				}
			}
		}

		/// <summary>
		/// Draw the icons of all active buffs.
		/// </summary>
		/// <param name="position">The center position of the row of icons.</param>
		public void Draw(Vector2 position)
		{
			if (clientConfig.ShowBuffIcons)
            {
				// Draw all active buffs in a row centered on the position
				int i = 0;
				foreach (Buff buff in buffDict.Values)
				{
					Vector2 drawPosition = position;
					drawPosition.X += (i - buffDict.Values.Count / 2) * IconSpacing;
					if (buffDict.Values.Count % 2 == 0)
					{
						drawPosition.X += IconSpacing / 2;
					}
					buff.Draw(drawPosition);
					i++;
				}
			}
		}

		/// <summary>
		/// Clear all buffs.
		/// </summary>
		public void Clear()
		{
			foreach (Buff buff in buffDict.Values)
			{
				buff.Clear();
			}
			buffDict.Clear();
		}
	}
}
