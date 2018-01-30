﻿using System;
using Octokit.GraphQL.Core.Generation.Models;
using Octokit.GraphQL.Core.Introspection;
using Xunit;

namespace Octokit.GraphQL.Core.Generation.UnitTests
{
    public class EntityGenerationTests
    {
        const string MemberTemplate = @"namespace Test
{{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Octokit.GraphQL.Core;
    using Octokit.GraphQL.Core.Builders;

    public class Entity : QueryableValue<Entity>{1}
    {{
        public Entity(Expression expression) : base(expression)
        {{
        }}
{0}
        internal static Entity Create(Expression expression)
        {{
            return new Entity(expression);
        }}
    }}
}}";

        [Fact]
        public void Generates_Property_For_Scalar_Field()
        {
            var expected = FormatMemberTemplate("public int? Foo { get; }");

            var model = new TypeModel
            {
                Name = "Entity",
                Kind = TypeKind.Object,
                Fields = new[]
                {
                    new FieldModel
                    {
                        Name = "foo",
                        Type = TypeModel.Int()
                    },
                }
            };

            var result = CodeGenerator.Generate(model, "Test", null);
            
            Assert.Equal(new GeneratedFile(@"Model\Entity.cs", expected), result);
        }

        [Fact]
        public void Generates_Property_For_Deprecated_Scalar_Field()
        {
            var expected = FormatMemberTemplate($@"[Obsolete(@""Old and unused"")]{Environment.NewLine}        public int? Foo {{ get; }}");

            var model = new TypeModel
            {
                Name = "Entity",
                Kind = TypeKind.Object,
                Fields = new[]
                {
                    new FieldModel
                    {
                        Name = "foo",
                        Type = TypeModel.Int(),
                        IsDeprecated = true,
                        DeprecationReason = "Old and unused"
                    },
                }
            };

            var result = CodeGenerator.Generate(model, "Test", null);
            
            Assert.Equal(new GeneratedFile(@"Model\Entity.cs", expected), result);
        }

        [Fact]
        public void Generates_Property_For_Deprecated_WithEmptyReason_Scalar_Field()
        {
            var expected = FormatMemberTemplate($@"[Obsolete]{Environment.NewLine}        public int? Foo {{ get; }}");

            var model = new TypeModel
            {
                Name = "Entity",
                Kind = TypeKind.Object,
                Fields = new[]
                {
                    new FieldModel
                    {
                        Name = "foo",
                        Type = TypeModel.Int(),
                        IsDeprecated = true,
                        DeprecationReason = string.Empty
                    },
                }
            };

            var result = CodeGenerator.Generate(model, "Test", null);

            Assert.Equal(new GeneratedFile(@"Model\Entity.cs", expected), result);
        }

        [Fact]
        public void Generates_Property_For_Deprecated_WithWhitespaceReason_Scalar_Field()
        {
            var expected = FormatMemberTemplate($@"[Obsolete]{Environment.NewLine}        public int? Foo {{ get; }}");

            var model = new TypeModel
            {
                Name = "Entity",
                Kind = TypeKind.Object,
                Fields = new[]
                {
                    new FieldModel
                    {
                        Name = "foo",
                        Type = TypeModel.Int(),
                        IsDeprecated = true,
                        DeprecationReason = " "
                    },
                }
            };

            var result = CodeGenerator.Generate(model, "Test", null);

            Assert.Equal(new GeneratedFile(@"Model\Entity.cs", expected), result);
        }

        [Fact]
        public void Generates_Property_For_Deprecated_WithoutReason_Scalar_Field()
        {
            var expected = FormatMemberTemplate($@"[Obsolete]{Environment.NewLine}        public int? Foo {{ get; }}");

            var model = new TypeModel
            {
                Name = "Entity",
                Kind = TypeKind.Object,
                Fields = new[]
                {
                    new FieldModel
                    {
                        Name = "foo",
                        Type = TypeModel.Int(),
                        IsDeprecated = true
                    },
                }
            };

            var result = CodeGenerator.Generate(model, "Test", null);
            
            Assert.Equal(new GeneratedFile(@"Model\Entity.cs", expected), result);
        }

        [Fact]
        public void Generates_Property_For_NonNull_Scalar_Field()
        {
            var expected = FormatMemberTemplate("public int FooBar { get; }");

            var model = new TypeModel
            {
                Name = "Entity",
                Kind = TypeKind.Object,
                Fields = new[]
                {
                    new FieldModel
                    {
                        Name = "fooBar",
                        Type = TypeModel.NonNull(TypeModel.Int()),
                    },
                }
            };

            var result = CodeGenerator.Generate(model, "Test", null);

            Assert.Equal(new GeneratedFile(@"Model\Entity.cs", expected), result);
        }

        [Fact]
        public void Generates_Property_For_Object_Field()
        {
            var expected = FormatMemberTemplate("public Other Foo => this.CreateProperty(x => x.Foo, Test.Other.Create);");

            var model = new TypeModel
            {
                Name = "Entity",
                Kind = TypeKind.Object,
                Fields = new[]
                {
                    new FieldModel
                    {
                        Name = "foo",
                        Type = TypeModel.Object("Other")
                    },
                }
            };

            var result = CodeGenerator.Generate(model, "Test", null);

            Assert.Equal(new GeneratedFile(@"Model\Entity.cs", expected), result);
        }

