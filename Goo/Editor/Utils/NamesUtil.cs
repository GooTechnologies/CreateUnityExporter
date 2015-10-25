public class NamesUtil {
	private const string HexCharacters = "0123456789abcdef";
	
	public static string Clean( string name ) {
		return name.Replace( " ", "_" ).Replace( ".", "_" ).Replace( ":", "_" ).Replace( "-", "_" ).Replace( "+", "_" ).Replace( "/", "_" ).Replace( "\\", "_" ).Replace( "(", "_" ).Replace( ")", "_" );
	}
	
	public static string CleanLowerCase( string name ) {
		return Clean( name ).ToLower();
	}
	
	public static string CleanMaterial( string name ) {
		return CleanLowerCase( name ).Replace( "_(instance)", "" );
	}
	
	public static string CleanShader( string name ) {
		return name.Substring( name.LastIndexOf( '/' ) + 1 );
	}
	
	public static string FirstToLower( string name ) {
		return name.Substring( 0, 1 ).ToLower() + name.Substring( 1 );
	}

	public static string CleanType( System.Object obj ) {
		string type = obj.GetType().ToString();
		if( type.LastIndexOf( '.' ) > 0 ) {
			return type.Substring( type.LastIndexOf( '.' ) + 1 );
		}
		return type;
	}
	
	public static string GenerateUniqueId() {
		// RFC4122 GUID without dashes
		return System.Guid.NewGuid().ToString().Replace("-", "");
	}
	
	public static string GetHexString(byte[] pBytes) {
		var hexChars = new char[pBytes.Length * 2];
		for (int i = 0; i < pBytes.Length; ++i) {
			hexChars[i * 2] = HexCharacters[(pBytes[i] >> 4) & 0xf];
			hexChars[i * 2 + 1] = HexCharacters[pBytes[i]& 0xf];
		}
		
		return new string(hexChars);
	}
	
	public static string GenerateBinaryId(byte[] pData, string pUserId) {
		return GenerateBinaryId(pData, 0, pData.Length, pUserId);
	}
	
	public static string GenerateBinaryId(byte[] pData, int pOffset, int pLength, string pUserId) {
		var userIdBytes = System.Text.UTF8Encoding.UTF8.GetBytes(pUserId);
		
		var extendedData = new byte[pLength + userIdBytes.Length];
		System.Buffer.BlockCopy(pData, pOffset, extendedData, 0, pLength);
		userIdBytes.CopyTo(extendedData, pLength);
		
		var hash = System.Security.Cryptography.SHA1.Create().ComputeHash(extendedData);
		
		return GetHexString(hash);
	}
}


