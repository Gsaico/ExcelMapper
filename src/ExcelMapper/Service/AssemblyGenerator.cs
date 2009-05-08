using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ExcelMapper.DTO;
using ExcelMapper.Service.FileService;

using Microsoft.CSharp;

namespace ExcelMapper.Service
{
    public class AssemblyGenerator : IAssemblyGenerator
    {
        private readonly IFileWriter _fileWriter;

        public AssemblyGenerator(IFileWriter fileWriter)
        {
            _fileWriter = fileWriter;
        }

        public bool Compile(List<ClassProperties> classPropertiesList, AssemblyProperties assemblyProperties, string logFile)
        {
            CSharpCodeProvider codeProvider = new CSharpCodeProvider(new Dictionary<string, string>
                {
                    { "CompilerVersion", "v3.5" }
                });

            CompilerParameters parameters = new CompilerParameters
                {
                    OutputAssembly = assemblyProperties.FullName
                };
            
            //assemblyProperties.References.Select(reference => parameters.ReferencedAssemblies.Add(reference));
            //assemblyProperties.Resources.Select(resource => parameters.EmbeddedResources.Add(resource));

            CompilerResults compilerResults = codeProvider.CompileAssemblyFromFile(parameters, classPropertiesList.Select(x => x.FullName).ToArray());

            GenerateErrorReport(compilerResults.Errors, logFile);

            return compilerResults.Errors.Count == 0;
        }

        private void GenerateErrorReport(CompilerErrorCollection errorsCollection, string logFile)
        {
            if (errorsCollection.Count == 0)
            {
                return;
            }
            _fileWriter.Create(logFile, FileMode.Append, FileAccess.Write);
            foreach (CompilerError error in errorsCollection)
            {
                _fileWriter.WriteLine(String.Format("{0}: {1}", error.FileName, error.ErrorText));
            }
            _fileWriter.Close();
        }
    }
}