using MajorItemByAreaTracker.Settings;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RandomizerCore.Extensions;
using RandomizerMod.RC;
using System;
using System.Linq;
using System.Reflection;

namespace MajorItemByAreaTracker
{
    public static class HashModifier
    {
        [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
        internal class HashIgnoreAttribute : Attribute { };

        private class HashIgnoreContractResolver : DefaultContractResolver
        {
            public static HashIgnoreContractResolver Instance { get; } = new();

            private HashIgnoreContractResolver() : base() { }

            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                JsonProperty property = base.CreateProperty(member, memberSerialization);
                if (member.GetCustomAttribute(typeof(HashIgnoreAttribute)) != null)
                {
                    property.Ignored = true;
                }
                return property;
            }
        }

        public static int AdjustHash(RandoController rc, int orig)
        {
            if (MajorItemByAreaTracker.Instance.GS.Enabled)
            {
                return typeof(TrackerGlobalSettings).GetProperties()
                    .Where(p => p.GetCustomAttribute<HashIgnoreAttribute>() == null)
                    .OrderBy(p => p.Name)
                    .Select(p => $"{p.Name}: {p.GetValue(MajorItemByAreaTracker.Instance.GS)}".GetStableHashCode())
                    .Aggregate(98711, (current, v) => 70877 * current + v);
            }
            return 0;
        }
    }
}
