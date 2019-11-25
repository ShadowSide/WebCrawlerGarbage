using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Diagnostics.Contracts;

namespace WebCrawler
{
    public enum CharClass
    {
        Letter = 0,
        Digit,
        Control,
        Whitespace,
        Symbol,
        NewLine
    }

    public class Token
    {
        public static CharClass GetCharClass(char c)
        {
            if (Char.IsControl(c))
                return CharClass.Control;
            if (c == '\n' || c == '\r')
                return CharClass.NewLine;
            if (Char.IsWhiteSpace(c))
                return CharClass.Whitespace;
            if (Char.IsLetter(c))
                return CharClass.Letter;
            if (Char.IsDigit(c))
                return CharClass.Digit;
            return CharClass.Symbol;
        }

        public Token(string token) { Value = token; FirstCharClass = GetCharClass(token[0]); }
        public string Value { get; set; }
        public CharClass FirstCharClass { get; set; }
    }

    public class Tokenizer
    {
        public Token[] Tokenize(string code)
        {
            var tokens = TokenizeInternal(code);
            return GroupingTokens(tokens);
        }

        private LinkedList<Token> TokenizeInternal(string code)
        {
            var result = new LinkedList<Token>();
            CharClass? lastCharClass = null;
            var equalCharClases = new[] { CharClass.Digit, CharClass.Letter };
            foreach (var c in code)
            {
                var currentCharClass = Token.GetCharClass(c);
                if (
                    lastCharClass == currentCharClass
                    && currentCharClass != CharClass.Symbol
                    || (
                        lastCharClass != null
                        && equalCharClases.Contains(currentCharClass)
                        && equalCharClases.Contains(lastCharClass.Value)))
                {
                    result.Last.Value.Value += c;
                    lastCharClass = currentCharClass;
                    continue;
                }
                result.AddLast(new Token(c.ToString()));
                lastCharClass = null;
            }
            return result;
        }

        private void Iterate(ref LinkedListNode<Token> it)
        {
            it = it.Next;
        }

        private Token[] GroupingTokens(LinkedList<Token> tokens)
        {
            var result = new LinkedList<Token>();
            for (var it = tokens.First; it != null;)
            {
                if (it.Previous == null)
                {
                    result.AddLast(it.Value);
                    Iterate(ref it);
                    continue;
                }
                if (it.Value.FirstCharClass == CharClass.Digit)
                {
                    var digits = it.Value.Value.TakeWhile(Char.IsDigit);
                    var letters = it.Value.Value.SkipWhile(Char.IsDigit).ToArray();
                    if (letters.Any())
                    {
                        result.AddLast(new Token(digits.ToArray().ToString()));
                        result.AddLast(new Token(letters.ToString()));
                        Iterate(ref it);
                        continue;
                    }
                }
                result.AddLast(it.Value);
                Iterate(ref it);
            }
            return result.ToArray();
        }
    }

    public delegate ParseResult<AstType> Parser<AstType>(IEnumerable<Token> tokens);

    public class ParseResult<AstType>
    {
        public AstType Ast { get; set; }
        public IEnumerable<Token> Remains { get; set; }
        public virtual bool OkParsed { get; }
    }

    public class Unparsed<AstType> : ParseResult<AstType>
    {
        public override bool OkParsed { get { return false; } }
    }

    public class Parsed<AstType> : ParseResult<AstType>
    {
        public override bool OkParsed { get { return true; } }
        public Parsed(AstType ast, IEnumerable<Token> remainsSourceTokens)
        {
            Ast = ast;
            Remains = remainsSourceTokens;
        }
    }

    public static class ParserCommon
    {
        public static Parser<AstType> SuccessParseNothing<AstType>(Func<AstType> astMaker = null)
        {
            return (tokens) =>
            {
                var astMaker_ = astMaker;
                if (astMaker == null)
                    astMaker_ = () => default(AstType);
                return new Parsed<AstType>(astMaker_(), tokens);
            };
        }
    };

