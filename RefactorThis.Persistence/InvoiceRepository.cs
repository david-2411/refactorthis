using RefactorThis.Domain.Entities;
using RefactorThis.Domain.Repositories;

namespace RefactorThis.Persistence {
	public class InvoiceRepository: IInvoiceRepository
	{
		private Invoice _invoice;

		public Invoice GetInvoice( string reference )
		{
			return _invoice;
		}

		public void SaveInvoice( Invoice invoice )
		{
			//saves the invoice to the database
		}

		public void AddInvoice( Invoice invoice )
		{
			_invoice = invoice;
		}
	}
}