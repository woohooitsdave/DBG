using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TKSprites
{
    internal class DBG : GameWindow
    {

        public bool DRAW_BLOCKS = true, DRAW_PLAYER = true, DRAW_GUI = true;

        public static DBG MainWindow = null;
        public Camera MainCamera = new Camera(0, 0, 800, 600);
        private int ibo_elements;
        //private Dictionary<string, int> textures = new Dictionary<string, int>();
        //private List<Sprite> sprites = new List<Sprite>();
        private Matrix4 ortho3d;
        private Matrix2d ortho2d;
        private bool updated = false;
        private float avgfps = 60;
        private Random r = new Random();
        public Vector2 mousePos = new Vector2(0, 0);

        public double Ticker = 0.0;

        //State Managers
        public ShaderManager ShaderMgr = new ShaderManager();
        public InputManager InputMgr = new InputManager();
        public BlockTextureManager TextureMgr;
        public Environment EnviroMgr = new Environment();

        //TextWriter
        public UIManager UIMgr = new UIManager();

        //World
        public Map world;
        public Player player;
        public Item item;

        [STAThread]
        public static void Main()
        {
            using (DBG window = new DBG())
            {
                MainWindow = window;
                window.Run(60.0, 60.0);
            }
        }

        public DBG()
            : base(800, 600, new OpenTK.Graphics.GraphicsMode(new OpenTK.Graphics.ColorFormat(8, 8, 8, 8), 3, 3, 4), "OpenTK Sprite Demo", GameWindowFlags.Default, DisplayDevice.Default, 3, 0, OpenTK.Graphics.GraphicsContextFlags.ForwardCompatible)
        {
            MainCamera.Size = new SizeF(ClientSize.Width, ClientSize.Height);
            ortho3d = Matrix4.CreateOrthographic(ClientSize.Width, ClientSize.Height, 0.0f, 50.0f);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            GL.ClearColor(Color.CornflowerBlue);
            GL.Viewport(0, 0, Width, Height);
            
            TextureMgr = new BlockTextureManager(loadImage(@"tiles\atlas.png"));

            // Create a lot of sprites
            /*for (int i = 0; i < 10000; i++)
            {
                addSprite();
            }*/

            world = new Map();
            world.Generate();

            player = new Player(50 * Properties.Settings.Default.TileSize, 0);
            item = new Item(50 * Properties.Settings.Default.TileSize, 0);

            UIMgr.AddString(new Vector2(1, Height - 20), "Time:");//DBG.MainWindow.ClientSize.Height-20)
            UIMgr.AddString(new Vector2(1, Width - 40), "Health:");
            
            MainCamera.CenterOnTarget(player.Position);

            // Load shaders
            ShaderMgr.LoadShaders();
            GL.UseProgram(ShaderMgr.CurrentShader.ProgramID);

            GL.GenBuffers(1, out ibo_elements);

            // Enable blending based on the texture alpha
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);            

        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            if (updated)
            {
                base.OnRenderFrame(e);
                GL.Viewport(0, 0, Width, Height);
                //Prepare3d();
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                int offset = 0;

                GL.UseProgram(ShaderMgr.CurrentShader.ProgramID);
                ShaderMgr.CurrentShader.EnableVertexAttribArrays();

                GL.BindTexture(TextureTarget.Texture2D, TextureMgr.blockAtlas.TextureID);
                foreach (Block t in world.Blocks)
                {
                    if (DRAW_BLOCKS && t.IsVisible)
                    {
                        if (ShaderMgr.MultiShaderMode)
                        {
                            GL.UseProgram(ShaderMgr.GetShader((TextureMgr.blockAtlas.TextureID - 1) % ShaderMgr.Count).ProgramID);
                        }

                        GL.UniformMatrix4(ShaderMgr.CurrentShader.GetUniform("mvp"), false, ref t.ModelViewProjectionMatrix);
                        GL.Uniform1(ShaderMgr.CurrentShader.GetAttribute("mytexture"), TextureMgr.blockAtlas.TextureID);
                        GL.DrawElements(BeginMode.Points, 6, DrawElementsType.UnsignedInt, offset * sizeof(uint));
                        offset += 6;
                    }
                }
                if (DRAW_PLAYER && player.sprite.IsVisible)
                {
                    if (ShaderMgr.MultiShaderMode)
                    {
                        GL.UseProgram(ShaderMgr.GetShader((player.sprite.TextureID - 1) % ShaderMgr.Count).ProgramID);
                    }

                    GL.BindTexture(TextureTarget.Texture2D, player.sprite.TextureID);

                    GL.UniformMatrix4(ShaderMgr.CurrentShader.GetUniform("mvp"), false, ref player.sprite.ModelViewProjectionMatrix);
                    GL.Uniform1(ShaderMgr.CurrentShader.GetAttribute("mytexture"), player.sprite.TextureID);
                    GL.DrawElements(BeginMode.LineLoop, 6, DrawElementsType.UnsignedInt, offset * sizeof(uint));
                    offset += 6;

                    GL.BindTexture(TextureTarget.Texture2D, item.sprite.TextureID);

                    GL.UniformMatrix4(ShaderMgr.CurrentShader.GetUniform("mvp"), false, ref item.sprite.ModelViewProjectionMatrix);
                    GL.Uniform1(ShaderMgr.CurrentShader.GetAttribute("mytexture"), item.sprite.TextureID);
                    GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, offset * sizeof(uint));
                    offset += 6;
                }

                foreach (GLText glt in UIMgr.Elements)
                {
                    if (DRAW_GUI)// (myChar.IsVisible)
                    {
                        //Prepare2d();

                        if (ShaderMgr.MultiShaderMode)
                        {
                            GL.UseProgram(ShaderMgr.GetShader(0).ProgramID);
                        }

                        GL.BindTexture(TextureTarget.Texture2D, glt.TextureID);

                        GL.UniformMatrix4(ShaderMgr.CurrentShader.GetUniform("mvp"), false, ref glt.ModelViewProjectionMatrix);
                        GL.Uniform1(ShaderMgr.CurrentShader.GetAttribute("mytexture"), glt.TextureID);
                        GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, offset * sizeof(uint));
                        offset += 6;
                    }

                }
                ShaderMgr.CurrentShader.DisableVertexAttribArrays();

                GL.Flush();
                SwapBuffers();
            }
        }

        protected void Prepare3d()
        {
            //glViewport(0, 0, m_Setup.width, m_Setup.height);
            GL.Viewport(0, 0, Width, Height);
            //glMatrixMode(GL_PROJECTION);
            GL.MatrixMode(MatrixMode.Projection);

            //glLoadIdentity();
            GL.LoadIdentity();

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            GL.DepthFunc(DepthFunction.Equal);
            GL.Enable(EnableCap.DepthTest);

        }

        protected void Prepare2d()
        {
            //glMatrixMode(GL_PROJECTION);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();

            //gluOrtho2D(0.0f, m_Setup.width, m_Setup.height, 0.0f);
            GL.Ortho(0.0f, ClientSize.Width, ClientSize.Height, 0.0f, -1.0f, 1.0f);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            //GL.Translate(0.375, 0.375, 0.0);

            GL.Disable(EnableCap.DepthTest);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            Ticker += e.Time;
            if (Ticker > 1.0)
            {
             //   myText.UpdateText(Ticker.ToString());
                Ticker -= 1.0;
            }
            
            KeyboardState keyboardState = OpenTK.Input.Keyboard.GetState();

            player.Update((float)e.Time);
            bool moverequest = false;



            // Move view based on key input
            //float moveSpeed = 2f;// 50.0f * ((keyboardState[Key.ShiftLeft] || keyboardState[Key.ShiftRight]) ? 3.0f : 1.0f); // Hold shift to move 3 times faster!

            // Up-down movement
            if (InputMgr.IsDown(Key.Up, Key.W))
            {
                //player.MoveY((moveSpeed));// * (float)e.Time));
            }
            else if (InputMgr.IsDown(Key.Down, Key.S))
            {
                //MainCamera.Y -= moveSpeed;
            }
            

            // Left-right movement
            if (InputMgr.IsDown(Key.Left, Key.A))
            {
                player.speedX -= player.accX;
                moverequest = true;

            }
            if (InputMgr.IsDown(Key.Right, Key.D))
            {
                player.speedX += player.accX;
                moverequest = true;

            }

            if (InputMgr.IsDown(Key.W) && !player.jumping && !player.jumpKeyDown)
            {
                player.jumping = true;
                player.jumpKeyDown = true;
                player.speedY += player.jumpStartSpeedY;
            } else
            {
                player.jumpKeyDown = false;
            }

            if (player.speedX > player.maxSpeedX) player.speedX = player.maxSpeedX;
            if (player.speedX < -player.maxSpeedX) player.speedX = -player.maxSpeedX;
            if (player.speedY < -player.maxSpeedY) player.speedY = -player.maxSpeedY; //Terminal Velocity

            player.speedY -= player.accY; //Apply force of gravity

            if (!moverequest)
            {
                if (player.speedX < 0) player.speedX += player.decX; if (player.speedX > 0) player.speedX -= player.decX;

                // Deceleration may produce a speed that is greater than zero but
                // smaller than the smallest unit of deceleration. These lines ensure
                // that the player does not keep travelling at slow speed forever after
                // decelerating.
                if (player.speedX > 0 && player.speedX < player.decX) player.speedX = 0;
                if (player.speedX < 0 && player.speedX > -player.decX) player.speedX = 0;
            }



            //if(player.Position != player.LastPosition)
            MainCamera.CenterOnTarget(player.Position);//
            


            // Quit if requested
            if (InputMgr.IsDown(Key.Escape))
            {
                Exit();
            }
            if (InputMgr.IsDown(Key.Q))
            {
                int leftOf = ((int)player.Left / Properties.Settings.Default.TileSize);
                int heightOf = ((int)player.Top / Properties.Settings.Default.TileSize) - 1;
                world.blocks[leftOf, heightOf].TileID = 5;
                world.blocks[leftOf + 1, heightOf].TileID = 5;
            }
            if (InputMgr.IsDown(Key.Plus, Key.KeypadPlus))
            {
                Console.WriteLine(String.Format("player x: {0}, y: {1}. w: {2}", player.position.X, player.position.Y, (world.Width * Properties.Settings.Default.TileSize) - MainCamera.Width));
                Console.WriteLine(String.Format("sprite: x: {0}, y: {1}. w: {2}", player.sprite.Position.X, player.sprite.Position.Y, (world.Width * Properties.Settings.Default.TileSize) - MainCamera.Width));
            }



            // Update graphics
            List<Vector2> verts = new List<Vector2>();
            List<Vector2> texcoords = new List<Vector2>();
            List<int> inds = new List<int>();

            int vertcount = 0;
            int viscount = 0;

            // Get data for visible sprites
            //foreach (Sprite s in sprites)
            if (DRAW_BLOCKS)
            {
                for (int y = 0; y < world.blocks.GetLength(1); y++)
                {
                    for (int x = 0; x < world.blocks.GetLength(0); x++)
                    {
                        if (world.blocks[x, y].IsVisible) { 
                            //verts.AddRange(Sprite.GetVertices());
                            verts.AddRange(Sprite.GetVertices());
                            //texcoords.AddRange(t.GetTexCoords());
                            texcoords.AddRange(TextureMgr.GetTexCoords(world.blocks[x,y].TileID));
                            inds.AddRange(Sprite.GetIndices(vertcount));
                            vertcount += 4;
                            viscount++;

                            world.blocks[x, y].CalculateModelMatrix();
                            world.blocks[x, y].ModelViewProjectionMatrix = world.blocks[x, y].ModelMatrix * ortho3d;
                        }
                    }
                }                
            }

            if (DRAW_PLAYER && player.sprite.IsVisible)
            {
                /*Vector2mousePos = new Vector2(e.X, e.Y);
                mousePos.X += MainCamera.X;
                mousePos.Y = ClientSize.Height - mousePos.Y + MainCamera.Y;
                if (mousePos.X < player.Left + (player.Width / 2)) player.facingRight = false;
                */
                //else player.facingRight = true;
                //if (player.facingRight)
                //    player.sprite.Size = new Size(-32, 48);
                //else player.sprite.Size = new Size(32, 48);

                verts.AddRange(Sprite.GetVertices());
                texcoords.AddRange(player.sprite.GetTexCoords());
                inds.AddRange(Sprite.GetIndices(vertcount));
                vertcount += 4;
                viscount++;

                player.sprite.CalculateModelMatrix();
                player.sprite.ModelViewProjectionMatrix = player.sprite.ModelMatrix * ortho3d;

                verts.AddRange(Sprite.GetVertices());
                texcoords.AddRange(item.sprite.GetTexCoords());
                inds.AddRange(Sprite.GetIndices(vertcount));
                vertcount += 4;
                viscount++;

                item.sprite.CalculateModelMatrix();
                item.sprite.ModelViewProjectionMatrix = item.sprite.ModelMatrix * ortho3d;
            }
            foreach (GLText glt in UIMgr.Elements)
            {

                if (DRAW_GUI)//(myChar.IsVisible)
                {
                    verts.AddRange(Sprite.GetVertices());
                    texcoords.AddRange(DBG_GL.GLHelper.GetCommonTexCoords());
                    inds.AddRange(Sprite.GetIndices(vertcount));
                    vertcount += 4;
                    viscount++;

                    glt.CalculateModelMatrix();
                    glt.ModelViewProjectionMatrix = glt.ModelMatrix * ortho3d;
                }

            }
            // Buffer vertex coordinates
            GL.BindBuffer(BufferTarget.ArrayBuffer, ShaderMgr.CurrentShader.GetBuffer("v_coord"));
            GL.BufferData<Vector2>(BufferTarget.ArrayBuffer, (IntPtr) (verts.Count * Vector2.SizeInBytes), verts.ToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(ShaderMgr.CurrentShader.GetAttribute("v_coord"), 2, VertexAttribPointerType.Float, false, 0, 0);

            // Buffer texture coords
            GL.BindBuffer(BufferTarget.ArrayBuffer, ShaderMgr.CurrentShader.GetBuffer("v_texcoord"));
            GL.BufferData<Vector2>(BufferTarget.ArrayBuffer, (IntPtr) (texcoords.Count * Vector2.SizeInBytes), texcoords.ToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(ShaderMgr.CurrentShader.GetAttribute("v_texcoord"), 2, VertexAttribPointerType.Float, true, 0, 0);
            
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            // Buffer indices
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo_elements);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr) (inds.Count * sizeof(int)), inds.ToArray(), BufferUsageHint.StaticDraw);

            updated = true;

            // Display average FPS and sprite statistics in title bar
            avgfps = (avgfps + (1.0f / (float) e.Time)) / 2.0f;
            Title = String.Format("OpenTK Sprite Demo ({0} sprites, {1} drawn, FPS:{2:0.00})", 0/*sprites.Count*/, viscount, avgfps);
        }


        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            ortho3d = Matrix4.CreateOrthographic(ClientSize.Width, ClientSize.Height, -1.0f, 2.0f);
            MainCamera.Size = new SizeF(ClientSize.Width, ClientSize.Height);
        }

        /// <summary>
        /// Loads a texture from a Bitmap
        /// </summary>
        /// <param name="image">Bitmap to make a texture from</param>
        /// <returns>ID of texture, or -1 if there is an error</returns>
        private int loadImage(Bitmap image)
        {
            int texID = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, texID);
            BitmapData data = image.LockBits(new System.Drawing.Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            image.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            return texID;
        }

        /// <summary>
        /// Overload to make a texture from a filename
        /// </summary>
        /// <param name="filename">File to make a texture from</param>
        /// <returns>ID of texture, or -1 if there is an error</returns>
        public int loadImage(string filename)
        {
            try
            {
                Image file = Image.FromFile(filename);
                return loadImage(new Bitmap(file));
            }
            catch (FileNotFoundException)
            {
                return -1;
            }
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
            InputMgr.UpdateMousePos(e.X + MainCamera.X, ClientSize.Height - e.Y + MainCamera.Y);
            /*
            mousePos = new Vector2(e.X, e.Y);
            mousePos.X += MainCamera.X;
            mousePos.Y = ClientSize.Height - mousePos.Y + MainCamera.Y;
            if (mousePos.X < player.Left + (player.Width / 2)) player.facingRight = false;
            else player.facingRight = true;
            */
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            // Selection example
            // First, find coordinates of mouse in global space
            Vector2 clickPoint = new Vector2(e.X, e.Y);
            clickPoint.X += MainCamera.X;
            clickPoint.Y = ClientSize.Height - clickPoint.Y + MainCamera.Y;

            // Find target Sprite
            Block clickedBlock = null;
            //foreach (Sprite s in sprites)

            foreach (Block s in world.Blocks)
            {
                // We can only click on visible Sprites
                if (s.IsVisible)
                {
                    if (s.IsInside(clickPoint))
                    {
                        // We store the last sprite found to get the topmost one (they're searched in the same order they're drawn)
                        clickedBlock = s;
                    }
                }
            }

            // Change the texture on the clicked Sprite
            if (clickedBlock != null)
            {
                Console.WriteLine(String.Format("block x: {0}, y: {1}. w: {2}", clickedBlock.Position.X, clickedBlock.Position.Y, (world.Width * Properties.Settings.Default.TileSize) - MainCamera.Width));

                /* if (clickedSprite.TextureID == textures["stone"])
                 {
                     clickedSprite.TextureID = textures["dirt"];
                 }
                 else if (clickedSprite.TextureID == textures["dirt"])
                 {
                     clickedSprite.TextureID = textures["sand"];
                 }
                 else
                 {
                     clickedSprite.TextureID = textures["stone"];
                 }*/
            }
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);
            InputMgr.KeyDown(e.Key);
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            base.OnKeyUp(e);

            InputMgr.KeyUp(e.Key);

            // Change shader
            if (e.Key == Key.V && !ShaderMgr.MultiShaderMode)
            {
                //currentShader = (currentShader + 1) % shaders.Count;
                ShaderMgr.IncShader();
                GL.UseProgram(ShaderMgr.CurrentShader.ProgramID);
            }

            // Enable shader based on texture ID
            if (e.Key == Key.M)
            {
                // Toggle the value
                ShaderMgr.MultiShaderMode ^= true;
            }

            
        }

    }
}




