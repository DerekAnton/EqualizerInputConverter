using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        private string[] fileLocations;
        private string parseTarget;
        private string exportValue;
        string frequencies = "", gains = "", qualities = "";

        public Form1()
        {
            InitializeComponent();
            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(Form1_DragEnter);
            this.DragDrop += new DragEventHandler(Form1_DragDrop);
            exportValue = "";
        }

        void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        void Form1_DragDrop(object sender, DragEventArgs e)
        {
            fileLocations = (string[])e.Data.GetData(DataFormats.FileDrop);
            parseTarget = System.IO.File.ReadAllText(fileLocations[0]);
            parseHeader(parseTarget);
        }

        void parseHeader(string target)
        {
            target = target.Replace("\r\n", String.Empty);
            target = target.Replace("Filter Settings fileRoom EQ V5.13Dated:", String.Empty);
            //remove the date time (might be off by 1 character if it is a 2 digit date?)
            target = target.Substring(20, target.Length - 20);
            target = target.Replace("PMNotes:flat-bothEqualiser: GenericAverage 1", String.Empty);
            if(target[0].Equals(' '))
            {
                target = target.Substring(1, target.Length - 1);
            }

            int areaOfFocus = 0; // This denotes the 61 characters that makes up each line of values to pull from the txt file.
            for (int counter = 0; counter < 20; counter ++) //for each filter (there will always be 20 filters)
            {

                if (target.Substring(14 + areaOfFocus, 8).Equals(" None   "))
                {
                    // the empty filter case. nothing to do here.
                    areaOfFocus += 14;
                }
                else
                {
                    parseSubstring(target.Substring(areaOfFocus, 61), counter);
                    areaOfFocus = 61 * counter;
                }
            }
            exportValue = frequencies + gains + qualities;
            Console.Write(exportValue);
            return;
        }

        void parseSubstring(string substring, int index)
        {

            if (index == 0)
                frequencies = "[Frequencies]";

            string fholder = substring.Substring(28, 7);
            fholder = fholder.Replace(" ", String.Empty);
            frequencies += "Frequency" + (index+1) + "=" + fholder + "\r\n";

            if (index == 0)
                gains = "[Gains]";

            string gholder = substring.Substring(45, 5);
            gholder = gholder.Replace(" ", String.Empty);
            gains += "Gain" + (index+1) + "=" + gholder + "\r\n";
                
            if (index == 0)
                qualities = "[Qualities]";

            string qholder = substring.Substring(57, 4);
            qholder = qholder.Replace(" ", String.Empty);
            qualities += "Quality" + (index+1) + "=" + qholder + "\r\n";
        }
    }
}
