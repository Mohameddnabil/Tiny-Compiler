using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public enum Token_Class
{
    Else, If, Then,
    Read, Until, Write,
    Dot, Semicolon, Comma, LParanthesis, RParanthesis, Assign, LessThanOp, endl,
    GreaterThanOp, NotEqualOp, PlusOp, MinusOp, MultiplyOp, DivideOp, Return, equal, notequal,
    Idenifier, Number,LComment, RComment,Comment,Quote,String,Andop,Orop,Int,Float, Repeat, Elseif,LBraces,RBraces,
    datatype_int, datatype_float, datatype_string,Wrongone,end, main
}
namespace JASON_Compiler
{
    

    public class Token
    {
       public string lex;
       public Token_Class token_type;
    }

    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();

        public Scanner()
        {
            ReservedWords.Add("int", Token_Class.datatype_int);
            ReservedWords.Add("float", Token_Class.datatype_float);
            ReservedWords.Add("string", Token_Class.datatype_string);
            ReservedWords.Add("read", Token_Class.Read);
            ReservedWords.Add("write", Token_Class.Write);
            ReservedWords.Add("repeat", Token_Class.Repeat);
            ReservedWords.Add("until", Token_Class.Until);
            ReservedWords.Add("if", Token_Class.If);
            ReservedWords.Add("elseif", Token_Class.Elseif);
            ReservedWords.Add("else", Token_Class.Else);
            ReservedWords.Add("then", Token_Class.Then);
            ReservedWords.Add("return", Token_Class.Return);
            ReservedWords.Add("endl", Token_Class.endl);
            ReservedWords.Add("end", Token_Class.end);
            ReservedWords.Add("main", Token_Class.main);
            


            Operators.Add(";", Token_Class.Semicolon);
            Operators.Add(",", Token_Class.Comma);
            Operators.Add("(", Token_Class.LParanthesis);
            Operators.Add(")", Token_Class.RParanthesis);
            Operators.Add(":=", Token_Class.Assign);
            Operators.Add("<", Token_Class.LessThanOp);
            Operators.Add(">", Token_Class.GreaterThanOp);
            Operators.Add("<>", Token_Class.NotEqualOp);
            Operators.Add("+", Token_Class.PlusOp);
            Operators.Add("-", Token_Class.MinusOp);
            Operators.Add("*", Token_Class.MultiplyOp);
            Operators.Add("/", Token_Class.DivideOp);
            Operators.Add("&&", Token_Class.Andop);
            Operators.Add("||", Token_Class.Orop);
            Operators.Add("{", Token_Class.LBraces);
            Operators.Add("}", Token_Class.RBraces);
            Operators.Add("=", Token_Class.equal);




        }
        int line = 1;
        public void StartScanning(string SourceCode)
        {
            line = 1;
            for (int i = 0; i < SourceCode.Length; i++)
            {

                int j = i;
                char CurrentChar = SourceCode[i];
                string CurrentLexeme = "";

                if (CurrentChar == '\n')
                    line++;
                if (CurrentChar == ' ' || CurrentChar == '\t' || CurrentChar == '\r' || CurrentChar == '\n')
                    continue;

                if (isLetter(CurrentChar)) //if you read a character
                {
                    for (; j < SourceCode.Length && (isLetter(SourceCode[j]) || isDigit(SourceCode[j])); j++)
                        CurrentLexeme += SourceCode[j];

                    i = j - 1;
                    FindTokenClass(CurrentLexeme);
                }

                else if (isDigit(CurrentChar))
                {
                    for (; j < SourceCode.Length && (SourceCode[j] == '.' || isDigit(SourceCode[j])); j++)
                        CurrentLexeme += SourceCode[j];

                    i = j - 1;
                    FindTokenClass(CurrentLexeme);
                }

                //comment

                else if (i+1 < SourceCode.Length && ( ( SourceCode[i].ToString() + SourceCode[i + 1]).Equals("/*") ) ) 
                {
                    for (int o = i; o < SourceCode.Length; o++) 
                    {
                        CurrentLexeme += SourceCode[o];

                        if (o + 1 < SourceCode.Length && ( (SourceCode[o].ToString() + SourceCode[o + 1]).Equals("*/") ) )
                        {
                            CurrentLexeme += "/";
                            FindTokenClass(CurrentLexeme);
                            i = o + 1;
                            break;
                        }
                        else if (o == SourceCode.Length - 1)
                        {
                            FindTokenClass(CurrentLexeme);
                            i = SourceCode.Length - 1;
                            break;
                        }

                    }
                }
                //string
                else if (i < SourceCode.Length && SourceCode[i] == '\"' )
                {
                    CurrentLexeme += SourceCode[i];
                    i++;

                    for (int o = i; o < SourceCode.Length; o++) //String
                    {
                        CurrentLexeme += SourceCode[o];

                        if (o < SourceCode.Length && SourceCode[o] == '\"' )
                        {
                            Console.WriteLine(CurrentLexeme);
                            FindTokenClass(CurrentLexeme);
                            i = o;
                            break;
                        }
                        else if (o == SourceCode.Length - 1)
                        {
                            FindTokenClass(CurrentLexeme);
                            i = SourceCode.Length - 1;
                            break;
                        }
                    }


                }

                else if (i + 1 < SourceCode.Length && ( (SourceCode[i].ToString() + SourceCode[i + 1]).Equals(":=") ) )
                {
                    CurrentLexeme += ":=";
                    FindTokenClass(CurrentLexeme);
                    i++;
                }
                else if (i + 1 < SourceCode.Length && (SourceCode[i].ToString() + SourceCode[i + 1]).Equals("<>") )
                {
                    CurrentLexeme += "<>";
                    FindTokenClass(CurrentLexeme);
                    i++;
                }
                else if (i + 1 < SourceCode.Length && (SourceCode[i].ToString() + SourceCode[i + 1]).Equals("&&"))
                {
                    CurrentLexeme += "&&";
                    FindTokenClass(CurrentLexeme);
                    i++;
                }
                else if (i + 1 < SourceCode.Length && (SourceCode[i].ToString() + SourceCode[i + 1]).Equals("||"))
                {
                    CurrentLexeme += "||";
                    FindTokenClass(CurrentLexeme);
                    i++;
                }
                else
                {

                    CurrentLexeme += SourceCode[j];
                    FindTokenClass(CurrentLexeme);

                }

            }

            Tiny_Compiler.TokenStream = Tokens;
        }


