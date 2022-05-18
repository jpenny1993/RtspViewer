using System;
using System.Collections.Generic;

namespace RtspViewer.WinUi.Converters
{
    public sealed class EnumListItemSource<TEnum> where TEnum : struct, IConvertible
    {
        public string FullTypeString { get; set; }

        public string Name { get; set; }

        public TEnum Value { get; set; }

        public EnumListItemSource(TEnum value, string name, string fullTypeString)
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException($"{nameof(TEnum)} must be an enum type.", nameof(value));
            }

            Name = name;
            Value = value;
            FullTypeString = fullTypeString;
        }

        public static List<EnumListItemSource<TEnum>> GetListItems()
        {
            var type = typeof(TEnum);
            var names = Enum.GetNames(type);
            var values = (TEnum[])Enum.GetValues(type);
            var list = new List<EnumListItemSource<TEnum>>();
            for (var i = 0; i < names.Length; i++)
            {
                var item = new EnumListItemSource<TEnum>(values[i], names[i], $"{type.Name}.{names[i]}");
                list.Add(item);
            }
            return list;
        }
    }
}
