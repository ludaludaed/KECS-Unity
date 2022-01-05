using System;
using System.Text;

namespace Ludaludaed.KECS.Unity.Editor {
    public static class StringExtensions {
        public static string ClearString(this string src) {
            src = src.Replace("_", "");
            var sb = new StringBuilder();
            var needUp = true;
            foreach (var c in src) {
                if (char.IsLetterOrDigit(c)) {
                    sb.Append(needUp ? char.ToUpperInvariant(c) : c);
                    needUp = false;
                } else {
                    needUp = true;
                }
            }

            return sb.ToString();
        }

        public static string GetCleanGenericTypeName(this Type type) {
            if (!type.IsGenericType) {
                return type.Name;
            }

            var constraints = "";
            foreach (var constraint in type.GetGenericArguments()) {
                constraints += constraints.Length > 0 ? $", {GetCleanGenericTypeName(constraint)}" : constraint.Name;
            }

            return $"{type.Name.Substring(0, type.Name.LastIndexOf("`", StringComparison.Ordinal))}<{constraints}>";
        }
    }
}