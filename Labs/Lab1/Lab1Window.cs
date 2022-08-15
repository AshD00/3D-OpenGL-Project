using System;
using Labs.Utility;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Labs.Lab1
{
    public class Lab1Window : GameWindow
    {
        private int[] mVertexBufferObjectIDArray = new int[2];
        private ShaderUtility mShader;
        /* House
         * uint[] indices = new uint[] { 0, 2, 1,
                                      1, 2, 3,
                                      3, 2, 4,
                                      5, 7, 6,
                                      7, 5, 8,
                                      9, 11,10,
                                      11,12,10,
                                      10,15,13,
                                      15,14,13,
                                      16,19,17,
                                      17,19,18,
                                      19,20,21,
                                      1, 21,20 }; 
        Fan test:
        uint[] indices = new uint[] { 0,4,3,
                                      2,
                                      1 };
        Strip test:
        uint[] indices = new uint[] { 4,3,0,
                                      2,
                                      1
        };*/

        uint[] indices = new uint[] { 0, 2, 1,
                                      4,
                                      3,
                                      7, 6, 5,
                                      8,
                                      9, 11,10,
                                      12,
                                      10,15,13,
                                      14,
                                      16,19,17,
                                      18,
                                      19,20,21,
                                      1
                                      
                                      
        };

        public Lab1Window()
            : base(
                800, // Width
                600, // Height
                GraphicsMode.Default,
                "Lab 1 Hello, Triangle",
                GameWindowFlags.Default,
                DisplayDevice.Default,
                3, // major
                3, // minor
                GraphicsContextFlags.ForwardCompatible
                )
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            GL.Enable(EnableCap.CullFace);
            GL.ClearColor(Color4.CornflowerBlue);

            /*float[] vertices = new float[] { 0.8f, 0.8f,
                                             -0.8f, 0.8f,
                                             -0.8f, -0.8f,
                                              0.8f, 0.8f,
                                             -0.8f, -0.8f,
                                              0.8f, -0.8f};*/

            /*float[] vertices = new float[]  { -0.4f, 0.0f,
                                               0.4f, 0.0f,
                                               0.0f, 0.6f,
                                              -0.8f,-0.6f,
                                               0.0f,-0.6f,
                                               0.8f,-0.6f};*/


            //House:
            float[] vertices = new float[]  { -0.8f,  0.2f, //0
                                              -0.4f,  0.6f,
                                              -0.4f,  0.2f,
                                               0.4f,  0.6f,
                                               0.8f,  0.2f,
                                              -0.2f,  0.6f, //5
                                              -0.2f,  0.8f,
                                               0f,    0.8f,
                                               0f,    0.6f,
                                               0.6f,  0.2f,
                                               0.6f, -0.2f, //10
                                               0,     0.2f,
                                               0,    -0.2f,
                                               0.6f, -0.6f,
                                               0.4f, -0.6f,
                                               0.4f, -0.2f, //15
                                               0.2f, -0.2f,
                                               0.2f, -0.6f,
                                              -0.6f, -0.6f,
                                              -0.6f, -0.2f,
                                              -0.4f, -0.2f, //20
                                              -0.6f,  0.2f };

            /*float[] vertices = new float[] { 0.0f, 0.8f,
                                             0.8f, 0.4f,
                                             0.6f, -0.6f,
                                            -0.6f, -0.6f,
                                            -0.8f, 0.4f};*/



            GL.GenBuffers(2, mVertexBufferObjectIDArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mVertexBufferObjectIDArray[0]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * sizeof(float)), vertices, BufferUsageHint.StaticDraw);

            int size;
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);

            if (vertices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Vertex data not loaded onto graphics card correctly");
            }

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mVertexBufferObjectIDArray[1]);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * sizeof(uint)), indices, BufferUsageHint.StaticDraw);

            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);

            if (indices.Length * sizeof(uint) != size)
            {
                throw new ApplicationException("Index data not loaded onto graphics card correctly");
            }

            #region Shader Loading Code - Can be ignored for now

            mShader = new ShaderUtility( @"Lab1/Shaders/vSimple.vert", @"Lab1/Shaders/fSimple.frag");

            #endregion

            base.OnLoad(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mVertexBufferObjectIDArray[0]);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mVertexBufferObjectIDArray[1]);

            // shader linking goes here
            #region Shader linking code - can be ignored for now

            GL.UseProgram(mShader.ShaderProgramID);
            int vPositionLocation = GL.GetAttribLocation(mShader.ShaderProgramID, "vPosition");
            GL.EnableVertexAttribArray(vPositionLocation);
            GL.VertexAttribPointer(vPositionLocation, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);

            #endregion

            //GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
            //GL.DrawElements(PrimitiveType.Triangles, 9, DrawElementsType.UnsignedInt, 0);
            //GL.DrawElements(PrimitiveType.TriangleFan, 5, DrawElementsType.UnsignedInt, 0);
            //GL.DrawElements(PrimitiveType.TriangleStrip, 6, DrawElementsType.UnsignedInt, 0);
            GL.DrawElements(PrimitiveType.TriangleStrip, 5, DrawElementsType.UnsignedInt, 0);
            GL.DrawElements(PrimitiveType.TriangleFan, 4, DrawElementsType.UnsignedInt, 5 * sizeof(uint));
            GL.DrawElements(PrimitiveType.TriangleStrip, 6, DrawElementsType.UnsignedInt, 7 * sizeof(uint));
            GL.DrawElements(PrimitiveType.TriangleStrip, 6, DrawElementsType.UnsignedInt, 11 * sizeof(uint));
            GL.DrawElements(PrimitiveType.TriangleStrip, 6, DrawElementsType.UnsignedInt, 15 * sizeof(uint));
            GL.DrawElements(PrimitiveType.TriangleStrip, 6, DrawElementsType.UnsignedInt, 19 * sizeof(uint));


            this.SwapBuffers();
        }

        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
            GL.DeleteBuffers(2, mVertexBufferObjectIDArray);
            GL.UseProgram(0);
            mShader.Delete();
        }
    }
}
