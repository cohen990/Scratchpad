using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages;
using Microsoft.Ajax.Utilities;

namespace scratchpad.Reversi
{
    public static class ReverseWords
    {
        public static string Reverse(string input)
        {
            if (input.IsNullOrWhiteSpace())
                return "";


            string output = "";

            for (int i = GetLastIndex(input); i >= 0; i--)
            {
                output += input[i];
            }
            return (output);
        }

        public static int GetLastIndex(string input)
        {
            int i = input.Length - 1;
            return (i);
        }

        public static int NextNonCharIndex(string input, int currentIndex)
        {
            for (int i = currentIndex; i < input.Length; i++)
            {
                if (input[i] == ' ')
                {
                    return (i);
                }
            }
        return (input.Length);
        }

        public static bool IsLetter(char input)
        {
            string nonAlphabetRegex = "a-z";
            newLetter = input.ToString().ToLower();
        }
    }
}