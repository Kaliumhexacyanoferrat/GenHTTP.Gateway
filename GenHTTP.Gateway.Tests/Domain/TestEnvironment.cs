namespace GenHTTP.Gateway.Tests.Domain;

public class TestEnvironment : Environment
{

        #region Get-/Setters

    public DirectoryInfo Root { get; }

        #endregion

        #region Initialization

    protected TestEnvironment(DirectoryInfo root) : base(CreateDirectory(root, "config"), CreateDirectory(root, "data"), CreateDirectory(root, "certs"))
    {
            Root = root;
        }

    public static TestEnvironment Create()
    {
            var root = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()));

            return new TestEnvironment(root);
        }

    private static string CreateDirectory(DirectoryInfo root, string subfolder)
    {
            return Directory.CreateDirectory(Path.Combine(root.FullName, subfolder)).FullName;
        }

        #endregion

        #region Functionality

    public void Cleanup()
    {
            if (Root.Exists)
            {
                try
                {
                    Root.Delete(true);
                }
                catch { /* nop */ }
            }
        }

        #endregion

}