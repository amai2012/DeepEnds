//------------------------------------------------------------------------------
// <copyright file="Parse.cs" company="Zebedee Mason">
//     Copyright (c) 2016 Zebedee Mason.
//
//      The author's copyright is expressed through the following notice, thus
//      giving effective rights to copy and use this software to anyone, as shown
//      in the license text.
//
//     NOTICE:
//      This software is released under the terms of the GNU Lesser General Public
//      license; a copy of the text has been released with this package (see file
//      LICENSE, where the license text also appears), and can be found on the
//      GNU web site, at the following address:
//
//           http://www.gnu.org/licenses/old-licenses/lgpl-2.1.html
//
//      Please refer to the license text for any license information. This notice
//      has to be considered part of the license, and should be kept on every copy
//      integral or modified, of the source files. The removal of the reference to
//      the license will be considered an infringement of the license itself.
// </copyright>
//------------------------------------------------------------------------------

namespace DeepEnds.Decompile
{
    using Mono.Cecil;
    using System.Collections.Generic;

    public class Parse
    {
        private DeepEnds.Core.Parser parser;

        public Parse(DeepEnds.Core.Parser parser)
        {
            this.parser = parser;
        }

        private void Add(DeepEnds.Core.Dependent.Dependency leaf, TypeReference type)
        {
            var a = type.ToString();
            a = a.Replace("<", ",").Replace(">", ",").Replace("`", ",").Replace("[]", string.Empty).Replace("&", string.Empty).Replace("/", string.Empty);
            foreach (var bit in a.Split(','))
            {
                int i;
                if (!System.Int32.TryParse(bit, out i))
                {
                    this.parser.AddDependency(leaf, bit);
                }
            }
        }

        private void Process(DeepEnds.Core.Dependent.Dependency leaf, TypeDefinition type)
        {
            foreach (var field in type.Fields)
            {
                this.Add(leaf, field.FieldType);
            }

            foreach (var inter in type.Interfaces)
            {
                this.Add(leaf, inter);
            }

            foreach (var even in type.Events)
            {
                this.Add(leaf, even.EventType);
            }

            foreach (var prop in type.Properties)
            {
                this.Add(leaf, prop.PropertyType);
            }

            foreach (var attr in type.CustomAttributes)
            {
                this.Add(leaf, attr.Constructor.DeclaringType);
            }

            foreach (var method in type.Methods)
            {
                foreach (var p in method.CustomAttributes)
                {
                    this.Add(leaf, p.Constructor.DeclaringType);
                }

                foreach (var p in method.Parameters)
                {
                    this.Add(leaf, p.ParameterType);
                }

                this.Add(leaf, method.ReturnType);
                if (method.Body == null)
                {
                    continue;
                }

                foreach (var local in method.Body.Variables)
                {
                    this.Add(leaf, local.VariableType);
                }

                foreach (var instruction in method.Body.Instructions)
                {
                    var methodReference = instruction.Operand as MethodReference;
                    if (methodReference != null)
                    {
                        this.Add(leaf, methodReference.DeclaringType);
                        this.Add(leaf, methodReference.ReturnType);
                    }

                    var field = instruction.Operand as FieldDefinition;
                    if (field != null)
                    {
                        this.Add(leaf, field.DeclaringType);
                    }

                    var property = instruction.Operand as PropertyDefinition;
                    if (property != null)
                    {
                        this.Add(leaf, property.DeclaringType);
                        this.Add(leaf, property.PropertyType);
                    }
                }
            }
        }

        private void Add(TypeDefinition type)
        {
            var parent = this.parser.Dependencies.GetPath(type.Namespace, ".");
            var leaf = this.parser.Create(type.Name, type.FullName, parent);
            this.Process(leaf, type);
        }

        public void Read(string filename, System.IO.TextWriter logger)
        {
            try
            {
                ModuleDefinition module = ModuleDefinition.ReadModule(filename);
                foreach (TypeDefinition type in module.Types)
                {
                    if (type.FullName.Contains("`"))
                    {
                        continue;
                    }

                    if (type.FullName.Contains("<"))
                    {
                        continue;
                    }

                    if (!type.IsClass && !type.IsInterface)
                    {
                        continue;
                    }

                    this.Add(type);
                }
            }
            catch
            {
                logger.Write("! Cannot read file, is it actually a .NET assembly? ");
                logger.WriteLine(filename);
            }
        }
    }
}
