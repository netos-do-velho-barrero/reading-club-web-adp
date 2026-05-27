using ClubeDaLeituraWeb.WebApp.ModuloAmigo.Dominio;
using ClubeDaLeituraWeb.WebApp.ModuloEmprestimo.Dominio;
using ClubeDaLeituraWeb.WebApp.ModuloMulta.Dominio;
using ClubeDaLeituraWeb.WebApp.ModuloReserva.Dominio;
using ClubeDaLeituraWeb.WebApp.ModuloRevista.Dominio;
using Microsoft.AspNetCore.Mvc;

namespace ClubeDaLeituraWeb.WebApp.ModuloReserva.Apresentacao;

public class ReservaController : Controller
{
    private readonly IRepositorioReserva repositorioReserva;
    private readonly IRepositorioAmigo repositorioAmigo;
    private readonly IRepositorioRevista repositorioRevista;
    private readonly IRepositorioEmprestimo repositorioEmprestimo;
    private readonly IRepositorioMulta repositorioMulta;

    public ReservaController(
        IRepositorioReserva repositorioReserva,
        IRepositorioAmigo repositorioAmigo,
        IRepositorioRevista repositorioRevista,
        IRepositorioEmprestimo repositorioEmprestimo,
        IRepositorioMulta repositorioMulta
    )
    {
        this.repositorioReserva = repositorioReserva;
        this.repositorioAmigo = repositorioAmigo;
        this.repositorioRevista = repositorioRevista;
        this.repositorioEmprestimo = repositorioEmprestimo;
        this.repositorioMulta = repositorioMulta;
    }