/*
  
    OLD SAMPLE CODE


    /// <summary>
        /// Creates a new sprite with a random texture and transform
        /// </summary>
        private void addSprite()
        {
            // Assign random texture
            Sprite s = new Sprite(textures.ElementAt(r.Next(0, textures.Count)).Value, 50, 50);

            // Transform sprite randomly
            s.Position = new Vector2(r.Next(-8000, 8000), r.Next(-6000, 6000));
            float scale = 16f;// 300.0f * (float) r.NextDouble() + 0.5f;
            s.Size = new SizeF(scale, scale);
            //s.Rotation = (float) r.NextDouble() * 2.0f * 3.141f;

            sprites.Add(s);
        }



    
            // Update positions
            /*Parallel.ForEach(sprites, delegate(Sprite s)
            {
                s.Position += new Vector2((float)(e.Time * s.Scale.X * Math.Cos(s.Rotation)), (float)(e.Time * s.Scale.Y * Math.Sin(s.Rotation)));
            });*/


    //if(MainCamera.X > -16f) MainCamera.X -= moveSpeed * (float) e.Time;
    //player.sprite.Position.X -= moveSpeed * (float)e.Time;
    //player.MoveX(-(moveSpeed));// * (float)e.Time));
    //MainCamera.CenterOnTarget(player.sprite.Position);//
    //MainCamera.MoveX(-(moveSpeed * (float)e.Time));

        
