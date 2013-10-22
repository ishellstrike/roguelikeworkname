using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace jarg {
    public class LineBatch : IDisposable {
        #region Members

        private readonly BasicEffect m_basicEffect;
        private readonly GraphicsDevice m_device;
        private readonly List<VertexPositionColor> m_vertices = new List<VertexPositionColor>();
        private bool m_isDisposed;

        #endregion

        #region Constructor

        /// <summary>
        /// </summary>
        /// <param name="graphicsDevice"></param>
        public LineBatch(GraphicsDevice graphicsDevice) {
            if (graphicsDevice == null) {
                throw new ArgumentNullException("graphicsDevice");
            }

            m_device = graphicsDevice;
            //m_vertexDeclaration = new Vert exDeclaration(graphicsDevice, );

            m_basicEffect = new BasicEffect(graphicsDevice);
            m_basicEffect.VertexColorEnabled = true;

            UpdateProjection(graphicsDevice);
        }

        public void UpdateProjection(GraphicsDevice graphicsDevice) {
            m_basicEffect.Projection = Matrix.CreateOrthographicOffCenter(
                0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height, 0, 0, 1);
        }

        #endregion

        #region Implementation

        /// <summary>
        /// </summary>
        public void Dispose() {
            if (!m_isDisposed) {
                m_isDisposed = true;

                if (m_basicEffect != null) {
                    m_basicEffect.Dispose();
                }

                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// </summary>
        public void Clear() {
            m_vertices.Clear();
        }

        /// <summary>
        /// </summary>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <param name="color1"></param>
        /// <param name="color2"></param>
        /// <param name="width"></param>
        public void AddLine(Vector2 vertex1, Vector2 vertex2, Color color, float width) {
            AddLine(vertex1, vertex2, color, color, width);
        }

        /// <summary>
        /// </summary>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <param name="color"></param>
        /// <param name="width"></param>
        public void AddLine(Vector2 vertex1, Vector2 vertex2, Color color1, Color color2, float width) {
            AddLine(vertex1, vertex2, color1, color2, width, width);
        }

        /// <summary>
        /// </summary>
        /// <param name="vertex1"></param>
        /// <param name="vertex2"></param>
        /// <param name="color"></param>
        /// <param name="width"></param>
        public void AddLine(Vector2 vertex1, Vector2 vertex2, Color color1, Color color2, float width1, float width2) {
            Vector2 normal = vertex2 - vertex1;
            normal.Normalize();
            normal = new Vector2(normal.Y, -normal.X);

            var v1 = new Vector3(vertex1 - 0.5f*width1*normal, 0);
            var v2 = new Vector3(vertex1 + 0.5f*width1*normal, 0);
            var v3 = new Vector3(vertex2 - 0.5f*width2*normal, 0);
            var v4 = new Vector3(vertex2 + 0.5f*width2*normal, 0);

            m_vertices.Add(new VertexPositionColor(v1, color1));
            m_vertices.Add(new VertexPositionColor(v2, color1));
            m_vertices.Add(new VertexPositionColor(v3, color2));

            m_vertices.Add(new VertexPositionColor(v3, color2));
            m_vertices.Add(new VertexPositionColor(v2, color1));
            m_vertices.Add(new VertexPositionColor(v4, color2));
        }

        /// <summary>
        /// </summary>
        public void Draw() {
            //m_device.VertexDeclaration = m_vertexDeclaration;


            //m_basicEffect.Begin();
            //m_basicEffect.CurrentTechnique.Passes[0].Begin();

            int primitiveCount = m_vertices.Count/3;

            foreach (EffectPass pass in m_basicEffect.CurrentTechnique.Passes) {
                pass.Apply();

                if (m_vertices.Count > 0) {
                    m_device.DrawUserPrimitives(PrimitiveType.TriangleList,
                                                m_vertices.ToArray(), 0, primitiveCount);
                }
            }
        }

        #endregion
    }
}