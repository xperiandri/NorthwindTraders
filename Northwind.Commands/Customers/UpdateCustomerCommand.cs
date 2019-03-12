using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Validators;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Northwind.Application.Exceptions;
using Northwind.Domain.Entities;
using Northwind.Persistence;

namespace Northwind.Application.Customers.Commands.UpdateCustomer
{
    public class UpdateCustomerCommand : IRequest
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
    }
    public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
    {
        public UpdateCustomerCommandValidator()
        {
            RuleFor(x => x.Id).MaximumLength(5).NotEmpty();
            RuleFor(x => x.Address).MaximumLength(60);
            RuleFor(x => x.City).MaximumLength(15);
            RuleFor(x => x.CompanyName).MaximumLength(40).NotEmpty();
            RuleFor(x => x.ContactName).MaximumLength(30);
            RuleFor(x => x.ContactTitle).MaximumLength(30);
            RuleFor(x => x.Country).MaximumLength(15);
            RuleFor(x => x.Fax).MaximumLength(24).NotEmpty();
            RuleFor(x => x.Phone).MaximumLength(24).NotEmpty();
            RuleFor(x => x.PostalCode).MaximumLength(10);
            RuleFor(x => x.Region).MaximumLength(15);

            RuleFor(c => c.PostalCode).Matches(@"^\d{4}$")
                .When(c => c.Country == "Australia")
                .WithMessage("Australian Postcodes have 4 digits");

            RuleFor(c => c.Phone)
                .Must(HaveQueenslandLandLine)
                .When(c => c.Country == "Australia" && c.PostalCode.StartsWith("4"))
                .WithMessage("Customers in QLD require at least one QLD landline.");
        }

        private static bool HaveQueenslandLandLine(UpdateCustomerCommand model, string phoneValue, PropertyValidatorContext ctx)
        {
            return model.Phone.StartsWith("07") || model.Fax.StartsWith("07");
        }
    }

    public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, Unit>
    {
        private readonly NorthwindDbContext _context;

        public UpdateCustomerCommandHandler(NorthwindDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Customers
                .SingleAsync(c => c.CustomerId == request.Id, cancellationToken);

            if (entity == null)
            {
                throw new NotFoundException(nameof(Customer), request.Id);
            }

            entity.Address = request.Address;
            entity.City = request.City;
            entity.CompanyName = request.CompanyName;
            entity.ContactName = request.ContactName;
            entity.ContactTitle = request.ContactTitle;
            entity.Country = request.Country;
            entity.Fax = request.Fax;
            entity.Phone = request.Phone;
            entity.PostalCode = request.PostalCode;

            _context.Customers.Update(entity);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
