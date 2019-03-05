﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeeSynk.Core.Components.Types.Render
{
    public class ComponentRender : IComponent
    {
        public int BitMaskID => (int)Component.RENDER;

        private int _vaoID;
        /// <summary>
        /// The ID of the VAO that this object should be rendered with.
        /// </summary>
        public int VAO_ID { get => _vaoID; }

        private int _iboID;
        /// <summary>
        /// The ID of the IBO that this object should be rendered with.
        /// </summary>
        public int IBO_ID { get => _iboID; }

        private int _shaderID;
        /// <summary>
        /// The ID of the shader that this object should be rendered with.
        /// </summary>
        public int SHADER_ID { get => _shaderID; }

        //Render Layer
        //Render method (2D or 3D)
        //Is simple sprite?  Idk
        //Has unique VAO
        //Position in vao if not unique

        public ComponentRender(int vaoID, int iboID, int shaderID)
        {
            _vaoID = vaoID;
            _iboID = iboID;
            _shaderID = shaderID;
        }

        public void Update(float time)
        {
            //throw new NotImplementedException();
        }
    }
}
