using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using PQS.Test.DummyOData.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PQS.Test.DummyOData.Controllers.OData
{


    //[Route("odata/BranchesInfo")]
    //[ApiController]
    public class BranchesInfoController : ODataController
    {

        [EnableQuery]
        [HttpGet]
        public async Task<IQueryable<Branch>> Get()
        {
            throw new Exception("SOY UNA EXCEPCION INESPERADA , BUUUUUU ");
            var dataSet = await DataSource.GetBranchesAsync(this.HttpContext.RequestAborted);

            return dataSet.AsQueryable();
        }
    }
}
