using System;
using System.Diagnostics;

namespace GrammarChecker
{
    class Program
    {
        //Every checking -> adding common mistakes -> in mistake class it async finding solutions!!!!!!!
        static void Main(string[] args)
        {
            //Module tests
            //string text3 = "             Yes          ,          I like spaces     : For an exanple 1,              0 and 1.0    thing to do (     yep    )   ,what do you think about    it ? ";
            //Console.WriteLine(GrammarChecker.ClearSpaces(text3, SpecialCharacters.Coma));

            var sw1 = new Stopwatch();
            sw1.Start();
            var a = LanguageManager.Russian.Count;
            sw1.Stop();
            Console.WriteLine(sw1.ElapsedMilliseconds);

            var sw = new Stopwatch();
            sw.Start();
            GrammarChecker.GetSpellCorrections("превет", LanguageManager.Russian, 5);
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);

            var sw2 = new Stopwatch();
            sw2.Start();
            GrammarChecker.GetSpellCorrections("превет", LanguageManager.Russian, 5);
            sw2.Stop();
            Console.WriteLine(sw2.ElapsedMilliseconds);

            Console.ReadLine();
        }
    }
}