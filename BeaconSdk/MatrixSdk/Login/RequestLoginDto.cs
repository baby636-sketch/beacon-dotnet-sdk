namespace MatrixSdk.Login
{ 
    public record Identifier(string Type, string User);
    
    public record RequestLoginDto(Identifier Identifier, string Password, string DeviceId, string Type);
}


// See: https://stackoverflow.com/questions/62648189/testing-c-sharp-9-0-in-vs2019-cs0518-isexternalinit-is-not-defined-or-imported
namespace System.Runtime.CompilerServices
{
    using ComponentModel;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public record IsExternalInit;
}