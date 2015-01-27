using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineTree.Language.Lexer
{
    public static class StringExtensions
    {
        /// <summary>
        /// Gets the <see cref="char"/> at a given index.  If the index is out of range, returns an ASCII NULL (\0).
        /// </summary>
        /// <param name="source"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static char CharAt(this string source, int index)
        {
            if (source == null || index < 0 || index >= source.Length)
            {
                return char.MinValue;
            }

            return source[index];
        }

        /// <summary>
        /// Gets the code of the character at a given index.  If the index is out of range, <c>0</c> is returned.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <seealso cref="CharAt(string, int)"/>
        public static int CharCodeAt(this string source, int index)
        {
            return CharAt(source, index);
        }
    }
}