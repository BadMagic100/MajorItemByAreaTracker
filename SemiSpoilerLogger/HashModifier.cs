using MajorItemByAreaTracker.UI;
using MenuChanger.Extensions;
using MonoMod.RuntimeDetour;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RandomizerMod.RC;
using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace MajorItemByAreaTracker
{
    public static class HashModifier
    {
        private static Hook? hook;
        private static readonly MethodInfo randoControllerHash = typeof(RandoController).GetMethod(nameof(RandoController.Hash));

        public static void Hook()
        {
            hook = new Hook(randoControllerHash, typeof(HashModifier).GetMethod(nameof(AdjustHash), BindingFlags.NonPublic | BindingFlags.Static));
        }

        private class IgnoreUIContractResolver : DefaultContractResolver
        {
            public static IgnoreUIContractResolver Instance { get; } = new();

            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                JsonProperty property = base.CreateProperty(member, memberSerialization);
                if (member.GetValueType() == typeof(UIDisplayType))
                {
                    property.Ignored = true;
                }
                return property;
            }
        }

        private static int AdjustHash(Func<RandoController, int> orig, RandoController self)
        {
            int result = orig(self);
            if (MajorItemByAreaTracker.Instance.GS.Enabled)
            {
                using SHA256Managed sha256 = new();
                using StringWriter sw = new();
                JsonSerializer ser = new()
                { 
                    DefaultValueHandling = DefaultValueHandling.Include,
                    Formatting = Formatting.None,
                    TypeNameHandling = TypeNameHandling.Auto,
                    ContractResolver = IgnoreUIContractResolver.Instance,
                };
                ser.Serialize(sw, MajorItemByAreaTracker.Instance.GS);

                StringBuilder sb = sw.GetStringBuilder();
                byte[] sha = sha256.ComputeHash(Encoding.UTF8.GetBytes(sw.ToString()));

                int seed = 17;
                for (int i = 0; i < sha.Length; i++) seed = 31 * seed ^ sha[i];

                result += seed;
            }
            return result;
        }
    }
}
