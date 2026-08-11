using Gamestore.BLL.DTOs.Order;
using Gamestore.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gamestore.API.Controllers;

[ApiController]
[Route("orders")]
public class OrderController(IOrderService orderService) : ControllerBase
{
    [HttpPost("payment")]
    public async Task<IActionResult> CreatePayment([FromBody] PaymentRequestDto request)
    {
        var result = await orderService.PayAsync(request);

        return result switch
        {
            { InvoiceFile: not null } => File(result.InvoiceFile, "application/pdf", "invoice.pdf"),
            { IBoxResult: not null } => Ok(result.IBoxResult),
            _ => Ok(),
        };
    }

    [HttpGet]
    public async Task<IActionResult> GetAllOrders()
    {
        var orders = await orderService.GetOrdersAsync();
        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(Guid id)
    {
        var order = await orderService.GetOrderByIdAsync(id);
        return Ok(order);
    }

    [HttpGet("{id}/details")]
    public async Task<IActionResult> GetOrderDetails(Guid id)
    {
        var details = await orderService.GetOrderDetailsAsync(id);
        return Ok(details);
    }

    [HttpGet("cart")]
    public async Task<IActionResult> GetCart()
    {
        var cart = await orderService.GetCartAsync();
        return Ok(cart);
    }

    [HttpGet("payment-methods")]
    public IActionResult GetPaymentMethods()
    {
        var paymentMethods = orderService.GetPaymentMethods();
        return Ok(new { paymentMethods });
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetHistory([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        var history = await orderService.GetHistoryAsync(startDate, endDate);
        return Ok(history);
    }

    [HttpDelete("cart/{key}")]
    public async Task<IActionResult> DeleteFromCart(string key)
    {
        await orderService.RemoveFromCartAsync(key);
        return Ok();
    }
}