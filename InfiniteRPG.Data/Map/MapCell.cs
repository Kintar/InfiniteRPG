using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InfiniteRPG.Data.Map
{
    public class MapCell : IEnumerable<int>
    {
        private readonly int[] tileRefs;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<int> GetEnumerator()
        {
            foreach (var val in tileRefs) yield return val;
        }

        public bool IsLayerTransition
        {
            get { return tileRefs[(int)MapLayer.Stairs] > 1; }
        }

        public bool IsBlockedOnLayer(MapLayer layer)
        {
            return tileRefs[(int) layer] > 1;
        }

        public MapCell(int[] tileRefs)
        {
            Contract.Requires(tileRefs != null);
            Contract.Requires(tileRefs.Length > 0);
            
            this.tileRefs = tileRefs;
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
