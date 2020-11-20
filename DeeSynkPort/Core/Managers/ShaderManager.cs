﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace DeeSynk.Core.Managers
{
    /// <summary>
    /// Pulls the shaders from the resources folder, compiles them, and links them with a set of programs corresponding
    /// to each set of shaders. These are stored in a dictionary, whose keys are the filenames of the shaders, and whose 
    /// values are the GL generated integers for each program. Uses a 'singleton' format; only one instance of the class
    /// will ever exist, and it will be owned by itself. This will allow all other classes in the namespace to consistently
    /// reference the same class.
    /// </summary>
    public class ShaderManager : IManager
    {
        private static ShaderManager _shaderManager;            //--DIF--//

        //private string _vertPath = @"C:\Users\Nicholas\source\repos\nicholaslaw07\DeeSynk\DeeSynkPort\Resources\Shaders\Vertex";
        //private string _geomPath = @"C:\Users\Nicholas\source\repos\nicholaslaw07\DeeSynk\DeeSynkPort\Resources\Shaders\Geometry";
        //private string _fragPath = @"C:\Users\Nicholas\source\repos\nicholaslaw07\DeeSynk\DeeSynkPort\Resources\Shaders\Fragment";
        //private string _compPath = @"C:\Users\Nicholas\source\repos\nicholaslaw07\DeeSynk\DeeSynkPort\Resources\Shaders\Compute";

        private string _vertPath = @"C:\Users\Chuck\source\repos\nicholaslaw07\DeeSynk\DeeSynkPort\Resources\Shaders\Vertex";
        private string _geomPath = @"C:\Users\Chuck\source\repos\nicholaslaw07\DeeSynk\DeeSynkPort\Resources\Shaders\Geometry";
        private string _fragPath = @"C:\Users\Chuck\source\repos\nicholaslaw07\DeeSynk\DeeSynkPort\Resources\Shaders\Fragment";
        private string _compPath = @"C:\Users\Chuck\source\repos\nicholaslaw07\DeeSynk\DeeSynkPort\Resources\Shaders\Compute";

        private Dictionary<string, int> _programs;

        /// <summary>
        /// Instantiates the program dictionary and begins the chain of events to create the programs and store them in said dictionary.
        /// </summary>
        private ShaderManager()     //--DIF--//
        {
            _programs = new Dictionary<string, int>();
        }

        /// <summary>
        /// Returns the self-owned instance of this class. Since its static, it can be called without the class existing,
        /// and if so it will return a newly created instance. Otherwise, it will return the instance that already exists.
        /// </summary>
        /// <returns></returns>
        public static ref ShaderManager GetInstance()
        {
            if(_shaderManager == null)
                _shaderManager = new ShaderManager();

            return ref _shaderManager;
        }

        public void Load()  //This should be a generic method in the interface
        {
            CreatePrograms();
            CreateCompute();
        }

        /// <summary>
        /// Takes the shader source code, and GL compiles into a shader corresponding to that shader's type. The shader is bound to the
        /// GL context, and is referenced in the integer return value generated by GL.
        /// </summary>
        private int CompileShader(ShaderType type, string source)
        {
            var shader = GL.CreateShader(type);

            GL.ShaderSource(shader, source);
            GL.CompileShader(shader);

            var info = GL.GetShaderInfoLog(shader);

            if (!string.IsNullOrWhiteSpace(info))
                Console.WriteLine($"GL.CompileShader [{type}] had info log: {info}");

            return shader;
        }

        /// <summary>
        /// Retrieves source code for all shaders and iteratively creates a new program corresponding to each shader type,
        /// which is then thrown in a dictionary _shaders.
        /// </summary>
        private void CreatePrograms()
        {
            string[] vertexShadersDirs = Directory.GetFiles(_vertPath);
            string[] vertFileNames = vertexShadersDirs.Select(vs => Path.GetFileNameWithoutExtension(vs)).ToArray();

            string[] geometryShadersDirs = Directory.GetFiles(_geomPath);
            string[] geomFileNames = geometryShadersDirs.Select(gs => Path.GetFileNameWithoutExtension(gs)).ToArray();

            string[] fragmentShadersDirs = Directory.GetFiles(_fragPath);
            string[] fragFileNames = fragmentShadersDirs.Select(fs => Path.GetFileNameWithoutExtension(fs)).ToArray();

            for(int i=0; i < vertexShadersDirs.Length; i++)  //we use vertex to index since vertex shaders are not optional
            {
                var Program = GL.CreateProgram();                                               // creates a new program id in the GL context
                var Shaders = new List<int>();

                using(var fileStreamV = new FileStream(vertexShadersDirs[i], FileMode.Open, FileAccess.Read))
                {
                    using (var StreamReader = new StreamReader(fileStreamV, Encoding.UTF8))
                        Shaders.Add(CompileShader(ShaderType.VertexShader, StreamReader.ReadToEnd()));
                }

                if (geomFileNames.Contains(vertFileNames[i]))
                {
                    int idx = geomFileNames.Select((s, j) => new {j, s})
                                            .Where(t => t.s == vertFileNames[i])
                                            .Select(t => t.j)
                                            .ToList().First();
                    using (var fileStreamG = new FileStream(geometryShadersDirs[idx], FileMode.Open, FileAccess.Read))
                    {
                        using (var StreamReader = new StreamReader(fileStreamG, Encoding.UTF8))
                            Shaders.Add(CompileShader(ShaderType.GeometryShader, StreamReader.ReadToEnd()));
                    }
                }

                if (fragFileNames.Contains(vertFileNames[i]))
                {
                    int idx = fragFileNames.Select((s, j) => new { j, s })
                                            .Where(t => t.s == vertFileNames[i])
                                            .Select(t => t.j)
                                            .ToList().First();
                    using (var fileStreamF = new FileStream(fragmentShadersDirs[idx], FileMode.Open, FileAccess.Read))
                    {
                        using (var StreamReader = new StreamReader(fileStreamF, Encoding.UTF8))
                            Shaders.Add(CompileShader(ShaderType.FragmentShader, StreamReader.ReadToEnd()));
                    }
                }

                foreach (var shader in Shaders)
                    GL.AttachShader(Program, shader);                                           // attaches each type of shader to the generated program

                GL.LinkProgram(Program);                                                        // links the created program to the GL context, does not give this program focus

                foreach(var shader in Shaders)
                {
                    GL.DetachShader(Program, shader);                                           // after linking the program, you can detach and delete the shaders you used to
                    GL.DeleteShader(shader);                                                    // create the program that you just linked
                }

                var info = GL.GetProgramInfoLog(Program);
                if (info != "")
                    Console.WriteLine();

                _programs.Add(vertFileNames[i], Program);                                            // adds the program created to the shaders dictionary
            }
        }

        private void CreateCompute()
        {
            string[] computeShadersDirs = Directory.GetFiles(_compPath);
            string[] compFileNames = computeShadersDirs.Select(cs => Path.GetFileNameWithoutExtension(cs)).ToArray();

            for(int i=0; i<computeShadersDirs.Length; i++)
            {
                int program = GL.CreateProgram();
                int shader = 0;

                using (var fileStreamC = new FileStream(computeShadersDirs[i], FileMode.Open, FileAccess.Read))
                {
                    using (var StreamReader = new StreamReader(fileStreamC, Encoding.UTF8))
                        shader = CompileShader(ShaderType.ComputeShader, StreamReader.ReadToEnd());
                }

                GL.AttachShader(program, shader);
                GL.LinkProgram(program);

                GL.DetachShader(program, shader);
                GL.DeleteShader(shader);

                var info = GL.GetProgramInfoLog(program);
                if(info != "")
                    Console.WriteLine();

                _programs.Add(compFileNames[i], program);
            }
        }
        
        /// <summary>
        /// Retrieves the program corresponding to the filename of the associated shaders.
        /// </summary>
        /// <param name="name">shader/filename</param>
        /// <returns>program ID</returns>
        public int GetProgram(string name)  //Add ability to get multiple shaders
        {
            int programOut = -1;  //if -1 is returned, querying method will handle error
            _programs.TryGetValue(name, out programOut);  //Add error console output?

            return programOut;
        }


        /// <summary>
        /// Used during the unloading phase of the program.  Runs through the list of programID's and deletes each one from the GL context.
        /// </summary>
        public void UnLoad()  //This should be a generic method in the interface
        {
            foreach(string key in _programs.Keys)
            {
                int programID = -1;
                if(_programs.TryGetValue(key, out programID))
                {
                    GL.DeleteProgram(programID);
                    //Add a verification statement that waits until the shader is infact deleted.  Double check to see if shaders need to be unlinked before deletion and then return a bool.  Add a deconstructor/finalizer?
                }
            }
        }
    }
}
