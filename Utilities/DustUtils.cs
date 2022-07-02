using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace RoRMod.Utilities
{
	internal static class DustUtils
    {
		/// <summary>
		/// Create the dust for an explosion.
		/// </summary>
		/// <param name="center">The center of the explosion</param>
		/// <param name="radius">The radius of the explosion</param>
		/// <param name="dustID">The DustID used for the explosion. Uses HeatRay as default.</param>
		public static void Explosion(Vector2 center, float radius, short dustID = DustID.HeatRay)
		{
			const float dustPerUnit = 0.1f;
			float dustAmount = radius * dustPerUnit;

			// Outer Ring
			for (int i = 0; i < dustAmount; i++)
            {
				Vector2 outer_offset = Main.rand.NextVector2CircularEdge(radius, radius);
				Dust dust = Dust.NewDustPerfect(center + outer_offset, dustID, Velocity: Vector2.Zero);
				dust.noGravity = true;
			}

			// Inner Dust
			for (int i = 0; i < dustAmount * MathHelper.Pi; i++)
			{
				Vector2 inner_offset = Main.rand.NextVector2Circular(radius, radius);
				Vector2 inner_velocity = inner_offset.SafeNormalize(Vector2.Zero);
				Dust dust = Dust.NewDustPerfect(center + inner_offset, dustID, Velocity: inner_velocity, Scale: 1.5f);
				dust.noGravity = true;
			}
		}

		/// <summary>
		/// Create the dust for a lightning bolt between two points.
		/// </summary>
		/// <param name="start">The first endpoint of the bolt.</param>
		/// <param name="end">The second endpoint of the bolt.</param>
		/// <param name="segmentSpacing">The average distance between segment points.</param>
		/// <param name="sway">Controls the amount of displacement for each segment.</param>
		public static void LightningBolt(Vector2 start, Vector2 end, float segmentSpacing = 12, float sway = 80)
        {			
			List<Vector2> vertices = CreateLightningBoltVertices(start, end, segmentSpacing, sway);

			for (int i = 1; i < vertices.Count; i++)
            {
				Vector2 A = vertices[i - 1];
				Vector2 B = vertices[i];

				// Dust at vertices
				Dust.NewDustPerfect(B, DustID.Electric).noGravity = true;

				// Dust along segment
				Vector2 segmentVector = B - A;
				float segmentLength = segmentVector.Length();
				for (int j = 0; j < segmentLength; j++)
                {
					Vector2 point = A + segmentVector * Main.rand.NextFloat();
					Dust.NewDustPerfect(point, DustID.Flare_Blue).noGravity = true;
				}
			}
		}

		/// <summary>
		/// Create a chain of lightning bolts between a list of locations
		/// </summary>
		/// <param name="locations"></param>
		public static void ChainLightning(List<Vector2> locations)
        {
			// Chain only with two or more locations
			if (locations.Count < 2)
			{
				return;
			}

			// With 2 locations, create one bolt
			if (locations.Count == 2)
			{
				LightningBolt(locations[0], locations[1]);
				return;
			}

			// Use MST to create chain
			var graph = WeightedGraph<Vector2, float>.FullyConnectedGraph(locations, Vector2.DistanceSquared);
			var edges = graph.MinimumSpanningTree();
			foreach (var edge in edges)
			{
				LightningBolt(edge.A, edge.B);
			}
		}

		/// <summary>
		/// Calculates the vertices for a lightning bolt
		/// </summary>
		/// <param name="start">The starting position of the bolt</param>
		/// <param name="end">The end position of the bolt</param>
		/// <param name="segmentSpacing">The average distance between vertices</param>
		/// <param name="sway">A modifier to the sideways displacement of the vertices</param>
		/// <returns>The vertices</returns>
		public static List<Vector2> CreateLightningBoltVertices(Vector2 start, Vector2 end, float segmentSpacing = 12, float sway = 80)
		{
			// Adapted from: https://gamedevelopment.tutsplus.com/tutorials/how-to-generate-shockingly-good-2d-lightning-effects--gamedev-2681
			List<Vector2> vertices = new List<Vector2>();

			// Calculate vectors and constants
			Vector2 boltVector = end - start;
			Vector2 boltNormal = new Vector2(boltVector.Y, -boltVector.X).SafeNormalize(Vector2.Zero);
			float boltLength = boltVector.Length();
			float jaggedness = sway != 0 ? 1 / sway : 0;

			// Split line into segments
			List<float> vertexLocations = new List<float>();
			vertexLocations.Add(0);
			for (int i = 0; i < boltLength / segmentSpacing; i++)
			{
				vertexLocations.Add(Main.rand.NextFloat());
			}
			vertexLocations.Add(1);
			vertexLocations.Sort();

			// Loop over endpoints and Add displacement to all points along the line and create segment list
			float prevDisplacement = 0;
			for (int i = 1; i < vertexLocations.Count; i++)
			{
				float vertexLocation = vertexLocations[i];

				// used to prevent sharp angles by ensuring very close positions also have small perpendicular variation.
				float scale = boltLength * jaggedness * (vertexLocation - vertexLocations[i - 1]);

				// defines an envelope. Points near the middle of the bolt can be further from the central line.
				float envelope = vertexLocation > 0.95f ? 20 * (1 - vertexLocation) : 1;

				float displacement = sway * Main.rand.NextFloat(-1, 1);
				displacement -= (displacement - prevDisplacement) * (1 - scale);
				displacement *= envelope;

				Vector2 point = start + vertexLocation * boltVector + displacement * boltNormal;

				vertices.Add(point);

				prevDisplacement = displacement;
			}

			return vertices;
		}
	}
}
