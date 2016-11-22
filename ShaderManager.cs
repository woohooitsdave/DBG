using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace TKSprites
{
    public class ShaderManager
    {

        private int currentShader = 1;
        private List<ShaderProgram> shaders = new List<ShaderProgram>();
        private bool multishadermode = false;

        internal void Add(ShaderProgram shader)
        {
            shaders.Add(shader);
        }

        internal void IncShader()
        {
            currentShader++;
            if (currentShader >= shaders.Count) currentShader = 0;
        }

        internal void LoadShaders()
        {
            // Load shaders
            //shaders.Add(new ShaderProgram("sprite.vert", "sprite.frag", true)); // Normal sprite
            Add(new ShaderProgram(@"shaders\sprite.vert", @"shaders\sprite.frag", true)); // Normal sprite
            Add(new ShaderProgram(@"shaders\white.vert", @"shaders\white.frag", true)); // Just draws the whole sprite white
            Add(new ShaderProgram(@"shaders\onecolor.vert", @"shaders\onecolor.frag", true)); // Uses the color in the upper-left corner of the sprite, but with the correct alpha
            GL.UseProgram(CurrentShader.ProgramID);
        }
        


        internal ShaderProgram CurrentShader { get { return shaders[currentShader]; } }
        internal bool MultiShaderMode { get { return multishadermode; } set { multishadermode = value; } }
        internal int Count { get { return shaders.Count; } }
        internal ShaderProgram GetShader(int index) { if (index >= 0 && index < shaders.Count) return shaders[index]; else return shaders[0]; }

    }
}
