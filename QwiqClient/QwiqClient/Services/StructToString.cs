using System;

namespace QwiqClient.Services
{
    public static class StructToString
    {
        public static string CreateString(Type structType)
        {
            var structStr = $"public struct {structType.Name}\r\n";
            structStr += "{\r\n";

            var field = structType.GetFields();
            foreach (var property in field)
            {
                structStr += $"{property.Attributes.ToString().ToLower()} {ConvertFieldName(property.FieldType.Name)} {property.Name};\r\n";
            }

            structStr += "}";
            return structStr;
        }

        private static string ConvertFieldName(string fileTypeName)
        {
            switch (fileTypeName)
            {
                case "Int32": return "int";
                case "String": return "string";
            }

            return fileTypeName;
        }
    }
}
