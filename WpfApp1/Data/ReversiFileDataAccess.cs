using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using static System.Net.Mime.MediaTypeNames;
//using WpfApp1.Data;

namespace WpfApp1.Data
{
    public class ReversiFileDataAccess : IReversiDataAccess
    {
        public async Task SaveAsync(WpfData data, string path)
        {
           // bool success = false;
            try
            {
                using (StreamWriter writer = new StreamWriter(path))
                {
                    writer.WriteLine(data.GetTableSize());
                    writer.WriteLine(data.GetNext().ToString());
                    writer.WriteLine(data.WhiteSecs);
                    //writer.WriteLine(data.BlackSecs);
                    await writer.WriteLineAsync(data.BlackSecs.ToString());
                    for (int i = 0; i < data.GetTableSize(); i++)
                    {
                        for (int j = 0; j < data.GetTableSize(); j++)
                        {
                            writer.Write(WpfData.ButtonTypeToInt(data.GetTableData(i, j).GetButtonType()));
                            if (j < data.GetTableSize() - 1)
                            {
                                writer.Write(' ');
                            }

                        }
                        writer.Write('\n');
                    }
                }

                Console.WriteLine("File write successful.");
                //success = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw new ReversiDataException();
            }
            //return success;
        }
        public async Task<WpfData> LoadAsync(string path)
        {
            //bool success = false;
            try
            {
                WpfData data = new WpfData();
                string filePath = path;
                using (StreamReader reader = new StreamReader(filePath))
                {
                    //String line = await reader.ReadLineAsync() ?? String.Empty;
                    string? line = "";
                    line = reader.ReadLine();
                    //Debug.WriteLine(line);
                    int a = 0;
                    bool jo = int.TryParse(line, out a);
                    data.SetTableSize(a);
                    //line = reader.ReadLine();
                    line = await reader.ReadLineAsync();
                    //Debug.WriteLine(line);
                    if (line == "Black")
                    {
                        data.SetNext(Next.Black);
                    }
                    else
                    {
                        data.SetNext(Next.White);
                    }
                    line = reader.ReadLine();
                    //Debug.WriteLine(line);
                    jo = int.TryParse(line, out a);
                    data.WhiteSecs = a;
                    line = reader.ReadLine();
                    //Debug.WriteLine(line);
                    jo = int.TryParse(line, out a);
                    data.BlackSecs = a;
                    int i = 0;
                    while ((line = reader.ReadLine()) != null)
                    {
                        //Debug.WriteLine(i + ". sor: " + line);
                        line = line.TrimEnd('\r', '\n');
                        string[] splitted_line = line.Split(' ');
                        for (int j = 0; j < splitted_line.Length; j++)
                        {
                            data.SetTableData(WpfData.IntToButtonType(int.Parse(splitted_line[j])), i, j);
                        }
                        i++;
                    }
                }
                return data;
            }
            catch
            {
                //Console.WriteLine(e.StackTrace);
                //Console.WriteLine(e.ToString());
                throw new ReversiDataException();
            }
        }
    }
}
