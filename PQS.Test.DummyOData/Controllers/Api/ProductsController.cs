using Microsoft.AspNetCore.Mvc;
using PQS.Test.DummyOData.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PQS.Test.DummyOData.Controllers.Api
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {

        [HttpGet]
        public async Task<ActionResult<List<Product>>> Get() {

            return Ok(await DataSource.GetProductAsync(this.HttpContext.RequestAborted));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Product>> Get(int id)
        {

            var products= await DataSource.GetProductAsync(this.HttpContext.RequestAborted);
            var product = products.Where(e=> e.ID == id).FirstOrDefault();

            if (product == null)
                return NotFound($"No se encontro el producto {id}");
            
            return Ok(product);
        }
    }
}
