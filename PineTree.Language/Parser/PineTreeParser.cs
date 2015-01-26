using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PineTree.Language.Lexer;
using PineTree.Language.Syntax;

namespace PineTree.Language.Parser
{
    public class PineTreeParser
    {
        private Token _current;
        private bool _enforceSemi;
        private Token _last;
        private PineTreeLexer _lexer;

        private Token _next;

        public PineTreeParser()
            : this(new PineTreeLexer())
        {
        }

        public PineTreeParser(PineTreeLexer lexer)
        {
            _lexer = lexer;
            _enforceSemi = true;
        }

        public SyntaxNode Parse()
        {
            InitializeTokens(true);
            List<SyntaxNode> nodes = new List<SyntaxNode>();

            try
            {
                while (_current != TokenType.EOF)
                {
                    nodes.Add(ParseClassOrMethod());
                }
            }
            catch (SyntaxException)
            {
                _lexer.Clear();
                throw;
            }

            return new SourceDocument(nodes);
        }

        public SyntaxNode Parse(string source)
        {
            _lexer.Feed(source);
            try
            {
                return Parse();
            }
            catch (SyntaxException)
            {
                _lexer.Clear();
                throw;
            }
        }

        public Expression ParseExpression(string source)
        {
            _lexer.Feed(source);
            InitializeTokens(false);
            try
            {
                return ParseExpression();
            }
            catch (SyntaxException)
            {
                _lexer.Clear();
                throw;
            }
        }

        public SourceDocument ParseScript(string source)
        {
            _lexer.Feed(source);
            InitializeTokens(false);
            List<SyntaxNode> nodes = new List<SyntaxNode>();
            try
            {
                while (_current != TokenType.EOF)
                {
                    if (_current == "import")
                    {
                        nodes.Add(ParseImportStatement());
                    }
                    else if ((PeekToken(1) == TokenType.Identifier && _next.Catagory == TokenCatagory.Identifier)
                         || (_next == "class" || _current == "class"))
                    {
                        nodes.Add(ParseClassOrMethod());
                    }
                    else
                    {
                        nodes.Add(ParseStatement());
                    }
                }
            }
            catch (SyntaxException)
            {
                _lexer.Clear();
                throw;
            }

            return new SourceDocument(nodes);
        }

        public SyntaxNode ParseStatement(string source)
        {
            _lexer.Feed(source);
            InitializeTokens(false);

            try
            {
                return ParseStatement();
            }
            catch (SyntaxException)
            {
                _lexer.Clear();
                throw;
            }
        }

        private void AcceptToken()
        {
            _last = _current;
            _current = _next;
            _next = _lexer.LexToken(g => !g.IsTrivia());
        }

        private void InitializeTokens(bool enforceSemi)
        {
            _enforceSemi = enforceSemi;
            AcceptToken();
            AcceptToken();
            _last = null;
        }

        private IEnumerable<VariableDeclaration> ParseArgumentList()
        {
            Take(TokenType.OpenPara);

            List<VariableDeclaration> arguments = new List<VariableDeclaration>();

            while (_current != TokenType.ClosePara)
            {
                VariableDeclaration variable = ParseVariableDeclaration();
                if (variable != null)
                {
                    arguments.Add(variable);
                }

                if (_current == TokenType.Comma && _current != TokenType.ClosePara)
                {
                    Take(TokenType.Comma);
                }
            }

            Take(TokenType.ClosePara);

            return arguments;
        }

        private AssignmentType ParseAssignmentType()
        {
            if (_current.Catagory != TokenCatagory.Assignment)
            {
                throw UnexpectedToken("assignment operator");
            }

            var op = _current.Type;
            AcceptToken();

            switch (op)
            {
                case TokenType.Assignment:
                    return AssignmentType.Regular;

                case TokenType.AssignmentAdd:
                    return AssignmentType.AddTo;

                case TokenType.AssignmentSubtract:
                    return AssignmentType.SubractFrom;

                case TokenType.AssignmentMultiply:
                    return AssignmentType.MultiplyBy;

                case TokenType.AssignmentDivide:
                    return AssignmentType.DivideBy;

                default:
                    return AssignmentType.Regular;
            }
        }

