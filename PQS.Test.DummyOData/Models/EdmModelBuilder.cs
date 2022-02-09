namespace PQS.Test.DummyOData.Models
{
    using Microsoft.OData.Edm;
    using Microsoft.OData.ModelBuilder;

    /// <summary>
    /// Contiene los metodos de configuracion de OData
    /// </summary>
    internal static class EdmModelBuilder
    {

        /// <summary>
        /// Retorna las entidades exxpuestas mediante OData
        /// </summary>
        /// <returns>Retorna un model o con todas las entidades expuestas por OData</returns>
        /// <param name="defaultMaxPage">Setea el mayor tamaño de pagina y el mayor top value por defecto</param>
        public static IEdmModel GetEdmModel(int defaultMaxPage=5)
        {
            var builder = new ODataConventionModelBuilder();


            #region Mapeo a OData Controllers


            // hay que tener cuidado de no generar un entity set con el mismo nombre que un controlador de API. Da error
            // por eso el subfijo Info, aparte sirve para indicar que son readonly
            builder.EntitySet<Product>("ProductsInfo")
                .EntityType
                .HasKey(entity => entity.ID)
                .Page(defaultMaxPage, defaultMaxPage); 

            builder.EntitySet<Branch>("BranchesInfo")
                .EntityType
                .HasKey(entity => entity.Id)
                .Page(defaultMaxPage, defaultMaxPage);


            #endregion Mapeo a OData Controllers

            return builder.GetEdmModel();

        }

    }

    static class EntitySetExtensions
    {
    }
}
