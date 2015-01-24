using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineTree.Language.Lexer
{
    public static class StringExtensions
    {
        public static char CharAt(this string source, int index)
        {
            if (source == null || index < 0 || index >= source.Length)
            {
                return char.MinValue;
            }

            return source[index];
        }

        public static int CharCodeAt(this string source, int index)
        {
            return CharAt(source, index);
        }
    }
}