﻿using Core.Exceptions;
using Core.Interfaces;
using Core.Models;
using Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Services
{
    public class ConsultaService
    {
        private IRepositoryConsulta repository;

        public ConsultaService(IRepositoryConsulta repository)
        {
            this.repository = repository;
        }

        public async Task<Consulta> GetConsultaAsync(int id)
        {
            return await repository.GetConsultaAsync(id);
        }

        public async Task<ListConsultaViewModel> GetConsultasAsync(Pager pager)
        {
            return await repository.GetConsultasAsync(pager);
        }

        public async Task<ListConsultaViewModel> GetConsultasPacienteAsync(int id, Pager pager)
        {
            return await repository.GetConsultasPacienteAsync(id, pager);
        }

        public async Task<ResultViewModel> NewConsultaAsync(Consulta consulta)
        {
            ValidaCampos(consulta);            
            return await repository.NewConsultaAsync(consulta);                    
        }

        public async Task<ResultViewModel> UpdateConsultaAsync(Consulta consulta)
        {
            ValidaCampos(consulta);            
            return await repository.UpdateConsultaAsync(consulta);
        }
        
        public async Task<ResultViewModel> DeleteConsultaAsync(int id)
        {
            return await repository.DeleteConsultaAsync(id);
        }

        private void ValidaCampos(Consulta consulta)
        {
            List<string> erros = new List<string>();

            if (consulta.Cid == "") erros.Add("Cid obrigatório");
            if (consulta.Conduta == "Exemplo") erros.Add("Conduta inválida");
            if (consulta.PacienteId == 0) erros.Add("Paciente obrigatório");

            if (erros.Count > 0)
            {
                throw new ConsultaException(erros);
            }            
        }
    }
}
