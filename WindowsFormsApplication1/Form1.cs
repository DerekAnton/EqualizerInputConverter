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
        string frequencies = "", gains = "", qualities = "", header, footer;

        public Form1()
        {
            InitializeComponent();
            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(Form1_DragEnter);
            this.DragDrop += new DragEventHandler(Form1_DragDrop);
            exportValue = "";
            header = "[Speakers]\r\nSpeakerId0=0\r\nSpeakerTargets0=all\r\nSpeakerName0=All\r\nSpeakerId1=1\r\nSpeakerTargets1=L\r\nSpeakerName1=Left\r\nSpeakerId2=2\r\nSpeakerTargets2=R\r\nSpeakerName2=Right\r\nSpeakerId3=3\r\nSpeakerTargets3=C\r\nSpeakerName3=Center\r\nSpeakerId4=4\r\nSpeakerTargets4=SUB\r\nSpeakerName4=Subwoofer\r\nSpeakerId5=5\r\nSpeakerTargets5=RL\r\nSpeakerName5=Left rear\r\nSpeakerId6=6\r\nSpeakerTargets6=RR\r\nSpeakerName6=Right rear\r\nSpeakerId7=7\r\nSpeakerTargets7=SL\r\nSpeakerName7=Left side\r\nSpeakerId8=8\r\nSpeakerTargets8=SR\r\nSpeakerName8=Right side\r\n[General]\r\nPreAmp=2.5\r\n";
            footer = "[Configuration]\r\nHotKey =\r\n";
        }

        void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        void Form1_DragDrop(object sender, DragEventArgs e)
        {
            fileLocations = (string[])e.Data.GetData(DataFormats.FileDrop);
            parseTarget = System.IO.File.ReadAllText(fileLocations[0]);
            parseMain(parseTarget);
        }

        void parseMain(string target)
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

            createFile();
            return; // End of conversion
        }

        void parseSubstring(string substring, int index)
        {

            if (index == 0)
                frequencies = "[Frequencies]\r\n";

            string fholder = substring.Substring(28, 7);
            fholder = fholder.Replace(" ", String.Empty);
            fholder = fholder.Replace(",", String.Empty);
            frequencies += "Frequency" + (index+1) + "=" + fholder + "\r\n";

            if (index == 0)
                gains = "[Gains]\r\n";

            string gholder = substring.Substring(45, 5);
            gholder = gholder.Replace(" ", String.Empty);
            gains += "Gain" + (index+1) + "=" + gholder + "\r\n";
                
            if (index == 0)
                qualities = "[Qualities]\r\n";

            string qholder = substring.Substring(57, 4);
            qholder = qholder.Replace(" ", String.Empty);
            qualities += "Quality" + (index+1) + "=" + qholder + "\r\n";
        }

        void createFile()
        {
            string directory = @"C:\Users\Public\PeaceConverter\";
            System.IO.Directory.CreateDirectory(directory);
            exportValue = header + frequencies + gains + qualities + footer;
            System.IO.File.WriteAllText(@"C:\Users\Public\PeaceConverter\ConvertedFile.peace", exportValue);
            System.Windows.Forms.MessageBox.Show("Conversion success \r\nFile saved to "+ directory);
        }
    }
}
