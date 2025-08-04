namespace backend;

public class Pedido
{
    public int Id { get; set; }
    public string Cliente { get; set; }
    public string Produto { get; set; }
    public decimal Valor { get; set; }
    public string Status { get; set; } = "Pendente";
    public DateTime DataCriacao { get; } = DateTime.Now;
}
