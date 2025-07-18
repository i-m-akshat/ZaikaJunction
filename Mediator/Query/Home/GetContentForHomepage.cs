using MediatR;
using Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mediator.Query.Home
{
    public record GetContentForHomepage : IRequest<HomePageViewModel>;
}
