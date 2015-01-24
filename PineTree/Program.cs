using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PineTree.DomGenerator;
using PineTree.Interpreter;
using PineTree.Interpreter.Runtime;
using PineTree.Language;
using PineTree.Language.Lexer;
using PineTree.Language.Parser;

namespace PineTree
{
    internal class Program
    {
        private class ScriptContext
        {
            public void println(string value)
            {
                Console.WriteLine(value);
            }

            public string readln()
            {
                return Console.ReadLine();
            }
        }

        private static void GenerateXml(string path)
        {
            PineTreeDocumentGenerator gen = new PineTreeDocumentGenerator();

            PineTreeParser parser = new PineTreeParser();
            var ast = parser.ParseScript(File.ReadAllText(path));

            var document = gen.CreateDocument(ast);

            document.Save(Path.ChangeExtension(path, "xml"));

            Console.WriteLine(document.ToString());
            Console.ReadKey();
        }

        private static void Main(string[] args)
        {
            if (Array.IndexOf(args, "-x") != -1)
            {
                GenerateXml(args[Array.IndexOf(args, "-x") + 1]);
                return;
            }

            PineTreeEngine interpreter = new PineTreeEngine();

            foreach (var file in Directory.GetFiles(".").Where(g => Path.GetExtension(g) == ".pt"))
            {
                string contents = File.ReadAllText(file);
                interpreter.Execute(contents);
            }

            interpreter.SetValue("console", new ScriptContext());
            interpreter.SetValue("type", new Func<RuntimeValue, string>(g => g.Value.TypeName));
            interpreter.SetValue("exit", new Action(() => Environment.Exit(0)));

            do
            {
                Console.Write("PineTree> ");
                try
                {
                    string code = Console.ReadLine();

                    var value = interpreter.Execute(code);
                    if (value != RuntimeValue.Null)
                    {
                        Console.WriteLine(value.ToString());
                    }
                }
                catch (SyntaxException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch (RuntimeException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch (NotImplementedException)
                {
                    Console.WriteLine("Not implemented.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            } while (true);
        }
    }
}