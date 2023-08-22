using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using GenHTTP.Api.Content;
using GenHTTP.Api.Protocol;

namespace GenHTTP.Gateway.Tests.Domain
{

    public class InlineHandler : IHandler
    {

        #region Get-/Setters

        public IHandler Parent { get; }

        private Func<IHandler, IRequest, ValueTask<IResponse?>> Logic { get; }

        #endregion

        #region Initialization

        public InlineHandler(IHandler parent, Func<IHandler, IRequest, ValueTask<IResponse?>> logic)
        {
            Parent = parent;
            Logic = logic;
        }

        #endregion

        #region Functionality

        public ValueTask<IResponse?> HandleAsync(IRequest request)
        {
            return Logic(this, request);
        }

        public IAsyncEnumerable<ContentElement> GetContentAsync(IRequest request) => AsyncEnumerable.Empty<ContentElement>();

        public ValueTask PrepareAsync() => new();

        #endregion

    }

    public class InlineHandlerBuilder : IHandlerBuilder
    {
        private readonly Func<IHandler, IRequest, ValueTask<IResponse?>> Logic;

        public static InlineHandlerBuilder Create(Func<IHandler, IRequest, ValueTask<IResponse?>> logic)
        {
            return new(logic);
        }

        private InlineHandlerBuilder(Func<IHandler, IRequest, ValueTask<IResponse?>> logic)
        {
            Logic = logic;
        }

        public IHandler Build(IHandler parent)
        {
            return new InlineHandler(parent, Logic);
        }

    }

}
