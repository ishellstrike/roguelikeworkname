using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace rglikeworknamelib
{
    public class Camera {
        public Matrix Projection;
        public Matrix View;

        public Vector3 Position;
        public Vector3 Target;
        public float Cameradistance = 30;
        public Vector3 Translation;
        public Quaternion rot = Quaternion.Identity;
        public Vector3 Front;
        public Vector3 Back;
        public Vector3 Left;
        public Vector3 Right;
        public Vector3 Up;

        public void Update(Vector3 moving, GameTime gt) {

            Position = Vector3.Transform(new Vector3(0, 0, Cameradistance), rot);
            Position += Target;

            Position = Vector3.Transform(Position - Target, rot) + Target;

            Up = Vector3.Transform(Vector3.Up, rot);
            View = Matrix.CreateLookAt(Position, Target, Up);

            Front = Vector3.Transform(Vector3.Forward, rot);
            Back = Vector3.Transform(Vector3.Backward, rot);
            Left = Vector3.Transform(Vector3.Left, rot);
            Right = Vector3.Transform(Vector3.Right, rot);

           // generateFrustum();
            Matrix invv = Matrix.Invert(View);
            Position.X = invv.M41;
            Position.Y = invv.M42;
            Position.Z = invv.M43;
        }


        public Camera(Vector3 position, GraphicsDevice graphicsDevice)
        {
            Position = position;

            Translation = Vector3.Zero;
            GraphicsDevice = graphicsDevice;
            GeneratePerspectiveProjectionMatrix(MathHelper.ToRadians(45));
        }

        protected GraphicsDevice GraphicsDevice;
        public Matrix Rotation;

        public void GeneratePerspectiveProjectionMatrix(float FieldOfView)
        {
            //PresentationParameters pp = GraphicsDevice.PresentationParameters;

            float aspectRatio = Settings.Resolution.X / Settings.Resolution.Y;

            Projection = Matrix.CreatePerspectiveFieldOfView(FieldOfView, aspectRatio, 0.1f, 1000000.0f);
        }
    }
}
