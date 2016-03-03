using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using HexHelper.Hex.Interface;

namespace HexHelper.JsonApi.Utils
{
    /// <summary>
    /// Helper class to download a remote file, and keep a cache if so desired.
    /// </summary>
    public class CachedRemoteFile
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="aUrl">URL of file to download</param>
        /// <param name="aFileService">Optional file service. If none is provided, there will be no caching.</param>
        public CachedRemoteFile( string aUrl, IFileService aFileService = null )
        {
            mUrl = aUrl;
            mFile = aFileService;
        }

        /// <summary>
        /// Downloads file (or retrieves it from cache if fresh)
        /// </summary>
        /// <returns>true if file could be downloaded successfully</returns>
        public async Task<bool> DownloadFile()
        {
            var theFileName = Path.GetFileName( mUrl );
            try
            {
                if( mFile.Exists( scRelativeDirectory, theFileName ) )
                {
                    var theLastWriteTime = mFile.LastWriteTime( scRelativeDirectory, theFileName );
                    var theDiff = DateTime.Now - theLastWriteTime;
                    if( theDiff < CacheExpiration )
                    {
                        Content = await mFile.LoadFile( scRelativeDirectory, theFileName );
                        return true;
                    }
                }
            }
            catch { }

            try
            {
                using( var theHttpClient = new HttpClient() )
                {
                    Content = await theHttpClient.GetStringAsync( mUrl );
                    try
                    {
                        await mFile.SaveFile( scRelativeDirectory, theFileName, Content );
                    }
                    catch { }
                }
            }
            catch
            {
                Content = null;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Content of the downloaded file.
        /// </summary>
        public string Content { get; private set; } = null;

        /// <summary>
        /// Whether file was downloaded/cached and has any content.
        /// </summary>
        public bool HasContent {
            get
            {
                return !String.IsNullOrWhiteSpace( Content );
            }
        }

        /// <summary>
        /// When cache will go stale.
        /// </summary>
        public TimeSpan CacheExpiration { get; set; } = TimeSpan.FromHours( 8 );

        private readonly string mUrl;
        private readonly IFileService mFile;
        private const string scRelativeDirectory = "CachedRemoteFiles";
    }
}
