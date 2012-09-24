using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InfiniteRPG.Data.Sprites
{
    public enum CharacterState
    {
        Moving,
        Idle
    }

    public enum CharacterFacing
    {
        South,
        West,
        East,
        North,
    }

    public class CharacterSprite
    {
        public Vector2 Position { get; set; }
        public Texture2D Sprite { get; set; }
        public CharacterState State { get; set; }
        public CharacterFacing Facing { get; set; }

        public CharacterSprite(Texture2D sprite)
        {
            Sprite = sprite;
            Position = Vector2.Zero;
            State = CharacterState.Idle;
            Facing = CharacterFacing.South;
        }
    }
}
