using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InfiniteRPG.Data.Map
{
    public class MapCell
    {
        private readonly int[] tileRefs;

        public bool IsLayerTransition { get; protected set; }

        public MapCell(int[] tileRefs, bool isLayerTransition)
        {
            Contract.Requires(tileRefs != null);
            Contract.Requires(tileRefs.Length > 0);
            
            this.tileRefs = tileRefs;
            IsLayerTransition = isLayerTransition;
        }

        public void Draw(Vector2 position, SpriteBatch batch, Tileset tileset)
        {
            Contract.Requires(position != null);
            Contract.Requires(batch != null);
            Contract.Requires(tileset != null);

            var depth = 1f;
            foreach (var tile in tileRefs)
            {
                if (tile != 0)
                {
                    tileset.DrawTile(position, tile, batch, depth);
                }
                depth -= .1f;
            }
        }
    }
}