    public static class ParserExtension
    {
        public static ParseResult<AstResultType> TransformAst<AstType, AstResultType>(
            this ParseResult<AstType> parseResult,
            Func<AstType, AstResultType> astTransform)
        {
            if (parseResult is var parsed && parsed.OkParsed)
            {
                return new Parsed<AstResultType>(astTransform(parsed.Ast), parsed.Remains);
            }
            return new Unparsed<AstResultType>();
        }

        public static Parser<AstResultType> TransformAst<AstType, AstResultType>(
            this Parser<AstType> parser, 
            Func<AstType, AstResultType> astTransformer)
        {
            return (tokens) =>
            {
                return parser(tokens).TransformAst(astTransformer);
            };
        }

        public static Parser<AstResultType> Then<AstType, ThenAstType, AstResultType>(
            this Parser<AstType> parser,
            Parser<ThenAstType> thenParser,
            Func<AstType, ThenAstType, AstResultType> astCombiner)
        {
            return (tokens) =>
            {
                if (parser(tokens) is var parsed && parsed.OkParsed)
                {
                    var thenParsed = thenParser(parsed.Remains);
                    if (thenParsed.OkParsed)
                        return new Parsed<AstResultType>(astCombiner(parsed.Ast, thenParsed.Ast), thenParsed.Remains);
                }
                return new Unparsed<AstResultType>();
            };
        }

        public static Parser<ThenAstType> Then<AstType, ThenAstType>(
            this Parser<AstType> parser,
            Parser<ThenAstType> thenParser)
        {
            return parser.Then(thenParser, (_, __) => __);
        }
    }

    public abstract class ParserBase
    {
        //[Pure]
        public static Parser<AstType> SuccessParseNothing<AstType>(Func<AstType> astMaker = null)
        {
            return ParserCommon.SuccessParseNothing(astMaker);
        }

        protected Parser<AstResultType> ConsumeN<AstType, AstResultType>(
            Parser<AstType> parser,
            Func<IEnumerable<AstType>, AstResultType> astCombiner, int N = 0)
        {
            return (tokens) =>
            {
               
            };
        }

        protected Parser<AstResultType> ConsumeAnyAmount<AstType, AstResultType>(
            Parser<AstType> parser,
            Func<IEnumerable<AstType>, AstResultType> astCombiner, int N = 0)
        {
            return (tokens) =>
            {

            };
        }

        protected Parser<AstResultType> ConsumeNOrMore<AstType, AstResultType>(
            Parser<AstType> parser, 
            Func<IEnumerable<AstType>, AstResultType> astCombiner, int N = 0)
        {
            return (tokens) =>
            {
                var astCombiner_ = astCombiner;
                if (astCombiner == null)
                    astCombiner_ = _ => null;
                var remains = tokens;
                var parsed = new List<Parsed>();
                var NParser = SuccessParseNothing();
                while (N-- >= 0)
                    NParser = NParser.Then(parser, parsed.Add);
                if (!(NParser(tokens) is Parsed nparsed))
                    return new Unparsed();
                remains = nparsed.Remains;
                while (parser(remains) is Parsed currentParsed)
                {
                    parsed.Add(currentParsed);
                    remains = currentParsed.Remains;
                }
                return new Parsed(astCombiner_(parsed.Select(_ => _.Ast)), remains);
            };
        }

        protected Parser ExpectEnd()
        {
            return (tokens) =>
            {
                if (tokens.Any())
                    return new Unparsed();
                return new Parsed();
            };
        }

        protected Parser ExpectToken(string token, Func<AstBase> astMaker = null)
        {
            return (tokens) =>
            {
                var astMaker_ = astMaker;
                if (astMaker == null)
                    astMaker_ = () => null;
                var nextToken = tokens.FirstOrDefault();
                if (nextToken != null && nextToken.Value == token)
                    return new Parsed(astMaker_(), tokens.Skip(1));
                return new Unparsed();
            };
        }