        void FindTokenClass(string Lex) // Tokennsss
        {
            Token_Class TC;
            Token Tok = new Token();
            Tok.lex = Lex;
            //Is it a reserved word?
            if(isReservedword(Lex))
            {
                Tok.token_type = ReservedWords[Lex];
                Tokens.Add(Tok);
            }
            //Is it an identifier?
            else if (isIdentifier(Lex))
            {
                Tok.token_type = Token_Class.Idenifier;
                Tokens.Add(Tok);
            }

            //Is it a Constant?
            else if (isNumber(Lex))
            {
                Tok.token_type = Token_Class.Number;
                Tokens.Add(Tok);
            }
            //Is it an operator?
            else if (isOperator(Lex))
            {
                Tok.token_type = Operators[Lex];
                Tokens.Add(Tok);
            }
            else if (isComment(Lex))
            {
                Tok.lex = Lex;
                Tok.token_type = Token_Class.Comment;
                Tokens.Add(Tok);
            }
            else if (isString(Lex))
            {
                Tok.lex = Lex;
                Tok.token_type = Token_Class.String;
                Tokens.Add(Tok);
            }
            //Is it an undefined?
            else 
            {
                Errors.Error_List.Add("Line "+ line +"   Error at : " + Lex);
            }
        }

        // Functionsssssssss
        bool isLetter(char source)
        {
            return ((source <= 'z' && source >= 'a') || (source <= 'Z' && source >= 'A'));
        }
        bool isDigit(char source)
        {
            return (source >= '0' && source <= '9');
        }


        bool isIdentifier(string lex)
        {
            Regex reg = new Regex(@"[a-zA-Z]([a-z]|[A-Z]|[0-9])*$",RegexOptions.Compiled);
            return reg.IsMatch(lex);
        }
        bool isNumber(string lex)
        {
            Regex reg = new Regex(@"^[0-9]+([.][0-9]+)?$", RegexOptions.Compiled);
            return reg.IsMatch(lex);
        }

        bool isReservedword(string lex)
        {
           return ReservedWords.ContainsKey(lex);
        }
        bool isOperator(string lex)
        {
            return Operators.ContainsKey(lex);
        }
        bool isComment(string lex)
        {
            Regex reg = new Regex("([/][/*])(.*?)([/*][/])", RegexOptions.Singleline);
            return reg.IsMatch(lex);
           // return (lex.Contains("/*") && lex.Contains("*/"));
        }
        bool isString(string lex)
        {
            Regex reg = new Regex("([\"])(.*?)([\"])", RegexOptions.Compiled);
            return reg.IsMatch(lex);
            
        }
    }
}