        private ClassDeclaration ParseClass(Visibility visibility)
        {
            TakeKeyword("class");
            var name = Take(TokenType.Identifier).Value;
            string supertype = "object";
            if (_current == "extends")
            {
                AcceptToken();
                supertype = Take(TokenType.Identifier).Value;
            }

            Take(TokenType.OpenBracket);

            List<FieldDeclaration> fields = new List<FieldDeclaration>();
            List<MethodDeclaration> methods = new List<MethodDeclaration>();
            List<PropertyDeclaration> properties = new List<PropertyDeclaration>();
            List<ConstructorDeclaration> constructors = new List<ConstructorDeclaration>();

            while (_current != TokenType.CloseBracket)
            {
                var node = ParseClassMember(name);

                switch (node.SyntaxType)
                {
                    case SyntaxTypes.FieldDeclaration:
                        fields.Add((FieldDeclaration)node);
                        break;

                    case SyntaxTypes.PropertyDeclaration:
                        properties.Add((PropertyDeclaration)node);
                        break;

                    case SyntaxTypes.MethodDeclaration:
                        methods.Add((MethodDeclaration)node);
                        break;

                    case SyntaxTypes.ConstructorDeclaration:
                        constructors.Add((ConstructorDeclaration)node);
                        break;
                }
            }
            Take(TokenType.CloseBracket);

            return new ClassDeclaration(name, supertype, fields, properties, methods, constructors);
        }

        private SyntaxNode ParseClassMember(string type)
        {
            var visibility = ParseVisibility(true);

            if (_next == TokenType.OpenPara && _current == type)
            {
                return ParseConstructor(visibility);
            }
            else
            {
                string name;
                string returnType;

                ParseTypeAndName(out returnType, out name, "void");

                if (_current == TokenType.OpenBracket)
                {
                    if (returnType == "void")
                    {
                        throw UnexpectedToken("type");
                    }
                    return ParseProperty(visibility, name, returnType);
                }
                else if (_current == TokenType.OpenPara)
                {
                    return ParseMethod(visibility, name, returnType);
                }
                else if (_current == TokenType.Assignment || _current == TokenType.Semicolon)
                {
                    if (_current == TokenType.Semicolon)
                    {
                        Take(TokenType.Semicolon);
                        return new FieldDeclaration(name, type, null);
                    }

                    var expression = ParseExpression();
                    if (_current == TokenType.Semicolon || _enforceSemi)
                    {
                        Take(TokenType.Semicolon);
                    }
                    return new FieldDeclaration(name, type, expression);
                }
            }
            return null;
        }

        private SyntaxNode ParseClassOrMethod()
        {
            Visibility visibility = ParseVisibility(true);

            SyntaxNode node;

            if (_current == "class")
            {
                node = ParseClass(visibility);
            }
            else
            {
                string type, name;
                ParseTypeAndName(out type, out name, "void");

                node = ParseMethod(visibility, name, type);
            }

            return node;
        }

        private ConstructorDeclaration ParseConstructor(Visibility visibility)
        {
            string name = Take(TokenType.Identifier).Value;
            var args = ParseArgumentList();
            var body = ParseLexicalScope();

            return new ConstructorDeclaration(name, visibility, args, body);
        }

        private ElseStatement ParseElseStatement()
        {
            TakeKeyword("else");
            var body = ParseLexicalScopeOrStatement();

            return new ElseStatement(body);
        }

        private IfStatement ParseIfStatement()
        {
            TakeKeyword("if");
            var predicate = ParsePredicate();
            var body = ParseLexicalScopeOrStatement();
            ElseStatement elseStatement = null;
            if (_current == "else")
            {
                elseStatement = ParseElseStatement();
            }

            return new IfStatement(predicate, body, elseStatement);
        }

        private SyntaxNode ParseImportStatement()
        {
            StringBuilder value = new StringBuilder();

            TakeKeyword("import");

            while (_current != TokenType.Semicolon)
            {
                value.Append(_current.Value);
                AcceptToken();
            }

            Take(TokenType.Semicolon);

            return new ImportStatement(value.ToString());
        }

        private LambdaExpression ParseLambdaExpression(IEnumerable<VariableDeclaration> arguments)
        {
            Take(TokenType.FatArrow);

            SyntaxNode body;

            if (_current == TokenType.OpenBracket)
            {
                body = ParseLexicalScope();
            }
            else
            {
                body = new ReturnStatement(ParseExpression());
            }

            return new LambdaExpression(arguments, body);
        }

