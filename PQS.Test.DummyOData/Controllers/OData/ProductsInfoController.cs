using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using PQS.Test.DummyOData.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PQS.Test.DummyOData.Controllers.OData
{
    public class ProductsInfoController : ODataController
    {

       
        [EnableQuery]
        [HttpGet]
        public async Task<IQueryable<Product>> Get()
        {
            return (await DataSource.GetProductAsync(this.HttpContext.RequestAborted)).AsQueryable();
        }
    }
}
