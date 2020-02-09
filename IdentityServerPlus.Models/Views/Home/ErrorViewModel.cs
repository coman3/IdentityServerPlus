namespace IdentityServer.Models.Views
{
    public class ErrorViewModel {
        public ErrorViewModel() { }

        public ErrorViewModel(string error) {
            Error = new ErrorMessage { Error = error };
        }

        public ErrorMessage Error { get; set; }
    }

    public class ErrorMessage
    {
        public string DisplayMode { get; set; }
        //
        // Summary:
        //     The UI locales passed from the authorization request.
        public string UiLocales { get; set; }
        //
        // Summary:
        //     Gets or sets the error code.
        public string Error { get; set; }
        //
        // Summary:
        //     Gets or sets the error description.
        public string ErrorDescription { get; set; }
        //
        // Summary:
        //     The per-request identifier. This can be used to display to the end user and can
        //     be used in diagnostics.
        public string RequestId { get; set; }
        //
        // Summary:
        //     The redirect URI.
        public string RedirectUri { get; set; }
        //
        // Summary:
        //     The response mode.
        public string ResponseMode { get; set; }
        //
        // Summary:
        //     The client id making the request (if available).
        public string ClientId { get; set; }
    }
}