        [Fact]
        public void Generates_Property_For_Deprecated_Object_Field()
        {
            var expected = FormatMemberTemplate($@"[Obsolete(@""Old and unused"")]{Environment.NewLine}        public Other Foo => this.CreateProperty(x => x.Foo, Test.Other.Create);");

            var model = new TypeModel
            {
                Name = "Entity",
                Kind = TypeKind.Object,
                Fields = new[]
                {
                    new FieldModel
                    {
                        Name = "foo",
                        Type = TypeModel.Object("Other"),
                        IsDeprecated = true,
                        DeprecationReason = "Old and unused"
                    },
                }
            };

            var result = CodeGenerator.Generate(model, "Test", null);

            Assert.Equal(new GeneratedFile(@"Model\Entity.cs", expected), result);
        }

        [Fact]
        public void Generates_Property_For_Deprecated_WithoutReason_Object_Field()
        {
            var expected = FormatMemberTemplate($@"[Obsolete]{Environment.NewLine}        public Other Foo => this.CreateProperty(x => x.Foo, Test.Other.Create);");

            var model = new TypeModel
            {
                Name = "Entity",
                Kind = TypeKind.Object,
                Fields = new[]
                {
                    new FieldModel
                    {
                        Name = "foo",
                        Type = TypeModel.Object("Other"),
                        IsDeprecated = true
                    },
                }
            };

            var result = CodeGenerator.Generate(model, "Test", null);

            Assert.Equal(new GeneratedFile(@"Model\Entity.cs", expected), result);
        }

        [Fact]
        public void Generates_Property_For_NonNull_Object_Field()
        {
            var expected = FormatMemberTemplate("public Other Foo => this.CreateProperty(x => x.Foo, Test.Other.Create);");

            var model = new TypeModel
            {
                Name = "Entity",
                Kind = TypeKind.Object,
                Fields = new[]
                {
                    new FieldModel
                    {
                        Name = "foo",
                        Type = TypeModel.NonNull(TypeModel.Object("Other")),
                    },
                }
            };

            var result = CodeGenerator.Generate(model, "Test", null);

            Assert.Equal(new GeneratedFile(@"Model\Entity.cs", expected), result);
        }

        [Fact]
        public void Generates_Property_For_Interface_Field()
        {
            var expected = FormatMemberTemplate("public IOther Foo => this.CreateProperty(x => x.Foo, Test.Internal.StubIOther.Create);");

            var model = new TypeModel
            {
                Name = "Entity",
                Kind = TypeKind.Object,
                Fields = new[]
                {
                    new FieldModel
                    {
                        Name = "foo",
                        Type = TypeModel.Interface("Other")
                    },
                }
            };

            var result = CodeGenerator.Generate(model, "Test", null);

            Assert.Equal(new GeneratedFile(@"Model\Entity.cs", expected), result);
        }

        [Fact]
        public void Generates_Property_For_Deprecated_Interface_Field()
        {
            var expected = FormatMemberTemplate($@"[Obsolete(@""Old and unused"")]{Environment.NewLine}        public IOther Foo => this.CreateProperty(x => x.Foo, Test.Internal.StubIOther.Create);");

            var model = new TypeModel
            {
                Name = "Entity",
                Kind = TypeKind.Object,
                Fields = new[]
                {
                    new FieldModel
                    {
                        Name = "foo",
                        Type = TypeModel.Interface("Other"),
                        IsDeprecated = true,
                        DeprecationReason = "Old and unused"
                    },
                }
            };

            var result = CodeGenerator.Generate(model, "Test", null);

            Assert.Equal(new GeneratedFile(@"Model\Entity.cs", expected), result);
        }

        [Fact]
        public void Generates_Property_For_Deprecated_WithoutReason_Interface_Field()
        {
            var expected = FormatMemberTemplate($@"[Obsolete]{Environment.NewLine}        public IOther Foo => this.CreateProperty(x => x.Foo, Test.Internal.StubIOther.Create);");

            var model = new TypeModel
            {
                Name = "Entity",
                Kind = TypeKind.Object,
                Fields = new[]
                {
                    new FieldModel
                    {
                        Name = "foo",
                        Type = TypeModel.Interface("Other"),
                        IsDeprecated = true,
                    },
                }
            };

            var result = CodeGenerator.Generate(model, "Test", null);

            Assert.Equal(new GeneratedFile(@"Model\Entity.cs", expected), result);
        }

        [Fact]
        public void Generates_Property_For_List_Field()
        {
            var expected = FormatMemberTemplate("public IQueryableList<Other> Foo => this.CreateProperty(x => x.Foo);");

            var model = new TypeModel
            {
                Name = "Entity",
                Kind = TypeKind.Object,
                Fields = new[]
                {
                    new FieldModel
                    {
                        Name = "foo",
                        Type = TypeModel.List(TypeModel.Object("Other")),
                    },
                }
            };

            var result = CodeGenerator.Generate(model, "Test", null);

            Assert.Equal(new GeneratedFile(@"Model\Entity.cs", expected), result);
        }

