﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeeSynk.Core.Components;
using DeeSynk.Core.Components.GraphicsObjects.Lights;
using DeeSynk.Core.Components.Types.Render;
using DeeSynk.Core.Managers;
using DeeSynk.Core.Systems;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace DeeSynk.Core
{
    /// <summary>
    /// All objects and mechanic modeling should be housed here.
    /// </summary>
    public class Game
    {
        private World _world;
        private SystemInput _systemInput;
        public SystemInput SystemInput { get => _systemInput; }

        public Game()
        {
            Load();
        }

        /// <summary>
        /// Responsible for loading all resources that will be used within the game and engine.
        /// </summary>
        public void Load()
        {
            Managers.ShaderManager.GetInstance().Load();
            Managers.TextureManager.GetInstance().Load();
            Managers.ModelManager.GetInstance().Load();
            _world = new World();
            _systemInput = new SystemInput();
        }

        public void PushCameraRef(ref Camera camera)
        {
            _world.PushCameraRef(ref camera);
        }

        public void LoadGameData()
        {
            _world.CreateGameObject(Component.RENDER | Component.MODEL_STATIC | Component.TRANSFORM);
            _world.CreateGameObject(Component.RENDER | Component.MODEL_STATIC | Component.TRANSFORM | Component.TEXTURE);

            _world.CreateGameObject(Component.LIGHT);
            _world.CreateGameObject(Component.CAMERA);

            _world.InitData();

            _world.TextureComps[1] = new ComponentTexture(TextureManager.GetInstance().GetTexture("wood"), 0);

            _world.SystemVAO.InitVAORange(Buffers.VERTICES_NORMALS_ELEMENTS | Buffers.INTERLEAVED, 0, 0);
            _world.SystemVAO.InitVAORange(Buffers.VERTICES | Buffers.UVS | Buffers.FACE_ELEMENTS | Buffers.INTERLEAVED, 1, 1);


            var sm = ShaderManager.GetInstance();

            _world.RenderComps[0].PROGRAM_ID = sm.GetProgram("coloredPhongShaded");
            _world.RenderComps[1].PROGRAM_ID = sm.GetProgram("shadowTextured2");

            _world.RenderComps[0].ValidateData();
            _world.RenderComps[1].ValidateData();

            _world.LightComps[2] = new ComponentLight(LightType.SPOTLIGHT, 
                                                        new SpotLight(Color4.Red, 
                                                                        new Vector3(-3.0f, 5.0f, 6.0f), new Vector3(0), new Vector3(0, 1, 0),
                                                                        0.25f, 1.0f, 5f, 11f));

            _world.LightComps[2].LightObject.BuildUBO(3, 8);
            _world.LightComps[2].LightObject.AddShadowMap(2048, 2048);

            _world.LightComps[3] = new ComponentLight(LightType.SPOTLIGHT,
                                            new SpotLight(Color4.Blue,
                                                            new Vector3(3.0f, 5.0f, 6.0f), new Vector3(0), new Vector3(0, 1, 0),
                                                            0.25f, 1.0f, 5f, 11f));

            _world.LightComps[3].LightObject.BuildUBO(4, 8);
            _world.LightComps[3].LightObject.AddShadowMap(2048, 2048);

            _world.LightComps[4] = new ComponentLight(LightType.SPOTLIGHT,
                                new SpotLight(new Color4(0.0f, 1.0f, 0.0f, 1.0f),
                                                new Vector3(0.0f, 5.0f, 6.0f), new Vector3(0), new Vector3(0, 1, 0),
                                                0.25f, 1.0f, 5f, 11f));

            _world.LightComps[4].LightObject.BuildUBO(5, 8);
            _world.LightComps[4].LightObject.AddShadowMap(2048, 2048);

        }

        /// <summary>
        /// Performs and update call on the world object associated with Game.
        /// </summary>
        /// <param name="time">Previous time step.</param>
        public void Update(float time)
        {
            _world.Update(time);
            //Console.WriteLine(GL.GetError().ToString());
        }

        public void Render()
        {
            _world.Render();
        }
    }
}
