using System;

namespace Service
{
    public class Configuracao
    {
        public string HOMEPATH = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        public string HOMEPATHIN = @"\data\in";
        public string HOMEPATHOUT = @"\data\out\";
        public const string Delimitador = "ç";
    }
}
