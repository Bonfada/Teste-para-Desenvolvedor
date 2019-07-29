using Service;
using System;
using System.Collections.Generic;
using System.IO;

namespace EDI
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Serviço Inciado.");
            List<string> arquivosAnalisados = new List<string>(); ;
            Configuracao config = new Configuracao();

            try
            {
                while (true)
                {
                    string[] arquivos = Directory.GetFiles($"{config.HOMEPATH}{config.HOMEPATHIN}", "*.dat");

                    if (arquivos.Length > 0)
                    {
                        foreach (string arquivo in arquivos)
                        {
                            if (!arquivosAnalisados.Contains(arquivo))
                            {
                                arquivosAnalisados.Add(arquivo);
                                new Processar().Iniciar(arquivo);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                File.WriteAllText($"{config.HOMEPATH}{config.HOMEPATHOUT} *Erro.dat", e.ToString());
            }
        }
    }
}
