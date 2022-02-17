using System;
using System.Linq.Expressions;

namespace DelegateDecompiler.Sandbox.Net5
{
    public sealed class Program
    {
        public static void Main(string[] args)
        {
            TestBoolConstantInMethodCall();
            TestByteConstantInMethodCall();
            TestCharConstantInMethodCall();
            TestBoolEquality();
            TestBoolNegation();
            TestBoolReturn();
            TestIntEquality();

            int n = args.Length - 18;
            Action<IFoo> action = foo => foo.Property = n % -1;

            var postProcessors = new ExpressionVisitor[]
            {
                new SpecialNameMethodReplacer(),
                new PartialEvaluator(),
            };

            Expression expr = action.Decompile();

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Decompiled as:");
            Console.ResetColor();
            Console.WriteLine(expr);
            Console.WriteLine();

            foreach (var postProcessor in postProcessors)
            {
                expr = postProcessor.Visit(expr);

                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("After post-processing by " + postProcessor.GetType().Name + ":");
                Console.ResetColor();
                Console.WriteLine(expr);
                Console.WriteLine();
            }

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Final result:");
            Console.ResetColor();
            Console.WriteLine(expr);
        }

        private static void TestBoolConstantInMethodCall()
        {
            void methodCall(bool value) { }

            var @delegate = new Action(() => methodCall(true));

            var expr = @delegate.Decompile();
        }

        private static void TestByteConstantInMethodCall()
        {
            void methodCall(byte value) { }

            var @delegate = new Action(() => methodCall(10));

            var expr = @delegate.Decompile();
        }

        private static void TestCharConstantInMethodCall()
        {
            void methodCall(char value) { }

            var @delegate = new Action(() => methodCall('a'));

            var expr = @delegate.Decompile();
        }

        private static void TestBoolEquality()
        {
            var mybool = true;
            var @delegate = new Func<bool>(() => mybool == false);

            var expr = @delegate.Decompile();
        }

        private static void TestBoolNegation()
        {
            var someBoolean = true;
            var @delegate = new Func<bool>(() => !someBoolean);

            var expr = @delegate.Decompile();
        }

        private static void TestBoolReturn()
        {
            var @delegate = new Func<bool>(() => false);

            var expr = @delegate.Decompile();
        }

        private static void TestIntEquality()
        {
            var i = 10;
            var @delegate = new Func<bool>(() => i == 100L);

            var expr = @delegate.Decompile();
        }
    }
}

public interface IFoo
{
    int Property { get; set; }
    IBar Bar { get; set; }
}

public interface IBar
{
    event Action<IFoo> Event;
}