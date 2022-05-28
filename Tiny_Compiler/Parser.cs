using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JASON_Compiler;
namespace Tiny_Compiler
{
    public class Node
    {
        public List<Node> Children = new List<Node>();

        public string Name;
        public Node(string N)
        {
            this.Name = N;
        }
    }
   public class Parser
    {
        int InputPointer = 0;
        List<Token> TokenStream;
        public Node root;
        public Node StartParsing(List<Token> TokenStream)
        {  
            this.InputPointer = 0;
            
            List<Token> newtokens = new List<Token>();

            foreach(var token in TokenStream)
            {
                if (token.token_type != Token_Class.Comment)
                    newtokens.Add(token);
            }

            this.TokenStream = newtokens;

            root = new Node("Program");
            root.Children.Add(Program());
            return root;
        }

        Node FunctionCall()
        {
            Node FunCall = new Node("Function Call");

            FunCall.Children.Add(match(Token_Class.Idenifier));
            FunCall.Children.Add(match(Token_Class.LParanthesis));
            FunCall.Children.Add(FunctionParameters());
            FunCall.Children.Add(match(Token_Class.RParanthesis));
            
            return FunCall;
        }
        Node FunctionParameters()
        {
            Node FunPara = new Node("FunctionParameters");
            if (Token_Class.Idenifier == TokenStream[InputPointer].token_type)
            {
                FunPara.Children.Add(match(Token_Class.Idenifier));
                FunPara.Children.Add(AnotherParams());
            }
            else if(Token_Class.Number == TokenStream[InputPointer].token_type)
            {
                FunPara.Children.Add(match(Token_Class.Number));
                FunPara.Children.Add(AnotherParams());
            }
            
            
            return FunPara;
        }
        Node AnotherParams()
        {
            Node AnotherPerms = new Node("AnotherParmeters");
            if(Token_Class.Comma == TokenStream[InputPointer].token_type)
            {
                AnotherPerms.Children.Add(match(Token_Class.Comma));
                AnotherPerms.Children.Add(IdentifierORNumber());
                AnotherPerms.Children.Add(AnotherParams());
            }
            return AnotherPerms;
        }
        Node IdentifierORNumber()
        {
            Node idornum = new Node("Identifier OR Number");
            if(Token_Class.Idenifier == TokenStream[InputPointer].token_type)
            {
                idornum.Children.Add(match(Token_Class.Idenifier));
            }
            else
            {
                idornum.Children.Add(match(Token_Class.Number));
            }
            return idornum;
        }
       Node Term() { 

        Node term = new Node("Term");
        
            if(Token_Class.Number == TokenStream[InputPointer].token_type)
            {
                term.Children.Add(match(Token_Class.Number));
            }
            else
            {
                term.Children.Add(match(Token_Class.Idenifier));
                term.Children.Add(TermDash());
            }
            return term;
        }
        Node TermDash()
        {
            if (InputPointer >= TokenStream.Count)
                return null;

            Node termdash = new Node("Term Dash");
            if(Token_Class.LParanthesis == TokenStream[InputPointer].token_type)
            {
                termdash.Children.Add(match(Token_Class.LParanthesis));
                termdash.Children.Add(FunctionParameters());
                termdash.Children.Add(match(Token_Class.RParanthesis));
            }
            return termdash;
        }
        Node BigEq()
        {
            Node bigeq = new Node("Big Equation");
            if(Token_Class.LParanthesis == TokenStream[InputPointer].token_type)
            {
                bigeq.Children.Add(match(Token_Class.LParanthesis));
                bigeq.Children.Add(BigEq());
                bigeq.Children.Add(match(Token_Class.RParanthesis));
                bigeq.Children.Add(OPequ());
            }
            else
            {
                bigeq.Children.Add(Equation());
                bigeq.Children.Add(OPequ());
            }
            return bigeq;
        }
        Node OPequ()
        {
            Node xdash = new Node("X dash");
            if(Arithmetic_operators() != Token_Class.Wrongone)
            {
                xdash.Children.Add(match(Arithmetic_operators()));
                xdash.Children.Add(BigEq());
            }
            return xdash;
        }
        Node Equation()
        {
            Node equ = new Node("Equation");
            if(Token_Class.Idenifier == TokenStream[InputPointer].token_type || Token_Class.Number == TokenStream[InputPointer].token_type)
            {
                equ.Children.Add(Term());
            }
            return equ;
        }
        Token_Class Arithmetic_operators()
        {
            if (InputPointer >= TokenStream.Count)
                return Token_Class.Wrongone;

            Token_Class[] ops = { Token_Class.PlusOp, Token_Class.MinusOp, Token_Class.MultiplyOp, Token_Class.DivideOp };
            foreach (var operation in ops)
            {
                
                if (operation == TokenStream[InputPointer].token_type)
                {
                    return operation;
                    
                }
            }
            
            return Token_Class.Wrongone;
        }

