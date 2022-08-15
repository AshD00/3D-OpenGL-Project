using Labs.Utility;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using System.Drawing.Imaging;


namespace Labs.ACW
{
    public class ACWWindow : GameWindow
    {
        public ACWWindow()
            : base(
                800, // Width
                600, // Height
                GraphicsMode.Default,
                "Assessed Coursework",
                GameWindowFlags.Default,
                DisplayDevice.Default,
                3, // major
                3, // minor
                GraphicsContextFlags.ForwardCompatible
                )
        {
        }

        //Defining the variables and arrays needed throughout the project
        private int[] mVBO_IDs = new int[10];
        private int[] mVAO_IDs = new int[6];
        private ShaderUtility mShader;
        private ModelUtility mSphereModelUtility, mSurpriseModelUtility, mCylinderModelUtility, mCubeModelUtility;
        private Matrix4 mView, sView, mSphereModel, mGroundModel, mSurpriseModel, mCylinderModel, mCylinderModel2, mCylinderModel3, mCubeModel;
        private Vector4[] lightPositions = new Vector4[4];
        private Vector3[] lightColours = new Vector3[4];
        private Camera view = new Camera();
        private bool mobile = true;
        private int uView, mTexture_ID;
        Bitmap TextureBitmap;
        BitmapData TextureData;


        protected override void OnLoad(EventArgs e)
        {
            // Set some GL state
            GL.ClearColor(Color4.Black);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);

            //Assign mShader to the vertex and fragment files
            mShader = new ShaderUtility(@"ACW/Shaders/vPassThrough.vert", @"ACW/Shaders/fLighting.frag");

            GL.UseProgram(mShader.ShaderProgramID);

            //Assigns the following variables to their counterparts within the shader file
            int vPositionLocation = GL.GetAttribLocation(mShader.ShaderProgramID, "vPosition");
            int vNormalLocation = GL.GetAttribLocation(mShader.ShaderProgramID, "vNormal");

            int vTextureLocation = GL.GetAttribLocation(mShader.ShaderProgramID, "vTexCoords");
            int uTextureSamplerLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uTextureSampler");
            GL.Uniform1(uTextureSamplerLocation, 0);

            //Fetches the texture for the floor and back wall
            string filepath = @"ACW\floor.jpg";
            if (System.IO.File.Exists(filepath))
            {
                TextureBitmap = new Bitmap(filepath);
                TextureBitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
                TextureData = TextureBitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, TextureBitmap.Width,
                TextureBitmap.Height), ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            }
            else
            {
                throw new Exception("Could not find file " + filepath);
            }

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.GenTextures(1, out mTexture_ID);
            GL.BindTexture(TextureTarget.Texture2D, mTexture_ID);

            GL.TexImage2D(TextureTarget.Texture2D,
                            0, PixelInternalFormat.Rgba, TextureData.Width, TextureData.Height,
                            0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                            PixelType.UnsignedByte, TextureData.Scan0);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            TextureBitmap.UnlockBits(TextureData);

            TextureBitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

            GL.GenVertexArrays(mVAO_IDs.Length, mVAO_IDs);
            GL.GenBuffers(mVBO_IDs.Length, mVBO_IDs);

            //Defining the vertices for the floor and wall
            float[] floorVertices = new float[] {
                                             -10, 0, -10,0,1,0, 0.0f, 0.0f,
                                             -10, 0, 10,0,1,0, 0.0f, 1.0f,
                                             10, 0, 10,0,1,0, 1.0f, 1.0f,
                                             10, 0, -10,0,1,0, 1.0f, 0.0f,
            };

            float[] wallVertices = new float[]
            {
                                             -10,  20, -10,0,1,0, 0.0f, 1.0f,
                                             -10,   0, -10,0,1,0, 0.0f, 0.0f,
                                              10,   0, -10,0,1,0, 1.0f, 0.0f,
                                              10,  20, -10,0,1,0, 1.0f, 1.0f,
            };

