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
        public Vector2 WorldLocation { get; protected set; }
        public Tileset Tileset { get; set; }

        private int[][] tileLayers;

        public MapSection(int[][] tileLayers, int width, int height, Vector2 worldLocation, Tileset tileset)
        {
            Contract.Requires(tileLayers != null);
            Contract.Requires(tileLayers.Length > 0);
            Contract.Requires(tileLayers[0].Length > 0);
            Contract.Requires(width + height > 2); // Both must be > 0
            Contract.Requires(worldLocation != null);
            Contract.Requires(tileset != null);

            this.tileLayers = tileLayers;
            Width = width;
            Height = height;
            WorldLocation = worldLocation;
            Tileset = tileset;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var layer in tileLayers)
            {
                var x = 0;
                var y = 0;
                foreach (var tile in layer)
                {
                    var tilePos = Vector2.Add(WorldLocation, new Vector2(x * Tileset.TileWidth, y * Tileset.TileHeight));
                    x++;
                    if (x % Width == 0)
                    {
                        x = 0;
                        y++;
                    }
                    Tileset.DrawTile(tilePos, tile, spriteBatch);
                }
            }
        }

        public static MapSection FromTMXFile(string fileName, Vector2 location, Tileset tileset)
        {
            using (var stream = File.OpenRead(fileName))
            {
                var doc = XDocument.Load(stream);

                var mapNode = doc.Root;
                var layers = new List<int[]>();

                var width = int.Parse(mapNode.Attributes().First(x => x.Name == "width").Value);
                var height = int.Parse(mapNode.Attributes().First(x => x.Name == "height").Value);
                foreach (var layer in mapNode.DescendantNodes().OfType<XElement>().Where(x => x.Name == "layer"))
                {
                    var data = layer.DescendantNodes().OfType<XElement>().First(x => x.Name == "data");
                    var layerData = data
                        .DescendantNodes()
                        .OfType<XElement>()
                        .Where(x => x.Name == "tile")
                        .Select(tile => int.Parse(tile.Attributes().First(x => x.Name == "gid").Value))
                        .ToArray();
                    layers.Add(layerData);
                }

                return new MapSection(layers.ToArray(), width, height, location, tileset);
            }
        }
    }

    
}
