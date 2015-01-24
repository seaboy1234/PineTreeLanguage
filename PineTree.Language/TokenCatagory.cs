using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineTree.Language
{
    public enum TokenCatagory
    {
        None,

        Trivia,
        Literal,
        Identifier,
        Punctuation,
        Arithmetic,
        Assignment,
        Logical,
        Bitwise
    }
}