//------------------------------------------------------------------------------
// <copyright file="ParseTree.cs" company="Zebedee Mason">
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

namespace DeepEnds.CSharp
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class ParseTree
    {
        internal class TypeWalker : CSharpSyntaxWalker
        {
            private DeepEnds.Core.Parser parser;
            private DeepEnds.Core.Dependent.Dependency leaf;
            private SemanticModel model;

            public TypeWalker(DeepEnds.Core.Parser parser, DeepEnds.Core.Dependent.Dependency leaf, SemanticModel model)
            {
                this.parser = parser;
                this.leaf = leaf;
                this.model = model;
            }

            public override void Visit(SyntaxNode node)
            {
                base.Visit(node);
                var type = node as TypeSyntax;
                if (type == null)
                {
                    return;
                }

                var typeSymbol = this.model.GetSymbolInfo(type).Symbol as INamedTypeSymbol;
                if (typeSymbol != null)
                {
                    var fullName = typeSymbol.ToString();
                    this.parser.AddDependency(this.leaf, fullName);
                    return;
                }
            }
        }

        internal abstract class BaseTypeWalker
        {
            public abstract bool VisitBaseType(BaseTypeDeclarationSyntax basetype, string path);

            public bool Visit(BaseTypeDeclarationSyntax basetype, string path)
            {
                if (basetype == null)
                {
                    return true;
                }

                if (!this.VisitBaseType(basetype, path))
                {
                    return false;
                }

                var fullPath = basetype.Identifier.ToString();
                if (path != string.Empty)
                {
                    fullPath = path + "." + fullPath;
                }

                var classtype = basetype as ClassDeclarationSyntax;
                if (classtype != null)
                {
                    foreach (var member in classtype.Members)
                    {
                        if (!this.Visit(member as BaseTypeDeclarationSyntax, fullPath))
                        {
                            return false;
                        }
                    }

                    return true;
                }

                var structtype = basetype as StructDeclarationSyntax;
                if (structtype != null)
                {
                    foreach (var member in structtype.Members)
                    {
                        if (!this.Visit(member as BaseTypeDeclarationSyntax, fullPath))
                        {
                            return false;
                        }
                    }

                    return true;
                }

                return true;
            }

            public bool Visit(NamespaceDeclarationSyntax space, string path)
            {
                var fullPath = space.Name.ToString();
                if (path != string.Empty)
                {
                    fullPath = path + "." + fullPath;
                }

                foreach (var member in space.Members)
                {
                    var basetype = member as BaseTypeDeclarationSyntax;
                    if (basetype != null)
                    {
                        if (!this.Visit(basetype, fullPath))
                        {
                            return false;
                        }

                        continue;
                    }

                    var spacetype = member as NamespaceDeclarationSyntax;
                    if (spacetype != null)
                    {
                        if (!this.Visit(spacetype, fullPath))
                        {
                            return false;
                        }
                    }
                }

                return true;
            }

            public void Visit(SyntaxTree tree)
            {
                var root = tree.GetRoot() as CompilationUnitSyntax;
                foreach (var member in root.Members)
                {
                    var space = member as NamespaceDeclarationSyntax;
                    if (space == null)
                    {
                        continue;
                    }

                    if (!this.Visit(space, string.Empty))
                    {
                        break;
                    }
                }
            }
        }

        internal class MakeLeaves : BaseTypeWalker
        {
            private readonly DeepEnds.Core.Parser parser;

            private readonly DeepEnds.Core.Sources sources;

            private readonly string project;
            private readonly string filename;

            public MakeLeaves(DeepEnds.Core.Parser parser, DeepEnds.Core.Sources sources, string project, string filename)
            {
                this.parser = parser;
                this.sources = sources;
                this.project = project;
                this.filename = filename;
            }

            public override bool VisitBaseType(BaseTypeDeclarationSyntax basetype, string path)
            {
                var branch = this.parser.Dependencies.GetPath(path, ".");
                var leaf = this.parser.Create(basetype.Identifier.ValueText, path + "." + basetype.Identifier.ValueText, branch);
                this.sources.Create(leaf, new SourceProvider(leaf, this.project, this.filename));
                return true;
            }
        }

        internal class AddDependencies : BaseTypeWalker
        {
            private DeepEnds.Core.Parser parser;
            private readonly DeepEnds.Core.Linked.Dependencies dependencies;
            private SemanticModel model;

            public AddDependencies(DeepEnds.Core.Parser parser, SemanticModel model)
            {
                this.parser = parser;
                this.dependencies = parser.Dependencies;
                this.model = model;
            }

            public override bool VisitBaseType(BaseTypeDeclarationSyntax basetype, string path)
            {
                var branch = this.dependencies.GetPath(path, ".");
                foreach (var child in branch.Children)
                {
                    if (child.Name == basetype.Identifier.ValueText)
                    {
                        TypeWalker walker = new TypeWalker(this.parser, child, this.model);
                        walker.Visit(basetype);
                        break;
                    }
                }

                return true;
            }
        }

        public static void Process(DeepEnds.Core.Parser parser, DeepEnds.Core.Sources sources, SyntaxTree tree, string project, string filename)
        {
            var make = new MakeLeaves(parser, sources, project, filename);
            make.Visit(tree);
        }

        public static void Link(DeepEnds.Core.Parser parser, SyntaxTree tree, SemanticModel model)
        {
            var deps = new AddDependencies(parser, model);
            deps.Visit(tree);
        }

        internal class FindLeaf : BaseTypeWalker
        {
            private string path;

            public BaseTypeDeclarationSyntax Found { get; set; }

            public FindLeaf(string path)
            {
                this.path = path;
                this.Found = null;
            }

            public override bool VisitBaseType(BaseTypeDeclarationSyntax basetype, string path)
            {
                if (path + "." + basetype.Identifier == this.path)
                {
                    this.Found = basetype;
                    return false;
                }

                return true;
            }
        }

        public static BaseTypeDeclarationSyntax Find(string path, SyntaxTree tree)
        {
            var find = new FindLeaf(path);
            find.Visit(tree);
            return find.Found;
        }
    }
}
