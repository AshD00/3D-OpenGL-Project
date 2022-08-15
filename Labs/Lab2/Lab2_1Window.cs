using Labs.Utility;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;

namespace Labs.Lab2
{
    class Lab2_1Window : GameWindow
    {        
        private int[] mTriangleVertexBufferObjectIDArray = new int[2];
        private int[] mSquareVertexBufferObjectIDArray = new int[2];
        private ShaderUtility mShader;
        private int[] mVertexArrayObjectIDs = new int[2];

        public Lab2_1Window()
            : base(
                800, // Width
                600, // Height
                GraphicsMode.Default,
                "Lab 2_1 Linking to Shaders and VAOs",
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
            GL.Enable(EnableCap.DepthTest);
            GL.ClearColor(Color4.CadetBlue);

            float[] tVertices = new float[] { -0.8f,  0.8f, 0.4f, 0.0f, 0.0f, 0.8f,
                                              -0.6f, -0.4f, 0.4f, 1.0f, 1.0f, 0.2f,
                                               0.2f,  0.2f, 0.4f, 0.6f, 0.0f, 1.0f};

            uint[] tIndices = new uint[] { 0, 1, 2 };

            float[] sVertices = new float[] { -0.2f, -0.4f, 0.2f,  1.0f,  0.2f,  1.0f,
                                               0.8f, -0.4f, 0.2f, -1.0f, -1.0f, -0.4f,
                                               0.8f,  0.6f, 0.2f, -0.6f,  0.8f,  0.8f,
                                              -0.2f,  0.6f, 0.2f,  0.8f,  1.0f,  0.4f};

            uint[] sIndices = new uint[] { 0, 1, 2, 3 };


            GL.GenBuffers(2, mTriangleVertexBufferObjectIDArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mTriangleVertexBufferObjectIDArray[0]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(tVertices.Length * sizeof(float)), tVertices, BufferUsageHint.StaticDraw);

            int size;
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);

            if (tVertices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Vertex data not loaded onto graphics card correctly");
            }

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mTriangleVertexBufferObjectIDArray[1]);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(tIndices.Length * sizeof(int)), tIndices, BufferUsageHint.StaticDraw);

            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);

            if (tIndices.Length * sizeof(int) != size)
            {
                throw new ApplicationException("Index data not loaded onto graphics card correctly");
            }

            GL.GenBuffers(2, mSquareVertexBufferObjectIDArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mSquareVertexBufferObjectIDArray[0]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(sVertices.Length * sizeof(float)), sVertices, BufferUsageHint.StaticDraw);

            int ssize;
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out ssize);

            if (sVertices.Length * sizeof(float) != ssize)
            {
                throw new ApplicationException("Vertex data not loaded onto graphics card correctly");
            }


            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mSquareVertexBufferObjectIDArray[1]);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(sIndices.Length * sizeof(int)), sIndices, BufferUsageHint.StaticDraw);

            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out ssize);

            if (sIndices.Length * sizeof(float) != ssize)
            {
                throw new ApplicationException("Vertex data not loaded onto graphics card correctly");
            }

            #region Shader Loading Code

            mShader = new ShaderUtility(@"Lab2/Shaders/vLab21.vert", @"Lab2/Shaders/fSimple.frag");

            int vColourLocation = GL.GetAttribLocation(mShader.ShaderProgramID, "vColour");
            GL.EnableVertexAttribArray(vColourLocation);

            int vPositionLocation = GL.GetAttribLocation(mShader.ShaderProgramID, "vPosition");
            GL.EnableVertexAttribArray(vPositionLocation);

            #endregion


            GL.GenVertexArrays(2, mVertexArrayObjectIDs);
            
            GL.BindVertexArray(mVertexArrayObjectIDs[0]);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mTriangleVertexBufferObjectIDArray[0]);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mTriangleVertexBufferObjectIDArray[1]);
            GL.VertexAttribPointer(vPositionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.VertexAttribPointer(vColourLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(vColourLocation);
            GL.EnableVertexAttribArray(vPositionLocation);


            GL.BindVertexArray(mVertexArrayObjectIDs[1]);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mSquareVertexBufferObjectIDArray[0]);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mSquareVertexBufferObjectIDArray[1]);
            GL.VertexAttribPointer(vPositionLocation, 4, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.VertexAttribPointer(vColourLocation, 4, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(vColourLocation);
            GL.EnableVertexAttribArray(vPositionLocation);


            base.OnLoad(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            //int vColourLocation = GL.GetAttribLocation(mShader.ShaderProgramID, "vColour");
            //int vPositionLocation = GL.GetAttribLocation(mShader.ShaderProgramID, "vPosition");
            //GL.EnableVertexAttribArray(vPositionLocation);

            //base.OnRenderFrame(e);
            //GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


            #region Shader Loading Code

            GL.UseProgram(mShader.ShaderProgramID);
            //GL.EnableVertexAttribArray(vPositionLocation);
            //GL.VertexAttribPointer(vPositionLocation, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);

            #endregion

            //int uColourLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uColour");

            //GL.Uniform4(uColourLocation, Color4.Blue);
            //GL.BindBuffer(BufferTarget.ArrayBuffer, mSquareVertexBufferObjectIDArray[0]);
            //GL.BindBuffer(BufferTarget.ElementArrayBuffer, mSquareVertexBufferObjectIDArray[1]);
            //GL.VertexAttribPointer(vPositionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            //GL.VertexAttribPointer(vColourLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float)); 
            //GL.DrawElements(PrimitiveType.TriangleFan, 4, DrawElementsType.UnsignedInt, 0);

            //GL.BindBuffer(BufferTarget.ArrayBuffer, mTriangleVertexBufferObjectIDArray[0]);
            //GL.BindBuffer(BufferTarget.ElementArrayBuffer, mTriangleVertexBufferObjectIDArray[1]);
            //GL.VertexAttribPointer(vPositionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            //GL.VertexAttribPointer(vColourLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            //GL.Uniform4(uColourLocation, Color4.Red);
            //GL.DrawElements(PrimitiveType.Triangles, 3, DrawElementsType.UnsignedInt, 0);

            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.BindVertexArray(mVertexArrayObjectIDs[1]);
            GL.DrawElements(PrimitiveType.TriangleFan, 4, DrawElementsType.UnsignedInt, 0);

            GL.BindVertexArray(mVertexArrayObjectIDs[0]);
            GL.DrawElements(PrimitiveType.Triangles, 3, DrawElementsType.UnsignedInt, 0);

            GL.BindVertexArray(0);


            this.SwapBuffers();
        }

        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
            GL.DeleteBuffers(2, mTriangleVertexBufferObjectIDArray);
            GL.DeleteBuffers(2, mSquareVertexBufferObjectIDArray);
            GL.BindVertexArray(0);
            GL.DeleteVertexArrays(2, mVertexArrayObjectIDs);
            GL.UseProgram(0);
            mShader.Delete();
        }
    }
}
