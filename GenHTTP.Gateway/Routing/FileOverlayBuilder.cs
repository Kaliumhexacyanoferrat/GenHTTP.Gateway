using GenHTTP.Api.Content;

namespace GenHTTP.Gateway.Routing;

public class FileOverlayBuilder : IConcernBuilder
{

        #region Get-/Setters

    public Environment Environment { get; }

        #endregion

        #region Initialization

    public FileOverlayBuilder(Environment environment)
    {
        Environment = environment;
    }

        #endregion

        #region Functionality

    public IConcern Build(IHandler parent)
    {
        return new FileOverlay(parent, Environment);
    }

        #endregion

}
