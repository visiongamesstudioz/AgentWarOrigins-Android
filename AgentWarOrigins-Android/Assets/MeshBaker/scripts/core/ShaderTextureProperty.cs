using System;
using System.Collections.Generic;

namespace DigitalOpus.MB.Core
{
    [Serializable]
    public class ShaderTextureProperty
    {
        public string name;
        public bool isNormalMap;

        public ShaderTextureProperty(string n, bool norm)
        {
            this.name = n;
            this.isNormalMap = norm;
        }

        public static string[] GetNames(List<ShaderTextureProperty> props)
        {
            string[] strArray = new string[props.Count];
            for (int index = 0; index < strArray.Length; ++index)
                strArray[index] = props[index].name;
            return strArray;
        }
    }
}
