using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WriteFileHooker
{
	public class WinEnum
	{
		public const int PROCESS_WM_READ = 0x0010;
		public const int PROCESS_DUP_HANDLE = 0x0040;

		public const int SYSTEMHANDLEINFORMATION = 16;

		public enum FileInformationClass
		{
			FileBasicInfo,
			FileStandardInfo,
			FileNameInfo,
			FileStreamInfo,
			FileCompressionInfo,
			FileAttributeTagInfo,
			FileIdBothDirectoryInfo,
			FileIdBothDirectoryRestartInfo,
			FileRemoteProtocolInfo,
			FileFullDirectoryInfo,
			FileFullDirectoryRestartInfo,
			FileStorageInfo,
			FileAlignmentInfo,
			FileIdInfo,
			FileIdExtdDirectoryInfo,
			FileIdExtdDirectoryRestartInfo,


		}

	}
}
