namespace Unosquare.Blazorific.Common
{
    using System;
    using System.Collections.Generic;

    public sealed class AttributeDictionary
    {
        private readonly Action ChangeCallback;
        private readonly Dictionary<string, object> Attributes =
            new Dictionary<string, object>(16, StringComparer.InvariantCultureIgnoreCase);

        internal AttributeDictionary(Action changeCallback)
        {
            ChangeCallback = changeCallback;
        }

        public object this[string name]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(name)) return null;
                return Attributes.TryGetValue(name, out var value) ? value : null;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(name)) return;
                Attributes[name] = value;
                ChangeCallback?.Invoke();
            }
        }

        public string CssClass
        {
            get => this["class"] as string;
            set => this["class"] = value;
        }

        public string Style
        {
            get => this["style"] as string;
            set => this["style"] = value;
        }

        public IEnumerable<string> Names => Attributes.Keys;

        public IEnumerable<object> Values => Attributes.Values;

        public IReadOnlyDictionary<string, object> Dictionary => Attributes;

        public void Remove(string name)
        {
            if (Attributes.ContainsKey(name))
            {
                Attributes.Remove(name);
                ChangeCallback?.Invoke();
            }
        }

        public void Clear()
        {
            Attributes.Clear();
            ChangeCallback?.Invoke();
        }

        public IDictionary<string, object> ToDictionary => new Dictionary<string, object>(Attributes);

        public void Replace(IDictionary<string, object> other)
        {
            Attributes.Clear();
            Merge(other);
        }

        public void Merge(IDictionary<string, object> other)
        {
            if (other == null)
            {
                foreach (var kvp in other)
                    Attributes[kvp.Key] = kvp.Value;
            }

            ChangeCallback?.Invoke();
        }
    }
}
