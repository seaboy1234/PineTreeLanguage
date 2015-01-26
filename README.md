# PineTree
*A Toy Programming Language*

## Project Structure
`PineTree.Language` includes the lexer and parser as well as the Ast nodes.
`PineTree.DomGenerator` includes the syntax analyzer tool which consumes Ast nodes and outputs an XML representation of the tree.
`PineTree.Interpreter` includes the interpreter runtime and standard library.
`PineTree` is the frontend REPL program.

## Contributing
Create a fork of the repository and send pull requests.  Large experimental features and changes should be made on a separate branch.

It is suggested that you familiarize yourself with the code-style.

This project is compiled against C# 6 / Roslyn.  It is suggested that you use Visual Studio 2015 for development.