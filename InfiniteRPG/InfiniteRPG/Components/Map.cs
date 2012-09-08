using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace InfiniteRPG.Components
{
    class Map
    {
    }

    class MapSection
    {
        
    }

    class MapTile
    {
        public static readonly int LayerCount = Enum.GetValues(typeof (TileLayers)).Length;

        public MapSection Section { get; protected set; }
        public Vector2 Location { get; protected set; }

        public int[] LayerContents { get; protected set; }

        MapTile(MapSection owner, Vector2 location)
        {
            Section = owner;
            Location = location;
            LayerContents = new int[LayerCount];
        }
    }

    enum TileLayers 
    {
        Background,
        Foreground,
        Overlay,
    }
}
