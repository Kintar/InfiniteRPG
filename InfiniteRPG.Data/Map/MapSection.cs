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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace InfiniteRPG.Data.Map
{
    /// <summary>
    /// Defines a section of a larger map
    /// </summary>
    public class MapSection
    {
        public int Width { get; internal set; }
        public int Height { get; internal set; }
        public MapCell[] Cells { get; internal set; }

        public MapSection(MapCell[] cells, int width, int height)
        {
            Contract.Requires(cells != null);
            Contract.Requires(cells.Length > 0);
            Contract.Requires(width + height > 2); // Both must be > 0

            Cells = cells;
            Width = width;
            Height = height;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 location, Tileset tileset)
        {
            Contract.Requires(spriteBatch != null);
            Contract.Requires(tileset != null);

            var x = 0;
            var y = 0;
            foreach (var cell in Cells)
            {
                var tilePos = Vector2.Add(location, new Vector2(x * tileset.TileWidth, y * tileset.TileHeight));
                cell.Draw(tilePos, spriteBatch, tileset);
                x++;
                if (x % Width != 0) continue;
                x = 0;
                y++;
            }
        }
    }

    public class MapSectionReader : ContentTypeReader<MapSection>
    {
        protected override MapSection Read(ContentReader input, MapSection existingInstance)
        {
            var cells = new List<MapCell>();
            var width = input.ReadInt32();
            var height = input.ReadInt32();
            var layerCount = input.ReadInt32();

            if (Enum.GetValues(typeof(MapLayer)).Length != layerCount)
                throw new ContentLoadException("MapSection data was saved with a different version of the game!");

            var count = width*height;
            do
            {
                var refs = new int[layerCount];
                for (var i = 0; i < layerCount; i++)
                {
                    refs[i] = input.ReadInt32();
                }
                cells.Add(new MapCell(refs));
                count--;
            } while (count > 0);

            if (existingInstance != null)
            {
                existingInstance.Width = width;
                existingInstance.Height = height;
                existingInstance.Cells = cells.ToArray();
            }
            else
            {
                existingInstance = new MapSection(cells.ToArray(), width, height);
            }

            return existingInstance;
        }
    }
}
