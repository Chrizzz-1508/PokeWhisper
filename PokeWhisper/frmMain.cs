using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace PokeWhisper
{
    public partial class frmMain : Form
    {
        List<Pokemon> lsPokedex = new List<Pokemon>();
        public frmMain()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string sUser = "";
            List<string> sPokemonNumbers = new List<string>();

            using (StreamReader sr = new StreamReader("Pokedex.csv"))
            {
                while(sr.Peek() > 0)
                {
                    string sTemp = sr.ReadLine();
                    string[] sTempSplit = sTemp.Split(';');
                    Pokemon p = new Pokemon(sTempSplit[0], sTempSplit[1], sTempSplit[2]);
                    lsPokedex.Add(p);
                }
                sr.Close();
            }

            using (StreamReader sr = new StreamReader("whisper.ini"))
            {
                sr.ReadLine();
                sUser = sr.ReadLine().Split('"')[1];
                sr.Close();
            }


            using (StreamReader sr = new StreamReader("Pokemon_trainers.ini"))
            {
                string[] sUserblocks = sr.ReadToEnd().Split('[');
                foreach (string s in sUserblocks)
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        string sUsername = s.Split(']')[0];
                        if(sUsername == sUser)
                        {
                            string[] sLines = s.Split(']')[1].Split('\n');
                            foreach(string sLine in sLines)
                            {
                                sPokemonNumbers.Add(sLine.Split('=')[0].Replace("\"", ""));
                            }
                        }
                    }
                }
                sr.Close();
            }

            string sNormal = "";

            for (int i = 0; i < lsPokedex.Count; i++)
            {
                for(int i2 = 0; i2 < sPokemonNumbers.Count; i2++)
                {
                    if(lsPokedex[i].sNumber == sPokemonNumbers[i2])
                    {
                        sNormal += "#" + lsPokedex[i].sNumber + " - ";
                        if (Properties.Settings.Default.language == "de") sNormal += lsPokedex[i].sGerman + ", ";
                        else sNormal += lsPokedex[i].sEnglish + ", ";
                    }
                }                            
            }

            if(sNormal.Length > 0)
            {
                sNormal = sNormal.Remove(sNormal.Length - 2);
            }

            string sShiny = "";

            for (int i = 0; i < lsPokedex.Count; i++)
            {
                for (int i2 = 0; i2 < sPokemonNumbers.Count; i2++)
                {
                    if (lsPokedex[i].sNumber + "S" == sPokemonNumbers[i2])
                    {
                        sShiny += "#" + lsPokedex[i].sNumber + " - ";
                        if (Properties.Settings.Default.language == "de") sShiny += lsPokedex[i].sGerman + ", ";
                        else sShiny += lsPokedex[i].sEnglish + ", ";
                    }
                }
            }

            if (sShiny.Length > 0)
            {
                sShiny = sShiny.Remove(sShiny.Length - 2);
            }



            using (StreamWriter sw = new StreamWriter("whisperoutput.ini",false))
            {
                sw.WriteLine("[Message]");

                double iNormalLength = Convert.ToDouble(sNormal.Length) / 1950;
                double iNormalParts = Math.Ceiling(iNormalLength);
                int iNormalTotal = sNormal.Length;
                int iNormalCurrentLength = 0;
                for(int i = 0; i < iNormalParts; i++)
                {
                    if (iNormalTotal < 1950) iNormalCurrentLength = iNormalTotal;
                    else iNormalCurrentLength = 1950;
                    sw.WriteLine("normal" + i.ToString() + "=\"" + sNormal.Substring(i*1950, iNormalCurrentLength) + "\"");
                    iNormalTotal -= 1950;
                }

                double iShinyLength = Convert.ToDouble(sShiny.Length) / 1950;
                double iShinyParts = Math.Ceiling(iShinyLength);
                int iShinyTotal = sShiny.Length;
                int iShinyCurrentLength = 0;
                for (int i = 0; i < iShinyParts; i++)
                {
                    if (iShinyTotal < 1950) iShinyCurrentLength = iShinyTotal;
                    else iShinyCurrentLength = 1950;
                    sw.WriteLine("shiny" + i.ToString() + "=\"" + sShiny.Substring(i * 1950, iShinyCurrentLength) + "\"");
                    iShinyTotal -= 1950;
                }

                sw.Flush();
                sw.Close();
            }
            Application.Exit();
        }
    }

    public class Pokemon
    {
        public string sNumber;
        public string sGerman;
        public string sEnglish;

        public Pokemon(string sNumber, string sEnglish, string sGerman)
        {
            this.sNumber = sNumber;
            this.sGerman = sGerman;
            this.sEnglish = sEnglish;
        }

        public override string ToString()
        {
            return "English: " + sEnglish + " Deutsch: " + sGerman;
        }
    }
}
