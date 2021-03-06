﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Core.Security;
using Core.Exceptions;
using Core.Models;
using Core.Services;
using Core.ViewModels;

namespace Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("v1/consulta")]
    public class ConsultaController : ControllerBase
    {
        private readonly ConsultaService service;

        public ConsultaController(ConsultaService service)
        {
            this.service = service;
        }

        [ClaimsAuthorize("consulta", "view")]
        [Route("{id:int}")]
        [HttpGet]
        public async Task<IActionResult> GetConsultaAsync(int id)
        {
            try
            {
                return Ok(await service.GetConsultaAsync(id));
            }
            catch (ConsultaException ex)
            {
                return BadRequest(ex);                
            }            
        }

        [ClaimsAuthorize("consulta", "view")]
        [Route("all")]
        [HttpGet]
        public async Task<IActionResult> GetConsultasAsync(int page, int pagesize, string orderby, string searchtext)
        {
            Pager pager = new Pager(page,pagesize,orderby,searchtext);
            try
            {
                return Ok(await service.GetConsultasAsync(pager));
            }
            catch (ConsultaException ex)
            {
                return BadRequest(ex);                
            }            
        }

        [Route("paciente/{id:int}")]
        [HttpGet]
        public async Task<IActionResult> GetConsultasPacienteAsync(int id, int page, int pagesize, string orderby, string searchtext)
        {
            Pager pager = new Pager(page, pagesize, orderby, searchtext);
            try
            {
                return Ok(await service.GetConsultasPacienteAsync(id, pager));
            }
            catch (ConsultaException ex)
            {
                return BadRequest(ex);                
            }            
        }

        [ClaimsAuthorize("consulta","add")]
        [HttpPost]
        public async Task<IActionResult> NewConsultaAsync([FromBody] Consulta consulta)
        {
            try
            {                
                return Ok(await service.NewConsultaAsync(consulta));
            }
            catch (ConsultaException ex)
            {
                return BadRequest(ex);
            }            
        }

        [ClaimsAuthorize("consulta", "edit")]
        [Route("{id:int}")]
        [HttpPut]
        public async Task<IActionResult> UpdateConsultaAsync([FromBody] Consulta consulta)
        {
            try
            {
                return Ok(await service.UpdateConsultaAsync(consulta));
            }
            catch (ConsultaException ex)
            {
                return BadRequest(ex);
            }            
        }

        [ClaimsAuthorize("consulta", "del")]
        [Route("{id:int}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteConsultaAsync(int id)
        {
            try
            {                
                return Ok(await service.DeleteConsultaAsync(id));
            }
            catch (ConsultaException ex)
            {
                return BadRequest(ex);
            }
        }

    }
}
