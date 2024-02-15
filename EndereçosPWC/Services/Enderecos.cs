using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using EndereçosPWC.Exceptions;

namespace EndereçosPWC.Services
{
    public class Enderecos
    {
        public static string[] SepararStrings(string adress)
        {
            adress = adress.Replace(",", "");
            string[] substrings = adress.Split(" ");
            return substrings;
        }

        public static string BuscarNumero(string[] substrings)
        {
            string number = "";
            string[] numberPatterns = {@"\b[0-9]+\b", @"\b[0-9]+[a-zA-Z]{1}\b"};
            string[] abreviattionPatterns = {@"\bn\b", @"\bno\b", @"\bN\b", @"\bNo\b", @"\bNO\b"};
            bool numberValidate = false;
            int ssCount = substrings.Count() - 1;
            int count = 0;
            

            do
            {

                foreach (string nPattern in numberPatterns)
                {
                    if (Regex.IsMatch(substrings[ssCount], nPattern))
                    {
                        foreach (string pattern in abreviattionPatterns)
                        {
                            if (Regex.IsMatch(substrings[ssCount - 1], pattern))
                            {
                                number += substrings[ssCount - 1] + " ";
                                break;
                            }
                        }
                        
                        number += substrings[ssCount];

                        if (count != 0 && Regex.IsMatch(substrings[ssCount + 1], @"\b[a-zA-Z]{1}\b"))
                        {
                            number += " " + substrings[ssCount + 1];
                        }

                        numberValidate = true;
                        break;
                    }
                    else if (Regex.IsMatch(substrings[count], nPattern))
                    {
                        foreach (string pattern in abreviattionPatterns)
                        {
                            if (count != 0 && Regex.IsMatch(substrings[count - 1], pattern))
                            {
                                number += substrings[count - 1] + " ";
                            }
                        }

                        number += substrings[count];

                        if (Regex.IsMatch(substrings[count + 1], @"\b[a-zA-Z]{1}\b"))
                        {
                            number += " " + substrings[count + 1];
                        }

                        numberValidate = true;
                        break;
                    }
                }

                ssCount--;
                count++;
            }
            while (!numberValidate);

            return number;
        }

        public static string RemoverNumero(string number, string[] substrings)
        {
            List<string> noNumberSubstrings = new List<string>();

            for (int i = 0, n = substrings.Length; i < n; i++)
            {
                if (!number.Contains(substrings[i]))
                {
                    noNumberSubstrings.Add(substrings[i]);
                }
            }

            string street = string.Join(" ", noNumberSubstrings);
            
            return street;
        }

        public static void ArquivarEndereco(string street, string number)
        {
            bool added;
            using (StreamReader sr = new StreamReader("ArquivoEnderecos.txt"))
            {
                added = sr.ReadToEnd().Contains($"\"{street}\", \"{number}\"");
            }

            using (StreamWriter sw = File.AppendText("ArquivoEnderecos.txt"))
            {
                if (added)
                {
                    throw new EnderecoJaFoiSalvoException();
                }
                else
                {
                    sw.WriteLine($"\"{street}\", \"{number}\"");
                }
            }
        }

        public static void VisualizarArquivo()
        {
            using (StreamReader sr = new StreamReader("ArquivoEnderecos.txt"))
            {
                Console.WriteLine(sr.ReadToEnd());
            }
        }
    }
}