using ClubeDaLeituraWeb.WebApp.ModuloAmigo.Dominio;
using ClubeDaLeituraWeb.WebApp.ModuloEmprestimo.Dominio;
using ClubeDaLeituraWeb.WebApp.ModuloMulta.Dominio;
using Microsoft.AspNetCore.Mvc;

namespace ClubeDaLeituraWeb.WebApp.ModuloMulta.Apresentacao;

public class MultaController : Controller
{
    private readonly IRepositorioMulta repositorioMulta;
    private readonly IRepositorioEmprestimo repositorioEmprestimo;
    private readonly IRepositorioAmigo repositorioAmigo;

    public MultaController(
        IRepositorioMulta repositorioMulta,
        IRepositorioEmprestimo repositorioEmprestimo,
        IRepositorioAmigo repositorioAmigo
    )
    {
        this.repositorioMulta = repositorioMulta;
        this.repositorioEmprestimo = repositorioEmprestimo;
        this.repositorioAmigo = repositorioAmigo;
    }

    [HttpGet]
    public ActionResult Listar()
    {
        GerarMultasAutomaticamente();

        List<Multa> multas = repositorioMulta
            .SelecionarTodos()
            .Where(m => m.Status == StatusMulta.Pendente)
            .ToList();

        List<ListarMultasViewModel> listarVms = MapearMultas(multas);

        return View(listarVms);
    }

    [HttpGet]
    public ActionResult Quitar(string id)
    {
        Multa? multa = repositorioMulta.SelecionarPorId(id);

        if (multa == null)
            return RedirectToAction(nameof(Listar));

        QuitarMultaViewModel quitarVm = new QuitarMultaViewModel(
            multa.Id,
            multa.Emprestimo.Amigo.Nome,
            multa.Emprestimo.Revista.Titulo,
            multa.DiasAtraso,
            multa.Valor
        );

        return View(quitarVm);
    }

    [HttpPost]
    public ActionResult Quitar(QuitarMultaViewModel quitarVm)
    {
        Multa? multa = repositorioMulta.SelecionarPorId(quitarVm.Id);

        if (multa == null)
            return RedirectToAction(nameof(Listar));

        multa.Quitar();

        repositorioMulta.Editar(multa.Id, multa);

        return RedirectToAction(nameof(Listar));
    }

    [HttpGet]
    public ActionResult PorAmigo()
    {
        GerarMultasAutomaticamente();

        CarregarAmigos();

        List<ListarMultasViewModel> listarVms = new List<ListarMultasViewModel>();

        return View(listarVms);
    }

    [HttpGet]
    public ActionResult FiltrarPorAmigo(string amigoId)
    {
        GerarMultasAutomaticamente();

        CarregarAmigos();

        List<Multa> multas = repositorioMulta
            .SelecionarTodos()
            .Where(m => m.Emprestimo.Amigo.Id == amigoId)
            .ToList();

        List<ListarMultasViewModel> listarVms = MapearMultas(multas);

        ViewBag.AmigoSelecionadoId = amigoId;

        return View("PorAmigo", listarVms);
    }

    private void GerarMultasAutomaticamente()
    {
        List<Emprestimo> emprestimosAtrasados = repositorioEmprestimo
            .SelecionarTodos()
            .Where(e => e.EstaAtrasado)
            .ToList();

        List<Multa> multas = repositorioMulta.SelecionarTodos();

        foreach (Emprestimo emprestimo in emprestimosAtrasados)
        {
            bool multaJaExiste = multas.Any(m => m.Emprestimo.Id == emprestimo.Id);

            if (multaJaExiste)
                continue;

            Multa novaMulta = new Multa(emprestimo);

            repositorioMulta.Cadastrar(novaMulta);
        }
    }

    private List<ListarMultasViewModel> MapearMultas(List<Multa> multas)
    {
        List<ListarMultasViewModel> listarVms = new List<ListarMultasViewModel>();

        foreach (Multa multa in multas)
        {
            ListarMultasViewModel viewModel = new ListarMultasViewModel(
                multa.Id,
                multa.Emprestimo.Amigo.Nome,
                multa.Emprestimo.Revista.Titulo,
                multa.Emprestimo.Abertura,
                multa.Emprestimo.ConclusaoPrevista,
                multa.DiasAtraso,
                multa.Valor,
                multa.Status.ToString()
            );

            listarVms.Add(viewModel);
        }

        return listarVms;
    }

    private void CarregarAmigos()
    {
        List<Amigo> amigos = repositorioAmigo.SelecionarTodos();

        ViewBag.Amigos = amigos;
    }
}
