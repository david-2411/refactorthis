using System;
using System.Linq;
using RefactorThis.Application.Services;
using RefactorThis.Domain.Entities;
using RefactorThis.Domain.Repositories;

namespace RefactorThis.Domain
{
	public class InvoiceService : IInvoiceService
    {
		private readonly IInvoiceRepository _invoiceRepository;

		public InvoiceService(IInvoiceRepository invoiceRepository )
		{
			_invoiceRepository = invoiceRepository;
		}

		public string ProcessPayment(Payment payment)
		{
			var inv = _invoiceRepository.GetInvoice(payment.Reference);

			var responseMessage = string.Empty;

			if (inv == null)
			{
				throw new InvalidOperationException("There is no invoice matching this payment");
			}

			if (inv.Amount == 0)
			{
				return handleZeroAmountInvoice(inv);
			}

            return handleNonZeroAmountInvoice(inv, payment);
		}
        
        private string handleZeroAmountInvoice(Invoice inv)
        {
            if (inv.Payments == null || !inv.Payments.Any())
            {
                return "no payment needed";
            }
            throw new InvalidOperationException("The invoice is in an invalid state, it has an amount of 0 and it has payments.");
        }

        private string handleNonZeroAmountInvoice(Invoice inv, Payment payment)
        {
            if (inv.Payments != null && inv.Payments.Any())
            {
                return processExistingPayments(inv, payment);
            }
            return processNewPayment(inv, payment);
        }

        private string processExistingPayments(Invoice inv, Payment payment)
        {
            var remainingAmount = inv.Amount - inv.AmountPaid;

            if (inv.Payments.Sum(x => x.Amount) == inv.Amount)
            {
                return "invoice was already fully paid";
            }

            if (payment.Amount > remainingAmount)
            {
                return "the payment is greater than the partial amount remaining";
            }

            updateInvoice(inv, payment);
            return (remainingAmount - payment.Amount) == 0
                ? "final partial payment received, invoice is now fully paid"
                : "another partial payment received, still not fully paid";
        }

        private string processNewPayment(Invoice inv, Payment payment)
        {
            if (payment.Amount > inv.Amount)
            {
                return "the payment is greater than the invoice amount";
            }

            updateInvoice(inv, payment);
            return payment.Amount == inv.Amount
                ? "invoice is now fully paid"
                : "invoice is now partially paid";
        }

        private void updateInvoice(Invoice inv, Payment payment)
        {
            inv.AmountPaid += payment.Amount;
            inv.TaxAmount += payment.Amount * 0.14m;
            inv.Payments.Add(payment);
            inv.Save();
        }
    }
}