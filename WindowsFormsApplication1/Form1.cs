using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        private string[] fileLocations;
        private string parseTarget, exportValue, frequencies, gains, qualities, vanishingCageDirectory, draggedFileName, header, footer, directory;
        private bool noDumpStored = false;

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult popupResult = MessageBox.Show("Would you like to change the default directory to convert to?", "Alert", MessageBoxButtons.YesNo);
            if (popupResult == DialogResult.Yes)
            {
                directory = loadFromDumpFile(noDumpStored);
                File.WriteAllText(directory + draggedFileName + ".peace", exportValue);
                DialogResult popupResult2 = MessageBox.Show("Default has been changed successfully");
            }
            else
            {/*do nothing*/}
        }

        public Form1()
        {
            InitializeComponent();
            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(Form1_DragEnter);
            this.DragDrop += new DragEventHandler(Form1_DragDrop);
            frequencies = "";
            gains = "";
            qualities = "";
            exportValue = "";
            directory = "";
            header = "[Speakers]\r\nSpeakerId0=0\r\nSpeakerTargets0=all\r\nSpeakerName0=All\r\nSpeakerId1=1\r\nSpeakerTargets1=L\r\nSpeakerName1=Left\r\nSpeakerId2=2\r\nSpeakerTargets2=R\r\nSpeakerName2=Right\r\nSpeakerId3=3\r\nSpeakerTargets3=C\r\nSpeakerName3=Center\r\nSpeakerId4=4\r\nSpeakerTargets4=SUB\r\nSpeakerName4=Subwoofer\r\nSpeakerId5=5\r\nSpeakerTargets5=RL\r\nSpeakerName5=Left rear\r\nSpeakerId6=6\r\nSpeakerTargets6=RR\r\nSpeakerName6=Right rear\r\nSpeakerId7=7\r\nSpeakerTargets7=SL\r\nSpeakerName7=Left side\r\nSpeakerId8=8\r\nSpeakerTargets8=SR\r\nSpeakerName8=Right side\r\n[General]\r\nPreAmp=2.5\r\n";
            footer = "[Configuration]\r\nHotKey =\r\n";
            vanishingCageDirectory = "C:\\Users\\Public\\VanishingCage\\"; // Create a default directory to use if nothing was selected.
            Directory.CreateDirectory(vanishingCageDirectory);
            pictureBox1.Image = Properties.Resources.final_box_sized;
        }

        void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        void Form1_DragDrop(object sender, DragEventArgs e)
        {
            fileLocations = (string[])e.Data.GetData(DataFormats.FileDrop);
            StreamReader file = new StreamReader(fileLocations[0]);
            string line = "";
            int counter = 0;
            while ((line = file.ReadLine()) != null)
            {
                //  Skip the first 8 lines, as these are the header.
                if (counter > 8) 
                    parseTarget += line;
                counter++;
            }

            draggedFileName = Path.GetFileNameWithoutExtension(fileLocations[0]);
            parseMain(parseTarget, draggedFileName);
        }

        void parseMain(string target, string filename)
        {
            int areaOfFocus = 0; // This denotes the 61 characters that makes up each line of values to pull from the txt file.
            for (int counter = 0; counter < 20; counter ++) //for each filter (there will always be 20 filters)
            {

                if (target.Substring(14 + areaOfFocus, 8).Equals(" None   "))
                {
                    // The empty filter case. nothing to do here.
                    areaOfFocus += 22;
                }
                else
                {
                    parseSubstring(target.Substring(areaOfFocus, 61), counter);
                    areaOfFocus += 61;
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

            string gholder = substring.Substring(44, 5);
            gholder = gholder.Replace(" ", String.Empty);
            gains += "Gain" + (index+1) + "=" + gholder + "\r\n";
                
            if (index == 0)
                qualities = "[Qualities]\r\n";

            string qholder = substring.Substring(57, 4);
            qholder = qholder.Replace(" ", String.Empty);
            qualities += "Quality" + (index+1) + "=" + qholder + "\r\n";
        }

        string loadFromDumpFile(bool noDumpStored)
        {
            string directory = "";
            DialogResult popupResult = MessageBox.Show("Please select a default folder to convert into.", "Alert", MessageBoxButtons.OK);

            if (popupResult == DialogResult.OK)
            {
                var fD = new FolderBrowserDialog();

                if (fD.ShowDialog() == DialogResult.OK)
                {
                    directory = fD.SelectedPath + "\\";
                    File.WriteAllText(vanishingCageDirectory + "SavedDirectory.dump", directory );
                    if (directory == "" || directory == null)
                    {
                        directory = vanishingCageDirectory;// Save to default directory 
                    }
                }
                else
                {
                    directory = vanishingCageDirectory; // Save to default directory 
                }
            }
            return directory;
        }

        void createFile()
        {
            // Default directory


            if (File.Exists(vanishingCageDirectory + "SavedDirectory.dump"))
            {
                directory = File.ReadAllText(vanishingCageDirectory + "SavedDirectory.dump");
                noDumpStored = false;
            }
            else
            {
                noDumpStored = true;
                directory = loadFromDumpFile(noDumpStored);
            }

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Create the output file in string form
            exportValue = header + frequencies + gains + qualities + footer;
            File.WriteAllText(directory + "ConvertedFile.peace", exportValue);
            MessageBox.Show("Conversion Success! saved to "+ directory);
        }

    }
}
