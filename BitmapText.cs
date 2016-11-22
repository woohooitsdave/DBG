using System;
using System.Collections.Generic;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing.Imaging;

namespace TKSprites
{

    public class GLText : Sprite
    {
        public bool isGUI = true;

        private Font TextFont = new Font(FontFamily.GenericMonospace, 8);
        private Bitmap TextBitmap;
        private Brush colour = Brushes.Magenta;
        private string value = "";
        
        public GLText(Vector2 pos, Font f, Brush c, string text)
        {
            Position = pos;
            SizeF measured = DBGExtensionLibrary.GraphicsHelper.MeasureString(text, f);
            Size = new Size((int)measured.Width, (int)measured.Height);
            //Console.WriteLine(String.Format("{0} : {1}",text, DBGExtensionLibrary.GraphicsHelper.MeasureString(text, f)));
            //_clientSize = size;
            value = text;
            colour = c;

            TextFont = f;

            TextBitmap = new Bitmap((int)Size.Width, (int)Size.Height);
            TextBitmap.MakeTransparent();
            TextureID = CreateTexture();
            UpdateText();
        }


        /// <summary>
        /// Calculates a model matrix for the transforms applied to this Sprite
        /// </summary>
        public override void CalculateModelMatrix()
        {
            Vector3 translation = new Vector3();

            if (isGUI) translation = new Vector3(Position.X - ((DBG.MainWindow.ClientSize.Width - Size.Width) / 2), Position.Y - ((DBG.MainWindow.ClientSize.Height - Size.Height) / 2), 0.0f);
            //if (isGUI) translation = new Vector3(Position.X - ((DBG.MainWindow.Width - Size.Width) / 2), Position.Y - ((DBG.MainWindow.ClientSize.Height - Size.Height) / 2), 0.0f);
            //if (isGUI) translation = new Vector3(DBG.MainWindow.Width + Position.X, DBG.MainWindow.Height - Position.Y, 0.0f);
            else translation = new Vector3(Position.X - DBG.MainWindow.ClientSize.Width / 2 - DBG.MainWindow.MainCamera.X, Position.Y - DBG.MainWindow.ClientSize.Height / 2 - DBG.MainWindow.MainCamera.Y, 0.0f);

            ModelMatrix = Matrix4.CreateScale(Scale.X, Scale.Y, 1.0f) * Matrix4.CreateRotationZ(Rotation) * Matrix4.CreateTranslation(translation);
        }


        private int CreateTexture()
        {
            int textureId;
            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (float)TextureEnvMode.Replace);//Important, or wrong color on some computers
            Bitmap bitmap = TextBitmap;
            bitmap.MakeTransparent();
            GL.GenTextures(1, out textureId);
            GL.BindTexture(TextureTarget.Texture2D, textureId);

            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Nearest);
            GL.Finish();
            bitmap.UnlockBits(data);
            return textureId;
        }

        public void Dispose()
        {
            if (TextureID > -1) GL.DeleteTexture(TextureID);
        }

        public void UpdateText(string txt)
        {
            value = txt;
            UpdateText();
        }

        public void UpdateText(string txt, Brush col)
        {
            value = txt;
            colour = col;
            UpdateText();
        }

        private void UpdateText()
        {
                using (Graphics gfx = Graphics.FromImage(TextBitmap))
                {
                    gfx.Clear(Color.Transparent);
                    gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;// AntiAlias;//.SingleBitPerPixelGridFit;//.ClearTypeGridFit;
                    gfx.DrawString(value, TextFont, colour, new PointF(0,0));
                }

                BitmapData data = TextBitmap.LockBits(new Rectangle(0, 0, TextBitmap.Width, TextBitmap.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, TextBitmap.Width, TextBitmap.Height, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                TextBitmap.UnlockBits(data);
        }

    }

}
