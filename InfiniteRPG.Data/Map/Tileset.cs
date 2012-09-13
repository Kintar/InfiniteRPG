using System.Diagnostics.Contracts;
using InfiniteRPG.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InfiniteRPG.Data.Map
{
    public class Tileset
    {
        public string Name { get; protected set; }
        public int TileWidth { get; protected set; }
        public int TileHeight { get; protected set; }
        public Texture2D Texture { get; protected set; }
        public int TileCount { get; protected set; }

        private readonly int tilesWide;
        private readonly int tilesHigh;

        public Tileset(string name, Texture2D texture, int tileWidth, int tileHeight)
        {
            Contract.Requires(!string.IsNullOrEmpty(name));
            Contract.Requires(texture != null);
            Contract.Requires(MathUtils.IsPowerOfTwo(texture.Width));
            Contract.Requires(MathUtils.IsPowerOfTwo(texture.Height));
            Contract.Requires(MathUtils.IsPowerOfTwo(tileWidth));
            Contract.Requires(MathUtils.IsPowerOfTwo(tileHeight));

            Name = name;
            Texture = texture;
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            tilesWide = texture.Width / tileWidth;
            tilesHigh = texture.Height / tileHeight;
            TileCount = tilesWide * tilesHigh;
        }

        public void DrawTile(Vector2 position, int tileNumber, SpriteBatch batch, float depth, Color? color = null)
        {
            Contract.Requires(batch != null);
            Contract.Requires(position != null);
            Contract.Requires(tileNumber > 0);
            Contract.Requires(tileNumber <= TileCount);

            var myColor = color.HasValue ? color.Value : Color.White;
            
            var ypos = ((tileNumber - 1) / tilesWide) * TileHeight;
            var xpos = ((tileNumber - 1) % tilesWide) * TileWidth;

            batch.Draw(Texture, position, new Rectangle(xpos, ypos, TileWidth, TileHeight), myColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, depth);
        }
    }
}