        private LexicalScope ParseLexicalScope()
        {
            Take(TokenType.OpenBracket);

            List<SyntaxNode> statements = new List<SyntaxNode>();

            while (_current != TokenType.CloseBracket)
            {
                statements.Add(ParseStatement());
            }

            Take(TokenType.CloseBracket);

            return new LexicalScope(statements);
        }

        private SyntaxNode ParseLexicalScopeOrExpression()
        {
            if (_current == TokenType.OpenBracket)
            {
                return ParseLexicalScope();
            }
            else
            {
                return ParseExpression();
            }
        }

        private SyntaxNode ParseLexicalScopeOrStatement()
        {
            if (_current == TokenType.OpenBracket)
            {
                return ParseLexicalScope();
            }
            else
            {
                return ParseStatement();
            }
        }

        private SyntaxNode ParseMethod(Visibility visibility, string name, string type)
        {
            if (type == "var")
            {
                throw UnexpectedToken("Type");
            }
            IEnumerable<VariableDeclaration> arguments = ParseArgumentList();

            LexicalScope body = ParseLexicalScope();

            return new MethodDeclaration(name, type, visibility, arguments, body);
        }

        private Expression ParsePredicate()
        {
            Take(TokenType.OpenPara);
            var expression = ParseLevel2Expression();
            Take(TokenType.ClosePara);
            return expression;
        }

        private PropertyDeclaration ParseProperty(Visibility visibility, string name, string type)
        {
            Take(TokenType.OpenBracket);

            bool hasGet = false;
            bool hasSet = false;

            MethodDeclaration getMethod = null;
            MethodDeclaration setMethod = null;

            while (_current != TokenType.CloseBracket)
            {
                if (_current == "get" && !hasGet)
                {
                    Take(_current.Type);
                    var body = ParseLexicalScope();
                    getMethod = new MethodDeclaration("get_\{name}", type, visibility, new VariableDeclaration[0], body);
                    hasGet = true;
                }
                else if (_current == "set" && !hasSet)
                {
                    Take(_current.Type);
                    var body = ParseLexicalScope();
                    setMethod = new MethodDeclaration("set_\{name}", type, visibility, new[] { new VariableDeclaration("value", type, null) }, body);
                    hasSet = true;
                }
                else
                {
                    throw UnexpectedToken("\{hasGet ? "" : "'get', "}\{hasSet ? "" : "'set', "}'}'");
                }
            }
            Take(TokenType.CloseBracket);
            if (!hasGet)
            {
                throw UnexpectedToken("'get'");
            }

            return new PropertyDeclaration(name, type, visibility, getMethod, setMethod);
        }

        private ReturnStatement ParseReturnStatement()
        {
            AcceptToken();

            var expression = ParseExpression();

            return new ReturnStatement(expression);
        }

        private SyntaxNode ParseStatement()
        {
            SyntaxNode node;
            if (_current == TokenType.Keyword)
            {
                switch (_current.Value)
                {
                    case "if":
                        node = ParseIfStatement();
                        break;

                    case "var":
                        node = ParseVariableDeclaration();
                        break;

                    case "new":
                        node = ParseNewExpression();
                        break;

                    case "return":
                        node = ParseReturnStatement();
                        break;

                    default:
                        node = ParseExpressionTerminal();
                        break;
                }
            }
            else if (_current == TokenType.OpenBracket)
            {
                node = ParseLexicalScope();
            }
            else if (_current == TokenType.Semicolon)
            {
                AcceptToken();
                return new EmptyStatement();
            }
            else
            {
                if (_next == TokenType.Identifier && _current.Catagory == TokenCatagory.Identifier)
                {
                    node = ParseVariableDeclaration();
                }
                else
                {
                    node = ParseExpression();
                }
            }
            if (node == null)
            {
                throw UnexpectedToken("statement");
            }
            if ((_enforceSemi && _last != TokenType.CloseBracket) || _current == TokenType.Semicolon)
            {
                Take(TokenType.Semicolon);
            }

            return node;
        }

        private void ParseTypeAndName(out string type, out string name, params string[] specialWords)
        {
            if (specialWords.Contains(_current.Value))
            {
                type = Take(TokenType.Keyword).Value;
            }
            else
            {
                type = Take(TokenType.Identifier).Value;
            }

            name = Take(TokenType.Identifier).Value;
        }

