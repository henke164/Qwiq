using QwiqCache.Models;
using System.CodeDom.Compiler;
using System.Linq;
using System.Text.RegularExpressions;

namespace QwiqCache.Services
{
    public class StructGenerator
    {
        public StructResult BuildStruct(string structString)
        {
            var name = Regex.Match(structString, "public struct [a-zA-Z]+");
            var structName = name.Value.Replace("public struct ", "");

            var codeDomProvider = CodeDomProvider.CreateProvider("CSharp");
            var compilerParameters = new CompilerParameters
            {
                GenerateExecutable = false,
                GenerateInMemory = true
            };

            compilerParameters.ReferencedAssemblies.Add("system.dll");

            var compilerResults = codeDomProvider.CompileAssemblyFromSource(compilerParameters, structString);

            if (compilerResults.Errors.Count > 0)
            {
                return null;
            }

            var assembly = compilerResults.CompiledAssembly;

            var instance = assembly.CreateInstance(structName);

            return new StructResult
            {
                Type = instance.GetType(),
                Name = structName
            };
        }
    }
}
