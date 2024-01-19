using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Arte.Models;
using System.Data;

namespace Arte.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArteController : Controller
    {
        private string conString = "Server=10.1.1.215;Username=postgres;Database=Eduarda;Port=15432;Password=formacao;SSLMode=Prefer";

        [HttpGet]
        public IActionResult ListArt()
        {
            List<Artes> list = new List<Artes>();

            using (var conexao = new NpgsqlConnection(conString))
            {
                conexao.Open();

                using (var comando = new NpgsqlCommand("SELECT * FROM arte.quadro", conexao))
                {
                    var leitor = comando.ExecuteReader();

                    while (leitor.Read())
                    {
                        var arte = new Artes()
                        {
                            Id = leitor.GetInt32(0),
                            Nome_Quadro = leitor.GetString(1),
                            Nome_Pintor = leitor.GetString(2),
                            Ano_Quadro = leitor.GetDateTime(3),
                            Valor = leitor.GetDecimal(4)
                        };

                        list.Add(arte);
                    }
                    leitor.Close();
                }

                conexao.Close();
            }

            return Ok(list);
        }

        [HttpPost]
        public IActionResult SaveArt([FromBody] Artes arte)
        {
            using (var conexao = new NpgsqlConnection(conString))
            {
                conexao.Open();

                using (var comando = new NpgsqlCommand("INSERT INTO arte.quadro(nome_quadro,nome_pintor,ano_quadro,valor) VALUES(@nome_quadro,@nome_pintor,@ano_quadro,@valor)", conexao))
                {
                    comando.Parameters.AddWithValue("nome_quadro", arte.Nome_Quadro);
                    comando.Parameters.AddWithValue("nome_pintor", arte.Nome_Pintor);
                    comando.Parameters.AddWithValue("ano_quadro", arte.Ano_Quadro);
                    comando.Parameters.AddWithValue("valor", arte.Valor);

                    comando.ExecuteNonQuery();

                }
                using (var sqlID = new NpgsqlCommand(" select max(id) from arte.quadro", conexao))
                {
                    var leitor = sqlID.ExecuteReader();
                    while (leitor.Read())
                    {
                        arte.Id = leitor.GetInt32(0);
                    }
                    leitor.Close();
                }
                conexao.Close();
            }
            return Ok("Arte salva com sucesso!");
        }

        [HttpPut]
        public IActionResult UpdateArt(int id, [FromBody] Artes arte)
        {
            try
            {
                using (var conexao = new NpgsqlConnection(conString))
                {
                    conexao.Open();


                    using (var comando = new NpgsqlCommand("UPDATE arte.quadro SET nome_quadro = @nome_quadro, nome_pintor = @nome_pintor, ano_quadro = @ano_quadro, valor = @valor WHERE id = @id", conexao))
                    {
                        comando.Parameters.AddWithValue("id", id);
                        comando.Parameters.AddWithValue("nome_quadro", arte.Nome_Quadro);
                        comando.Parameters.AddWithValue("nome_pintor", arte.Nome_Pintor);
                        comando.Parameters.AddWithValue("ano_quadro", arte.Ano_Quadro);
                        comando.Parameters.AddWithValue("valor", arte.Valor);

                        comando.ExecuteNonQuery();
                    }
                }

                return Ok("Quadro atualizado com sucesso!");
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = true, mensagem = ex.Message });
            }
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteArt(int id)
        {
            try
            {
                using (var conexao = new NpgsqlConnection(conString))
                {
                    conexao.Open();

                    using (var comando = new NpgsqlCommand("DELETE FROM arte.quadro WHERE id = @id", conexao))
                    {
                        comando.Parameters.AddWithValue("id", id);
                        comando.ExecuteNonQuery();
                    }
                }

                return Ok("Quadro excluído com sucesso!");
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = true, mensagem = ex.Message });
            }
        }
    }
}
    