        [Fact]
        public void Generates_Property_For_List_Of_Ints()
        {
            var expected = FormatMemberTemplate("public IEnumerable<int> Foo { get; }");

            var model = new TypeModel
            {
                Name = "Entity",
                Kind = TypeKind.Object,
                Fields = new[]
                {
                    new FieldModel
                    {
                        Name = "foo",
                        Type = TypeModel.List(TypeModel.NonNull(TypeModel.Int())),
                    },
                }
            };

            var result = CodeGenerator.Generate(model, "Test", null);

            Assert.Equal(new GeneratedFile(@"Model\Entity.cs", expected), result);
        }

        [Fact]
        public void Generates_Property_For_List_Of_Nullable_Ints()
        {
            var expected = FormatMemberTemplate("public IEnumerable<int?> Foo { get; }");

            var model = new TypeModel
            {
                Name = "Entity",
                Kind = TypeKind.Object,
                Fields = new[]
                {
                    new FieldModel
                    {
                        Name = "foo",
                        Type = TypeModel.List(TypeModel.Int()),
                    },
                }
            };

            var result = CodeGenerator.Generate(model, "Test", null);

            Assert.Equal(new GeneratedFile(@"Model\Entity.cs", expected), result);
        }

        [Fact]
        public void Generates_Property_For_List_Of_Strings()
        {
            var expected = FormatMemberTemplate("public IEnumerable<string> Foo { get; }");

            var model = new TypeModel
            {
                Name = "Entity",
                Kind = TypeKind.Object,
                Fields = new[]
                {
                    new FieldModel
                    {
                        Name = "foo",
                        Type = TypeModel.List(TypeModel.String()),
                    },
                }
            };

            var result = CodeGenerator.Generate(model, "Test", null);

            Assert.Equal(new GeneratedFile(@"Model\Entity.cs", expected), result);
        }

        [Fact]
        public void Generates_Property_For_Deprecated_List_Field()
        {
            var expected = FormatMemberTemplate($@"[Obsolete(@""Old and unused"")]{Environment.NewLine}        public IQueryableList<Other> Foo => this.CreateProperty(x => x.Foo);");

            var model = new TypeModel
            {
                Name = "Entity",
                Kind = TypeKind.Object,
                Fields = new[]
                {
                    new FieldModel
                    {
                        Name = "foo",
                        Type = TypeModel.List(TypeModel.Object("Other")),
                        IsDeprecated = true,
                        DeprecationReason = "Old and unused"
                    },
                }
            };

            var result = CodeGenerator.Generate(model, "Test", null);

            Assert.Equal(new GeneratedFile(@"Model\Entity.cs", expected), result);
        }

        [Fact]
        public void Generates_Property_For_Deprecated_WithoutReason_List_Field()
        {
            var expected = FormatMemberTemplate($@"[Obsolete]{Environment.NewLine}        public IQueryableList<Other> Foo => this.CreateProperty(x => x.Foo);");

            var model = new TypeModel
            {
                Name = "Entity",
                Kind = TypeKind.Object,
                Fields = new[]
                {
                    new FieldModel
                    {
                        Name = "foo",
                        Type = TypeModel.List(TypeModel.Object("Other")),
                        IsDeprecated = true,
                    },
                }
            };

            var result = CodeGenerator.Generate(model, "Test", null);

            Assert.Equal(new GeneratedFile(@"Model\Entity.cs", expected), result);
        }

        [Fact]
        public void Generates_Property_For_NonNull_List_Of_NonNull_Enums_Field()
        {
            var expected = FormatMemberTemplate("public IEnumerable<Bar> Foo { get; }");

            var model = new TypeModel
            {
                Name = "Entity",
                Kind = TypeKind.Object,
                Fields = new[]
                {
                    new FieldModel
                    {
                        Name = "foo",
                        Type = TypeModel.NonNull(TypeModel.List(TypeModel.NonNull(TypeModel.Enum("Bar")))),
                    },
                }
            };

            var result = CodeGenerator.Generate(model, "Test", null);

            Assert.Equal(new GeneratedFile(@"Model\Entity.cs", expected), result);
        }

        [Fact]
        public void Generates_Property_For_NonNull_List_Of_Nullable_Enums_Field()
        {
            var expected = FormatMemberTemplate("public IEnumerable<Bar?> Foo { get; }");

            var model = new TypeModel
            {
                Name = "Entity",
                Kind = TypeKind.Object,
                Fields = new[]
                {
                    new FieldModel
                    {
                        Name = "foo",
                        Type = TypeModel.NonNull(TypeModel.List(TypeModel.Enum("Bar"))),
                    },
                }
            };

            var result = CodeGenerator.Generate(model, "Test", null);

            Assert.Equal(new GeneratedFile(@"Model\Entity.cs", expected), result);
        }

        [Fact]
        public void Generates_Property_For_Union_Field()
        {
            var expected = FormatMemberTemplate("public Bar Foo => this.CreateProperty(x => x.Foo, Test.Bar.Create);");

            var model = new TypeModel
            {
                Name = "Entity",
                Kind = TypeKind.Object,
                Fields = new[]
                {
                    new FieldModel
                    {
                        Name = "foo",
                        Type = TypeModel.NonNull(TypeModel.Union("Bar")),
                    },
                }
            };

            var result = CodeGenerator.Generate(model, "Test", null);

            Assert.Equal(new GeneratedFile(@"Model\Entity.cs", expected), result);
        }

