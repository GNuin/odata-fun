using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PQS.Test.DummyOData.Models
{
    public class DataSource
    {
        static List<Product> _data = null;
        static List<Branch> _branches = null;


        static readonly SemaphoreSlim _dataLock = new(1);

        private static async Task<T> GetDataAsync<T>(string filename, CancellationToken cancellationToken = default) where T : class
        {

            using Stream file = File.OpenRead(filename);
            {
                return await JsonSerializer.DeserializeAsync<T>(file, null, cancellationToken);
            }

        }


        public static async Task<List<Product>> GetProductAsync(CancellationToken cancellationToken = default)
        {
            if (_data != null)
                return _data;

            await _dataLock.WaitAsync(cancellationToken);

            try
            {
                // docle checkeo 
                if (_data == null)
                {
                    _data = await GetDataAsync<List<Product>>(@"./Data/products.json", cancellationToken);
                }

                return _data;

            }
            finally
            {
                _dataLock.Release();
            }
        }

        public static async Task<List<Branch>> GetBranchesAsync(CancellationToken cancellationToken = default)
        {
            if (_branches != null)
                return _branches;

            await _dataLock.WaitAsync(cancellationToken);

            try
            {
                // docle checkeo 
                if (_branches == null)
                {
                    _branches = await GetDataAsync<List<Branch>>(@"./Data/branches.json", cancellationToken);
                }

                return _branches;

            }
            finally
            {
                _dataLock.Release();
            }
        }
    }
}
