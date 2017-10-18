using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace DebugLogging
{
    public class Log
    {
        public void writeLog(string text, bool debug = true)
        {
            using (StreamWriter file = new StreamWriter(Convert.ToString(Directory.GetCurrentDirectory()))) 
            {
                file.WriteLine(text);
            }
            

        }

    }

    public class ConsoleLog : TextWriter
    {
        TextBox _output = null;

        public void BoxWrite(TextBox output)
        {
            _output = output;
        }
        public override void Write(char value)
        {
            base.Write(value);
            _output.AppendText(value.ToString()); //when the data is written, append it to the text box
        }

        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }


    }

    public partial class FormConsole : Form
    {
        TextWriter _w = null;

        
    }
}
