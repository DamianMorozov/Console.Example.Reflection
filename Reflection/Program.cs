using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace ExampleReflection
{
    internal class Program
    {
        public int MyProperty { get; set; }

        private static void Main()
        {
            var numberMenu = -1;
            while (numberMenu != 0)
            {
                PrintCaption();
                try
                {
                    numberMenu = Convert.ToInt32(Console.ReadLine());
                }
                catch (Exception exception)
                {
                    Console.WriteLine("Error: " + exception.Message);
                    numberMenu = -1;
                }
                Console.WriteLine();
                PrintSwitch(numberMenu);
            }
        }

        private static void PrintCaption()
        {
            Console.Clear();
            Console.WriteLine(@"----------------------------------------------------------------------");
            Console.WriteLine(@"---                      Examples of Reflection                    ---");
            Console.WriteLine(@"----------------------------------------------------------------------");
            Console.WriteLine("0. Exit from console.");
            Console.WriteLine("1. Dictionary from Type.");
            Console.WriteLine("2. CompilerServices.");
            Console.WriteLine("3. Reflection vs StackTrace.");
            Console.WriteLine(@"----------------------------------------------------------------------");
            Console.Write("Type switch: ");
        }

        private static void PrintSwitch(int numberMenu)
        {
            Console.Clear();
            var isPrintMenu = false;
            switch (numberMenu)
            {
                case 1:
                    isPrintMenu = true;
                    PrintDictionaryFromType();
                    break;
                case 2:
                    isPrintMenu = true;
                    PrintCompilerServices();
                    break;
                case 3:
                    isPrintMenu = true;
                    PrintReflectionVsStackTrace();
                    break;
            }
            if (isPrintMenu)
            {
                Console.WriteLine(@"----------------------------------------------------------------------");
                Console.Write("Type any key to return in main menu.");
                Console.ReadKey();
            }
        }

        private static void PrintDictionaryFromType()
        {
            Console.WriteLine(@"----------------------------------------------------------------------");
            Console.WriteLine(@"---                     Dictionary from Type                       ---");
            Console.WriteLine(@"----------------------------------------------------------------------");

            Type t = typeof(Program);
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
            MemberInfo[] members = t.GetMembers(flags);
            Console.WriteLine($"Type {t.Name} has {members.Length} members: ");
            foreach (var member in members)
            {
                string access = "";
                string stat = "";
                var method = member as MethodBase;
                if (method != null)
                {
                    if (method.IsPublic) access = " Public";
                    else if (method.IsPrivate) access = " Private";
                    else if (method.IsFamily) access = " Protected";
                    else if (method.IsAssembly) access = " Internal";
                    else if (method.IsFamilyOrAssembly) access = " Protected Internal ";
                    if (method.IsStatic) stat = " Static";
                }
                var output = $"/* {member.MemberType} */ {access}{stat} {member.DeclaringType} {member.Name}";
                Console.WriteLine(output);
            }

            Console.WriteLine("----------------------");
            var simpleClass = new Program();
            var getDict = DictionaryFromType(simpleClass);
            foreach (var item in getDict)
            {
                Console.WriteLine("Key: " + item.Key + ". Value: " + item.Value);
            }
        }

        private static Dictionary<string, object> DictionaryFromType(object atype)
        {
            if (atype == null) return new Dictionary<string, object>();
            Type t = atype.GetType();
            PropertyInfo[] props = t.GetProperties();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (PropertyInfo prp in props)
            {
                object value = prp.GetValue(atype, new object[] { });
                dict.Add(prp.Name, value);
            }
            return dict;
        }

        private static void PrintCompilerServices()
        {
            Console.WriteLine(@"----------------------------------------------------------------------");
            Console.WriteLine(@"---                       CompilerServices                         ---");
            Console.WriteLine(@"----------------------------------------------------------------------");

            TraceMessage("Something happened.");
        }

        private static void TraceMessage(string message,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
        {
            Console.WriteLine("message: " + message);
            Console.WriteLine("member name: " + memberName);
            Console.WriteLine("source file path: " + sourceFilePath);
            Console.WriteLine("source line number: " + sourceLineNumber);
        }
        
        private static void PrintReflectionVsStackTrace()
        {
            Console.WriteLine(@"----------------------------------------------------------------------");
            Console.WriteLine(@"---                  Reflection vs StackTrace                      ---");
            Console.WriteLine(@"----------------------------------------------------------------------");

            var sw = Stopwatch.StartNew();
            var count = 50_000;
            Console.WriteLine($"StackTrace. Result: {PlayWithStackTrace(count)}. Time {sw.ElapsedMilliseconds} msec.");
            sw.Reset();
            sw.Start();
            Console.WriteLine($"Reflection. Result: {PlayWithReflection(count)}. Time {sw.ElapsedMilliseconds} msec.");
            sw.Stop();
        }

        private static int PlayWithStackTrace(int count)
        {
            var result = 0;
            for (var i = 0; i < count; i++)
            {
                var trace = new StackTrace(false);
                var method = trace.GetFrame(1).GetMethod();
                result += method != null ? 1 : 0;
            }
            return result;
        }

        private static int PlayWithReflection(int count)
        {
            var result = 0;
            for (var i = 0; i < count; i++)
            {
                var method = typeof(Program).GetProperty("MyProperty");
                result += method != null ? 1 : 0;
            }
            return result;
        }
    }
}
