// Decompiled with JetBrains decompiler
// Type: DigitalOpus.MB.Core.MBVersionEditor
// Assembly: MeshBakerEvalVersionEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2A8B8E34-4AF9-4434-BAA9-EF1230564558
// Assembly location: E:\Unity Workspace\AQHAT\Assets\MeshBaker\scripts\Editor\core\MeshBakerEvalVersionEditor.dll

using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DigitalOpus.MB.Core
{
    public class MBVersionEditor
    {
        private static MBVersionEditorInterface _MBVersion;

        private static MBVersionEditorInterface _CreateMBVersionConcrete()
        {
            return (MBVersionEditorInterface)Activator.CreateInstance(Type.GetType("DigitalOpus.MB.Core.MBVersionEditorConcrete,Assembly-CSharp-Editor"));
        }

        public static string GetPlatformString()
        {
            if (MBVersionEditor._MBVersion == null)
                MBVersionEditor._MBVersion = MBVersionEditor._CreateMBVersionConcrete();
            return MBVersionEditor._MBVersion.GetPlatformString();
        }

        public static void RegisterUndo(Object o, string s)
        {
            if (MBVersionEditor._MBVersion == null)
                MBVersionEditor._MBVersion = MBVersionEditor._CreateMBVersionConcrete();
            MBVersionEditor._MBVersion.RegisterUndo(o, s);
        }

        public static void SetInspectorLabelWidth(float width)
        {
            if (MBVersionEditor._MBVersion == null)
                MBVersionEditor._MBVersion = MBVersionEditor._CreateMBVersionConcrete();
            MBVersionEditor._MBVersion.SetInspectorLabelWidth(width);
        }
    }
}
