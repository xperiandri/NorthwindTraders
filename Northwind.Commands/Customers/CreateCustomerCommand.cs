using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Northwind.Application.Interfaces;
using Northwind.Domain.Entities;
using Northwind.Persistence;

namespace Northwind.Application.Customers.Commands.CreateCustomer
{
    public class CreateCustomerCommand : IRequest
    {
        public string Id { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string CompanyName { get; set; }

        public string ContactName { get; set; }

        public string ContactTitle { get; set; }

        public string Country { get; set; }

        public string Fax { get; set; }

        public string Phone { get; set; }

        public string PostalCode { get; set; }

        public string Region { get; set; }

        public class Handler : IRequestHandler<CreateCustomerCommand, Unit>
        {
            private readonly NorthwindDbContext _context;
            private readonly INotificationService _notificationService;
            private readonly IMediator _mediator;

            public Handler(
                NorthwindDbContext context,
                INotificationService notificationService,
                IMediator mediator)
            {
                _context = context;
                _notificationService = notificationService;
                _mediator = mediator;
            }

            public async Task<Unit> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
            {
                var entity = new Customer
                {
                    CustomerId = request.Id,
                    Address = request.Address,
                    City = request.City,
                    CompanyName = request.CompanyName,
                    ContactName = request.ContactName,
                    ContactTitle = request.ContactTitle,
                    Country = request.Country,
                    Fax = request.Fax,
                    Phone = request.Phone,
                    PostalCode = request.PostalCode
                };

                _context.Customers.Add(entity);

                await _context.SaveChangesAsync(cancellationToken);

                await _mediator.Publish(new CustomerCreated { CustomerId = entity.CustomerId });

                return Unit.Value;
            }
        }
    }

    public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
    {
        public CreateCustomerCommandValidator()
        {
            RuleFor(x => x.Id).Length(5).NotEmpty();
            RuleFor(x => x.Address).MaximumLength(60);
            RuleFor(x => x.City).MaximumLength(15);
            RuleFor(x => x.CompanyName).MaximumLength(40).NotEmpty();
            RuleFor(x => x.ContactName).MaximumLength(30);
            RuleFor(x => x.ContactTitle).MaximumLength(30);
            RuleFor(x => x.Country).MaximumLength(15);
            RuleFor(x => x.Fax).MaximumLength(24);
            RuleFor(x => x.Phone).MaximumLength(24);
            RuleFor(x => x.PostalCode).MaximumLength(10).NotEmpty();
            RuleFor(x => x.Region).MaximumLength(15);
        }
    }
}
