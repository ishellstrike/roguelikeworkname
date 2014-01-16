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
        public float Yaw;
        public Vector3 Target;
        public float Cameradistance = 30;
        public Vector3 Translation;

        public float Pitch;
        public float Roll;

        public void Update(Vector3 moving, GameTime gt) {
            var an = MathHelper.ToRadians(Yaw);
            Position.X = Target.X;
            Position.Y = Target.Y;
            Position.Z = Cameradistance;

            Target += moving;
            Vector3 newPosition = Position - Target;

            // Вычисляем новое местоположение камеры,
            // вращая ее вокруг осей

            Rotation = Matrix.CreateFromYawPitchRoll(0, MathHelper.ToRadians(Pitch), 0);

            newPosition = Vector3.Transform(newPosition, Rotation);
            View = Matrix.CreateLookAt(newPosition + Target, Target, Vector3.Up);
           // generateFrustum();
            Matrix invv = Matrix.Invert(View);
            Position.X = invv.M41;
            Position.Y = invv.M42;
            Position.Z = invv.M43;
        }

        /// <summary>
        /// Вращение камеры вокруг объекта
        /// </summary>
        /// <param name="cameraTarget">Координаты объекта</param>
        /// <param name="cameraRotationX">Вращение вокруг X</param>
        /// <param name="cameraRotationY">Вращение вокруг Y</param>
        /// <param name="cameraTargetDistance">Расстояние от камеры до объекта</param>
        /// <returns></returns>
        public static Matrix BuildViewMatrix(Vector3 cameraTarget, float cameraRotationX, float cameraRotationY,
                                             float cameraRotationZ, float cameraTargetDistance)
        {
            //матрица вращений
            Matrix cameraRot = Matrix.CreateRotationX(cameraRotationX)
                               * Matrix.CreateRotationY(cameraRotationY) * Matrix.CreateRotationZ(cameraRotationZ);

            //вычисляем куда смотреть
            var cameraOriginalTarget = new Vector3(0, 0, -1);
            var cameraOriginalUpVector = new Vector3(0, 1, 0);

            Vector3 cameraRotatedTarget = Vector3.Transform(cameraOriginalTarget, cameraRot);
            Vector3 cameraFinalTarget = cameraTarget + cameraRotatedTarget;
            Vector3 cameraRotatedUpVector = Vector3.Transform(cameraOriginalUpVector, cameraRot);
            //Vector3 cameraFinalUpVector = cameraTarget + cameraRotatedUpVector;

            //смотрим на объект
            Matrix viewMatrix = Matrix.CreateLookAt(cameraTarget, cameraFinalTarget, cameraRotatedUpVector);

            //отдаляем камеру на нужное расстояние от объекта
            viewMatrix.Translation += new Vector3(0, 0, -cameraTargetDistance);

            return viewMatrix;
        }

        public Camera(Vector3 position, GraphicsDevice graphicsDevice)
        {
            Position = position;

            Translation = Vector3.Zero;
            GraphicsDevice = graphicsDevice;
            GeneratePerspectiveProjectionMatrix(MathHelper.ToRadians(45));
        }

        protected GraphicsDevice GraphicsDevice;
        private float maxChaseDistance = 30;
        private float minChaseDistance = 20;
        public Matrix Rotation;

        public void GeneratePerspectiveProjectionMatrix(float FieldOfView)
        {
            //PresentationParameters pp = GraphicsDevice.PresentationParameters;

            float aspectRatio = Settings.Resolution.X / Settings.Resolution.Y;

            Projection = Matrix.CreatePerspectiveFieldOfView(FieldOfView, aspectRatio, 0.1f, 1000000.0f);
        }
    }
}
