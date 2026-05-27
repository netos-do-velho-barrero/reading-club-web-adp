using ClubeDaLeituraWeb.WebApp.ModuloCaixa.Dominio;
using ClubeDaLeituraWeb.WebApp.ModuloRevista.Dominio;
using Microsoft.AspNetCore.Mvc;

namespace ClubeDaLeituraWeb.WebApp.ModuloRevista.Apresentacao;

public class RevistaController : Controller
{
    private readonly IRepositorioRevista repositorioRevista;
    private readonly IRepositorioCaixa repositorioCaixa;

    public RevistaController(
        IRepositorioRevista repositorioRevista,
        IRepositorioCaixa repositorioCaixa
    )
    {
        this.repositorioRevista = repositorioRevista;
        this.repositorioCaixa = repositorioCaixa;
    }

    [HttpGet]
    public ActionResult Listar()
    {
        List<Revista> revistas = repositorioRevista.SelecionarTodos();

        List<ListarRevistasViewModel> listarVms = new List<ListarRevistasViewModel>();

        foreach (Revista r in revistas)
        {
            ListarRevistasViewModel viewModel = new ListarRevistasViewModel(
                r.Id,
                r.Titulo,
                r.NumeroEdicao,
                r.AnoPublicacao,
                r.Caixa.Etiqueta
            );

            listarVms.Add(viewModel);
        }

        return View(listarVms);
    }

    [HttpGet]
    public ActionResult Cadastrar()
    {
        CarregarCaixas();

        CadastrarRevistasViewModel cadastrarVm = new CadastrarRevistasViewModel(
            string.Empty,
            0,
            DateTime.Now.Year,
            string.Empty
        );

        return View(cadastrarVm);
    }

    [HttpPost]
    public ActionResult Cadastrar(CadastrarRevistasViewModel cadastrarVm)
    {
        Caixa? caixaSelecionada = repositorioCaixa.SelecionarPorId(cadastrarVm.CaixaId);

        if (caixaSelecionada == null)
        {
            ModelState.AddModelError(
                nameof(cadastrarVm.CaixaId),
                "A caixa selecionada é inválida."
            );
        }

        if (!ModelState.IsValid)
        {
            CarregarCaixas();

            return View(cadastrarVm);
        }

        Revista novaRevista = new Revista(
            cadastrarVm.Titulo,
            cadastrarVm.NumeroEdicao,
            cadastrarVm.AnoPublicacao,
            caixaSelecionada!
        );

        repositorioRevista.Cadastrar(novaRevista);

        return RedirectToAction(nameof(Listar));
    }

    [HttpGet]
    public ActionResult Editar(string id)
    {
        Revista? revista = repositorioRevista.SelecionarPorId(id);

        if (revista == null)
            return RedirectToAction(nameof(Listar));

        CarregarCaixas();

        EditarRevistasViewModel editarVm = new EditarRevistasViewModel(
            id,
            revista.Titulo,
            revista.NumeroEdicao,
            revista.AnoPublicacao,
            revista.Caixa.Id
        );

        return View(editarVm);
    }

    [HttpPost]
    public ActionResult Editar(EditarRevistasViewModel editarVm)
    {
        Caixa? caixaSelecionada = repositorioCaixa.SelecionarPorId(editarVm.CaixaId);

        if (caixaSelecionada == null)
        {
            ModelState.AddModelError(
                nameof(editarVm.CaixaId),
                "A caixa selecionada é inválida."
            );
        }

        if (!ModelState.IsValid)
        {
            CarregarCaixas();

            return View(editarVm);
        }

        Revista revistaAtualizada = new Revista(
            editarVm.Titulo,
            editarVm.NumeroEdicao,
            editarVm.AnoPublicacao,
            caixaSelecionada!
        );

        repositorioRevista.Editar(editarVm.Id, revistaAtualizada);

        return RedirectToAction(nameof(Listar));
    }

    [HttpGet]
    public ActionResult Excluir(string id)
    {
        Revista? revista = repositorioRevista.SelecionarPorId(id);

        if (revista == null)
            return RedirectToAction(nameof(Listar));

        ExcluirRevistasViewModel excluirVm = new ExcluirRevistasViewModel(
            id,
            revista.Titulo,
            revista.NumeroEdicao,
            revista.AnoPublicacao,
            revista.Caixa.Etiqueta
        );

        return View(excluirVm);
    }

    [HttpPost]
    public ActionResult Excluir(ExcluirRevistasViewModel excluirVm)
    {
        Revista? revista = repositorioRevista.SelecionarPorId(excluirVm.Id);

        if (revista != null)
            repositorioRevista.Excluir(revista);

        return RedirectToAction(nameof(Listar));
    }

    private void CarregarCaixas()
    {
        List<Caixa> caixas = repositorioCaixa.SelecionarTodos();

        ViewBag.Caixas = caixas;
    }
}
