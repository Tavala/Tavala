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
        public async Task<SubmitOrderHookResponse> Invoke(SubmitOrderHookRequest request, Func<SubmitOrderHookRequest, Task<SubmitOrderHookResponse>> func)
        {
            try
            {
                request = _ordersService.AddSkuToOrder(request);
            }
            catch(Exception Ex)
            {
                string error = Ex.Message + " " + Ex.InnerException;
            }
            var result = await func(request);
            return result;
        }
    }
}
