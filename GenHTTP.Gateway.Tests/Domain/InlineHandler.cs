using GenHTTP.Api.Content;
using GenHTTP.Api.Protocol;

namespace GenHTTP.Gateway.Tests.Domain;

public class InlineHandler : IHandler
{

    #region Get-/Setters

    private Func<IHandler, IRequest, ValueTask<IResponse?>> Logic { get; }

    #endregion

    #region Initialization

    public InlineHandler(Func<IHandler, IRequest, ValueTask<IResponse?>> logic)
    {
        Logic = logic;
    }

    #endregion

    #region Functionality

    public ValueTask<IResponse?> HandleAsync(IRequest request)
    {
        return Logic(this, request);
    }

    public ValueTask PrepareAsync() => new();

    #endregion

}

public class InlineHandlerBuilder : IHandlerBuilder
{
    private readonly Func<IHandler, IRequest, ValueTask<IResponse?>> _logic;

    public static InlineHandlerBuilder Create(Func<IHandler, IRequest, ValueTask<IResponse?>> logic)
    {
        return new(logic);
    }

    private InlineHandlerBuilder(Func<IHandler, IRequest, ValueTask<IResponse?>> logic)
    {
        _logic = logic;
    }

    public IHandler Build()
    {
        return new InlineHandler(_logic);
    }

}