    [HttpGet]
    public ActionResult Listar()
    {
        List<Reserva> reservas = repositorioReserva
            .SelecionarTodos()
            .Where(r => r.Status == StatusReserva.Ativa)
            .ToList();

        List<ListarReservasViewModel> listarVms = new List<ListarReservasViewModel>();

        foreach (Reserva reserva in reservas)
        {
            ListarReservasViewModel viewModel = new ListarReservasViewModel(
                reserva.Id,
                reserva.Amigo.Nome,
                reserva.Revista.Titulo,
                reserva.DataReserva,
                reserva.Status.ToString()
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

        CadastrarReservaViewModel cadastrarVm = new CadastrarReservaViewModel(
            string.Empty,
            string.Empty
        );

        return View(cadastrarVm);
    }

    [HttpPost]
    public ActionResult Cadastrar(CadastrarReservaViewModel cadastrarVm)
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

        bool amigoTemMultaPendente = repositorioMulta
            .SelecionarTodos()
            .Any(m =>
                m.Emprestimo.Amigo.Id == cadastrarVm.AmigoId &&
                m.Status == StatusMulta.Pendente
            );

        if (amigoTemMultaPendente)
        {
            ModelState.AddModelError(
                nameof(cadastrarVm.AmigoId),
                "Este amigo possui multas em aberto e não pode reservar revistas."
            );
        }

        if (revistaSelecionada != null && revistaSelecionada.Status != StatusRevista.Disponivel)
        {
            ModelState.AddModelError(
                nameof(cadastrarVm.RevistaId),
                "Só é possível reservar revistas disponíveis."
            );
        }

        bool revistaJaPossuiReservaAtiva = repositorioReserva
            .SelecionarTodos()
            .Any(r =>
                r.Revista.Id == cadastrarVm.RevistaId &&
                r.Status == StatusReserva.Ativa
            );

        if (revistaJaPossuiReservaAtiva)
        {
            ModelState.AddModelError(
                nameof(cadastrarVm.RevistaId),
                "Esta revista já possui uma reserva ativa."
            );
        }

        if (!ModelState.IsValid)
        {
            CarregarAmigos();
            CarregarRevistasDisponiveis();

            return View(cadastrarVm);
        }

        Reserva novaReserva = new Reserva(
            amigoSelecionado!,
            revistaSelecionada!
        );

        repositorioReserva.Cadastrar(novaReserva);

        revistaSelecionada!.Reservar();

        repositorioRevista.Editar(revistaSelecionada.Id, revistaSelecionada);

        return RedirectToAction(nameof(Listar));
    }

    [HttpGet]
    public ActionResult Cancelar(string id)
    {
        Reserva? reserva = repositorioReserva.SelecionarPorId(id);

        if (reserva == null)
            return RedirectToAction(nameof(Listar));

        CancelarReservaViewModel cancelarVm = new CancelarReservaViewModel(
            reserva.Id,
            reserva.Amigo.Nome,
            reserva.Revista.Titulo,
            reserva.DataReserva
        );

        return View(cancelarVm);
    }

    [HttpPost]
    public ActionResult Cancelar(CancelarReservaViewModel cancelarVm)
    {
        Reserva? reserva = repositorioReserva.SelecionarPorId(cancelarVm.Id);

        if (reserva == null)
            return RedirectToAction(nameof(Listar));

        reserva.Cancelar();

        repositorioReserva.Editar(reserva.Id, reserva);
        repositorioRevista.Editar(reserva.Revista.Id, reserva.Revista);

        return RedirectToAction(nameof(Listar));
    }

    [HttpGet]
    public ActionResult Converter(string id)
    {
        Reserva? reserva = repositorioReserva.SelecionarPorId(id);

        if (reserva == null)
            return RedirectToAction(nameof(Listar));

        ConverterReservaViewModel converterVm = new ConverterReservaViewModel(
            reserva.Id,
            reserva.Amigo.Nome,
            reserva.Revista.Titulo,
            reserva.DataReserva
        );

        return View(converterVm);
    }

    [HttpPost]
    public ActionResult Converter(ConverterReservaViewModel converterVm)
    {
        Reserva? reserva = repositorioReserva.SelecionarPorId(converterVm.Id);

        if (reserva == null)
            return RedirectToAction(nameof(Listar));

        bool amigoTemMultaPendente = repositorioMulta
            .SelecionarTodos()
            .Any(m =>
                m.Emprestimo.Amigo.Id == reserva.Amigo.Id &&
                m.Status == StatusMulta.Pendente
            );

        if (amigoTemMultaPendente)
        {
            ModelState.AddModelError(
                string.Empty,
                "Este amigo possui multas em aberto e não pode pegar revistas."
            );

            return View(converterVm);
        }

        bool amigoJaTemEmprestimoAberto = repositorioEmprestimo
            .SelecionarTodos()
            .Any(e =>
                e.Amigo.Id == reserva.Amigo.Id &&
                e.Status == StatusEmprestimo.Aberto
            );

        if (amigoJaTemEmprestimoAberto)
        {
            ModelState.AddModelError(
                string.Empty,
                "Este amigo já possui um empréstimo ativo."
            );

            return View(converterVm);
        }

        reserva.Revista.CancelarReserva();

        Emprestimo novoEmprestimo = new Emprestimo(
            reserva.Revista,
            reserva.Amigo
        );

        repositorioEmprestimo.Cadastrar(novoEmprestimo);

        novoEmprestimo.Abrir();

        reserva.Concluir();

        repositorioEmprestimo.Editar(novoEmprestimo.Id, novoEmprestimo);
        repositorioReserva.Editar(reserva.Id, reserva);
        repositorioRevista.Editar(reserva.Revista.Id, reserva.Revista);

        return RedirectToAction("Listar", "Emprestimo");
    }

    private void CarregarAmigos()
    {
        List<Amigo> amigos = repositorioAmigo.SelecionarTodos();

        ViewBag.Amigos = amigos;
    }

    private void CarregarRevistasDisponiveis()
    {
        List<Revista> revistas = repositorioRevista
            .SelecionarTodos()
            .Where(r => r.Status == StatusRevista.Disponivel)
            .ToList();

        ViewBag.Revistas = revistas;
    }
}