        [Fact]
        public void Generates_Property_For_Deprecated_Union_Field()
        {
            var expected = FormatMemberTemplate($@"[Obsolete(@""Old and unused"")]{Environment.NewLine}        public Bar Foo => this.CreateProperty(x => x.Foo, Test.Bar.Create);");

            var model = new TypeModel
            {
                Name = "Entity",
                Kind = TypeKind.Object,
                Fields = new[]
                {
                    new FieldModel
                    {
                        Name = "foo",
                        Type = TypeModel.NonNull(TypeModel.Union("Bar")),
                        IsDeprecated = true,
                        DeprecationReason = "Old and unused"
                    },
                }
            };

            var result = CodeGenerator.Generate(model, "Test", null);

            Assert.Equal(new GeneratedFile(@"Model\Entity.cs", expected), result);
        }

        [Fact]
        public void Generates_Property_For_Deprecated_WithoutReason_Union_Field()
        {
            var expected = FormatMemberTemplate($@"[Obsolete]{Environment.NewLine}        public Bar Foo => this.CreateProperty(x => x.Foo, Test.Bar.Create);");

            var model = new TypeModel
            {
                Name = "Entity",
                Kind = TypeKind.Object,
                Fields = new[]
                {
                    new FieldModel
                    {
                        Name = "foo",
                        Type = TypeModel.NonNull(TypeModel.Union("Bar")),
                        IsDeprecated = true
                    },
                }
            };

            var result = CodeGenerator.Generate(model, "Test", null);

            Assert.Equal(new GeneratedFile(@"Model\Entity.cs", expected), result);
        }

        [Fact]
        public void Generates_Property_For_List_Of_Unions_Field()
        {
            var expected = FormatMemberTemplate("public IQueryableList<Bar> Foo => this.CreateProperty(x => x.Foo);");

            var model = new TypeModel
            {
                Name = "Entity",
                Kind = TypeKind.Object,
                Fields = new[]
                {
                    new FieldModel
                    {
                        Name = "foo",
                        Type = TypeModel.List(TypeModel.Union("Bar")),
                    },
                }
            };

            var result = CodeGenerator.Generate(model, "Test", null);

            Assert.Equal(new GeneratedFile(@"Model\Entity.cs", expected), result);
        }

        [Fact]
        public void Generates_Property_For_Deprecated_List_Of_Unions_Field()
        {
            var expected = FormatMemberTemplate($@"[Obsolete(@""Old and unused"")]{Environment.NewLine}        public IQueryableList<Bar> Foo => this.CreateProperty(x => x.Foo);");

            var model = new TypeModel
            {
                Name = "Entity",
                Kind = TypeKind.Object,
                Fields = new[]
                {
                    new FieldModel
                    {
                        Name = "foo",
                        Type = TypeModel.List(TypeModel.Union("Bar")),
                        IsDeprecated = true,
                        DeprecationReason = "Old and unused"
                    },
                }
            };

            var result = CodeGenerator.Generate(model, "Test", null);

            Assert.Equal(new GeneratedFile(@"Model\Entity.cs", expected), result);
        }

        [Fact]
        public void Generates_Property_For_Deprecated_WithoutReason_List_Of_Unions_Field()
        {
            var expected = FormatMemberTemplate($@"[Obsolete]{Environment.NewLine}        public IQueryableList<Bar> Foo => this.CreateProperty(x => x.Foo);");

            var model = new TypeModel
            {
                Name = "Entity",
                Kind = TypeKind.Object,
                Fields = new[]
                {
                    new FieldModel
                    {
                        Name = "foo",
                        Type = TypeModel.List(TypeModel.Union("Bar")),
                        IsDeprecated = true,
                    },
                }
            };

            var result = CodeGenerator.Generate(model, "Test", null);

            Assert.Equal(new GeneratedFile(@"Model\Entity.cs", expected), result);
        }

        [Fact]
        public void Generates_Method_For_Object_Field_With_Args()
        {
            var expected = FormatMemberTemplate("public Other Foo(int bar) => this.CreateMethodCall(x => x.Foo(bar), Test.Other.Create);");

            var model = new TypeModel
            {
                Name = "Entity",
                Kind = TypeKind.Object,
                Fields = new[]
                {
                    new FieldModel
                    {
                        Name = "foo",
                        Type = TypeModel.Object("Other"),
                        Args = new[]
                        {
                            new InputValueModel
                            {
                                Name = "bar",
                                Type = TypeModel.NonNull(TypeModel.Int()),
                            }
                        }
                    },
                }
            };

            var result = CodeGenerator.Generate(model, "Test", null);

            Assert.Equal(new GeneratedFile(@"Model\Entity.cs", expected), result);
        }

        [Fact]
        public void Generates_Method_For_NonNull_Object_Field_With_Args()
        {
            var expected = FormatMemberTemplate("public Other Foo(int bar) => this.CreateMethodCall(x => x.Foo(bar), Test.Other.Create);");

            var model = new TypeModel
            {
                Name = "Entity",
                Kind = TypeKind.Object,
                Fields = new[]
                {
                    new FieldModel
                    {
                        Name = "foo",
                        Type = TypeModel.NonNull(TypeModel.Object("Other")),
                        Args = new[]
                        {
                            new InputValueModel
                            {
                                Name = "bar",
                                Type = TypeModel.NonNull(TypeModel.Int()),
                            }
                        }
                    },
                }
            };

            var result = CodeGenerator.Generate(model, "Test", null);

            Assert.Equal(new GeneratedFile(@"Model\Entity.cs", expected), result);
        }