        private VariableDeclaration ParseVariableDeclaration()
        {
            string type;
            string name;
            SyntaxNode value = null;

            ParseTypeAndName(out type, out name, "var");

            if (_current == TokenType.Assignment)
            {
                AcceptToken();
                value = ParseExpression();
            }

            return new VariableDeclaration(name, type, value);
        }

        private Visibility ParseVisibility(bool optional)
        {
            if (_current != TokenType.Keyword)
            {
                if (optional)
                {
                    return Visibility.Private;
                }
                else
                {
                    throw UnexpectedToken("'public', 'private', or 'protected'");
                }
            }
            if (_current == "public")
            {
                AcceptToken();
                return Visibility.Public;
            }
            else if (_current == "protected")
            {
                AcceptToken();
                return Visibility.Protected;
            }
            else if (_current == "private")
            {
                AcceptToken();
                return Visibility.Private;
            }
            else
            {
                if (!optional)
                {
                    throw UnexpectedToken("'public', 'private', or 'protected'");
                }
            }
            return Visibility.Private;
        }

        private Token PeekToken(int ahead)
        {
            return _lexer.PeekToken(ahead, g => !g.IsTrivia());
        }

        private SyntaxException SyntaxError(string message)
        {
            return new SyntaxException("\{_current.Start} \{_current.Span} \{message}");
        }

        #region Parse Expression

        private bool IsUnaryOperator()
        {
            switch (_current.Type)
            {
                case TokenType.PlusPlus:
                case TokenType.MinusMinus:
                case TokenType.LogicalNot:
                    return true;
            }
            switch (_next.Type)
            {
                case TokenType.PlusPlus:
                case TokenType.MinusMinus:
                    return true;
            }
            return false;
        }

        private ArithmeticOperator ParseArithmeticOperator()
        {
            if (_current.Catagory != TokenCatagory.Arithmetic)
            {
                throw UnexpectedToken("Arithmetic operator");
            }
            var last = _last;
            var op = _current.Type;
            AcceptToken();

            switch (op)
            {
                case TokenType.Addition:
                    return ArithmeticOperator.Add;

                case TokenType.Divide:
                    return ArithmeticOperator.Divide;

                case TokenType.Multiply:
                    return ArithmeticOperator.Multiply;

                case TokenType.Subtract:
                    return ArithmeticOperator.Subtract;

                case TokenType.Modulo:
                    return ArithmeticOperator.Modulo;

                // Unreachable
                default:
                    return ArithmeticOperator.Add;
            }
        }

        private Expression ParseArrayAccess(bool inObjectRef = false)
        {
            string target = Take(TokenType.Identifier).Value;

            Take(TokenType.OpenArrayAccessor);

            Expression index = ParseExpression();

            Take(TokenType.CloseArrayAccessor);

            var expression = new ArrayAccessExpression(target, index);
            if (_current == TokenType.Dot && !inObjectRef)
            {
                return ParseObjectReferenceExpression(expression);
            }
            return expression;
        }

        private Expression ParseArrayDeclaration(string identifier)
        {
            Take(TokenType.OpenArrayAccessor);

            if (_current == TokenType.CloseArrayAccessor)
            {
                Take(TokenType.CloseArrayAccessor);
                return new ArrayDeclarationExpression(identifier, null);
            }
            var expression = ParseExpression();

            Take(TokenType.CloseArrayAccessor);

            return new ArrayDeclarationExpression(identifier, expression);
        }

