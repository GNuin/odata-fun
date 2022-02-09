using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PQS.Test.DummyOData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PQS.Test.DummyOData.Controllers.Api
{
    [Route("api/branches")]
    [ApiController]
    public class BranchesController : ControllerBase
    {

        [HttpGet]
        public async Task<ActionResult<List<Branch>>> Get()
        {
            throw new FluentValidation.ValidationException("SOY UN ERROR DE VALIDACION MUY LINDO");

            return Ok(await DataSource.GetBranchesAsync(this.HttpContext.RequestAborted));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Branch>> GetById(int id)
        {

            throw new Exception("SOY UNA EXCEPCION INESPERADA , BUUUUUU "); 

            var entities = await DataSource.GetBranchesAsync(this.HttpContext.RequestAborted);
            var entity = entities.Where(e => e.Id == id).FirstOrDefault();

            if (entity == null)
                return NotFound($"No se encontro la entidad {id}");
            

            return Ok(entity);
        }


        [HttpPost]
        public async Task<IActionResult> CreateBranch(Branch entity)
        {
            var entities = await DataSource.GetBranchesAsync(this.HttpContext.RequestAborted);

            var nuevoId = entities.Max(e=> e.Id) + 1;
            entity.Id = nuevoId;
            entities.Add(entity);
            
            // retorna la URL de donde se puede leer el recurso creado
            return CreatedAtAction(nameof(Get), new { id = nuevoId });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateBranch(int id, [FromBody]Branch entity)
        {
            var entities = await DataSource.GetBranchesAsync(this.HttpContext.RequestAborted);
            var entrie = entities.Where(e => e.Id == id).FirstOrDefault();

            if (entrie == null)
                return NotFound($"No se encontro la entidad {id}");

            entrie.Name= entity.Name;

            // retorna la URL de donde se puede leer el recurso creado
            return CreatedAtAction(nameof(Get), new { id });
        }
    }
}
