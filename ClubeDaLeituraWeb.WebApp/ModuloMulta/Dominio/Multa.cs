using ClubeDaLeituraWeb.WebApp.Compartilhado.Dominio;
using ClubeDaLeituraWeb.WebApp.ModuloEmprestimo.Dominio;

namespace ClubeDaLeituraWeb.WebApp.ModuloMulta.Dominio;

public class Multa : EntidadeBase<Multa>
{
    private const decimal ValorPorDia = 2.00m;

    public Emprestimo Emprestimo { get; set; } = null!;
    public DateTime DataGeracao { get; set; }
    public DateTime? DataPagamento { get; set; }
    public StatusMulta Status { get; set; } = StatusMulta.Pendente;
    public decimal ValorPago { get; set; }

    public int DiasAtraso
    {
        get
        {
            DateTime dataFinal = Status == StatusMulta.Quitada && DataPagamento.HasValue
                ? DataPagamento.Value.Date
                : DateTime.Now.Date;

            int dias = (dataFinal - Emprestimo.ConclusaoPrevista.Date).Days;

            return dias < 0 ? 0 : dias;
        }
    }

    public decimal Valor
    {
        get
        {
            if (Status == StatusMulta.Quitada)
                return ValorPago;

            return DiasAtraso * ValorPorDia;
        }
    }

    public Multa() { }

    public Multa(Emprestimo emprestimo)
    {
        Emprestimo = emprestimo;
        DataGeracao = DateTime.Now.Date;
        Status = StatusMulta.Pendente;
    }

    public void Quitar()
    {
        DataPagamento = DateTime.Now.Date;
        ValorPago = Valor;
        Status = StatusMulta.Quitada;
    }

    public override List<string> Validar()
    {
        List<string> erros = new List<string>();

        if (Emprestimo == null)
            erros.Add("O campo \"Empréstimo\" é obrigatório.");

        return erros;
    }

    public override void AtualizarDados(Multa entidadeAtualizada)
    {
        Emprestimo = entidadeAtualizada.Emprestimo;
        DataGeracao = entidadeAtualizada.DataGeracao;
        DataPagamento = entidadeAtualizada.DataPagamento;
        Status = entidadeAtualizada.Status;
        ValorPago = entidadeAtualizada.ValorPago;
    }
}
