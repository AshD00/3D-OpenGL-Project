using Labs.Utility;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;

namespace Labs.Lab3
{
    public class Lab3Window : GameWindow
    {
        public Lab3Window()
            : base(
                800, // Width
                600, // Height
                GraphicsMode.Default,
                "Lab 3 Lighting and Material Properties",
                GameWindowFlags.Default,
                DisplayDevice.Default,
                3, // major
                3, // minor
                GraphicsContextFlags.ForwardCompatible
                )
        {
        }

        private int[] mVBO_IDs = new int[5];
        private int[] mVAO_IDs = new int[3];
        private ShaderUtility mShader;
        private ModelUtility mSphereModelUtility, mSurpriseModelUtility, mCylinderModelUtility;
        private Matrix4 mView, mSphereModel, mGroundModel, mSurpriseModel, mCylinderModel;

        protected override void OnLoad(EventArgs e)
        {
            // Set some GL state
            GL.ClearColor(Color4.Black);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);

            //mShader = new ShaderUtility(@"Lab3/Shaders/vLighting.vert", @"Lab3/Shaders/fPassThrough.frag");
            mShader = new ShaderUtility(@"Lab3/Shaders/vPassThrough.vert", @"Lab3/Shaders/fLighting.frag");
            GL.UseProgram(mShader.ShaderProgramID);
            
            int vPositionLocation = GL.GetAttribLocation(mShader.ShaderProgramID, "vPosition");
            int vNormalLocation = GL.GetAttribLocation(mShader.ShaderProgramID, "vNormal");

            GL.GenVertexArrays(mVAO_IDs.Length, mVAO_IDs);
            GL.GenBuffers(mVBO_IDs.Length, mVBO_IDs);

            float[] vertices = new float[] {-10, 0, -10,0,1,0,
                                             -10, 0, 10,0,1,0,
                                             10, 0, 10,0,1,0,
                                             10, 0, -10,0,1,0, };

            GL.BindVertexArray(mVAO_IDs[0]);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mVBO_IDs[0]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * sizeof(float)), vertices, BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(vPositionLocation);
            GL.VertexAttribPointer(vPositionLocation, 3, VertexAttribPointerType.Float, true, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(vNormalLocation);
            GL.VertexAttribPointer(vNormalLocation, 3, VertexAttribPointerType.Float, true, 6 * sizeof(float), 3*sizeof(float));


            int size;
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);
            if (vertices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Vertex data not loaded onto graphics card correctly");
            }

            GL.EnableVertexAttribArray(vPositionLocation);
            GL.VertexAttribPointer(vPositionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);

            mSphereModelUtility = ModelUtility.LoadModel(@"Utility/Models/sphere.bin");
            mSurpriseModelUtility = ModelUtility.LoadModel(@"Utility/Models/model.bin");
            mCylinderModelUtility = ModelUtility.LoadModel(@"Utility/Models/cylinder.bin");

