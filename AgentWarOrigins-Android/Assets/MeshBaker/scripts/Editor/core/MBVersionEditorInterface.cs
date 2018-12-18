using UnityEngine;

namespace DigitalOpus.MB.Core
{
    public interface MBVersionEditorInterface
    {
        string GetPlatformString();

        void RegisterUndo(Object o, string s);

        void SetInspectorLabelWidth(float width);
    }
}
