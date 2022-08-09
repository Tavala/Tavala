using System;
using TavalaExtension.Repositories;
using DirectScale.Disco.Extension.Services;
using DirectScale.Disco.Extension.Hooks.Orders;
using DirectScale.Disco.Extension;
using System.Threading.Tasks;
using System.Collections.Generic;

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
        private readonly IAssociateService _associateService;
        private const int ENROLLPACK = 275;

        public OrdersService(IOrdersRepository ordersRepository, IOrderService orderService, IItemService itemService, IAssociateService associateService)
        {
            _ordersRepository = ordersRepository ?? throw new ArgumentNullException(nameof(ordersRepository));
            _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            _itemService = itemService ?? throw new ArgumentNullException(nameof(itemService));
            _associateService = associateService ?? throw new ArgumentNullException(nameof(associateService));
        }
        public  SubmitOrderHookRequest AddSkuToOrder(SubmitOrderHookRequest request)
        {
            NewOrderDetail order = request.Order;

            if (_ordersRepository.CheckIfFirstOrder(request.Order.AssociateId)== false)
            {
                    bool sku275Exists = false;
                    foreach (var item in request.Order.LineItems)
                    {
                        if (item.ItemId == ENROLLPACK)
                        {
                            sku275Exists = true;
                            break;
                        }
                    }

                    if (sku275Exists == false)
                    {
                        Localization locationInfo = _associateService.GetLocalization(request.Order.AssociateId).GetAwaiter().GetResult(); ;
                        var associateType = _associateService.GetAssociate(order.AssociateId).Result.AssociateType;

                        string currencyCode = locationInfo.CurrencyCode;
                        switch (currencyCode)
                        {
                            case null:
                                currencyCode = "usd";
                                break;
                            default:
                                if (currencyCode.Trim().Length == 0)
                                    currencyCode = "usd";
                                break;
                        }

                        string languageCode = locationInfo.LanguageCode;
                        switch (languageCode)
                        {
                            case null:
                            languageCode = "en";
                                break;
                            default:
                                if (languageCode.Trim().Length == 0)
                                    languageCode = "en";
                                break;
                        }

                    int regionId = locationInfo.RegionId;
                        if(regionId == 0)
                        {
                            regionId = 1;
                        }

                        string countryCode = locationInfo.CountryCode;
                        switch (countryCode)
                        {
                            case null:
                                countryCode = "us";
                                break;
                            default:
                                if (countryCode.Trim().Length == 0)
                                    countryCode = "us";
                                break;
                        }

                        LineItem  enrollPack = _itemService.GetLineItemById(ENROLLPACK, 1, currencyCode, languageCode,
                                                regionId, (int)order.OrderType, associateType, order.StoreId, countryCode).GetAwaiter().GetResult(); ;
                        if (enrollPack == null) throw new Exception($"Cannot find item Enroll Pack'");

                        LineItem[] newSkus= request.Order.LineItems;

                        // We will resize the original array and increase the length by 1
                        Array.Resize(ref newSkus, newSkus.Length + 1);
                        // Adding a new element by adding a new item on the last element
                        newSkus[newSkus.Length - 1] = enrollPack;
                        request.Order.LineItems = newSkus;
                }
            }
            return request;
        }
    }
}