        private Expression ParseBasicOperation()
        {
            Take(TokenType.OpenPara);

            // Explicit lambda declaration
            if (_next.Catagory == TokenCatagory.Identifier)
            {
                List<VariableDeclaration> args = new List<VariableDeclaration>();
                while (_current != TokenType.ClosePara)
                {
                    string type = Take(TokenType.Identifier).Value;
                    string name = Take(TokenType.Identifier).Value;

                    args.Add(new VariableDeclaration(name, type, null));

                    if (_current != TokenType.ClosePara)
                    {
                        Take(TokenType.Comma);
                    }
                }

                Take(TokenType.ClosePara);

                return ParseLambdaExpression(args);
            }
            else if (_current == TokenType.ClosePara && _next == TokenType.FatArrow)
            {
                Take(_current.Type);
                return ParseLambdaExpression(new VariableDeclaration[0]);
            }

            var expression = ParseExpression();

            // Implicit Lambda
            if (_current == TokenType.Comma)
            {
                Take(TokenType.Comma);
                if (expression.SyntaxType != SyntaxTypes.IdentifierExpression)
                {
                    throw SyntaxError("Implicit lambda parameters must be identifiers.");
                }
                List<IdentifierExpression> expressions = new List<IdentifierExpression>() { (IdentifierExpression)expression };
                while (_current != TokenType.ClosePara)
                {
                    string name = Take(TokenType.Identifier).Value;

                    expressions.Add(new IdentifierExpression(name));

                    if (_current != TokenType.ClosePara)
                    {
                        Take(TokenType.Comma);
                    }
                }

                Take(TokenType.ClosePara);

                return ParseLambdaExpression(expressions.Select(g => new VariableDeclaration(g.Identifier, "var", null)));
            }

            Take(TokenType.ClosePara);

            if (_current == TokenType.FatArrow)
            {
                if (expression.SyntaxType != SyntaxTypes.IdentifierExpression)
                {
                    throw SyntaxError("Implicit lambda parameters must be identifiers.");
                }

                return ParseLambdaExpression(new List<VariableDeclaration>() { new VariableDeclaration(((IdentifierExpression)expression).Identifier, "var", null) });
            }
            else if (_current == TokenType.Dot)
            {
                return ParseObjectReferenceExpression(expression);
            }

            return expression;
        }

        private BitwiseOperator ParseBitwiseOperator()
        {
            if (_current.Catagory != TokenCatagory.Bitwise)
            {
                throw UnexpectedToken("Bitwise operator");
            }
            var op = _current.Type;
            AcceptToken();

            switch (op)
            {
                case TokenType.BitwiseAnd:
                    return BitwiseOperator.And;

                case TokenType.BitwiseOr:
                    return BitwiseOperator.Or;

                case TokenType.BitwiseXOR:
                    return BitwiseOperator.Xor;

                default: return BitwiseOperator.And;
            }
        }

        private Expression ParseExpression()
        {
            Expression left = ParseLevel1Expression();

            return left;
        }

        private Expression ParseExpressionTerminal()
        {
            if (_current == TokenType.Identifier)
            {
                if (_next == TokenType.OpenPara)
                {
                    return ParseMethodCall();
                }
                else if (_next == TokenType.OpenArrayAccessor)
                {
                    return ParseArrayAccess();
                }
                else if (_next == TokenType.FatArrow)
                {
                    return ParseLambdaExpression(new List<VariableDeclaration>() { new VariableDeclaration(Take(TokenType.Identifier).Value, "var", null) });
                }
                else if (_next == TokenType.Dot)
                {
                    return ParseObjectReferenceExpression(new IdentifierExpression(Take(TokenType.Identifier).Value));
                }
                else
                {
                    return new IdentifierExpression(Take(TokenType.Identifier).Value);
                }
            }
            else if (_current == TokenType.Keyword)
            {
                if (_current == "true")
                {
                    return new ConstantExpression(Take(_current.Type).Value, ConstantType.Boolean);
                }
                else if (_current == "false")
                {
                    return new ConstantExpression(Take(_current.Type).Value, ConstantType.Boolean);
                }
                else if (_current == "null")
                {
                    return new ConstantExpression(Take(_current.Type).Value, ConstantType.Null);
                }
                else if (_current == "new")
                {
                    return ParseNewExpression();
                }
            }
            else if (_current.Catagory == TokenCatagory.Literal)
            {
                if (_current == TokenType.StringLiteral)
                {
                    return new ConstantExpression(Take(_current.Type).Value, ConstantType.String);
                }
                else
                {
                    return ParseNumberLiteral();
                }
            }
            throw UnexpectedToken("expression terminal");
        }

        private Expression ParseLevel1Expression()
        {
            Expression left = ParseLevel2Expression();
            if (_current.Catagory == TokenCatagory.Assignment)
            {
                var op = ParseAssignmentType();
                Expression right = ParseLevel1Expression();

                return new AssignmentExpression(left, right, op);
            }
            return left;
        }

        private Expression ParseLevel2Expression()
        {
            Expression left = ParseLevel3Expression();

            if (_current == TokenType.LogicalAnd || _current == TokenType.LogicalOr)
            {
                var op = ParseLogicalOperator();
                Expression right = ParseLevel2Expression();

                return new LogicalExpression(left, right, op);
            }

            return left;
        }

