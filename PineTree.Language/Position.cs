namespace PineTree.Language
{
    public struct Position
    {
        public int FileIndex;
        public int LineIndex;
        public int LineNumber;

        public Position(int lineNumber, int lineIndex, int fileIndex)
        {
            LineNumber = lineNumber;
            LineIndex = lineIndex;
            FileIndex = fileIndex;
        }

        public override string ToString()
        {
            return "(\{FileIndex}) \{LineNumber} \{LineIndex}";
        }
    }
}