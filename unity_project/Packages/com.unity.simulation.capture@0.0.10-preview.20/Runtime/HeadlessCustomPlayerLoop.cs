#if PLATFORM_CLOUD_RENDERING && !UNITY_EDITOR
using System;
using System.Linq;
using System.IO;

using System.Collections;
using System.Collections.Generic;
using Unity.Simulation;
using UnityEngine;

using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

using UnityEngine.CloudRendering;

public class HeadlessCustomPlayerLoop
{
    public static class HeadlessServerLoopType
    {
        public struct HeadlessServerLoopTypePostLateUpdate { };
        public struct HeadlessServerLoopTypePostLateUpdateTwo { };
    }

    public static RenderTexture headlessTexture;
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Init()
    {
        if (headlessTexture == null)
        {
            var playerSettings = Resources.Load<PlayerResolutionSettings>("PlayerResolutionSettings");
            if (playerSettings == null)
            {
                headlessTexture.width = 640;
                headlessTexture.height = 480;
                Log.W("Player Settings resolution scriptable object not found, loading default of 640X480");
            }
            else
            {
                if (playerSettings.renderTexture != null)
                {
                    headlessTexture = playerSettings.renderTexture;
                    headlessTexture.width = playerSettings.playerResolution.width;
                    headlessTexture.height = playerSettings.playerResolution.height;
                }
                else
                {
                    headlessTexture = new RenderTexture(playerSettings.playerResolution.width, playerSettings.playerResolution.height, 1);
                }
            }
            
            if (headlessTexture.Create())
            {
                CloudGraphics.SetDefaultBackbufferSurface(headlessTexture);
            }
            else
            {
                Log.E("Failed to create a render texture for default backbuffer surface");
            }
        }
        var loopSystem = GenerateCustomLoop();
        PlayerLoop.SetPlayerLoop(loopSystem);
    }

    static void Insert(ref PlayerLoopSystem playerLoopSystem, Type playerLoopType, Func<List<PlayerLoopSystem>, bool> function)
    {
        for (int i = 0; i < playerLoopSystem.subSystemList.Length; i++)
        {
            var mainSystem = playerLoopSystem.subSystemList[i];
            if (mainSystem.type == playerLoopType)
            {
                var subSystemList = new List<PlayerLoopSystem>(mainSystem.subSystemList);
                if (function(subSystemList))
                {
                    mainSystem.subSystemList = subSystemList.ToArray();
                    playerLoopSystem.subSystemList[i] = mainSystem;
                    PlayerLoop.SetPlayerLoop(playerLoopSystem);
                    return;
                }
            }
        }
    }

    private static PlayerLoopSystem GenerateCustomLoop()
    {
        var playerLoop = PlayerLoop.GetCurrentPlayerLoop();

        Insert(ref playerLoop, typeof(PostLateUpdate), (subSystemList) =>
        {
            var headlessRenderCamera = new PlayerLoopSystem();
            headlessRenderCamera.type = typeof(HeadlessServerLoopType.HeadlessServerLoopTypePostLateUpdate);
            headlessRenderCamera.updateDelegate += () =>
            {
                {
                    GL.sRGBWrite = (QualitySettings.activeColorSpace == ColorSpace.Linear);

                    var cams = Camera.allCameras;
                    var offscreen = cams.Where(x => x.targetTexture != null);
                    var nonOffcreens = cams.Where(y => y.targetTexture == null);

                    foreach (var item in offscreen)
                    {
                        if (!item.enabled) continue;

                        item.Render();
                    }

                    Graphics.SetRenderTarget(null);

                    foreach (var item in nonOffcreens)
                    {
                        if (!item.enabled) continue;
                        item.Render();
                    }

                    Graphics.SetRenderTarget(null);
                }
            };

            for (int j = 0; j < subSystemList.Count; j++)
            {
                if (subSystemList[j].type == typeof(PostLateUpdate.FinishFrameRendering))
                {
                    subSystemList.Insert(j + 1, headlessRenderCamera);
                    return true;
                }
            }

            return true;
        });

        Insert(ref playerLoop, typeof(PostLateUpdate), (subSystemList) =>
        {
            var headlessRenderCamera = new PlayerLoopSystem();
            headlessRenderCamera.type = typeof(HeadlessServerLoopType.HeadlessServerLoopTypePostLateUpdateTwo);
            headlessRenderCamera.updateDelegate += () =>
            {

            };

            for (int j = 0; j < subSystemList.Count; j++)
            {
                if (subSystemList[j].type == typeof(PostLateUpdate.BatchModeUpdate))
                {
                    subSystemList.Insert(j + 1, headlessRenderCamera);
                    return true;
                }
            }

            return true;
        });

        return playerLoop;
    }
}
#endif