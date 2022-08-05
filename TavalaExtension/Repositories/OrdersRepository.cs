using System;
using System.Data.SqlClient;
using Dapper;
using DirectScale.Disco.Extension.Services;


namespace TavalaExtension.Repositories
{
    public interface IOrdersRepository
    {
        bool CheckIfFirstOrder(int distributorId);
    }
    public class OrdersRepository: IOrdersRepository
    {
        private readonly IDataService _dataService;

        public OrdersRepository(IDataService dataService, IOrderService orderService)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
        }
        public bool CheckIfFirstOrder(int distributorId)
        {
            bool exists = false;
            using (var dbConnection = new SqlConnection(_dataService.GetClientConnectionString().Result))
            {
                var query = $@"select count(*) from ORD_Order where DistributorId = @distributorId";
                int count =  dbConnection.QueryFirstOrDefault<int>(query, new { distributorId }); 
                if (count > 0)
                    exists = true;
                return exists;
            }
        }
    }
}
