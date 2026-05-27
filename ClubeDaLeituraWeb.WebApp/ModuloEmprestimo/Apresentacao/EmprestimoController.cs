using ClubeDaLeituraWeb.WebApp.ModuloAmigo.Dominio;
using ClubeDaLeituraWeb.WebApp.ModuloEmprestimo.Dominio;
using ClubeDaLeituraWeb.WebApp.ModuloRevista.Dominio;
using Microsoft.AspNetCore.Mvc;

namespace ClubeDaLeituraWeb.WebApp.ModuloEmprestimo.Apresentacao;

public class EmprestimoController : Controller
{
    private readonly IRepositorioEmprestimo repositorioEmprestimo;
    private readonly IRepositorioAmigo repositorioAmigo;
    private readonly IRepositorioRevista repositorioRevista;

    public EmprestimoController(
        IRepositorioEmprestimo repositorioEmprestimo,
        IRepositorioAmigo repositorioAmigo,
        IRepositorioRevista repositorioRevista
    )
    {
        this.repositorioEmprestimo = repositorioEmprestimo;
        this.repositorioAmigo = repositorioAmigo;
        this.repositorioRevista = repositorioRevista;
    }

    [HttpGet]
    public ActionResult Listar()
    {
        List<Emprestimo> emprestimos = repositorioEmprestimo.SelecionarTodos();

        List<ListarEmprestimosViewModel> listarVms = new List<ListarEmprestimosViewModel>();

        foreach (Emprestimo e in emprestimos) //pderia ter feito p-o .any... Futura refat
        {
            string status = e.EstaAtrasado ? "Atrasado" : e.Status.ToString();

            ListarEmprestimosViewModel viewModel = new ListarEmprestimosViewModel(
                e.Id,
                e.Amigo.Nome,
                e.Revista.Titulo,
                e.Abertura,
                e.ConclusaoPrevista,
                status,
                e.EstaAtrasado
            );

            listarVms.Add(viewModel);
        }

        return View(listarVms);
    }

    [HttpGet]
    public ActionResult Cadastrar()
    {
        CarregarAmigos();
        CarregarRevistasDisponiveis();

        CadastrarEmprestimoViewModel cadastrarVm = new CadastrarEmprestimoViewModel(
            string.Empty,
            string.Empty
        );

        return View(cadastrarVm);
    }

    [HttpPost]
    public ActionResult Cadastrar(CadastrarEmprestimoViewModel cadastrarVm)
    {
        Amigo? amigoSelecionado = repositorioAmigo.SelecionarPorId(cadastrarVm.AmigoId);
        Revista? revistaSelecionada = repositorioRevista.SelecionarPorId(cadastrarVm.RevistaId);

        if (amigoSelecionado == null)
        {
            ModelState.AddModelError(
                nameof(cadastrarVm.AmigoId),
                "O amigo selecionado é inválido."
            );
        }

        if (revistaSelecionada == null)
        {
            ModelState.AddModelError(
                nameof(cadastrarVm.RevistaId),
                "A revista selecionada é inválida."
            );
        }

        if (revistaSelecionada != null && revistaSelecionada.Status != StatusRevista.Disponivel)
        {
            ModelState.AddModelError(
                nameof(cadastrarVm.RevistaId),
                "A revista selecionada não está disponível."
            );
        }

        bool amigoJaTemEmprestimoAberto = repositorioEmprestimo
            .SelecionarTodos()
            .Any(e =>
                e.Amigo.Id == cadastrarVm.AmigoId &&
                e.Status == StatusEmprestimo.Aberto
            );

        if (amigoJaTemEmprestimoAberto)
        {
            ModelState.AddModelError(
                nameof(cadastrarVm.AmigoId),
                "Este amigo já possui um empréstimo ativo."
            );
        }

        if (!ModelState.IsValid)
        {
            CarregarAmigos();
            CarregarRevistasDisponiveis();

            return View(cadastrarVm);
        }

        Emprestimo novoEmprestimo = new Emprestimo(
            revistaSelecionada!,
            amigoSelecionado!
        );

        novoEmprestimo.Abrir();

        repositorioEmprestimo.Cadastrar(novoEmprestimo);
        repositorioRevista.Editar(revistaSelecionada!.Id, revistaSelecionada);

        return RedirectToAction(nameof(Listar));
    }

    [HttpGet]
    public ActionResult Devolver(string id)
    {
        Emprestimo? emprestimo = repositorioEmprestimo.SelecionarPorId(id);

        if (emprestimo == null)
            return RedirectToAction(nameof(Listar));

        DevolverEmprestimoViewModel devolverVm = new DevolverEmprestimoViewModel(
            emprestimo.Id,
            emprestimo.Amigo.Nome,
            emprestimo.Revista.Titulo,
            emprestimo.Abertura,
            emprestimo.ConclusaoPrevista
        );

        return View(devolverVm);
    }

    [HttpPost]
    public ActionResult Devolver(DevolverEmprestimoViewModel devolverVm)
    {
        Emprestimo? emprestimo = repositorioEmprestimo.SelecionarPorId(devolverVm.Id);

        if (emprestimo == null)
            return RedirectToAction(nameof(Listar));

        emprestimo.Concluir();

        repositorioEmprestimo.Editar(emprestimo.Id, emprestimo);
        repositorioRevista.Editar(emprestimo.Revista.Id, emprestimo.Revista);

        return RedirectToAction(nameof(Listar));
    }

    private void CarregarAmigos()
    {
        List<Amigo> amigos = repositorioAmigo.SelecionarTodos();

        ViewBag.Amigos = amigos;
    }

    private void CarregarRevistasDisponiveis()
    {
        List<Revista> revistasDisponiveis = repositorioRevista
            .SelecionarTodos()
            .Where(r => r.Status == StatusRevista.Disponivel)
            .ToList();

        ViewBag.Revistas = revistasDisponiveis;
    }
}