        [Fact]
        public void Generates_Method_For_Interface_Field_With_Args()
        {
            var expected = FormatMemberTemplate("public IOther Foo(int bar) => this.CreateMethodCall(x => x.Foo(bar), Test.Internal.StubIOther.Create);");

            var model = new TypeModel
            {
                Name = "Entity",
                Kind = TypeKind.Object,
                Fields = new[]
                {
                    new FieldModel
                    {
                        Name = "foo",
                        Type = TypeModel.Interface("Other"),
                        Args = new[]
                        {
                            new InputValueModel
                            {
                                Name = "bar",
                                Type = TypeModel.NonNull(TypeModel.Int()),
                            }
                        }
                    },
                }
            };

            var result = CodeGenerator.Generate(model, "Test", null);

            Assert.Equal(new GeneratedFile(@"Model\Entity.cs", expected), result);
        }

        [Fact]
        public void Generates_Method_For_List_Field_With_Int_Arg()
        {
            var expected = FormatMemberTemplate("public IQueryableList<Other> Foo(int? bar = null) => this.CreateMethodCall(x => x.Foo(bar));");

            var model = new TypeModel
            {
                Name = "Entity",
                Kind = TypeKind.Object,
                Fields = new[]
                {
                    new FieldModel
                    {
                        Name = "foo",
                        Type = TypeModel.List(TypeModel.Object("Other")),
                        Args = new[]
                        {
                            new InputValueModel
                            {
                                Name = "bar",
                                Type = TypeModel.Int(),
                            }
                        }
                    },
                }
            };

            var result = CodeGenerator.Generate(model, "Test", null);

            Assert.Equal(new GeneratedFile(@"Model\Entity.cs", expected), result);
        }

        [Fact]
        public void Generates_Method_For_List_Field_With_Object_List_Arg()
        {
            var expected = FormatMemberTemplate("public IQueryableList<Other> Foo(IEnumerable<Another> bar = null) => this.CreateMethodCall(x => x.Foo(bar));");

            var model = new TypeModel
            {
                Name = "Entity",
                Kind = TypeKind.Object,
                Fields = new[]
                {
                    new FieldModel
                    {
                        Name = "foo",
                        Type = TypeModel.List(TypeModel.Object("Other")),
                        Args = new[]
                        {
                            new InputValueModel
                            {
                                Name = "bar",
                                Type = TypeModel.List(TypeModel.Object("Another")),
                            }
                        }
                    },
                }
            };

            var result = CodeGenerator.Generate(model, "Test", null);

            Assert.Equal(new GeneratedFile(@"Model\Entity.cs", expected), result);
        }

        [Fact]
        public void Generates_Method_For_List_Field_With_NonNull_Enum_List_Arg()
        {
            var expected = FormatMemberTemplate("public IQueryableList<Other> Foo(IEnumerable<Another> bar = null) => this.CreateMethodCall(x => x.Foo(bar));");

            var model = new TypeModel
            {
                Name = "Entity",
                Kind = TypeKind.Object,
                Fields = new[]
                {
                    new FieldModel
                    {
                        Name = "foo",
                        Type = TypeModel.List(TypeModel.Object("Other")),
                        Args = new[]
                        {
                            new InputValueModel
                            {
                                Name = "bar",
                                Type = TypeModel.List(TypeModel.NonNull(TypeModel.Enum("Another"))),
                            }
                        }
                    },
                }
            };

            var result = CodeGenerator.Generate(model, "Test", null);

            Assert.Equal(new GeneratedFile(@"Model\Entity.cs", expected), result);
        }

        [Fact]
        public void Generates_Method_For_List_Field_With_Nullable_Enum_List_Arg()
        {
            var expected = FormatMemberTemplate("public IQueryableList<Other> Foo(IEnumerable<Another?> bar = null) => this.CreateMethodCall(x => x.Foo(bar));");

            var model = new TypeModel
            {
                Name = "Entity",
                Kind = TypeKind.Object,
                Fields = new[]
                {
                    new FieldModel
                    {
                        Name = "foo",
                        Type = TypeModel.List(TypeModel.Object("Other")),
                        Args = new[]
                        {
                            new InputValueModel
                            {
                                Name = "bar",
                                Type = TypeModel.List(TypeModel.Enum("Another")),
                            }
                        }
                    },
                }
            };

            var result = CodeGenerator.Generate(model, "Test", null);

            Assert.Equal(new GeneratedFile(@"Model\Entity.cs", expected), result);
        }

        [Fact]
        public void Generates_Method_For_Scalar_Field()
        {
            var expected = FormatMemberTemplate("public int? Foo(int? bar = null) => null;");

            var model = new TypeModel
            {
                Name = "Entity",
                Kind = TypeKind.Object,
                Fields = new[]
                {
                    new FieldModel
                    {
                        Name = "foo",
                        Type = TypeModel.Int(),
                        Args = new[]
                        {
                            new InputValueModel
                            {
                                Name = "bar",
                                Type = TypeModel.Int(),
                            }
                        }
                    },
                }
            };

            var result = CodeGenerator.Generate(model, "Test", null);

            Assert.Equal(new GeneratedFile(@"Model\Entity.cs", expected), result);
        }

