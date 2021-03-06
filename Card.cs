﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Hanabi
{
    /// <summary>Common type of a Hanabi playing card</summary>
    /// <remarks>Lack of properties set-accessors makes this
    /// class immutable everywhere except the constructor</remarks>
    class Card
    {
        #region Static Fields

        /// <summary>All possible card colors</summary>
        /// <remarks>More colors can be added, provided that the first letter of each color is different. If color
        /// has aliases, for example: <code>red = Red</code>, - their first letters will be ignored</remarks>
        public enum Colors
        {
            Red,
            Green,
            Blue,
            White,
            Yellow
        }

        /// <summary>Dictionary that helps to get the color from its first letter</summary>
        private static readonly ReadOnlyDictionary<char, Colors> ColorsByFirstLetter;

        public static readonly int RankLimit = 5;
        public static readonly int NumberOfColors;
        public static readonly int MaxAbbreviationLength = RankLimit.ToString().Length + 1;
        
        #endregion
        #region Props
        
        public Colors Color { get; }
        public int Rank { get; }

        /// <summary>Gives the card abbreviation string</summary>
        public string Abbreviation => Color.ToString()[0].ToString() + Rank;
        
        #endregion
        #region Constructors

        /// <summary>Initializes static props, that depend on the <see cref="Colors"/> enumerator</summary>
        /// <remarks>Throws an exception if two colors have same first letter.
        /// Ignores the first letters of colors aliases</remarks>
        static Card()
        {
            try
            {
                Dictionary<char, Colors> colorsByFirstLetter = Enum
                                            .GetValues(typeof(Colors))
                                            .Cast<Colors>()
                                            .Distinct()
                                            .ToDictionary(col => col.ToString()[0], col => col);
                ColorsByFirstLetter = new ReadOnlyDictionary<char, Colors>(colorsByFirstLetter);
                NumberOfColors = ColorsByFirstLetter.Count;
            }
            catch (ArgumentException e)
            {
                throw new ArgumentException("Two colors cant start with the same letter", e);
            }
        }

        /// <summary>Creates a new card instance with given color and rank</summary>
        public Card(Colors cardColor, int cardRank)
        {
            if (!Enum.IsDefined(typeof(Colors), cardColor.ToString()))
            {
                throw new ArgumentOutOfRangeException(nameof(cardColor));
            }
            if (cardRank < 1 || cardRank > RankLimit)
            {
                throw new ArgumentOutOfRangeException(nameof(cardRank));
            }
            Color = cardColor;
            Rank = cardRank;
        }

        /// <summary>Creates a new card instance using given card abbreviation</summary>
        /// <param name="cardAbbreviation">String of the form "CR" where C is the first letter of cards color and
        /// R is cards rank. For example: <value>"G1"</value>, <value>"B5"</value>, <value>"W2"</value></param>
        public Card(string cardAbbreviation)
        {
            if (string.IsNullOrEmpty(cardAbbreviation))
            {
                throw new GameCommandException("Card abbreviation expected");
            }
            if (cardAbbreviation.Length < 2)
            {
                throw new GameCommandException(
                    $"Card abbreviation must be at least 2 symbols long: {cardAbbreviation}");
            }
            if (cardAbbreviation.Length > MaxAbbreviationLength)
            {
                throw new GameCommandException(
                    $"Card abbreviation cant be more than {MaxAbbreviationLength} symbols long: {cardAbbreviation}");
            }
            Color = ParseColor(cardAbbreviation[0]);
            Rank = ParseRank(cardAbbreviation.Substring(1));
        }

        #endregion
        #region Methods

        /// <summary>Gives cards full name, that consists of color
        /// name and rank number, separated by a space</summary>
        /// <returns>Cards full name, For example: <value>"Green 1"</value>, <value>"Blue 5"</value></returns>
        public override string ToString()
        {
            return $"{Color} {Rank}";
        }

        /// <summary>Parses first character of a color name</summary>
        /// <param name = "firstLetterOfAColor">First letter of a  color, for example:
        /// <value>'R'</value>, <value>'Y'</value>, <value>'W'</value></param>
        /// <returns>Card color</returns>
        public static Colors ParseColor(char firstLetterOfAColor)
        {
            if (!ColorsByFirstLetter.ContainsKey(firstLetterOfAColor))
            {
                throw new GameCommandException("Unknown card color abbreviation: " + firstLetterOfAColor);
            }
            return ColorsByFirstLetter[firstLetterOfAColor];
        }

        /// <summary>Parses full string name of a color</summary>
        /// <param name = "colorName">Full color name, for example:
        /// <value>"Red"</value>, <value>"Yellow"</value>, <value>"White"</value></param>
        public static Colors ParseColor(string colorName)
        {
            if (string.IsNullOrEmpty(colorName))
            {
                throw new GameCommandException("Color name expected");
            }
            if (!Enum.IsDefined(typeof(Colors), colorName))
            {
                throw new GameCommandException("Unknown color name: " + colorName);
            }
            return (Colors)Enum.Parse(typeof(Colors), colorName);
        }

        /// <summary>Parses rank string</summary>
        /// <param name = "rank">String that contains card rank integer</param>
        public static int ParseRank(string rank)
        {
            if (string.IsNullOrEmpty(rank))
            {
                throw new GameCommandException("Rank string expected");
            }
            int r;
            if (!int.TryParse(rank, out r))
            {
                throw new GameCommandException("Card rank must be an integer: " + rank);
            }
            if (r < 1 || r > RankLimit)
            {
                throw new GameCommandException("Card rank out of range: " + r);
            }
            return r;
        }

        #endregion
    }
}