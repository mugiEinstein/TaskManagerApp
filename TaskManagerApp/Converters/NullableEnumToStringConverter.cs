using System;
using System.Globalization;
using System.Windows.Data;

namespace TaskManagerApp.Converters
{
    /// <summary>
    /// 将可空枚举 (e.g. TaskState?) 转换为字符串：null 显示为 “全部” 或 ConverterParameter 指定文本；枚举值显示 ToString()
    /// ConvertBack 可将字符串解析回枚举或 null
    /// </summary>
    public class NullableEnumToStringConverter : IValueConverter
    {
        /// <summary>
        /// value: TaskState?；parameter: optional string，作为 null 时的显示文本
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                if (parameter is string p && !string.IsNullOrEmpty(p))
                    return p;
                return "全部";
            }
            return value.ToString();
        }

        /// <summary>
        /// 将文本转换为枚举或 null。若不需要双向绑定，可抛异常或返回 null。
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                return Binding.DoNothing;

            if (value is string s)
            {
                if (string.IsNullOrEmpty(s) || s == "全部")
                    return null;
                Type enumType = Nullable.GetUnderlyingType(targetType);
                try
                {
                    return Enum.Parse(enumType, s);
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }
    }
}
