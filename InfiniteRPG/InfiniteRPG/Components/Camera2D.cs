using InfiniteRPG.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InfiniteRPG.Components
{
    class Camera2D : GameComponent
    {
        readonly CameraMovementState movementState;

        float rotation;
        Vector2 position;
        float zoomLevel;

        public Viewport Viewport { get; set; }

        public float X
        {
            get { return position.X; }
        }

        public float Y
        {
            get { return position.Y; }
        }

        public float Zoom
        {
            get { return zoomLevel; }
        }

        /// <summary>
        /// Radians per second
        /// </summary>
        public float RotationSpeed { get; set; }

        /// <summary>
        /// Percent per second
        /// </summary>
        public float ZoomSpeed { get; set; }

        /// <summary>
        /// Currently arbitrary
        /// </summary>
        public float MovementSpeed { get; set; }

        public Camera2D(Game game, CameraMovementState movementState) : base(game)
        {
            this.movementState = movementState;
        }

        public override void Initialize()
        {
            rotation = 0;
            zoomLevel = 1;
            position = new Vector2(0, 0);

            RotationSpeed = MathHelper.ToRadians(45);
            ZoomSpeed = 0.25f;
            MovementSpeed = 5;

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            var increment = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (increment.Equals(0f)) return;

            var movex = 0;
            var movey = 0;

            if (movementState.IsSet(CameraMovement.ZoomOut))
                zoomLevel -= ZoomSpeed * increment;

            if (movementState.IsSet(CameraMovement.ZoomIn))
                zoomLevel += ZoomSpeed * increment;

            if (movementState.IsSet(CameraMovement.Down))
                movey--;

            if (movementState.IsSet(CameraMovement.Up))
                movey++;

            if (movementState.IsSet(CameraMovement.Left))
                movex++;

            if (movementState.IsSet(CameraMovement.Right))
                movex--;

            if (movementState.IsSet(CameraMovement.RotateCW))
                rotation += RotationSpeed * increment;

            if (movementState.IsSet(CameraMovement.RotateCCW))
                rotation -= RotationSpeed * increment;

            // Create the normalized movement vector
            var movementVector = new Vector2(movex, movey);

            if (movex != 0 || movey != 0)
            {
                // Scale the movement vector according to our current zoom level and speed
                movementVector = Vector2.Multiply(movementVector, MovementSpeed * zoomLevel);
            }

            // Add our movement vector to the camera's current position
            position += movementVector;
        }

        public Matrix GetTransformMatrix()
        {
            return
                Matrix.CreateTranslation(position.X - (Viewport.Width * 0.5f), position.Y - (Viewport.Width * 0.5f), 0) *
                Matrix.CreateScale(zoomLevel) *
                Matrix.CreateTranslation(Viewport.Width * 0.5f, Viewport.Height * 0.5f, 0);
        }
    }
}
