using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InfiniteRPG.Data.Map
{
    /// <summary>
    /// Defines a section of a larger map
    /// </summary>
    public class MapSection
    {
        public int Width { get; protected set; }
        public int Height { get; protected set; }
        public Vector2 WorldLocation { get; set; }
        public Tileset Tileset { get; set; }

        private MapCell[] cells;

        public MapSection(MapCell[] cells, int width, int height, Vector2 worldLocation = default(Vector2), Tileset tileset = null)
        {
            Contract.Requires(cells != null);
            Contract.Requires(cells.Length > 0);
            Contract.Requires(width + height > 2); // Both must be > 0
            Contract.Requires(worldLocation != null);
            Contract.Requires(tileset != null);

            this.cells = cells;
            Width = width;
            Height = height;
            WorldLocation = worldLocation;
            Tileset = tileset;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Contract.Requires(spriteBatch != null);

            var x = 0;
            var y = 0;
            foreach (var cell in cells)
            {
                var tilePos = Vector2.Add(WorldLocation, new Vector2(x * Tileset.TileWidth, y * Tileset.TileHeight));
                cell.Draw(tilePos, spriteBatch, Tileset);
                x++;
                if (x % Width != 0) continue;
                x = 0;
                y++;
            }
        }
    }

    
}
