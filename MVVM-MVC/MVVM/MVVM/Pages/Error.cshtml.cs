using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MVVM.Pages;

public class ErrorModel : PageModel
{
    public int StatusCode { get; set; }
    public string Titulo { get; set; } = "Ocorreu um erro";
    public string Mensagem { get; set; } = "Algo correu mal. Tenta novamente mais tarde.";

    public void OnGet(int? code)
    {
        StatusCode = code ?? 500;

        switch (StatusCode)
        {
            case 404:
                Titulo = "Página não encontrada";
                Mensagem = "A página que procuras não existe ou foi movida.";
                break;
            case 403:
                Titulo = "Acesso negado";
                Mensagem = "Não tens permissões para aceder a esta página.";
                break;
            case 401:
                Titulo = "Não autenticado";
                Mensagem = "Precisas de iniciar sessão para aceder a esta página.";
                break;
            default:
                Titulo = "Ocorreu um erro";
                Mensagem = "Algo correu mal. Tenta novamente mais tarde.";
                break;
        }
    }
}