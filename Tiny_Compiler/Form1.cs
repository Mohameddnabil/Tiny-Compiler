using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tiny_Compiler;
using JASON_Compiler;
namespace JASON_Compiler
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            textBox1.ScrollBars = ScrollBars.Vertical;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            button2_Click(sender, e);

            textBox2.Clear();
            
            string Code = textBox1.Text;
            Tiny_Compiler.Start_Compiling(Code);
            PrintTokens();
            
            treeView1.Nodes.Add(Parser.PrintParseTree(Tiny_Compiler.treeroot));

            PrintErrors();
        }
        void PrintTokens()
        {
            for (int i = 0; i < Tiny_Compiler.Jason_Scanner.Tokens.Count; i++)
            {
               dataGridView1.Rows.Add(Tiny_Compiler.Jason_Scanner.Tokens.ElementAt(i).lex, Tiny_Compiler.Jason_Scanner.Tokens.ElementAt(i).token_type);
            }
        }

        void PrintErrors()
        {
            for(int i=0; i<Errors.Error_List.Count; i++)
            {
                textBox2.Text += Errors.Error_List[i];
                textBox2.Text += "\r\n";
            }
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            textBox2.Clear();
            Errors.Error_List.Clear();
            Tiny_Compiler.TokenStream.Clear();
            treeView1.Nodes.Clear();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
       
    }
}
