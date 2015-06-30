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
                Console.WriteLine("ListAssemblyDeps.exe <assembly-file-path>");
                Console.WriteLine("ListAssemblyDeps.exe <directory-path>");
                return 1;
            }

            try
            {
                var assemblyPath = args[0];
                if (Directory.Exists(assemblyPath))
                {
                    foreach (var assemblyFilePath in Directory.GetFiles(assemblyPath, "*.dll"))
                    {
                        DisplayAssemblyDetails(assemblyFilePath, 0, false);
                    }
                    return 0;
                }
                else
                {
                    DisplayAssemblyDetails(assemblyPath, 0, false);
                }
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
            if (!File.Exists(assemblyPath))
            {
                var msg = "Assemmbly '" + assemblyPath + "' does not exist.";
                Console.Error.WriteLine(msg);
                throw new Exception(msg);
            }

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
                if (File.Exists(referencedAssemblyPath))
                {
                    DisplayAssemblyDetails(referencedAssemblyPath, indent+2, true);
                }
                else
                {
                    Console.WriteLine(refIndentStr + "    Not found!");
                }
            }
        }
    }
}
