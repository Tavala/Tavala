using System;
using TavalaExtension.Repositories;
using DirectScale.Disco.Extension.Services;
using DirectScale.Disco.Extension.Hooks.Orders;

namespace TavalaExtension.Services
{

    public interface IOrdersService
    {
        SubmitOrderHookRequest AddSkuToOrder(SubmitOrderHookRequest request);
    }
    public class OrdersService: IOrdersService
    {
        private readonly IOrdersRepository _ordersRepository;
        private readonly IOrderService _orderService;
        public OrdersService(IOrdersRepository ordersRepository, IOrderService orderService)
        {
            _ordersRepository = ordersRepository ?? throw new ArgumentNullException(nameof(ordersRepository));
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
        }
        public SubmitOrderHookRequest AddSkuToOrder(SubmitOrderHookRequest request)
        {

            if(_ordersRepository.CheckIfFirstOrder(request.Order.AssociateId))
              {
                    bool sku275Exists = false;
                    foreach (var item in request.Order.LineItems)
                    {
                        if (item.ItemId == 275)
                        {
                            sku275Exists = true;
                            break;
                        }
                    }

                if (sku275Exists == false)
                {
                    var lineItem = request.Order.LineItems[0];
                    //lineItem.


                }
            }
            return request;
        }
    }
}
