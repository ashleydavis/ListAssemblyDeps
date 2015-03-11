using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ListAssemblyDeps
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("Bad arguments!");
                Console.WriteLine("Usage:");
                Console.WriteLine("AssemblyDeps.exe <config-filename>");
                return 1;
            }

            var assemblyPath = args[0];
            if (!File.Exists(assemblyPath))
            {
                Console.Error.WriteLine("Assemmbly '" + assemblyPath + "' does not exist.");
                return 1;
            }

            try
            {
                DisplayAssemblyDetails(assemblyPath, 0, false);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Exception occurred: " + ex.Message);
                Console.WriteLine(ex.ToString());
            }

            return 0;
        }

        private static void DisplayAssemblyDetails(string assemblyPath, int indent, bool isReference)
        {
            var assemblyDir = Path.GetDirectoryName(assemblyPath);
            var assembly = Assembly.LoadFile(assemblyPath);
            var assemblyName = assembly.GetName();

            var indentStr = new string(' ', indent*4);

            if (isReference)
            {
                Console.WriteLine(indentStr + "Found: " + assemblyName.Name + " (" + assemblyName.Version + ")");
            }
            else
            {
                Console.WriteLine(indentStr + assemblyName.Name + " (" + assemblyName.Version + ")");
            }

            var refIndentStr = new string(' ', (indent+1)*4);

            foreach (var referencedAssemblyName in assembly.GetReferencedAssemblies())
            {
                Console.WriteLine(refIndentStr + referencedAssemblyName.Name + " (" + referencedAssemblyName.Version + ")");

                var referencedAssemblyPath = Path.Combine(assemblyDir, referencedAssemblyName.Name + ".dll");
                if (File.Exists( referencedAssemblyPath))
                {
                    DisplayAssemblyDetails(referencedAssemblyPath, indent+2, true);
                }
            }
        }
    }
}
