using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Model
{
    public class Livre
    {
        private string _url;
        public string Nom { get; private set; }
        public string Id { get; private set; }
        public Uri MonBook { get; private set; }
        

        public Livre(string livre)
        {
            Id = livre.Split("-")[0];
            Nom = livre;
            _url = $"https://gutenberg.org/files/{Id}/{Id}-h/{Id}-h.htm";
            
        }
        public override string ToString()
        {
            return Nom;
        }

        public void DownloadLivre()
        {
            string directory = Path.Combine(Path.GetTempPath(), @$"livreTemp\");
            string filePath = Path.Combine(directory, $"{Id}.html");
            if (!File.Exists(filePath))
            {
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                try
                {
                    WebRequest request = WebRequest.Create(_url);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    List<string> lelivre = new List<string>();
                    while (!reader.EndOfStream)
                    {

                        lelivre
                        .Add(reader.ReadLine()
                        .Replace("—", "")
                        .Replace("&#151;", "")
                        .Replace("&mdash;", "")
                        .Replace("--", "")
                        .Replace("!", " !")
                        .Replace("?", " ?")
                        .Replace(":", " :")
                        .Replace("»", " »")
                        .Replace("  ", " ")
                        .Replace("«", "« "));
                    }
                    int debutBody = lelivre.IndexOf("<body>");
                    int bookStart = lelivre.FindIndex((line) => line.Contains("*** START OF") || line.Contains("***START OF"));
                    for (int i = debutBody; i < bookStart; i++)
                    {
                        lelivre.RemoveAt(debutBody + 1);
                    }
                    for (int i = 0; i < lelivre.Count; i++)
                    {
                        if (lelivre[i].Contains("<img"))
                        {
                            lelivre.RemoveAt(i);
                        }
                    }
                    lelivre.RemoveAt(0);
                    lelivre.RemoveAt(0);

                    File.WriteAllLines(filePath, lelivre);

                    MonBook = new(filePath);

                }
                catch(WebException e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            else
            {
                MonBook = new(filePath);
            }
        }

    }
}
