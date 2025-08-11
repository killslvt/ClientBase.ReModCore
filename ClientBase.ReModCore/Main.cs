using ClientBase.Features.Movement;
using ClientBase.Loader;
using ClientBase.SDK;
using MelonLoader;
using ReMod.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using VRC.Core;

namespace ClientBase
{
    //This is just a base for anyone wanting to make a client
    //Made by mhmswiss | https://discord.gg/Js5HJaWX2S
    //The ClientBase.rar file contains the icons needed to fix the System.IO.File.ReadAllBytes error
    internal class Main : MelonMod
    {
        #region ApplicationStart&Quit
        [Obsolete]
        public override void OnApplicationStart()
        {
            Logging.InitConsole();

            Logging.Log("Init Patches", LType.Info);
            Task.Run(() => SDK.Patching.Patch.Init());

            Logging.Log("Loading ClientBase", LType.Info);
            MelonCoroutines.Start(WaitForQuickMenu());

            Logging.Log("Initializing Mod Components", LType.Info);
            InitializeModComponents();
        }

        #region ComponentInit
        private static void InitializeModComponents()
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            IEnumerable<Type> types;

            try
            {
                types = executingAssembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types.Where(t => t != null);
            }

            var components = new List<ModComponents>();

            foreach (Type type in types)
            {
                if (!type.IsAbstract && type.BaseType == typeof(Loader.Module) && !type.IsDefined(typeof(ComponentDisabled), false))
                {
                    int priority = 0;
                    if (type.IsDefined(typeof(ComponentPriority), false))
                    {
                        priority = ((ComponentPriority)Attribute.GetCustomAttribute(type, typeof(ComponentPriority))).Priority;
                    }

                    components.Add(new ModComponents
                    {
                        Component = type,
                        Priority = priority
                    });
                }
            }

            foreach (var modComponents in components.OrderBy(component => component.Priority))
            {
                AddModComponent(modComponents.Component);
            }

            Logging.Log($"Created {Loader.QMLoader.Modules.Count} mod components.", LType.Info);
        }

        private static void AddModComponent(Type type)
        {
            try
            {
                Loader.Module syvoraModules = Activator.CreateInstance(type) as Loader.Module;
                Loader.QMLoader.Modules.Add(syvoraModules);
            }
            catch (Exception ex)
            {
                Logging.Log(string.Format("Failed creating {0}:\n{1}", type.Name, ex), LType.Error);
            }
        }
        #endregion

        public override void OnApplicationQuit()
        {
            //Nothing to do here.
            //Mostly used for saving configs or closing external apps for console logs
        }
        #endregion

        #region QMHandling
        private static IEnumerator WaitForQuickMenu()
        {
            while (APIUser.CurrentUser == null)
                yield return null;

            Logging.Log("Waiting for Quick Menu...", LType.Info);

            while (Camera.main == null || GameObject.Find("Canvas_QuickMenu(Clone)") == null)
                yield return null;

            Logging.Log("Quick Menu Loaded!", LType.Success);
            Loader.QMLoader.Init();
        }
        #endregion

        #region UpdateHandling
        public override void OnUpdate()
        {
            Flight.Update();
        }

        public override void OnFixedUpdate()
        {
            
        }
        #endregion

        #region SceneHandling
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            Logging.Log("Scene loaded: " + sceneName + " (Build Index: " + buildIndex + ")", LType.Debug);
        }

        public override void OnSceneWasUnloaded(int buildIndex, string sceneName)
        {
            Logging.Log("Scene unloaded: " + sceneName + " (Build Index: " + buildIndex + ")", LType.Debug);
        }
        #endregion
    }
}
