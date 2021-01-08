using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NudesForFree.Helper
{
	public class Util
	{
		public static string MD5Encode(string input)
		{
			var md5Hasher = System.Security.Cryptography.MD5.Create();
			var data = md5Hasher.ComputeHash(System.Text.Encoding.Default.GetBytes(input));

			return ByteToHexString(data);
		}
		private static string ByteToHexString(byte[] data)
		{
			var sBuilder = new System.Text.StringBuilder();
			for (var i = 0; i < data.Length; i++)
			{
				sBuilder.Append(data[i].ToString("X2"));
			}
			return sBuilder.ToString();
		}

		private static string NFFKey = "XIBIU";
		public static string GenerateFileName(int userID, string fileExtension, string prefix = "")
		{
			Random random = new Random();

			var filename = prefix +
				Helper.Util.MD5Encode(userID.ToString()+NFFKey) +
				Guid.NewGuid().ToString() +"."+
				fileExtension;

			return filename;
		}

	}
}