            GL.BindVertexArray(mVAO_IDs[1]);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mVBO_IDs[1]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(mSurpriseModelUtility.Vertices.Length * sizeof(float)), mSurpriseModelUtility.Vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mVBO_IDs[2]);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(mSurpriseModelUtility.Indices.Length * sizeof(float)), mSurpriseModelUtility.Indices, BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(vPositionLocation);
            GL.VertexAttribPointer(vPositionLocation, 3, VertexAttribPointerType.Float, true, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(vNormalLocation);
            GL.VertexAttribPointer(vNormalLocation, 3, VertexAttribPointerType.Float, true, 6 * sizeof(float), 3 * sizeof(float));

            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);
            if (mSurpriseModelUtility.Vertices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Vertex data not loaded onto graphics card correctly");
            }

            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);
            if (mSurpriseModelUtility.Indices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Index data not loaded onto graphics card correctly");
            }
           

            GL.BindVertexArray(mVAO_IDs[2]);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mVBO_IDs[3]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(mCylinderModelUtility.Vertices.Length * sizeof(float)), mCylinderModelUtility.Vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mVBO_IDs[4]);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(mCylinderModelUtility.Indices.Length * sizeof(float)), mCylinderModelUtility.Indices, BufferUsageHint.StaticDraw);
            
            GL.EnableVertexAttribArray(vPositionLocation);
            GL.VertexAttribPointer(vPositionLocation, 3, VertexAttribPointerType.Float, true, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(vNormalLocation);
            GL.VertexAttribPointer(vNormalLocation, 3, VertexAttribPointerType.Float, true, 6 * sizeof(float), 3 * sizeof(float));

            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);
            if (mCylinderModelUtility.Vertices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Vertex data not loaded onto graphics card correctly");
            }

            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);
            if (mCylinderModelUtility.Indices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Index data not loaded onto graphics card correctly");
            }

            GL.EnableVertexAttribArray(vPositionLocation);
            GL.VertexAttribPointer(vPositionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);

            GL.BindVertexArray(0);

            mView = Matrix4.CreateTranslation(0, -1.5f, 0);
            int uView = GL.GetUniformLocation(mShader.ShaderProgramID, "uView");
            GL.UniformMatrix4(uView, true, ref mView);

            mGroundModel = Matrix4.CreateTranslation(0, 0, -5f);
            //mSphereModel = Matrix4.CreateTranslation(0, 1, -5f);
            mSurpriseModel = Matrix4.CreateTranslation(0, 3, -5f);
            mCylinderModel = Matrix4.CreateTranslation(0, 1, -5f);

            LightUpdateR();
            LightUpdateG();
            LightUpdateB();

            //MaterialUpdate();


            base.OnLoad(e);        
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(this.ClientRectangle);
            if (mShader != null)
            {
                int uProjectionLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uProjection");
                Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(1, (float)ClientRectangle.Width / ClientRectangle.Height, 0.5f, 25);
                GL.UniformMatrix4(uProjectionLocation, true, ref projection);
            }
        }
        
        protected void LightUpdateR()
        {
            int uLightPositionLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uLight[0].Position");
            Vector4 lightPosition = new Vector4(-2, 3, -8.5f, 1);
            lightPosition = Vector4.Transform(lightPosition, mView);
            GL.Uniform4(uLightPositionLocation, lightPosition);

            int uAmbientLightLocation = GL.GetUniformLocation(mShader.ShaderProgramID,"uLight[0].AmbientLight");
            Vector3 colour = new Vector3(1.0f, 0.0f, 0.0f);
            GL.Uniform3(uAmbientLightLocation, colour);

            int uSpecularLightLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uLight[0].SpecularLight");
            //colour = new Vector3(0.316228f, 0.316228f, 0.316228f);
            GL.Uniform3(uSpecularLightLocation, colour);

            int uDiffuseLightLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uLight[0].DiffuseLight");
            //colour = new Vector3(0.54f, 0.89f, 0.63f);
            GL.Uniform3(uDiffuseLightLocation, colour);
        }

        protected void LightUpdateG()
        {
            int uLightPositionLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uLight[2].Position");
            Vector4 lightPosition = new Vector4(0, 3, -8.5f, 1);
            lightPosition = Vector4.Transform(lightPosition, mView);
            GL.Uniform4(uLightPositionLocation, lightPosition);

            int uAmbientLightLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uLight[2].AmbientLight");
            Vector3 colour = new Vector3(0.0f, 1.0f, 0.0f);
            GL.Uniform3(uAmbientLightLocation, colour);

            int uSpecularLightLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uLight[2].SpecularLight");
            //colour = new Vector3(0.316228f, 0.316228f, 0.316228f);
            GL.Uniform3(uSpecularLightLocation, colour);

            int uDiffuseLightLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uLight[2].DiffuseLight");
            //colour = new Vector3(0.54f, 0.89f, 0.63f);
            GL.Uniform3(uDiffuseLightLocation, colour);
        }

        protected void LightUpdateB()
        {
            int uLightPositionLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uLight[1].Position");
            Vector4 lightPosition = new Vector4(2, 3, -8.5f, 1);
            lightPosition = Vector4.Transform(lightPosition, mView);
            GL.Uniform4(uLightPositionLocation, lightPosition);

            int uAmbientLightLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uLight[1].AmbientLight");
            Vector3 colour = new Vector3(0.0f, 0.0f, 1.0f);
            GL.Uniform3(uAmbientLightLocation, colour);

            int uSpecularLightLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uLight[1].SpecularLight");
            //colour = new Vector3(0.316228f, 0.316228f, 0.316228f);
            GL.Uniform3(uSpecularLightLocation, colour);

            int uDiffuseLightLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uLight[1].DiffuseLight");
            //colour = new Vector3(0.54f, 0.89f, 0.63f);
            GL.Uniform3(uDiffuseLightLocation, colour);
        }

        protected void MaterialUpdate(Vector3 ambMat, Vector3 specMat, Vector3 diffMat)
        {
            int uMaterialShininess = GL.GetUniformLocation(mShader.ShaderProgramID, "uMaterial.Shininess");
            GL.Uniform1(uMaterialShininess, 32.0f);

            int uAmbientReflectivity = GL.GetUniformLocation(mShader.ShaderProgramID, "uMaterial.AmbientReflectivity");
            GL.Uniform3(uAmbientReflectivity, ambMat);

            int uSpecularReflectivity = GL.GetUniformLocation(mShader.ShaderProgramID, "uMaterial.SpecularReflectivity");
            GL.Uniform3(uSpecularReflectivity, specMat);

            int uDiffuseReflectivity = GL.GetUniformLocation(mShader.ShaderProgramID, "uMaterial.DiffuseReflectivity");
            GL.Uniform3(uDiffuseReflectivity, diffMat);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (e.KeyChar == 'w') {
                CameraTranslate(0.0f, 0.0f, 0.05f);
            }
            if (e.KeyChar == 's')
            {
                CameraTranslate(0.0f, 0.0f, -0.05f);
            }

            if (e.KeyChar == 'a')
            {
                CameraRotate(-0.025f);
            }
            if (e.KeyChar == 'd')
            {
                CameraRotate(0.025f);
            }

            if (e.KeyChar == 'z')
            {
                WorldRotate(-0.025f);
            }
            if (e.KeyChar == 'x')
            {
                WorldRotate(0.025f);
            }

            if (e.KeyChar == 'c')
            {
                ModelRotate(-0.025f);
            }
            if (e.KeyChar == 'v')
            {
                ModelRotate(0.025f);
            }

            LightUpdateR();
            LightUpdateG();
            LightUpdateB();

            base.OnKeyPress(e);
        }

        protected void CameraTranslate(Single x, Single y, Single z)
        {
            mView = mView * Matrix4.CreateTranslation(x, y, z);
            int uView = GL.GetUniformLocation(mShader.ShaderProgramID, "uView");
            GL.UniformMatrix4(uView, true, ref mView);
        }

        protected void CameraRotate(Single x)
        {
            mView = mView * Matrix4.CreateRotationY(x);
            int uView = GL.GetUniformLocation(mShader.ShaderProgramID, "uView");
            GL.UniformMatrix4(uView, true, ref mView);
        }

        protected void WorldRotate(Single x)
        {
            Vector3 t = mGroundModel.ExtractTranslation();
            Matrix4 translation = Matrix4.CreateTranslation(t);
            Matrix4 inverseTranslation = Matrix4.CreateTranslation(-t);
            mGroundModel = mGroundModel * inverseTranslation * Matrix4.CreateRotationY(x) *
            translation;
        }

        /*protected void SphereRotate(Single x)
        {
            Vector3 t = mSphereModel.ExtractTranslation();
            Matrix4 translation = Matrix4.CreateTranslation(t);
            Matrix4 inverseTranslation = Matrix4.CreateTranslation(-t);
            mSphereModel = mSphereModel * inverseTranslation * Matrix4.CreateRotationY(x) *
            translation;
        }*/
        
        protected void ModelRotate(Single x)
        {
            Vector3 t = mSurpriseModel.ExtractTranslation();
            Matrix4 translation = Matrix4.CreateTranslation(t);
            Matrix4 inverseTranslation = Matrix4.CreateTranslation(-t);
            mSurpriseModel = mSurpriseModel * inverseTranslation * Matrix4.CreateRotationY(x) *
            translation;

            Vector3 t2 = mCylinderModel.ExtractTranslation();
            Matrix4 translation2 = Matrix4.CreateTranslation(t2);
            Matrix4 inverseTranslation2 = Matrix4.CreateTranslation(-t2);
            mCylinderModel = mCylinderModel * inverseTranslation2 * Matrix4.CreateRotationY(x) *
            translation2;
        }

        /*protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


            int uModel = GL.GetUniformLocation(mShader.ShaderProgramID, "uModel");
            GL.UniformMatrix4(uModel, true, ref mGroundModel);  

            GL.BindVertexArray(mVAO_IDs[0]);
            GL.DrawArrays(PrimitiveType.TriangleFan, 0, 4);

            Matrix4 m = mSphereModel * mGroundModel;
            uModel = GL.GetUniformLocation(mShader.ShaderProgramID, "uModel");
            GL.UniformMatrix4(uModel, true, ref m); 

            GL.BindVertexArray(mVAO_IDs[1]);
            GL.DrawElements(PrimitiveType.Triangles, mSphereModelUtility.Indices.Length, DrawElementsType.UnsignedInt, 0);
            
            GL.BindVertexArray(0);
            this.SwapBuffers();
        }*/

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


            int uModel = GL.GetUniformLocation(mShader.ShaderProgramID, "uModel");
            GL.UniformMatrix4(uModel, true, ref mGroundModel);
            MaterialUpdate(new Vector3(0.05f, 0.05f, 0.05f), new Vector3(0.7f, 0.7f, 0.7f), new Vector3(0.5f, 0.5f, 0.5f));

            GL.BindVertexArray(mVAO_IDs[0]);
            GL.DrawArrays(PrimitiveType.TriangleFan, 0, 4);

            Matrix4 m = mSurpriseModel * mGroundModel;
            uModel = GL.GetUniformLocation(mShader.ShaderProgramID, "uModel");
            GL.UniformMatrix4(uModel, true, ref m);
            //MaterialUpdate(64.0f, new Vector3(0.25f, 0.2f, 0.1f), new Vector3(0.6f, 0.6f, 0.4f), new Vector3(0.8f, 0.6f, 0.2f));

            GL.BindVertexArray(mVAO_IDs[1]);
            GL.DrawElements(PrimitiveType.Triangles, mSurpriseModelUtility.Indices.Length, DrawElementsType.UnsignedInt, 0);

            Matrix4 c = mCylinderModel * mGroundModel;
            uModel = GL.GetUniformLocation(mShader.ShaderProgramID, "uModel");
            GL.UniformMatrix4(uModel, true, ref c);
            //MaterialUpdate(0.4f, new Vector3(0.2f, 0.2f, 0.2f), new Vector3(0.5f, 0.5f, 0.5f), new Vector3(0.5f, 0.5f, 0.5f));

            GL.BindVertexArray(mVAO_IDs[2]);
            GL.DrawElements(PrimitiveType.Triangles, mCylinderModelUtility.Indices.Length, DrawElementsType.UnsignedInt, 0);

            GL.BindVertexArray(0);
            this.SwapBuffers();
        }

        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.DeleteBuffers(mVBO_IDs.Length, mVBO_IDs);
            GL.DeleteVertexArrays(mVAO_IDs.Length, mVAO_IDs);
            mShader.Delete();
            base.OnUnload(e);
        }
    }
}