            //Binding them to the vertex array positions 0 and 4 respectively, and the buffer objects array 0 and 7, then enabling the vertices in the region
            GL.BindVertexArray(mVAO_IDs[0]);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mVBO_IDs[0]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(floorVertices.Length * sizeof(float)), floorVertices, BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(vPositionLocation);
            GL.VertexAttribPointer(vPositionLocation, 3, VertexAttribPointerType.Float, true, 8 * sizeof(float), 0);
            GL.EnableVertexAttribArray(vNormalLocation);
            GL.VertexAttribPointer(vNormalLocation, 3, VertexAttribPointerType.Float, true, 8 * sizeof(float), 3 * sizeof(float));           
            GL.EnableVertexAttribArray(vTextureLocation);
            GL.VertexAttribPointer(vTextureLocation, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));

            GL.BindVertexArray(mVAO_IDs[4]);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mVBO_IDs[7]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(wallVertices.Length * sizeof(float)), wallVertices, BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(vPositionLocation);
            GL.VertexAttribPointer(vPositionLocation, 3, VertexAttribPointerType.Float, true, 8 * sizeof(float), 0);
            GL.EnableVertexAttribArray(vNormalLocation);
            GL.VertexAttribPointer(vNormalLocation, 3, VertexAttribPointerType.Float, true, 8 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(vTextureLocation);
            GL.VertexAttribPointer(vTextureLocation, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));

            int size;
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);
            if (floorVertices.Length * sizeof(float) != size || wallVertices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Vertex data not loaded onto graphics card correctly");
            }

            //Fetching the models from their place within the project
            mSphereModelUtility = ModelUtility.LoadModel(@"Utility/Models/sphere.bin");
            mSurpriseModelUtility = ModelUtility.LoadModel(@"Utility/Models/model.bin");
            mCylinderModelUtility = ModelUtility.LoadModel(@"Utility/Models/cylinder.bin");
            mCubeModelUtility = ModelUtility.LoadModel(@"Utility/Models/lab22model.sjg");

            //Creating the models the code will use based upon the model files
            CreateModel(vPositionLocation, vNormalLocation, 1, 1, mSurpriseModelUtility);

            CreateModel(vPositionLocation, vNormalLocation, 3, 5, mCylinderModelUtility);

            CreateModel(vPositionLocation, vNormalLocation, 2, 3, mSphereModelUtility);

            CreateModel(vPositionLocation, vNormalLocation, 5, 8, mCubeModelUtility);

            //Resetting the vertexarray to 0
            GL.BindVertexArray(0);

            //Creating the two camera positions
            mView = Matrix4.CreateTranslation(0, -2.5f, -5);
            sView = Matrix4.CreateTranslation(0, -4.5f, -7);
            uView = GL.GetUniformLocation(mShader.ShaderProgramID, "uView");
            GL.UniformMatrix4(uView, true, ref mView);

            //Mapping the positions of the models
            mGroundModel = Matrix4.CreateTranslation(0, 0, -5f);
            mSphereModel = Matrix4.CreateTranslation(4, 3, 2f);
            mCubeModel = Matrix4.CreateTranslation(-4, 3, 2f);
            mSurpriseModel = Matrix4.CreateTranslation(0, 3, 0f);
            //The central model is rotated to face the camera
            view.ModelRotate(-1.7f, ref mSurpriseModel);
            mCylinderModel = Matrix4.CreateTranslation(0, 1, 0f);
            mCylinderModel2 = Matrix4.CreateTranslation(4, 1, 2f);
            mCylinderModel3 = Matrix4.CreateTranslation(-4, 1, 2f);

            //Defining the colour of the three lights
            lightColours[0] = new Vector3(1.0f, 0.0f, 0.0f);
            lightColours[1] = new Vector3(0.0f, 1.0f, 0.0f);
            lightColours[2] = new Vector3(0.0f, 0.0f, 1.0f);

            //Setting their positions
            lightPositions[0] = new Vector4(-2, 3, -2.5f, 1f);
            lightPositions[1] = new Vector4(0, 3, -2.5f, 1f);
            lightPositions[2] = new Vector4(2, 3, -2.5f, 1f);

            //Creating the lights
            LightUpdate(0, lightPositions[0], lightColours[0], mView);
            LightUpdate(1, lightPositions[1], lightColours[1], mView);
            LightUpdate(2, lightPositions[2], lightColours[2], mView);

            base.OnLoad(e);
        }

        /// <summary>
        /// This method is in charge of controlling and updating the three lights when the program is started and then any time the camera moves
        /// </summary>
        /// <param name="lightNum">The current lights number in the array</param>
        /// <param name="lightPosition">The position of the light</param>
        /// <param name="colour">The lights colour</param>
        /// <param name="view">The camera position currently in use</param>
        protected void LightUpdate(int lightNum, Vector4 lightPosition, Vector3 colour, Matrix4 view)
        {
            int uLightPositionLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uLight[" + lightNum + "].Position");
            lightPosition = Vector4.Transform(lightPosition, view);
            GL.Uniform4(uLightPositionLocation, lightPosition);

            int uAmbientLightLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uLight[" + lightNum + "].AmbientLight");
            GL.Uniform3(uAmbientLightLocation, colour);

            int uSpecularLightLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uLight[" + lightNum + "].SpecularLight");
            GL.Uniform3(uSpecularLightLocation, colour);

            int uDiffuseLightLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uLight[" + lightNum + "].DiffuseLight");
            GL.Uniform3(uDiffuseLightLocation, colour);
        }
        /// <summary>
        /// This method is in charge of updating the materials of the objects in the scene
        /// </summary>
        /// <param name="ambMat">How the material responds to ambient light</param>
        /// <param name="specMat">^^ for specular light</param>
        /// <param name="diffMat">^^^^ for diffusal lighting</param>
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
        /// <summary>
        /// This mode lis in charge of create and positioning the models when they are defined
        /// </summary>
        /// <param name="vPositionLocation">The link to the shader file for the position</param>
        /// <param name="vNormalLocation">^^ for light responsivity</param>
        /// <param name="VAOPos">It's position in the vertex array</param>
        /// <param name="VBOPos1">It's first position within the vertex buffer object array, as all models aside from the ground have 2 this is later added to</param>
        /// <param name="modelUtility">The current model</param>
        protected void CreateModel(int vPositionLocation, int vNormalLocation, int VAOPos, int VBOPos1, ModelUtility modelUtility)
        {
            int size;

            GL.BindVertexArray(mVAO_IDs[VAOPos]);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mVBO_IDs[VBOPos1]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(modelUtility.Vertices.Length * sizeof(float)), modelUtility.Vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mVBO_IDs[VBOPos1+1]);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(modelUtility.Indices.Length * sizeof(float)), modelUtility.Indices, BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(vPositionLocation);
            GL.VertexAttribPointer(vPositionLocation, 3, VertexAttribPointerType.Float, true, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(vNormalLocation);
            GL.VertexAttribPointer(vNormalLocation, 3, VertexAttribPointerType.Float, true, 6 * sizeof(float), 3 * sizeof(float));

            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);
            if (modelUtility.Vertices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Vertex data not loaded onto graphics card correctly");
            }

            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);
            if (modelUtility.Indices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Index data not loaded onto graphics card correctly");
            }
        }
        /// <summary>
        /// The method responsible for handling key presses
        /// </summary>
        /// <param name="e">The key that triggered the event</param>
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (e.KeyChar == 'c')
            {
                view.ModelRotate(-0.025f, ref mSurpriseModel);
                view.ModelRotate(-0.025f, ref mSphereModel);
                view.ModelRotate(-0.025f, ref mCylinderModel);
                view.ModelRotate(-0.025f, ref mCylinderModel2);
            }
            if (e.KeyChar == 'v')
            {
                view.ModelRotate(0.025f, ref mSurpriseModel);
                view.ModelRotate(0.025f, ref mSphereModel);
                view.ModelRotate(0.025f, ref mCylinderModel);
                view.ModelRotate(0.025f, ref mCylinderModel2);
            }
            if (e.KeyChar == 'z')
            {
                view.WorldRotate(-0.025f, ref mGroundModel);
            }
            if (e.KeyChar == 'x')
            {
                view.WorldRotate(0.025f, ref mGroundModel);
            }

            if (e.KeyChar == 'r')
            {
                if (mobile)
                {
                    GL.UniformMatrix4(uView, true, ref sView);
                    mobile = false;
                }
                else
                {
                    GL.UniformMatrix4(uView, true, ref mView);
                    mobile = true;
                }
            }

            if (mobile)
            {
                if (e.KeyChar == 'w')
                {
                    view.CameraTranslate(0.0f, 0.0f, 0.05f, ref mView);
                }
                if (e.KeyChar == 's')
                {
                    view.CameraTranslate(0.0f, 0.0f, -0.05f, ref mView);
                }

                if (e.KeyChar == 'a')
                {
                    view.CameraRotate(-0.025f, ref mView);
                }
                if (e.KeyChar == 'd')
                {
                    view.CameraRotate(0.025f, ref mView);
                }

                LightUpdate(0, lightPositions[0], lightColours[0], mView);
                LightUpdate(1, lightPositions[1], lightColours[1], mView);
                LightUpdate(2, lightPositions[2], lightColours[2], mView);
            }
            else
            {
                LightUpdate(0, lightPositions[0], lightColours[0], sView);
                LightUpdate(1, lightPositions[1], lightColours[1], sView);
                LightUpdate(2, lightPositions[2], lightColours[2], sView);
            }

            base.OnKeyPress(e);
        }
        /// <summary>
        /// If the user resizes their window this method ensures it stays appropriately sized
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(this.ClientRectangle);
            if (mShader != null)
            {
                int uProjectionLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uProjection");
                Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(1, (float)ClientRectangle.Width / ClientRectangle.Height, 0.5f, 25);
                GL.UniformMatrix4(uProjectionLocation, true, ref projection);
            }
            base.OnResize(e);
        }
        /// <summary>
        /// The method called every frame (144 times a second in my case)
        /// </summary>
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            view.ModelRotate(0.0025f, ref mCubeModel);
            base.OnUpdateFrame(e);
        }
        /// <summary>
        /// The method responsible for drawing the scene when it is first rendered
        /// </summary>
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            int uModel = GL.GetUniformLocation(mShader.ShaderProgramID, "uModel");
            GL.UniformMatrix4(uModel, true, ref mGroundModel);
            MaterialUpdate(new Vector3(0.05f, 0.05f, 0.05f), new Vector3(0.7f, 0.7f, 0.7f), new Vector3(0.5f, 0.5f, 0.5f));

            GL.BindVertexArray(mVAO_IDs[0]);
            GL.DrawArrays(PrimitiveType.TriangleFan, 0, 4);
            GL.BindVertexArray(mVAO_IDs[4]);
            GL.DrawArrays(PrimitiveType.TriangleFan, 0, 4);

            MaterialUpdate(new Vector3(0.05f, 0.05f, 0.05f), new Vector3(0.7f, 0.7f, 0.7f), new Vector3(0.5f, 0.5f, 0.5f));
            DrawModel(mSurpriseModel, mSurpriseModelUtility, 1);
            MaterialUpdate(new Vector3(0.2f, 0.1f, 0.1f), new Vector3(0.7f, 0.6f, 0.6f), new Vector3(0.6f, 0.4f, 0.4f));
            DrawModel(mCubeModel, mCubeModelUtility, 5);
            MaterialUpdate(new Vector3(0.25f, 0.2f, 0.1f), new Vector3(0.6f, 0.6f, 0.4f), new Vector3(0.8f, 0.6f, 0.2f));
            DrawModel(mSphereModel, mSphereModelUtility, 2);

            MaterialUpdate(new Vector3(0.05f, 0.05f, 0.07f), new Vector3(0.3f, 0.3f, 0.3f), new Vector3(0.2f, 0.2f, 0.2f));
            DrawModel(mCylinderModel, mCylinderModelUtility, 3);
            DrawModel(mCylinderModel2, mCylinderModelUtility, 3);
            DrawModel(mCylinderModel3, mCylinderModelUtility, 3);

            GL.BindVertexArray(0);
            this.SwapBuffers();
        }
        /// <summary>
        /// The method responsible for drawing the models during the render frame
        /// </summary>
        /// <param name="model">The current model</param>
        /// <param name="modelUtility">It's utility variable, referring to the actual file</param>
        /// <param name="VAOPos">Its position in the vertex array</param>
        protected void DrawModel(Matrix4 model, ModelUtility modelUtility, int VAOPos)
        {
            int uModel = GL.GetUniformLocation(mShader.ShaderProgramID, "uModel");

            Matrix4 m = model * mGroundModel;
            GL.UniformMatrix4(uModel, true, ref m);

            GL.BindVertexArray(mVAO_IDs[VAOPos]);
            GL.DrawElements(PrimitiveType.Triangles, modelUtility.Indices.Length, DrawElementsType.UnsignedInt, 0);
        }
        /// <summary>
        /// The unload method is responsible for deleting all data off of the GPU when completed
        /// </summary>
        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.DeleteBuffers(mVBO_IDs.Length, mVBO_IDs);
            GL.DeleteVertexArrays(mVAO_IDs.Length, mVAO_IDs);
            mShader.Delete();
            GL.DeleteTexture(mTexture_ID);
            base.OnUnload(e);
        }
    }
    /// <summary>
    /// The camera class which houses the camera itself
    /// </summary>
    public class Camera
    {
        //This method moves the mobile camera along the plane
        public void CameraTranslate(Single x, Single y, Single z, ref Matrix4 view)
        {
            ShaderUtility mShader = new ShaderUtility(@"ACW/Shaders/vPassThrough.vert", @"ACW/Shaders/fLighting.frag");
            view = view * Matrix4.CreateTranslation(x, y, z);
            int uView = GL.GetUniformLocation(mShader.ShaderProgramID, "uView");
            GL.UniformMatrix4(uView, true, ref view);
        }

        //This method turns the mobile camera on the horizontal axis
        public void CameraRotate(Single x, ref Matrix4 view)
        {
            ShaderUtility mShader = new ShaderUtility(@"ACW/Shaders/vPassThrough.vert", @"ACW/Shaders/fLighting.frag");
            view = view * Matrix4.CreateRotationY(x);
            int uView = GL.GetUniformLocation(mShader.ShaderProgramID, "uView");
            GL.UniformMatrix4(uView, true, ref view);
        }

        //This method rotates the entire world
        public void WorldRotate(Single x, ref Matrix4 model)
        {
            Vector3 t = model.ExtractTranslation();
            Matrix4 translation = Matrix4.CreateTranslation(t);
            Matrix4 inverseTranslation = Matrix4.CreateTranslation(-t);
            model = model * inverseTranslation * Matrix4.CreateRotationY(x) *
            translation;
        }

        //This method rotates any of the models, including the cylinders, the spheres and the custom 3D model
        public void ModelRotate(Single x, ref Matrix4 model)
        {
            Vector3 t = model.ExtractTranslation();
            Matrix4 translation = Matrix4.CreateTranslation(t);
            Matrix4 inverseTranslation = Matrix4.CreateTranslation(-t);
            model = model * inverseTranslation * Matrix4.CreateRotationY(x) *
            translation;
        }
    }
}