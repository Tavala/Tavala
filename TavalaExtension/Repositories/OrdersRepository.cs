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
                var query = $@"SELECT COUNT(*)
                            FROM ORD_OrderDetail od
                                JOIN ORD_Order o ON o.RecordNumber = od.OrderNumber
                            WHERE o.DistributorID = @distributorId
                                AND o.InvoiceDate IS NOT NULL
                                AND o.Void = 0";
                int count =  dbConnection.QueryFirstOrDefault<int>(query, new { distributorId }); 
                if (count > 0)
                    exists = true;
                return exists;
            }
        }
    }
}