        protected Parser ParseToken(Func<string, AstBase> astMaker)
        {
            return (tokens) =>
            {
                var nextToken = tokens.FirstOrDefault();
                if (nextToken != null && astMaker(nextToken.Value) is var ast && ast != null)
                    return new Parsed(ast, tokens.Skip(1));
                return new Unparsed();
            };
        }

        /*protected Func<IEnumerable<Token>, ParseResult> ParseFullToken(Func<Token, AstBase> astMaker)
        {
            return (tokens) =>
            {
                var nextToken = tokens.FirstOrDefault();
                if (nextToken != null && astMaker(nextToken) is var ast && ast != null)
                    return new Parsed(ast, tokens.Skip(1));
                return new Unparsed();
            };
        }*/

        protected Parser ConsumeByAny(params Parser[] parsers)
        {
            return (tokens) =>
            {
                if (parsers.Select(parser => parser(tokens)).FirstOrDefault() is var result && result != null && result.OkParsed)
                    return result;
                return new Unparsed();
            };
        }

        protected Parser ConsumeByAllAndReturnLast(params Parser[] parsers)
        {
            return (tokens) =>
            {
                ParseResult parsed = new Parsed() { Remains = tokens };
                foreach (var currentParser in parsers)
                {
                    if (currentParser(parsed.Remains) is var currentParsed && currentParsed.OkParsed)
                        parsed = currentParsed;
                    else
                    {
                        parsed = new Unparsed();
                        break;
                    }
                }
                return parsed;
            };
        }

        protected Parser ConsumeByAllAndCombine(Func<IEnumerable<AstBase>, AstBase> astCombiner, params Parser[] parsers)
        {
            return (tokens) =>
            {
                var astCombiner_ = astCombiner;
                if (astCombiner == null)
                    astCombiner_ = _ => null;
                var remains = tokens;
                var parsed = new List<Parsed>();
                foreach (var currentParser in parsers)
                {
                    if (currentParser(remains) is Parsed currentParsed)
                    {
                        parsed.Add(currentParsed);
                        remains = currentParsed.Remains;
                    }
                    else
                        return new Unparsed();
                }
                return new Parsed(astCombiner_(parsed.Select(_=>_.Ast)), remains);
            };
        }

        protected Parser ExpectTokens(Func<AstBase> astMaker, params string[] tokens)
        {
            var astMaker_ = astMaker;
            if (astMaker == null)
                astMaker_ = () => null;
            var expectTokens = tokens
                .Select(expectedToken => ExpectToken(expectedToken, () => null));
            var parsers = expectTokens
                .Concat(new[] { SuccessParseNothing(astMaker) })
                .ToArray();
            return ConsumeByAllAndReturnLast(parsers);
        }

        protected Parser ExpectTokens(params string[] tokens)
        {
            return ExpectTokens(null, tokens);
        }

        public ParseResult Parse(IEnumerable<Token> tokens)
        {
            var parser = MakeParser();
            var ast = parser(tokens);
            return ast;
        }

        protected abstract Parser MakeParser();
    }

    public sealed class CrawlerParser: ParserBase
    {
        protected override Parser MakeParser() 
        {
            return Statments()
                .Then(ExpectEnd());
        }

        private Parser Statments()
        {
            return ExpectToken("{")
                .Then(ConsumeNOrMoreAndCombine(Statment(), _ => new AstStatments() { Statments = _.ToArray() }))
                .Then(ExpectToken("}"));
        }

        private Parser Statment()
        {
            return ConsumeByAny(ifStatment(), varAssign(), funcAssign(), funcCall());
        }

        private Parser funcCall()
        {
            throw new NotImplementedException();
        }

        private Parser funcAssign()
        {
            throw new NotImplementedException();
        }

        private Parser varAssign()
        {
            throw new NotImplementedException();
        }

        private Parser ifStatment()
        {
            return ExpectTokens("if", "(")
                .Then(Expression())
                .Then(ExpectToken(")"))
                .Then(Statments())
                .OptionalThen(
                    ExpectToken("else")
                    .Then(Statments()));
        }

        private Parser Expression()
        {
            throw new NotImplementedException();
        }
    }
}
