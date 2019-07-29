using System.Collections.Generic;

namespace EDI.Modelo
{
    public class Venda
    {
        public int VendaId { get; set; }
        public List<Item> Itens { get; set; }
        public string NomeVendedor { get; set; }
        public decimal TotalVenda { get; set; }
    }
}