        [Fact]
        public void Generates_Method_For_Deprecated_WithoutReason_Scalar_Field()
        {
            var expected = FormatMemberTemplate($@"[Obsolete(@""Old and unused"")]{Environment.NewLine}        public int? Foo(int? bar = null) => null;");

            var model = new TypeModel
            {
                Name = "Entity",
                Kind = TypeKind.Object,
                Fields = new[]
                {
                    new FieldModel
                    {
                        Name = "foo",
                        Type = TypeModel.Int(),
                        IsDeprecated = true,
                        DeprecationReason = "Old and unused",
                        Args = new[]
                        {
                            new InputValueModel
                            {
                                Name = "bar",
                                Type = TypeModel.Int(),
                            }
                        }
                    },
                }
            };

            var result = CodeGenerator.Generate(model, "Test", null);

            Assert.Equal(new GeneratedFile(@"Model\Entity.cs", expected), result);
        }

        [Fact]
        public void Generates_Method_For_Deprecated_Scalar_Field()
        {
            var expected = FormatMemberTemplate($@"[Obsolete]{Environment.NewLine}        public int? Foo(int? bar = null) => null;");

            var model = new TypeModel
            {
                Name = "Entity",
                Kind = TypeKind.Object,
                Fields = new[]
                {
                    new FieldModel
                    {
                        Name = "foo",
                        Type = TypeModel.Int(),
                        IsDeprecated = true,
                        Args = new[]
                        {
                            new InputValueModel
                            {
                                Name = "bar",
                                Type = TypeModel.Int(),
                            }
                        }
                    },
                }
            };

            var result = CodeGenerator.Generate(model, "Test", null);

            Assert.Equal(new GeneratedFile(@"Model\Entity.cs", expected), result);
        }

        [Fact]
        public void Generates_Method_For_NonNull_Scalar_Field()
        {
            var expected = FormatMemberTemplate("public int Foo(int? bar = null) => null;");

            var model = new TypeModel
            {
                Name = "Entity",
                Kind = TypeKind.Object,
                Fields = new[]
                {
                    new FieldModel
                    {
                        Name = "foo",
                        Type = TypeModel.NonNull(TypeModel.Int()),
                        Args = new[]
                        {
                            new InputValueModel
                            {
                                Name = "bar",
                                Type = TypeModel.Int(),
                            }
                        }
                    },
                }
            };

            var result = CodeGenerator.Generate(model, "Test", null);

            Assert.Equal(new GeneratedFile(@"Model\Entity.cs", expected), result);
        }

        [Fact]
        public void Generates_Method_For_Union_Field()
        {
            var expected = FormatMemberTemplate("public Bar Foo(int? bar = null) => this.CreateMethodCall(x => x.Foo(bar), Test.Bar.Create);");

            var model = new TypeModel
            {
                Name = "Entity",
                Kind = TypeKind.Object,
                Fields = new[]
                {
                    new FieldModel
                    {
                        Name = "foo",
                        Type = TypeModel.Union("Bar"),
                        Args = new[]
                        {
                            new InputValueModel
                            {
                                Name = "bar",
                                Type = TypeModel.Int(),
                            }
                        }
                    },
                }
            };

            var result = CodeGenerator.Generate(model, "Test", null);

            Assert.Equal(new GeneratedFile(@"Model\Entity.cs", expected), result);
        }

        [Theory]
        [InlineData(TypeKind.Scalar, "Int", "int")]
        [InlineData(TypeKind.Scalar, "Boolean", "bool")]
        [InlineData(TypeKind.Scalar, "String", "string")]
        [InlineData(TypeKind.InputObject, "InputObj", "InputObj")]
        public void NonNull_Arg_With_Null_DefaultValue_Has_No_Default(TypeKind argType, string type, string csharpType)
        {
            var expected = FormatMemberTemplate($"public IOther Foo({csharpType} bar) => this.CreateMethodCall(x => x.Foo(bar), Test.Internal.StubIOther.Create);");

            var model = new TypeModel
            {
                Name = "Entity",
                Kind = TypeKind.Object,
                Fields = new[]
                {
                    new FieldModel
                    {
                        Name = "foo",
                        Type = TypeModel.Interface("Other"),
                        Args = new[]
                        {
                            new InputValueModel
                            {
                                Name = "bar",
                                Type = TypeModel.NonNull(new TypeModel
                                {
                                    Kind = argType,
                                    Name = type,
                                }),
                            }
                        }
                    },
                }
            };

            var result = CodeGenerator.Generate(model, "Test", null);

            Assert.Equal(new GeneratedFile(@"Model\Entity.cs", expected), result);
        }

