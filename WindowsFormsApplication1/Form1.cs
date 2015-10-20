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
        private LinkedList<string> frequencyValues = new LinkedList<string>();
        private LinkedList<string> gainValues = new LinkedList<string>();
        LinkedList<string> qualityValues = new LinkedList<string>();

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
            // handles mutiple files that have been selected for drag and drop
            // neeed to find the target, best case.
            /*foreach (string file in fileLocations)
            {
                parseTarget += file;
                parseTarget = System.IO.File.ReadAllText(file);
            }*/
            //This just blindly takes the first file read in (if there are multiples, use above code)
            // Oct 7, 2015 7:21:19 
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
                    exportValue += parseSubstring(target.Substring(areaOfFocus, 61));
                    areaOfFocus = 61 * counter;
                }
            }
            Console.Write(exportValue);
            return;
        }
        string parseSubstring(string substring)
        {

            frequencyValues.AddLast(substring.Substring(28, 7));
            gainValues.AddLast(substring.Substring(45,5));
            qualityValues.AddLast(substring.Substring(57, 4));

            string finalValue = "[Frequencies]";
            foreach (string value in frequencyValues)
            {
                int counter = 1;
                string holder = value;
                holder = value.Replace(" ", String.Empty);
                finalValue += "Frequency" + counter + "=" + holder + "\r\n";
                counter++;
            }

            finalValue += "[Gains]";
            foreach (string value in gainValues)
            {
                int counter = 1;
                finalValue += "Gain" + counter + "=" + value + "\r\n";
                counter++;
            }

            finalValue += "[Qualities]";
            foreach (string value in qualityValues)
            {
                int counter = 1;
                finalValue += "Quality" + counter + "=" + value + "\r\n";
                counter++;
            }
            return finalValue;
        }
    }
}
