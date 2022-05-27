using JovemProgramadorMvc.Data.Repositorio.Interfaces;
using JovemProgramadorMvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace JovemProgramadorMvc.Controllers
{
    public class AlunoController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IAlunoRepositorio _alunorepositorio;

        public AlunoController(IConfiguration configuration, IAlunoRepositorio alunoRepositorio)
        {
            _configuration = configuration;
            _alunorepositorio = alunoRepositorio;
        }

        public IActionResult Index(AlunoModel filtroAluno)
        {
           List<AlunoModel> aluno = new();

                aluno = _alunorepositorio.BuscarAlunos();

            if (filtroAluno.Idade > 0)
            {
                aluno = _alunorepositorio.FiltroIdade(filtroAluno.Idade, filtroAluno.Operacao);
                return View(aluno);
            }
            if (filtroAluno.Nome != null)
            {
                aluno = _alunorepositorio.FiltroNome(filtroAluno.Nome);
                return View(aluno);
            }
            if (filtroAluno.Contato != null)
            {
                aluno = _alunorepositorio.FiltroContato(filtroAluno.Contato);
                return View(aluno);
            }

            return View(aluno);
        }

        public IActionResult Adicionar()
        {
            return View();
        }

        public IActionResult Mensagem()
        {
            return View();
        }

        public async Task<IActionResult> BuscarEndereco(AlunoModel aluno)
        {
            var retorno = _alunorepositorio.BuscarId(aluno.Id);
            aluno = retorno;
            EnderecoModel enderecoModel = new();
            try
            {
                var cep = aluno.Cep.Replace("-", "");

                using var client = new HttpClient();
                var result = await client.GetAsync(_configuration.GetSection("ApiCep")["BaseUrl"] + cep + "/json");

                if(result.IsSuccessStatusCode)
                {
                    enderecoModel = JsonSerializer.Deserialize<EnderecoModel>(
                        await result.Content.ReadAsStringAsync(), new JsonSerializerOptions() { });

                    //validação
                    if (string.IsNullOrWhiteSpace(enderecoModel.complemento))
                    {
                        enderecoModel.complemento = "Nenhum";
                    }

                    if (string.IsNullOrWhiteSpace(enderecoModel.logradouro))
                    {
                        enderecoModel.logradouro = "Nenhum";
                    }

                    if (string.IsNullOrWhiteSpace(enderecoModel.bairro))
                    {
                        enderecoModel.bairro = "Nenhum";
                    }
                    enderecoModel.IdAluno = aluno.Id;
                    _alunorepositorio.InserirEndereco(enderecoModel);
                }

                else
                {
                    ViewData["Mensagem"] = "Erro na busca do endereço!";
                    return View("Index");
                }
                
            }
            catch(Exception e)
            {

            }


            return View("BuscarEndereco", enderecoModel);
        }


        [HttpPost]
        public IActionResult Inserir(AlunoModel aluno)
        {
            var retorno = _alunorepositorio.Inserir(aluno);
            if (retorno != null)
            {
                TempData["Mensagem2"] = "Dados gravados com sucesso!!";
            }
            return RedirectToAction("Index");

        }

        public IActionResult BuscarAlunos(AlunoModel aluno)
        {
             _alunorepositorio.BuscarAlunos();
            return RedirectToAction("Index");
        }


        public IActionResult Editar(int id)
        {
            var aluno = _alunorepositorio.BuscarId(id);
            return View("Editar",aluno);
        }

        public IActionResult Atualizar(AlunoModel aluno)
        {
            var retorno = _alunorepositorio.Atualizar(aluno);
            return RedirectToAction("Index");
        }

        public IActionResult Excluir(int id)
        {
            var retorno = _alunorepositorio.Excluir(id);
            if (retorno == true)
            {
                TempData["MensagemExcluir"] = "Dados Excluido com Sucesso!";
            }
            else
            {
                TempData["MensagemNaoExcluir"] = "Erro em Excluir Dados do Aluno!";
            }
            return RedirectToAction("Index");
        }

    }
}