        [Theory]
        [InlineData(TypeKind.Scalar, "Int", "int", "5", "5")]
        [InlineData(TypeKind.Scalar, "Boolean", "bool", "true", "true")]
        [InlineData(TypeKind.Scalar, "String", "string", "foo", "\"foo\"")]
        [InlineData(TypeKind.Enum, "EnumType", "EnumType", "FOO", "EnumType.Foo")]
        public void NonNull_Arg_With_DefaultValue_Has_Default(TypeKind argType, string type, string csharpType, string defaultValue, string csharpDefaultValue)
        {
            var expected = FormatMemberTemplate($"public IOther Foo({csharpType} bar = {csharpDefaultValue}) => this.CreateMethodCall(x => x.Foo(bar), Test.Internal.StubIOther.Create);");

            var model = new TypeModel
            {
                Name = "Entity",
                Kind = TypeKind.Object,
                Fields = new[]
                {
                    new FieldModel
                    {
                        Name = "foo",
                        Type = TypeModel.Interface("Other"),
                        Args = new[]
                        {
                            new InputValueModel
                            {
                                Name = "bar",
                                Type = TypeModel.NonNull(new TypeModel
                                {
                                    Kind = argType,
                                    Name = type,
                                }),
                                DefaultValue = defaultValue,
                            }
                        }
                    },
                }
            };

            var result = CodeGenerator.Generate(model, "Test", null);

            Assert.Equal(new GeneratedFile(@"Model\Entity.cs", expected), result);
        }

        [Theory]
        [InlineData(TypeKind.Scalar, "Int", "int?")]
        [InlineData(TypeKind.Scalar, "Boolean", "bool?")]
        [InlineData(TypeKind.Scalar, "String", "string")]
        [InlineData(TypeKind.InputObject, "InputObj", "InputObj")]
        public void Nullable_Arg_With_No_DefaultValue_Has_Default(TypeKind argType, string type, string csharpType)
        {
            var expected = FormatMemberTemplate($"public IOther Foo({csharpType} bar = null) => this.CreateMethodCall(x => x.Foo(bar), Test.Internal.StubIOther.Create);");

            var model = new TypeModel
            {
                Name = "Entity",
                Kind = TypeKind.Object,
                Fields = new[]
                {
                    new FieldModel
                    {
                        Name = "foo",
                        Type = TypeModel.Interface("Other"),
                        Args = new[]
                        {
                            new InputValueModel
                            {
                                Name = "bar",
                                Type = new TypeModel
                                {
                                    Kind = argType,
                                    Name = type,
                                },
                            }
                        }
                    },
                }
            };

            var result = CodeGenerator.Generate(model, "Test", null);

            Assert.Equal(new GeneratedFile(@"Model\Entity.cs", expected), result);
        }

        [Fact]
        public void Args_With_Default_Values_Come_After_Args_With_No_Default_Values()
        {
            var expected = FormatMemberTemplate(
                "public IOther Foo(int req1, int req2, int? opt1 = null, int opt2 = 5) => " + 
                "this.CreateMethodCall(x => x.Foo(req1, req2, opt1, opt2), Test.Internal.StubIOther.Create);");

            var model = new TypeModel
            {
                Name = "Entity",
                Kind = TypeKind.Object,
                Fields = new[]
                {
                    new FieldModel
                    {
                        Name = "foo",
                        Type = TypeModel.Interface("Other"),
                        Args = new[]
                        {
                            new InputValueModel { Name = "req1", Type = TypeModel.NonNull(TypeModel.Int()) },
                            new InputValueModel { Name = "opt1", Type = TypeModel.Int() },
                            new InputValueModel { Name = "req2", Type = TypeModel.NonNull(TypeModel.Int()) },
                            new InputValueModel { Name = "opt2", Type = TypeModel.NonNull(TypeModel.Int()), DefaultValue = "5" },
                        }
                    },
                }
            };

            var result = CodeGenerator.Generate(model, "Test", null);

            Assert.Equal(new GeneratedFile(@"Model\Entity.cs", expected), result);
        }

        [Fact]
        public void Generates_Doc_Comments_For_Class()
        {
            var expected = @"namespace Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Octokit.GraphQL.Core;
    using Octokit.GraphQL.Core.Builders;

    /// <summary>
    /// Testing if doc comments are generated.
    /// </summary>
    public class Entity : QueryableValue<Entity>
    {
        public Entity(Expression expression) : base(expression)
        {
        }

        internal static Entity Create(Expression expression)
        {
            return new Entity(expression);
        }
    }
}";

            var model = new TypeModel
            {
                Name = "Entity",
                Description = "Testing if doc comments are generated.",
                Kind = TypeKind.Object,
                Fields = new FieldModel[0],
            };

            var result = CodeGenerator.Generate(model, "Test", null);

            Assert.Equal(new GeneratedFile(@"Model\Entity.cs", expected), result);
        }

