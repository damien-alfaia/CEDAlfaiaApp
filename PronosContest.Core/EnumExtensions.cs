using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace CEDAlfaiaApp.Core
{
    public static class EnumExtensions
    {
        public static TAttribute GetAttribute<TAttribute>(this Enum enumValue)
            where TAttribute : Attribute
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<TAttribute>();
        }
    }
}
