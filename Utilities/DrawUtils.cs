using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics;
using Terraria.Graphics.Renderers;
using System;
using System.Collections.Generic;

namespace RoRMod.Utilities
{
    internal static class DrawUtils
    {
        /// <summary>
        /// Draws line stripe between vertices from a list
        /// </summary>
        /// <param name="vertices">The list of vertices in world space</param>
        /// <param name="color">The color of the line</param>
        public static void DrawLineStrip(IList<Vector2> verticesWorld, Color color = default)
        {
            GraphicsDevice device = Main.graphics.GraphicsDevice;
            VertexPositionColor[] vertices = WorldToVertexPositionColor(verticesWorld, color);
            VertexBuffer buffer = new VertexBuffer(device, typeof(VertexPositionColor), vertices.Length, BufferUsage.WriteOnly);
            buffer.SetData(vertices);
            device.SetVertexBuffer(buffer);
            BasicEffect shader = new BasicEffect(device);
            foreach (EffectPass effectPass in shader.CurrentTechnique.Passes)
            {
                // Apply Shader pass
                effectPass.Apply();

                // Draw primitives to form polygon
                device.DrawPrimitives(PrimitiveType.LineStrip, 0, vertices.Length - 1);
            }
        }

        /// <summary>
        /// Returns a vertex in screen space for a given world position
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        public static VertexPositionColor WorldToVertexPositionColor(Vector2 worldPosition, Color color = default)
        {
            VertexPositionColor vertex = new VertexPositionColor();
            Vector3 vertexPosition = new Vector3(worldPosition - Main.screenPosition, 0);
            vertex.Position = Vector3.Transform(vertexPosition, Main.GameViewMatrix.NormalizedTransformationmatrix);
            vertex.Color = color;
            return vertex;
        }

        /// <summary>
        /// Returns an array of vertices in screen space for a given list of world positions
        /// </summary>
        /// <param name="worldPositions"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static VertexPositionColor[] WorldToVertexPositionColor(IList<Vector2> worldPositions, Color color = default)
        {
            VertexPositionColor[] vertices = new VertexPositionColor[worldPositions.Count];
            for (int i = 0; i < worldPositions.Count; i++)
            {
                vertices[i] = WorldToVertexPositionColor(worldPositions[i], color);
            }
            return vertices;
        }
    }
}
