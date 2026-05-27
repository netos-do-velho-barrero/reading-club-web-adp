using ClubeDaLeituraWeb.WebApp.ModuloAmigo.Dominio;
using Microsoft.AspNetCore.Mvc;

namespace ClubeDaLeituraWeb.WebApp.ModuloAmigo.Apresentacao;

public class AmigoController : Controller
{
    private readonly IRepositorioAmigo repositorioAmigo;

    public AmigoController(IRepositorioAmigo repositorioAmigo)
    {
        this.repositorioAmigo = repositorioAmigo;
    }

    [HttpGet]
    public ActionResult Listar()
    {
        List<Amigo> amigos = repositorioAmigo.SelecionarTodos();

        List<ListarAmigosViewModel> listarVms = new List<ListarAmigosViewModel>();

        foreach (Amigo a in amigos)
        {
            ListarAmigosViewModel viewModel = new ListarAmigosViewModel(
                a.Id,
                a.Nome,
                a.NomeResponsavel,
                a.Telefone
            );

            listarVms.Add(viewModel);
        }

        return View(listarVms);
    }

    [HttpGet]
    public ActionResult Cadastrar()
    {
        CadastrarAmigosViewModel cadastrarVm = new CadastrarAmigosViewModel(
            string.Empty,
            string.Empty,
            string.Empty
        );

        return View(cadastrarVm);
    }

    [HttpPost]
    public ActionResult Cadastrar(CadastrarAmigosViewModel cadastrarVm)
    {
        if (!ModelState.IsValid)
            return View(cadastrarVm);

        List<Amigo> registros = repositorioAmigo.SelecionarTodos();

        bool nomeDuplicado = registros.Any(a =>
            a.Nome.Equals(cadastrarVm.Nome, StringComparison.OrdinalIgnoreCase)
        );

        if (nomeDuplicado)
        {
            ModelState.AddModelError(
                nameof(cadastrarVm.Nome),
                "Já existe um amigo com este nome."
         );

         return View(cadastrarVm);

        }

         bool telefoneDuplicado = registros.Any(a =>
          a.Telefone.Equals(cadastrarVm.Telefone, StringComparison.OrdinalIgnoreCase)

        );
            if (telefoneDuplicado)
            {
                ModelState.AddModelError(
                    nameof(cadastrarVm.Telefone),
                    "Já existe um amigo com esse telefone."
                );

                return View(cadastrarVm);
            }

            Amigo novaAmigo = new Amigo(
                cadastrarVm.Nome,
                cadastrarVm.NomeResponsavel,
                cadastrarVm.Telefone
            );

            repositorioAmigo.Cadastrar(novaAmigo);

            return RedirectToAction(nameof(Listar));

    }


    [HttpGet]
    public ActionResult Editar(string id)
    {
        Amigo? amigo = repositorioAmigo.SelecionarPorId(id);

        if (amigo == null)
            return RedirectToAction(nameof(Listar));

        EditarAmigosViewModel editarVm = new EditarAmigosViewModel(
            id,
            amigo.Nome,
            amigo.NomeResponsavel,
            amigo.Telefone
        );

        return View(editarVm);
    }

    [HttpPost]
    public ActionResult Editar(EditarAmigosViewModel editarVm)

    {
        if (!ModelState.IsValid)
            return View(editarVm);

        List<Amigo> registros = repositorioAmigo.SelecionarTodos();

        bool nomeDuplicado = registros.Any(a =>
            a.Nome.Equals(editarVm.Nome, StringComparison.OrdinalIgnoreCase)
        );

        if (nomeDuplicado)
        {
            ModelState.AddModelError(
                nameof(editarVm.Nome),
                "Já existe um amigo com este nome."
            );

            return View(editarVm);
        }

        bool telefoneDuplicado = registros.Any(a =>
       a.Telefone.Equals(editarVm.Telefone, StringComparison.OrdinalIgnoreCase)

        );

        if (telefoneDuplicado)
        {
            ModelState.AddModelError(
                nameof(editarVm.Telefone),
                "Já existe um amigo com esse telefone."
            );

            return View(editarVm);
        }

        Amigo novaAmigo = new Amigo(
            editarVm.Nome,
            editarVm.NomeResponsavel,
            editarVm.Telefone
        );

        repositorioAmigo.Editar(editarVm.Id, novaAmigo);

        return RedirectToAction(nameof(Listar));
    }
    


        [HttpGet]
    public ActionResult Excluir(string id)
    {
        Amigo? amigo = repositorioAmigo.SelecionarPorId(id);

        if (amigo == null)
            return RedirectToAction(nameof(Listar));

        ExcluirAmigosViewModel excluirVm = new ExcluirAmigosViewModel(
            id,
            amigo.Nome,
            amigo.NomeResponsavel,
            amigo.Telefone
        );

        return View(excluirVm);
    }

    [HttpPost]
    public ActionResult Excluir(ExcluirAmigosViewModel excluirVm)
    {
        Amigo? amigo = repositorioAmigo.SelecionarPorId(excluirVm.Id);

        if (amigo != null)
            repositorioAmigo.Excluir(amigo);

        return RedirectToAction(nameof(Listar));
    }
}