        private Expression ParseLevel3Expression()
        {
            Expression left = ParseLevel4Expression();

            if (_current.Catagory == TokenCatagory.Logical && _current != "!")
            {
                var op = ParseLogicalOperator();
                Expression right = ParseLevel3Expression();

                return new LogicalExpression(left, right, op);
            }

            return left;
        }

        private Expression ParseLevel4Expression()
        {
            Expression left = ParseLevel5Expression();

            // 10 - 3 + 2 should be parsed as ((10 - 3) + 2)
            while (_current == TokenType.Addition || _current == TokenType.Subtract)
            {
                var op = ParseArithmeticOperator();
                left = new ArithmeticExpression(left, ParseLevel5Expression(), op);
            }
            return left;
        }

        private Expression ParseLevel5Expression()
        {
            Expression left = ParseLevel6Expression();

            while (_current == TokenType.Multiply || _current == TokenType.Divide)
            {
                var op = ParseArithmeticOperator();
                left = new ArithmeticExpression(left, ParseLevel6Expression(), op);
            }

            return left;
        }

        private Expression ParseLevel6Expression()
        {
            Expression left = ParseLevel7Expression();
            if (_current == TokenType.Modulo)
            {
                var op = ParseArithmeticOperator();
                Expression right = ParseLevel6Expression();

                return new ArithmeticExpression(left, right, op);
            }
            return left;
        }

        private Expression ParseLevel7Expression()
        {
            Expression left = ParseLevel8Expression();

            if (_current.Catagory == TokenCatagory.Bitwise)
            {
                var op = ParseBitwiseOperator();
                Expression right = ParseLevel7Expression();

                return new BitwiseExpression(left, right, op);
            }

            return left;
        }

        private Expression ParseLevel8Expression()
        {
            if (_current == TokenType.OpenPara)
            {
                return ParseBasicOperation();
            }
            else if (IsUnaryOperator() || _next == TokenType.PlusPlus || _next == TokenType.PlusPlus)
            {
                return ParseUnaryExpression();
            }
            else
            {
                return ParseExpressionTerminal();
            }
        }

        private LogicOperation ParseLogicalOperator()
        {
            if (_current.Catagory != TokenCatagory.Logical)
            {
                throw UnexpectedToken("logical operator");
            }
            var op = _current.Type;
            AcceptToken();

            switch (op)
            {
                case TokenType.LogicalAnd:
                    return LogicOperation.And;

                case TokenType.LogicalEquals:
                    return LogicOperation.Equals;

                case TokenType.LogicalGreater:
                    return LogicOperation.GreaterThan;

                case TokenType.LogicalGreaterOrEqual:
                    return LogicOperation.GreaterThanOrEqual;

                case TokenType.LogicalLess:
                    return LogicOperation.LessThan;

                case TokenType.LogicalLessOrEqual:
                    return LogicOperation.LessThanOrEqual;

                case TokenType.LogicalNot:
                    return LogicOperation.Not;

                case TokenType.LogicalNotEquals:
                    return LogicOperation.NotEquals;

                case TokenType.LogicalOr:
                    return LogicOperation.Or;

                default: return LogicOperation.Not;
            }
        }

        private Expression ParseMethodCall(bool inObjectRef = false)
        {
            string identifier = Take(TokenType.Identifier).Value;
            Take(TokenType.OpenPara);

            List<Expression> arguments = new List<Expression>();

            while (_current != TokenType.ClosePara)
            {
                if (_current == TokenType.Comma)
                {
                    Take(TokenType.Comma);
                }

                var expression = ParseExpression();

                arguments.Add(expression);
            }

            Take(TokenType.ClosePara);

            var call = new MethodCallExpression(identifier, arguments);
            if (_current == TokenType.Dot && !inObjectRef)
            {
                return ParseObjectReferenceExpression(call);
            }

            return call;
        }