        [Fact]
        public void Generates_Doc_Comments_For_Property()
        {
            var expected = FormatMemberTemplate(@"/// <summary>
        /// Testing if doc comments are generated.
        /// </summary>
        public Other Foo => this.CreateProperty(x => x.Foo, Test.Other.Create);");

            var model = new TypeModel
            {
                Name = "Entity",
                Kind = TypeKind.Object,
                Fields = new[]
                {
                    new FieldModel
                    {
                        Name = "Foo",
                        Description = "Testing if doc comments are generated.",
                        Type = TypeModel.Object("Other")
                    },
                }
            };

            var result = CodeGenerator.Generate(model, "Test", null);

            Assert.Equal(new GeneratedFile(@"Model\Entity.cs", expected), result);
        }

        [Fact]
        public void Generates_Doc_Comments_For_Method()
        {
            var expected = FormatMemberTemplate(@"/// <summary>
        /// Testing if doc comments are generated.
        /// </summary>
        /// <param name=""arg1"">The first argument.</param>
        /// <param name=""arg2"">The second argument. With a windows newline. Ending with a space.</param>
        public Other Foo(int? arg1 = null, int? arg2 = null) => this.CreateMethodCall(x => x.Foo(arg1, arg2), Test.Other.Create);");

            var model = new TypeModel
            {
                Name = "Entity",
                Kind = TypeKind.Object,
                Fields = new[]
                {
                    new FieldModel
                    {
                        Name = "foo",
                        Description = "Testing if doc comments are generated.",
                        Type = TypeModel.Object("Other"),
                        Args = new[]
                        {
                            new InputValueModel
                            {
                                Name = "arg1",
                                Description = "The first argument.",
                                Type = TypeModel.Int(),
                            },
                            new InputValueModel
                            {
                                Name = "arg2",
                                Description = "The second argument.\r\nWith a windows newline. Ending with a space. ",
                                Type = TypeModel.Int(),
                            },
                        }
                    },
                }
            };

            var result = CodeGenerator.Generate(model, "Test", null);

            Assert.Equal(new GeneratedFile(@"Model\Entity.cs", expected), result);
        }

        [Fact]
        public void Generates_Multi_Line_Doc_Comments_For_Method()
        {
            var expected = FormatMemberTemplate(@"/// <summary>
        /// Testing if doc comments are generated.
        /// Testing if doc comments are generated.
        /// Testing if doc comments are generated.
        /// </summary>
        /// <param name=""arg1"">The first argument.</param>
        /// <param name=""arg2"">The second argument. With a linux newline. Ending with a newline.</param>
        public Other Foo(int? arg1 = null, int? arg2 = null) => this.CreateMethodCall(x => x.Foo(arg1, arg2), Test.Other.Create);");

            var model = new TypeModel
            {
                Name = "Entity",
                Kind = TypeKind.Object,
                Fields = new[]
                {
                    new FieldModel
                    {
                        Name = "foo",
                        Description = "Testing if doc comments are generated.\r\nTesting if doc comments are generated.\r\nTesting if doc comments are generated.\r\n",
                        Type = TypeModel.Object("Other"),
                        Args = new[]
                        {
                            new InputValueModel
                            {
                                Name = "arg1",
                                Description = "The first argument.",
                                Type = TypeModel.Int(),
                            },
                            new InputValueModel
                            {
                                Name = "arg2",
                                Description = "The second argument.\nWith a linux newline. Ending with a newline.\n",
                                Type = TypeModel.Int(),
                            },
                        }
                    },
                }
            };

            var result = CodeGenerator.Generate(model, "Test", null);

            Assert.Equal(new GeneratedFile(@"Model\Entity.cs", expected), result);
        }

        [Fact]
        public void Generates_Root_Query()
        {
            var expected = @"namespace Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Octokit.GraphQL.Core;
    using Octokit.GraphQL.Core.Builders;

    public class Entity : QueryableValue<Entity>, IQuery
    {
        public Entity() : base(null)
        {
        }

        public Entity(Expression expression) : base(expression)
        {
        }

        public Other Foo => this.CreateProperty(x => x.Foo, Test.Other.Create);

        internal static Entity Create(Expression expression)
        {
            return new Entity(expression);
        }
    }
}";

            var model = new TypeModel
            {
                Name = "Entity",
                Kind = TypeKind.Object,
                Fields = new[]
                {
                    new FieldModel
                    {
                        Name = "foo",
                        Type = TypeModel.Object("Other")
                    },
                }
            };

            var result = CodeGenerator.Generate(model, "Test", model.Name);

            Assert.Equal(new GeneratedFile(@"Entity.cs", expected), result);
        }

        [Fact]
        public void Generates_Mutation_Under_Query()
        {
            var expected = @"namespace Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Octokit.GraphQL.Core;
    using Octokit.GraphQL.Core.Builders;

    public class Mutation : QueryableValue<Mutation>, IMutation
    {
        public Mutation() : base(null)
        {
        }

        public Mutation(Expression expression) : base(expression)
        {
        }

        public Other Foo => this.CreateProperty(x => x.Foo, Test.Other.Create);

        internal static Mutation Create(Expression expression)
        {
            return new Mutation(expression);
        }
    }
}";

            var model = new TypeModel
            {
                Name = "Mutation",
                Kind = TypeKind.Object,
                Fields = new[]
                {
                    new FieldModel
                    {
                        Name = "foo",
                        Type = TypeModel.Object("Other")
                    },
                }
            };

            var result = CodeGenerator.Generate(model, "Test", "Query");

            Assert.Equal(new GeneratedFile(@"Mutation.cs", expected), result);
        }

        [Fact]
        public void Implements_Interfaces()
        {
            var expected = string.Format(
                MemberTemplate,
                null,
                ", IFoo, IBar");

            var model = new TypeModel
            {
                Name = "Entity",
                Kind = TypeKind.Object,
                Interfaces = new[]
                {
                    new TypeModel { Name = "Foo" },
                    new TypeModel { Name = "Bar" },
                }
            };

            var result = CodeGenerator.Generate(model, "Test", null);

            Assert.Equal(new GeneratedFile(@"Model\Entity.cs", expected), result);
        }

        private string FormatMemberTemplate(string members, string interfaces = null)
        {
            if (members != null)
            {
                members = "\r\n        " + members + "\r\n";
            }

            return string.Format(MemberTemplate, members, interfaces);
        }
    }
}
