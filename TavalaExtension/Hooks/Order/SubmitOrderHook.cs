using DirectScale.Disco.Extension.Hooks;
using DirectScale.Disco.Extension.Hooks.Orders;
using System;
using System.Threading.Tasks;
using TavalaExtension.Services;

namespace TavalaExtension.Hooks.Order
{
    public class SubmitOrderHook : IHook<SubmitOrderHookRequest, SubmitOrderHookResponse>
    {
        private readonly IOrdersService _ordersService;
        public SubmitOrderHook(IOrdersService ordersService)
        {
            _ordersService = ordersService ?? throw new ArgumentNullException(nameof(ordersService));
        }
        public Task<SubmitOrderHookResponse> Invoke(SubmitOrderHookRequest request, Func<SubmitOrderHookRequest, Task<SubmitOrderHookResponse>> func)
        {
            request = _ordersService.AddSkuToOrder(request);
            var result = func(request);
            return result;
        }
    }
}
