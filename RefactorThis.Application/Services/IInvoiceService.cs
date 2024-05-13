using RefactorThis.Domain.Entities;

namespace RefactorThis.Application.Services
{
    public interface IInvoiceService
    {
        string ProcessPayment(Payment payment);
    }
}
