using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

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

    }

    public enum MapLayer
    {
        Background,

    }
}
