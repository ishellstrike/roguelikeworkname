using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace rglikeworknamelib
{
    public class Camera {
        public Camera(float aspectRation, Vector3 lookAt)
            : this(aspectRation, MathHelper.PiOver4, lookAt, 0.1f, float.MaxValue) { }
 
        public Camera(float aspectRatio, float fieldOfView, Vector3 lookAt, float nearPlane, float farPlane)
        {
            this.aspectRatio = aspectRatio;
            this.fieldOfView = fieldOfView;           
            this.lookAt = lookAt;
            this.nearPlane = nearPlane;
            this.farPlane = farPlane;
        }

        internal Vector3 Up;
        public BoundingFrustum Bounding;

        /// <summary>
        /// Recreates our view matrix, then signals that the view matrix
        /// is clean.
        /// </summary>
        private void ReCreateViewMatrix() {
            Matrix rot = Matrix.CreateFromYawPitchRoll(0, pitch, 0) * Matrix.CreateRotationZ(yaw - (float)Math.PI);
            Forward = Vector3.Transform(Vector3.Up, rot);
            Backward = Vector3.Transform(Vector3.Down, rot);
            Left = Vector3.Transform(Vector3.Left, rot);
            Right = Vector3.Transform(Vector3.Right, rot);
            Up = Vector3.Transform(Vector3.Backward, rot);


            position = Vector3.Transform(Vector3.Backward, rot);

            position *= zoom;
            position += lookAt;
 
            //Calculate a new viewmatrix
            viewMatrix = Matrix.CreateLookAt(position, lookAt, Vector3.Backward);
            viewMatrixDirty = false;

            Bounding = new BoundingFrustum(ViewMatrix*ProjectionMatrix);
        }

        public override string ToString() {
            return String.Format("(y:{0}, p:{1})", yaw, pitch);
        }

        /// <summary>
        /// Recreates our projection matrix, then signals that the projection
        /// matrix is clean.
        /// </summary>
        private void ReCreateProjectionMatrix()
        {
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, AspectRatio, nearPlane , farPlane);
            projectionMatrixDirty = false;
        }

 
        /// <summary>
        /// Moves the camera and lookAt at to the right,
        /// as seen from the camera, while keeping the same height
        /// </summary>       
        public void MoveCameraRight(float amount)
        {
            Vector3 right = Vector3.Normalize(LookAt - Position); //calculate forward
            right = Vector3.Cross(right, Vector3.Up); //calculate the real right
            right.Y = 0;
            right.Normalize();
            LookAt += right * amount;
        }
 
        /// <summary>
        /// Moves the camera and lookAt forward,
        /// as seen from the camera, while keeping the same height
        /// </summary>       
        public void MoveCameraForward(float amount)
        {
            Vector3 forward = Vector3.Normalize(LookAt - Position);
            forward.Y = 0;
            forward.Normalize();
            LookAt += forward * amount;
        }

        public Vector3 Forward, Backward, Left, Right;


        private bool viewMatrixDirty = true;
        private bool projectionMatrixDirty = true;
 
        public float MinPitch = 0.001f;
        public float MaxPitch = MathHelper.PiOver2 - 0.3f;
        private float pitch = 0.001f;
        public float Pitch
        {
            get { return pitch; }
            set
            {
                viewMatrixDirty = true;
                pitch = MathHelper.Clamp(value, MinPitch, MaxPitch);              
            }
        }
 
        private float yaw;
        public float Yaw
        {
            get { return yaw; }
            set
            {
                viewMatrixDirty = true;
                yaw = value;
            }
        }
 
        private float fieldOfView;
        public float FieldOfView
        {
            get { return fieldOfView; }
            set
            {
                projectionMatrixDirty = true;
                fieldOfView = value;
            }
        }
 
        private float aspectRatio;
        public float AspectRatio
        {
            get { return aspectRatio; }
            set
            {
                projectionMatrixDirty = true;
                aspectRatio = value;
            }
        }
 
        private float nearPlane;
        public float NearPlane
        {
            get { return nearPlane; }
            set
            {
                projectionMatrixDirty = true;
                nearPlane = value;
            }
        }
 
        private float farPlane;
        public float FarPlane
        {
            get { return farPlane; }
            set
            {
                projectionMatrixDirty = true;
                farPlane = value;
            }
        }
 
        public float MinZoom = 5;
        public float MaxZoom = 100;
        private float zoom = 30;
        public float Zoom
        {
            get { return zoom; }
            set
            {
                viewMatrixDirty = true;
                zoom = MathHelper.Clamp(value, MinZoom, MaxZoom);
            }
        }
         
         
        private Vector3 position;
        public Vector3 Position
        {
            get
            {
                if (viewMatrixDirty)
                {
                    ReCreateViewMatrix();
                }
                return position;
            }
        }
 
        private Vector3 lookAt;
        public Vector3 LookAt
        {
            get { return lookAt; }
            set
            {
                viewMatrixDirty = true;
                lookAt = value;
            }
        }    
        public Matrix ViewProjectionMatrix
        {
            get {return ViewMatrix * ProjectionMatrix; }
        }
 
        private Matrix viewMatrix;       
        public Matrix ViewMatrix
        {
            get
            {
                if (viewMatrixDirty)
                {
                    ReCreateViewMatrix();
                }
                return viewMatrix;
            }
        }
 
        private Matrix projectionMatrix;
        public Matrix ProjectionMatrix
        {
            get
            {
                if (projectionMatrixDirty)
                {
                    ReCreateProjectionMatrix();
                }
                return projectionMatrix;
            }
        }
    }
}
