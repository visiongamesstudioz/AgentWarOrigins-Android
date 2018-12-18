﻿using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DigitalOpus.MB.Core
{
    public class MBVersion
    {
        private static MBVersionInterface _MBVersion;

        private static MBVersionInterface _CreateMBVersionConcrete()
        {
            return (MBVersionInterface)Activator.CreateInstance(Type.GetType("DigitalOpus.MB.Core.MBVersionConcrete,Assembly-CSharp"));
        }

        public static string version()
        {
            if (MBVersion._MBVersion == null)
                MBVersion._MBVersion = MBVersion._CreateMBVersionConcrete();
            return MBVersion._MBVersion.version();
        }

        public static int GetMajorVersion()
        {
            if (MBVersion._MBVersion == null)
                MBVersion._MBVersion = MBVersion._CreateMBVersionConcrete();
            return MBVersion._MBVersion.GetMajorVersion();
        }

        public static int GetMinorVersion()
        {
            if (MBVersion._MBVersion == null)
                MBVersion._MBVersion = MBVersion._CreateMBVersionConcrete();
            return MBVersion._MBVersion.GetMinorVersion();
        }

        public static bool GetActive(GameObject go)
        {
            if (MBVersion._MBVersion == null)
                MBVersion._MBVersion = MBVersion._CreateMBVersionConcrete();
            return MBVersion._MBVersion.GetActive(go);
        }

        public static void SetActive(GameObject go, bool isActive)
        {
            if (MBVersion._MBVersion == null)
                MBVersion._MBVersion = MBVersion._CreateMBVersionConcrete();
            MBVersion._MBVersion.SetActive(go, isActive);
        }

        public static void SetActiveRecursively(GameObject go, bool isActive)
        {
            if (MBVersion._MBVersion == null)
                MBVersion._MBVersion = MBVersion._CreateMBVersionConcrete();
            MBVersion._MBVersion.SetActiveRecursively(go, isActive);
        }

        public static Object[] FindSceneObjectsOfType(Type t)
        {
            if (MBVersion._MBVersion == null)
                MBVersion._MBVersion = MBVersion._CreateMBVersionConcrete();
            return MBVersion._MBVersion.FindSceneObjectsOfType(t);
        }

        public static bool IsRunningAndMeshNotReadWriteable(Mesh m)
        {
            if (MBVersion._MBVersion == null)
                MBVersion._MBVersion = MBVersion._CreateMBVersionConcrete();
            return MBVersion._MBVersion.IsRunningAndMeshNotReadWriteable(m);
        }

        public static Vector2[] GetMeshUV3orUV4(Mesh m, bool get3, MB2_LogLevel LOG_LEVEL)
        {
            if (MBVersion._MBVersion == null)
                MBVersion._MBVersion = MBVersion._CreateMBVersionConcrete();
            return MBVersion._MBVersion.GetMeshUV3orUV4(m, get3, LOG_LEVEL);
        }

        public static void MeshClear(Mesh m, bool t)
        {
            if (MBVersion._MBVersion == null)
                MBVersion._MBVersion = MBVersion._CreateMBVersionConcrete();
            MBVersion._MBVersion.MeshClear(m, t);
        }

        public static void MeshAssignUV3(Mesh m, Vector2[] uv3s)
        {
            if (MBVersion._MBVersion == null)
                MBVersion._MBVersion = MBVersion._CreateMBVersionConcrete();
            MBVersion._MBVersion.MeshAssignUV3(m, uv3s);
        }

        public static void MeshAssignUV4(Mesh m, Vector2[] uv4s)
        {
            if (MBVersion._MBVersion == null)
                MBVersion._MBVersion = MBVersion._CreateMBVersionConcrete();
            MBVersion._MBVersion.MeshAssignUV4(m, uv4s);
        }

        public static Vector4 GetLightmapTilingOffset(Renderer r)
        {
            if (MBVersion._MBVersion == null)
                MBVersion._MBVersion = MBVersion._CreateMBVersionConcrete();
            return MBVersion._MBVersion.GetLightmapTilingOffset(r);
        }

        public static Transform[] GetBones(Renderer r)
        {
            if (MBVersion._MBVersion == null)
                MBVersion._MBVersion = MBVersion._CreateMBVersionConcrete();
            return MBVersion._MBVersion.GetBones(r);
        }

        public static void OptimizeMesh(Mesh m)
        {
            if (MBVersion._MBVersion == null)
                MBVersion._MBVersion = MBVersion._CreateMBVersionConcrete();
            MBVersion._MBVersion.OptimizeMesh(m);
        }

        public static int GetBlendShapeFrameCount(Mesh m, int shapeIndex)
        {
            if (MBVersion._MBVersion == null)
                MBVersion._MBVersion = MBVersion._CreateMBVersionConcrete();
            return MBVersion._MBVersion.GetBlendShapeFrameCount(m, shapeIndex);
        }

        public static float GetBlendShapeFrameWeight(Mesh m, int shapeIndex, int frameIndex)
        {
            if (MBVersion._MBVersion == null)
                MBVersion._MBVersion = MBVersion._CreateMBVersionConcrete();
            return MBVersion._MBVersion.GetBlendShapeFrameWeight(m, shapeIndex, frameIndex);
        }

        public static void GetBlendShapeFrameVertices(Mesh m, int shapeIndex, int frameIndex, Vector3[] vs, Vector3[] ns, Vector3[] ts)
        {
            if (MBVersion._MBVersion == null)
                MBVersion._MBVersion = MBVersion._CreateMBVersionConcrete();
            MBVersion._MBVersion.GetBlendShapeFrameVertices(m, shapeIndex, frameIndex, vs, ns, ts);
        }

        public static void ClearBlendShapes(Mesh m)
        {
            if (MBVersion._MBVersion == null)
                MBVersion._MBVersion = MBVersion._CreateMBVersionConcrete();
            MBVersion._MBVersion.ClearBlendShapes(m);
        }

        public static void AddBlendShapeFrame(Mesh m, string nm, float wt, Vector3[] vs, Vector3[] ns, Vector3[] ts)
        {
            if (MBVersion._MBVersion == null)
                MBVersion._MBVersion = MBVersion._CreateMBVersionConcrete();
            MBVersion._MBVersion.AddBlendShapeFrame(m, nm, wt, vs, ns, ts);
        }
    }
}
