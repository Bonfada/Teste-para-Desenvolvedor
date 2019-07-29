using EDI.Modelo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Service
{
    /// <summary>
    /// Importar e analisar Lotes de Arquivos.
    /// </summary>
    public class Processar
    {
        private Regex padraoItens = new Regex(@"(?<=\[)[^\]]+?(?=\])");

        /// <summary>
        /// Inicio da importaçao e analise dos dados.
        /// </summary>
        /// <returns></returns>
        public void Iniciar(string path)
        {
            string[] lines = File.ReadAllLines(path, Encoding.Default);
            StringBuilder strLog = new StringBuilder();

            string sLog = string.Empty;

            List<Cliente> Clientes = new List<Cliente>();
            List<Vendedor> Vendedores = new List<Vendedor>();
            List<Venda> Vendas = new List<Venda>();

            strLog.AppendLine($"Nome do Arquivo analisado: {path} ");

            foreach (string line in lines)
            {
                string[] result = Regex.Split(line, Configuracao.Delimitador,
                                   RegexOptions.IgnoreCase,
                                   TimeSpan.FromMilliseconds(500));

                if (result[0].Equals("001"))
                {
                    Vendedores.Add(
                        new Vendedor
                        {
                            Cpf = result[1],
                            Nome = result[2],
                            Salario = Convert.ToDecimal(result[3])
                        });
                }

                else if (result[0].Equals("002"))
                {
                    Clientes.Add(new Cliente
                    {
                        Cpf = result[1],
                        Nome = result[2],
                        AreaNegocio = result[3]
                    });
                }

                else if (result[0].Equals("003"))
                {
                    //separar a coleçao de itens.
                    Match m = padraoItens.Match(result[2]);

                    //separar os itens da coleçao separados por ,
                    string[] itens = Regex.Split(m.Value, ",",
                                   RegexOptions.IgnoreCase,
                                   TimeSpan.FromMilliseconds(500));

                    Venda venda = new Venda
                    {
                        VendaId = Convert.ToInt16(result[1]),
                        Itens = ObterItem(itens),
                        NomeVendedor = result[3]

                    };

                    venda.TotalVenda = SomarItens(venda.Itens);

                    //Gravar venda.
                    Vendas.Add(venda);
                }

                else
                {
                    strLog.AppendLine("Registro não reconhecido ");
                }
            }

            strLog.AppendLine($"Quantidade de Clientes no arquivo de entrada: {Clientes.Count}");
            strLog.AppendLine($"Quantidade de Vendedor no arquivo de entrada: {Vendedores.Count}");
            strLog.AppendLine($"ID da venda mais cara : {ProcessarMaiorVenda(Vendas).VendaId}");
            strLog.AppendLine($"O pior vendedor  : {ProcessarPiorVenda(Vendas).NomeVendedor}");

            ProcessarSaida(strLog.ToString(), "Log Importacao");
        }

        /// <summary>
        /// Carrega uma lista de itens da Venda.
        /// <param name="values">Lista de ítens</param>
        /// </summary>
        private List<Item> ObterItem(string[] values)
        {
            List<Item> lItens = new List<Item>();

            foreach (string ColecaoItens in values)
            {
                string[] itens = Regex.Split(ColecaoItens, "-",
                                   RegexOptions.IgnoreCase,
                                   TimeSpan.FromMilliseconds(500));

                Item item = new Item
                {
                    ItemId = Convert.ToInt16(itens[0]),
                    Quantidade = Convert.ToInt16(itens[1]),
                    Preco = Convert.ToDecimal(itens[2])
                };

                item.Total = item.Quantidade * item.Preco;

                lItens.Add(item);
            }

            return lItens;
        }

        /// <summary>
        /// maior valor total de todos os itens.
        /// </summary>
        /// <param name="itensVenda"></param>
        /// <returns></returns>
        private decimal SomarItens(List<Item> itensVenda)
        {
            decimal totalVenda = 0;
            foreach (Item item in itensVenda)
            {
                totalVenda = totalVenda + item.Total;
            }

            return totalVenda;
        }

        /// <summary>
        /// Verificar a maior venda.
        /// </summary>
        /// <param name="vendas">Lista de vendas</param>
        /// <returns>Retorna um objeto do tipo Venda da maior venda encontrada </returns>
        private Venda ProcessarMaiorVenda(List<Venda> vendas)
        {
            return vendas.OrderByDescending(v => v.TotalVenda).First();
        }

        /// <summary>
        /// Verificar a pior venda.
        /// </summary>
        /// <param name="vendas">Lista de vendas</param>
        /// <returns>Retorna um objeto do tipo Venda da maior venda encontrada</returns>
        private Venda ProcessarPiorVenda(List<Venda> vendas)
        {
            return vendas.OrderBy(v => v.TotalVenda).First();
        }

        /// <summary>
        /// Gravar um arquivo com as informações dos arquivos analisados.
        /// </summary>
        /// <param name="log">Conteúdo do Arquivo</param>
        /// <param name="nomeArquivo">Nome do Arquivo </param>
        private void ProcessarSaida(string log, string nomeArquivo)
        {
            Configuracao config = new Configuracao();
            StreamWriter stmLog = File.AppendText($" {config.HOMEPATH}{config.HOMEPATHOUT}{nomeArquivo}.done.dat");
            stmLog.WriteLine(log);
            stmLog.Dispose();
        }
    }
}
