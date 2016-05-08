using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using Microsoft.CSharp;
using System.Collections;

namespace dynamic
{
    //忘了为什么使用静态类了，没测试其他的行不行
    public static class dynamic
    {
        //生成类的代码
        public static CompilerResults compileCode(string sourceCSFile, string dllName)
        {


            CSharpCodeProvider compiler = new CSharpCodeProvider(
                new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } });


            CompilerParameters cp = new CompilerParameters();
            cp.ReferencedAssemblies.Add("System.dll");
            cp.ReferencedAssemblies.Add("System.Data.dll");
            cp.ReferencedAssemblies.Add(dllName);
            cp.ReferencedAssemblies.Add("someInterface.dll");
            cp.OutputAssembly = "dynamic.dll";
            cp.GenerateExecutable = false;


            CompilerResults cr = compiler.CompileAssemblyFromFile(cp, sourceCSFile);


            return cr;


        }

        //生成类的代码,带参数
        private static CodeCompileUnit CompileUnit(string fullClassName, ArrayList fields)
        {


            CodeCompileUnit compunit = new CodeCompileUnit();
            CodeNamespace sample = new CodeNamespace("dynamicNS");
            CodeTypeDeclaration myclass = new CodeTypeDeclaration("dynamicClass");


            #region 设置变量值的函数
            CodeMemberMethod setValues = new CodeMemberMethod();
            setValues.Name = "setValues";
            setValues.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            CodeParameterDeclarationExpression cp2 = new CodeParameterDeclarationExpression("System.Object", "obj");
            setValues.Parameters.Add(cp2);


            CodeVariableDeclarationStatement cv2 = DeclareVariables(fullClassName, "dllClass");
            setValues.Statements.Add(cv2);


            CodeTypeReference ct = new CodeTypeReference(fullClassName);


            CodeAssignStatement dllclass = new CodeAssignStatement(new CodeVariableReferenceExpression("dllClass"), new CodeCastExpression(fullClassName, new CodeVariableReferenceExpression("obj")));


            setValues.Statements.Add(dllclass);


            for (int i = 0; i < fields.Count; i = i + 3)
            {
                string name = "dllClass." + fields[i].ToString();
                if (fields[i + 2].ToString().IndexOf(',') == -1)
                {
                    CodeAssignStatement param = new CodeAssignStatement(new CodeVariableReferenceExpression(name), new CodePrimitiveExpression(fields[i + 2]));
                    setValues.Statements.Add(param);
                }
                else
                {
                    string[] temp = fields[i + 2].ToString().Split(',');
                    int len = temp.Length;
                    if (fields[i + 1].ToString() == "Int32[]")
                    {
                        for (int j = 0; j < len; j++)
                        {
                            // Create an array indexer expression that references index 5 of array "x"
                            CodeArrayIndexerExpression ci1 = new CodeArrayIndexerExpression(new CodeVariableReferenceExpression(name), new CodePrimitiveExpression(j));
                            // Assigns the value of the 10 to the integer variable "i".
                            CodeAssignStatement as1 = new CodeAssignStatement(ci1, new CodePrimitiveExpression(Convert.ToInt32(temp[j].Trim())));
                            setValues.Statements.Add(as1);
                        }
                    }
                    else if (fields[i + 1].ToString() == "String[]")
                    {
                        for (int j = 0; j < len; j++)
                        {
                            CodeArrayIndexerExpression ci1 = new CodeArrayIndexerExpression(new CodeVariableReferenceExpression(name), new CodePrimitiveExpression(j));
                            CodeAssignStatement as1 = new CodeAssignStatement(ci1, new CodePrimitiveExpression(temp[j].Trim()));
                            setValues.Statements.Add(as1);
                        }
                    }
                    else
                    {
                    }
                }
            }
            #endregion


            compunit.Namespaces.Add(sample);
            sample.Imports.Add(new CodeNamespaceImport("System"));
            sample.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
            sample.Imports.Add(new CodeNamespaceImport("System.ComponentModel"));
            sample.Imports.Add(new CodeNamespaceImport("System.Data")); ;
            sample.Imports.Add(new CodeNamespaceImport("System.Text"));


            sample.Types.Add(myclass);
            myclass.Members.Add(setValues);
            return compunit;
        }

        public static void generateCode(string filenm, string fullclassname, ArrayList fields)
        {
            CSharpCodeProvider gen = new CSharpCodeProvider();
            StreamWriter sw = new StreamWriter(filenm, false);


            gen.GenerateCodeFromCompileUnit(CompileUnit(fullclassname, fields), sw, new CodeGeneratorOptions());
            sw.Close();
        }

        //在网上找的的函数，解决了声明变量的问题.
        private static CodeVariableDeclarationStatement DeclareVariables(string dataType, string Name)
        {
            // 为将要创建的变量类型创建一个CodeTypeReference对象， 
            // 这使得我们不必去关注该类数据在特定语言环境中的 
            // 与数据类型有关的细节问题。 
            CodeTypeReference tr = new CodeTypeReference(dataType);
            // CodeVariableDeclarationStatement对象使得我们不必纠缠于 
            // 与特定语言有关的下列细节：在该语言的变量声明语句中， 
            // 应该是数据类型在前，还是变量名称在前；声明变量时是 
            // 否要用到Dim之类的关键词. 
            CodeVariableDeclarationStatement Declaration =
            new CodeVariableDeclarationStatement(tr, Name);
            // CodeObjectCreateExpression负责处理所有调用构造器的细节。 
            // 大多数情况下应该是new，但有时要使用New。但不管怎样， 
            // 我们不必去关注这些由语言类型决定的细节. 
            CodeObjectCreateExpression newStatement = new
            CodeObjectCreateExpression();
            // 指定我们要调用其构造器的对象. 
            newStatement.CreateType = tr;
            // 变量将通过调用其构造器的方式初始化. 
            Declaration.InitExpression = newStatement;
            return Declaration;
        }
    }
}