        private Expression ParseNewExpression()
        {
            TakeKeyword("new");
            string identifier = Take(TokenType.Identifier).Value;
            if (_current == TokenType.OpenArrayAccessor)
            {
                return ParseArrayDeclaration(identifier);
            }
            Take(TokenType.OpenPara);

            List<Expression> arguments = new List<Expression>();

            while (_current != TokenType.ClosePara)
            {
                if (_current == TokenType.Comma && arguments.Count > 0)
                {
                    Take(TokenType.Comma);
                }

                var expression = ParseExpression();

                arguments.Add(expression);
            }

            Take(TokenType.ClosePara);

            var call = new NewExpression(identifier, arguments);
            if (_current == TokenType.Dot)
            {
                return ParseObjectReferenceExpression(call);
            }

            return call;
        }

        private ConstantExpression ParseNumberLiteral()
        {
            switch (_current.Type)
            {
                case TokenType.Number:
                    return new ConstantExpression(Take(TokenType.Number).Value, ConstantType.Int);

                case TokenType.HexNumber:
                    {
                        string value = Take(TokenType.HexNumber).Value.Substring(2);
                        long number = Convert.ToInt64(value, 16);
                        return new ConstantExpression(number.ToString(), ConstantType.Int);
                    }
                case TokenType.OctalNumber:
                    {
                        string value = Take(TokenType.OctalNumber).Value.Substring(2);
                        long number = Convert.ToInt64(value, 8);
                        return new ConstantExpression(number.ToString(), ConstantType.Int);
                    }
                case TokenType.FloatingPointNumber:
                    return new ConstantExpression(Take(TokenType.FloatingPointNumber).Value, ConstantType.Float);

                default:
                    throw UnexpectedToken("number constant");
            }
        }

        private Expression ParseObjectReferenceExpression(Expression root)
        {
            List<Expression> references = new List<Expression>();
            while (_current == TokenType.Dot)
            {
                Take(TokenType.Dot);

                Expression expression;

                if (_next == TokenType.OpenPara)
                {
                    expression = ParseMethodCall(true);
                }
                else if (_next == TokenType.OpenArrayAccessor)
                {
                    expression = ParseArrayAccess(true);
                }
                else
                {
                    expression = new IdentifierExpression(Take(TokenType.Identifier).Value);
                }

                references.Add(expression);
            }

            return new ObjectReferenceExpression(root, references);
        }

        private Expression ParseUnaryExpression()
        {
            Expression expression;
            if (_current == "!")
            {
                var op = ParseUnaryOperator();

                if (_current == TokenType.OpenPara)
                {
                    expression = ParseBasicOperation();
                }
                else
                {
                    expression = ParseExpression();
                }
                return new UnaryExpression(expression, op, true);
            }
            else if (_current == "++" || _current == "--")
            {
                var op = ParseUnaryOperator();
                string identifier = Take(TokenType.Identifier).Value;
                return new UnaryExpression(new IdentifierExpression(identifier), op, true);
            }
            else
            {
                string identifier = Take(TokenType.Identifier).Value;
                var op = ParseUnaryOperator();

                return new UnaryExpression(new IdentifierExpression(identifier), op, false);
            }
        }

        private UnaryOperator ParseUnaryOperator()
        {
            var op = _current.Type;
            AcceptToken();
            switch (op)
            {
                case TokenType.PlusPlus:
                    return UnaryOperator.Increment;

                case TokenType.MinusMinus:
                    return UnaryOperator.Decrement;

                case TokenType.LogicalNot:
                    return UnaryOperator.LogicalNot;

                default:
                    return UnaryOperator.None;
            }
        }

        #endregion Parse Expression

        private Token Take(TokenType type)
        {
            if (_current != type)
            {
                throw UnexpectedToken("\{type.ToString().ToLower()}");
            }
            AcceptToken();
            return _last;
        }

        private Token Take(TokenType type, string value)
        {
            if (_current != type || _current != value)
            {
                throw UnexpectedToken("'\{value}'");
            }
            AcceptToken();
            return _last;
        }

        private Token TakeKeyword(string keyword)
        {
            return Take(TokenType.Keyword, keyword);
        }

        private SyntaxException UnexpectedToken(string expected)
        {
            string message = "\{_current.Start} \{_current.Span} Unexpected Token '\{_current.Value}'.  Expected \{expected}";
            return new SyntaxException(message);
        }

        private SyntaxException UnexpectedToken(Token value, string expected)
        {
            string message = "\{value.Start} \{value.Span} Unexpected Token '\{value.Value}'.  Expected \{expected}";
            return new SyntaxException(message);
        }
    }
}