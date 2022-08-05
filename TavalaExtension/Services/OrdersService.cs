using System;
using TavalaExtension.Repositories;
using DirectScale.Disco.Extension.Services;
using DirectScale.Disco.Extension.Hooks.Orders;
using DirectScale.Disco.Extension;

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
        private readonly IItemService _itemService;

        public OrdersService(IOrdersRepository ordersRepository, IOrderService orderService, IItemService itemService)
        {
            _ordersRepository = ordersRepository ?? throw new ArgumentNullException(nameof(ordersRepository));
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            _itemService = itemService ?? throw new ArgumentNullException(nameof(itemService));
        }
        public SubmitOrderHookRequest AddSkuToOrder(SubmitOrderHookRequest request)
        {

            if(_ordersRepository.CheckIfFirstOrder(request.Order.AssociateId)== false)
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

                        
                        LineItem[] newSkus= request.Order.LineItems;

                        // We will resize the original array and increase the length by 1
                        Array.Resize(ref newSkus, newSkus.Length + 1);

                        LineItem lineItemEntry = request.Order.LineItems[0];
                        lineItemEntry.ItemId = 275;
                        lineItemEntry.Quantity = 1;

                        // Adding a new element by adding a new item on the last element
                        newSkus[newSkus.Length - 1] = lineItemEntry;

                        request.Order.LineItems = newSkus;
                    }
            }
            return request;
        }
    }
}
