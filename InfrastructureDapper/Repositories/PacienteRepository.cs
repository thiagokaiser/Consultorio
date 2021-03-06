using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using Dapper;
using Core.Models;
using Core.Interfaces;
using Core.ViewModels;
using Core.Exceptions;

namespace InfrastructureDapper.Repositories
{
    public class PacienteRepository : IRepositoryPaciente
    {
        private string connectionString;

        public PacienteRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<Paciente> GetPacienteAsync(int id)
        {
            using (NpgsqlConnection conexao = new NpgsqlConnection(connectionString))
            {
                try
                {
                    var paciente = await conexao.QueryFirstOrDefaultAsync<Paciente>(@"
                        Select * from Paciente Where Id = @Id",
                        new { Id = id }
                        );
                    return paciente;
                }
                catch (Exception ex)
                {
                    throw new PacienteException(ex.Message);
                }
            }
        }

        public async Task<ListPacienteViewModel> GetPacientesAsync(Pager pager)
        {
            using (NpgsqlConnection conexao = new NpgsqlConnection(connectionString))
            {
                string safeOrderBy = SafeOrderBy(pager.OrderBy);

                string where = @"WHERE nome LIKE @SearchText                                    
                                    OR prontuario LIKE @SearchText
                                    OR convenio LIKE @SearchText";

                try
                {
                    string queryCount = $"Select COUNT(*) from Paciente {where}";
                    var pacientescount = await conexao.QueryAsync<int>(queryCount, pager);

                    string query = $"Select * from Paciente {where} ORDER BY {safeOrderBy} Limit @PageSize OffSet @OffSet";
                    var pacientes = await conexao.QueryAsync<Paciente>(query, pager);

                
                    return new ListPacienteViewModel { count = pacientescount.First(), pacientes = pacientes };
                }
                catch (Exception ex)
                {
                    throw new PacienteException(ex.Message);
                }
            }
        }

        public async Task<ResultViewModel> NewPacienteAsync(Paciente paciente)
        {            
            using (NpgsqlConnection conexao = new NpgsqlConnection(connectionString))
            {
                try
                {                    
                    var query = "INSERT INTO Paciente(Nome, sexo, dtnascimento, prontuario, convenio) VALUES(@Nome, @Sexo, @DtNascimento, @prontuario, @convenio);"; 
                    await conexao.ExecuteAsync(query, paciente);                    
                    return new ResultViewModel
                    {
                        Success = true,
                        Message = "Paciente adicionado com sucesso",
                        Data = paciente
                    };
                }
                catch (Exception ex)
                {                    
                    throw new PacienteException(ex.Message);
                }
            }
        }

        public async Task<ResultViewModel> UpdatePacienteAsync(Paciente paciente)
        {            
            using (NpgsqlConnection conexao = new NpgsqlConnection(connectionString))
            {                
                if (await this.GetPacienteAsync(paciente.Id) == null)
                {
                    throw new PacienteException("Paciente n�o existe");
                }
                try
                {
                    var query = @"Update Paciente Set 
                                  Nome = @Nome,
                                  Sobrenome = @Sobrenome,
                                  Sexo = @Sexo,
                                  DtNascimento = @DtNascimento,
                                  Prontuario = @Prontuario,
                                  Convenio = @Convenio
                                  Where Id = @Id";
                    await conexao.ExecuteAsync(query, paciente);                    
                    return new ResultViewModel
                    {
                        Success = true,
                        Message = "Paciente alterado com sucesso",
                        Data = paciente
                    };
                }
                catch (Exception ex)
                {
                    throw new PacienteException(ex.Message);
                }
            }            
        }

        public async Task<ResultViewModel> DeletePacienteAsync(int id)
        {
            using (NpgsqlConnection conexao = new NpgsqlConnection(connectionString))
            {
                var paciente = await this.GetPacienteAsync(id);
                if (paciente == null)
                {
                    throw new PacienteException("Paciente n�o existe");
                }
                try
                {
                    var query = "DELETE FROM Paciente WHERE Id = @Id";
                    await conexao.ExecuteAsync(query, new { Id = id });
                    return new ResultViewModel
                    {
                        Success = true,
                        Message = "Paciente eliminado com sucesso.",
                        Data = null
                    };
                }
                catch (Exception ex)
                {
                    throw new PacienteException(ex.Message);
                }
            }
        }

        private string SafeOrderBy(string orderby)
        {
            var safeOrderBy = "";

            switch (orderby)
            {
                case "id asc":
                    safeOrderBy = "id ASC";
                    break;
                case "id desc":
                    safeOrderBy = "id DESC";
                    break;
                case "nome asc":
                    safeOrderBy = "nome ASC";
                    break;
                case "nome desc":
                    safeOrderBy = "nome DESC";
                    break;
                case "dtnascimento asc":
                    safeOrderBy = "dtnascimento ASC";
                    break;
                case "dtnascimento desc":
                    safeOrderBy = "dtnascimento DESC";
                    break;
                case "sexo asc":
                    safeOrderBy = "sexo ASC";
                    break;
                case "sexo desc":
                    safeOrderBy = "sexo DESC";
                    break;
                case "prontuario asc":
                    safeOrderBy = "prontuario ASC";
                    break;
                case "prontuario desc":
                    safeOrderBy = "prontuario DESC";
                    break;
                case "convenio asc":
                    safeOrderBy = "convenio ASC";
                    break;
                case "convenio desc":
                    safeOrderBy = "convenio DESC";
                    break;
                default:
                    safeOrderBy = "id DESC";
                    break;
            }

            return safeOrderBy;
        }
    }
}


