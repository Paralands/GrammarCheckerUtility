using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrammarChecker
{
    public class SpecialCharacter
    {
        /// <summary>
        /// Character that SpecialCharacter contains
        /// </summary>
        public char Character { get; }
        /// <summary>
        /// If there can be a space after it or not.
        /// </summary>
        public bool SpaceAfter { get; }

        /// <summary>
        /// If there can be a space before it or not.
        /// </summary>
        public bool SpaceBefore { get; }

        public SpecialCharacter(char character, bool spaceAfter, bool spaceBefore)
        {
            Character = character;
            SpaceAfter = spaceAfter;
            SpaceBefore = spaceBefore;
        }
    }

    public static class SpecialCharacters
    {
        public static readonly SpecialCharacter Dot = new SpecialCharacter('.', true, false);
        public static readonly SpecialCharacter Coma = new SpecialCharacter(',', true, false);
        public static readonly SpecialCharacter ExclamationMark = new SpecialCharacter('!', true, false);
        public static readonly SpecialCharacter QuestionMark = new SpecialCharacter('?', true, false);
        public static readonly SpecialCharacter Colon = new SpecialCharacter(':', true, false);
        public static readonly SpecialCharacter RightParenthesis = new SpecialCharacter(')', true, false);
        public static readonly SpecialCharacter LeftParenthesis = new SpecialCharacter('(', false, true);


        //public static readonly SpecialCharacter Apostrophe = new SpecialCharacter('\'', true, true);
        //public static readonly SpecialCharacter Quote = new SpecialCharacter('"', true, true);
        public static readonly List<SpecialCharacter> Enumeration = new List<SpecialCharacter> { Dot, Coma, ExclamationMark, QuestionMark, Colon, RightParenthesis, LeftParenthesis };
    }
}