        Node Expression()
        {
            if (InputPointer >= TokenStream.Count)
                return null;
            Node Exp = new Node("Expression");
            if(Token_Class.String == TokenStream[InputPointer].token_type)
            {
                Exp.Children.Add(match(Token_Class.String));
            }
            else if(Token_Class.LParanthesis == TokenStream[InputPointer].token_type) {
                Exp.Children.Add(match(Token_Class.LParanthesis));
                Exp.Children.Add(BigEq());
                Exp.Children.Add(match(Token_Class.RParanthesis));
                Exp.Children.Add(OPequ());
            }
            else if(Arithmetic_operators() != Token_Class.Wrongone)
            {
                Exp.Children.Add(OPequ());
            }
            else if (Token_Class.Idenifier == TokenStream[InputPointer].token_type || Token_Class.Number == TokenStream[InputPointer].token_type)
            {
                Exp.Children.Add(Term());
                Exp.Children.Add(ExpDash());
            }
            return Exp;
        }
        Node ExpDash()
        {
            Node expdash = new Node("Exp Dash");
            if(Arithmetic_operators() != Token_Class.Wrongone)
            {
                expdash.Children.Add(OPequ());
            }
            return expdash;
        }
        Node Assigment_Statement()
        {
            Node assign = new Node("Assigment Statement");

            assign.Children.Add(match(Token_Class.Idenifier));
            assign.Children.Add(match(Token_Class.Assign));
            assign.Children.Add(Expression());

            return assign;
        }
        Token_Class Get_Matched_Data_Type()
        {

            Token_Class[] DataTypes = { Token_Class.datatype_int, Token_Class.datatype_string, Token_Class.datatype_float };
            foreach (var datatype in DataTypes)
            {
                if (datatype == TokenStream[InputPointer].token_type)
                    return datatype;
            }
            return Token_Class.Wrongone;
        }
        Node Declaration_Statement()
        {
            Node declstmnt = new Node("Declaration Statement");
            var datatype = Get_Matched_Data_Type();

            declstmnt.Children.Add(match(datatype));
            declstmnt.Children.Add(IdentiferORAssign());
            declstmnt.Children.Add(Multiple());
            declstmnt.Children.Add(match(Token_Class.Semicolon));
            return declstmnt;
        }
        Node IdentiferORAssign()
        {

            Node idorassi = new Node("IdenorAssign");
            if (Token_Class.Idenifier == TokenStream[InputPointer].token_type)
            {
                idorassi.Children.Add(match(Token_Class.Idenifier));
                idorassi.Children.Add(IdentiferORAssignDash());
            }

            return idorassi;
        }
        Node IdentiferORAssignDash()
        {
            Node iddash = new Node("IdentiferORAssignDash");
            if (Token_Class.Assign == TokenStream[InputPointer].token_type)
            {
                iddash.Children.Add(match(Token_Class.Assign));
                iddash.Children.Add(Expression());
            }
            return iddash;
        }
        Node Multiple()
        {
            Node multi = new Node("Muliple");
            if (TokenStream[InputPointer].token_type == Token_Class.Comma)
            {
                multi.Children.Add(match(Token_Class.Comma));
                multi.Children.Add(IdentiferORAssign());
                multi.Children.Add(Multiple());
            }
            return multi;
        }
        Node WriteBody()
        {
            Node Bdy = new Node("Write Body");

            if (Token_Class.endl == TokenStream[InputPointer].token_type)
                Bdy.Children.Add(match(Token_Class.endl));
            else
                Bdy.Children.Add(Expression());

            return Bdy;
        }
        Node Write_Statement()
        {
            Node Write = new Node("Write Statement");
            Write.Children.Add(match(Token_Class.Write));

            Write.Children.Add(WriteBody());

            Write.Children.Add(match(Token_Class.Semicolon));

            return Write;
        }
        Node Read_Statement()
        {
            Node Read = new Node("Read Statement");

            Read.Children.Add(match(Token_Class.Read));
            Read.Children.Add(match(Token_Class.Idenifier));
            Read.Children.Add(match(Token_Class.Semicolon));

            return Read;
        }
        Node Return_Statement()
        {
            Node Ret = new Node("Return Statement");

            Ret.Children.Add(match(Token_Class.Return));
            Ret.Children.Add(Expression());
            Ret.Children.Add(match(Token_Class.Semicolon));

            return Ret;
        }
        Token_Class Conditions_Operater()
        {
            Node CondsOP = new Node("Conditions Operater");
            Token_Class[] Condsoperators = { Token_Class.LessThanOp, Token_Class.GreaterThanOp, Token_Class.NotEqualOp, Token_Class.equal };
            foreach (var condop in Condsoperators)
            {
                if (condop == TokenStream[InputPointer].token_type)
                {
                    return condop;
                }

            }
            return Token_Class.Wrongone;
        }
        Node Condition()
        {
            Node cond = new Node("Condition");

            cond.Children.Add(match(Token_Class.Idenifier));
            cond.Children.Add(match(Conditions_Operater()));
            cond.Children.Add(Term());

            return cond;

        }
        Node BoolOP()
        {
            Node blop = new Node("Boolen Operator");

            if (InputPointer >= TokenStream.Count)
                return blop;

            if (Token_Class.Andop == TokenStream[InputPointer].token_type)
            {
                blop.Children.Add(match(Token_Class.Andop));
            }
            else if (Token_Class.Orop == TokenStream[InputPointer].token_type)
            {
                blop.Children.Add(match(Token_Class.Orop));

            }

            return blop;
        }
        Node Condition_Statement()
        {
            Node CondStmnt = new Node("Condition Statement");
            CondStmnt.Children.Add(Condition());
            CondStmnt.Children.Add(MultipleConditions());
            return CondStmnt;
        }
        Node MultipleConditions()
        {
            Node MultipleCond = new Node("MultipleConditions");
            Node blop = BoolOP();
            if (blop.Children.Count > 0)
            {
                MultipleCond.Children.Add(blop);
                MultipleCond.Children.Add(Condition());
                MultipleCond.Children.Add(MultipleConditions());
            }
            return MultipleCond;
        }

        Node IF()
        {
            Node ifstmnt = new Node("If Statement");

            ifstmnt.Children.Add(match(Token_Class.If));
            ifstmnt.Children.Add(Condition_Statement());
            ifstmnt.Children.Add(match(Token_Class.Then));
            ifstmnt.Children.Add(Set_Statement());
            ifstmnt.Children.Add(End());

            return ifstmnt;
        }
        Node End()
        {
            Node end = new Node("End");

            if (Token_Class.Elseif == TokenStream[InputPointer].token_type)

                end.Children.Add(ElseIF());

            else if (Token_Class.Else == TokenStream[InputPointer].token_type)

                end.Children.Add(Else());

            else 
                end.Children.Add(match(Token_Class.end));

            return end;
        }
        Boolean IsStatment()
        {
            if (InputPointer >= TokenStream.Count)
                return false;
            Token_Class[] statements = {Token_Class.Write,Token_Class.Read, Token_Class.Repeat , Token_Class.If,Token_Class.Idenifier};
            
            foreach(var statement in statements)
            {
                if (statement == TokenStream[InputPointer].token_type)
                    return true;
            }

            if (Get_Matched_Data_Type() != Token_Class.Wrongone)
                return true;

            return false;
        }
        Node Set_Statement()
        {
            Node St_Stmnt = new Node("Set Statement");
            St_Stmnt.Children.Add(Set_Statement_Dash());
            return St_Stmnt;
        }
        Node Set_Statement_Dash()
        {
            Node st_stmnt_dash = new Node("Set Statement Dash");
            if (IsStatment())
            {
               
                st_stmnt_dash.Children.Add(Statement());
                st_stmnt_dash.Children.Add(Set_Statement_Dash());
            }
            return st_stmnt_dash;
        }
        Node Repeat()
        {
            Node rpt = new Node("Repeat");

            rpt.Children.Add(match(Token_Class.Repeat));
            rpt.Children.Add(StatmentPlus());
            rpt.Children.Add(match(Token_Class.Until));
            rpt.Children.Add(Condition_Statement());

            return rpt;
        }
        Node Statement()
        {
            if(InputPointer >= TokenStream.Count)
                         return null;
            Node stmnt = new Node("Statement");
            if (Token_Class.Write == TokenStream[InputPointer].token_type)
            {
                stmnt.Children.Add(Write_Statement());
            }
            else if (Token_Class.Read == TokenStream[InputPointer].token_type)
            {
                stmnt.Children.Add(Read_Statement());
            }
            else if (Token_Class.If == TokenStream[InputPointer].token_type)
            {
                stmnt.Children.Add(IF());
            }
            else if (Token_Class.Repeat == TokenStream[InputPointer].token_type)
            {
                stmnt.Children.Add(Repeat());
            }
            else if (Get_Matched_Data_Type() != Token_Class.Wrongone)
            {
                stmnt.Children.Add(Declaration_Statement());
            }
            else if(Token_Class.Idenifier == TokenStream[InputPointer].token_type)
            {
                stmnt.Children.Add(Assigment_Statement());
            }

            return stmnt;

        }
        Node StatmentPlus()
        {
            Node StmntPlus = new Node("Statment Plus");
            if (IsStatment())
            {
                StmntPlus.Children.Add(Statement());
                StmntPlus.Children.Add(Stmntplusdash());
            }
            return StmntPlus;
        }
        Node Stmntplusdash()
        {
            Node stmnt = new Node("Statment Plus Dash");
            var stmntplus = StatmentPlus();
            if (stmntplus.Children.Count > 0)
            {
                stmnt.Children.Add(stmntplus);
            }
            return stmnt;
        }
        Node ElseIF()
        {
            Node elsif = new Node("Else If");

            elsif.Children.Add(match(Token_Class.Elseif));
            elsif.Children.Add(Condition_Statement());
            elsif.Children.Add(match(Token_Class.Then));
            elsif.Children.Add(Set_Statement());
            elsif.Children.Add(End());

            return elsif;
        }
        Node Else()
        {
            Node els = new Node("Else");

            els.Children.Add(match(Token_Class.Else));
            els.Children.Add(Set_Statement());
            els.Children.Add(End());

            return els;
        }

        Node FunctionName()
        {
            Node FunName = new Node("Function Name");
            if (TokenStream[InputPointer].lex == "main")
            {
                
                return FunName;
            }

            FunName.Children.Add(match(Token_Class.Idenifier));

            return FunName;
        }
        Node Parameter()
        {
            Node Parm = new Node("Parameter");
            var datatype = Get_Matched_Data_Type();

            Parm.Children.Add(match(datatype));
            Parm.Children.Add(match(Token_Class.Idenifier));

            return Parm;
        }
        Node FuncionDecleration()
        {
            Node FunDecl = new Node("FuncionDecleration");
            var datatype = Get_Matched_Data_Type();
            if (datatype != Token_Class.Wrongone)
            {
                FunDecl.Children.Add(match(datatype));
                var funName = FunctionName();
                if (funName.Children.Count > 0)
                {
                    FunDecl.Children.Add(funName);
                    FunDecl.Children.Add(match(Token_Class.LParanthesis));
                    FunDecl.Children.Add(Parameters());
                    FunDecl.Children.Add(match(Token_Class.RParanthesis));
                }
                else
                {
                    FunDecl.Children.RemoveAt(FunDecl.Children.Count - 1);
                    InputPointer--;
                }

            }
            return FunDecl;
        }
        Node Parameters()
        {
            Node paramss = new Node("Parameters");

            if (Get_Matched_Data_Type() != Token_Class.Wrongone)
            {
                paramss.Children.Add(Parameter());
                paramss.Children.Add(Morethanone());
            }
            return paramss;
        }
        Node Morethanone()
        {
            Node more = new Node("More than one Parameter");
            if (Token_Class.Comma == TokenStream[InputPointer].token_type)
            {
                more.Children.Add(match(Token_Class.Comma));
                more.Children.Add(Parameter());
                more.Children.Add(Morethanone());
            }
            return more;
        }
        Node FunctionBody()
        {
            Node FunBd = new Node("Function Body");

            FunBd.Children.Add(match(Token_Class.LBraces));
            FunBd.Children.Add(Set_Statement());
            FunBd.Children.Add(Return_Statement());
            FunBd.Children.Add(match(Token_Class.RBraces));


            return FunBd;
        }
        Node Function_Statment()
        {
            Node funstmnt = new Node("Function Statement");
            var decl = FuncionDecleration();
            if (decl.Children.Count > 0)
            {
                funstmnt.Children.Add(decl);
                funstmnt.Children.Add(FunctionBody());
            }
            return funstmnt;
        }
        Node Main_Function()
        {
            Node main = new Node("Main");
            var datatype = Get_Matched_Data_Type();

            main.Children.Add(match(datatype));
            main.Children.Add(match(Token_Class.main));
            main.Children.Add(match(Token_Class.LParanthesis));
            main.Children.Add(match(Token_Class.RParanthesis));
            main.Children.Add(FunctionBody());

            return main;
        }
        Node Program()
        {
            Node program = new Node("Program");

            program.Children.Add(MoreFunsta());
            program.Children.Add(Main_Function());
            return program;
        }
        Node MoreFunsta()
        {
            Node MoreThanFun = new Node("More than Fun");

            if (Get_Matched_Data_Type() != Token_Class.Wrongone)
            {
                var funstmnt = Function_Statment();
                if (funstmnt.Children.Count > 0)
                {
                    MoreThanFun.Children.Add(funstmnt);
                    MoreThanFun.Children.Add(MoreFunsta());
                }
            }
            return MoreThanFun;
        }
        public Node match(Token_Class ExpectedToken)
        {

            if (InputPointer < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[InputPointer].token_type)
                {
                    InputPointer++;
                    Node newNode = new Node(ExpectedToken.ToString());
                    
                    return newNode;

                }

                else
                {
                   
                    Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[InputPointer].token_type.ToString() +
                        "  found\r\n");
                    InputPointer++;
                    return null;
                }
            }
            else
            {
                

                Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + "\r\n");
                InputPointer++;
                return null;
            }
            
        }

        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;

            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }
    }
}