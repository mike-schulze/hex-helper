using System;
using System.IO;
using System.Net.Http;
using System.Text;
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
            CacheFileName = Path.GetFileName( mUrl );
        }

        /// <summary>
        /// Downloads file (or retrieves it from cache if fresh)
        /// </summary>
        /// <returns>true if file could be downloaded successfully</returns>
        public async Task<bool> DownloadFile()
        {
            try
            {
                if( mFile.Exists( scRelativeDirectory, CacheFileName ) )
                {
                    var theLastWriteTime = mFile.LastWriteTime( scRelativeDirectory, CacheFileName );
                    var theDiff = DateTime.Now - theLastWriteTime;
                    if( theDiff < CacheExpiration )
                    {
                        Content = await mFile.LoadFile( scRelativeDirectory, CacheFileName );
                        return true;
                    }
                }
            }
            catch { }

            try
            {
                using( var theHttpClient = new HttpClient() )
                {
                    if( PostMessage != null )
                    {
                        var theMessage = await theHttpClient.PostAsync( mUrl, new StringContent( PostMessage, Encoding.UTF8, "application/json" ) );
                        if( theMessage.IsSuccessStatusCode )
                        {
                            Content = await theMessage.Content.ReadAsStringAsync();
                        }                        
                    }
                    else
                    {
                        Content = await theHttpClient.GetStringAsync( mUrl );
                    }
                    
                    if( HasContent )
                    {
                        try
                        {
                            await mFile.SaveFile( scRelativeDirectory, CacheFileName, Content );
                        }
                        catch { }

                        return true;
                    }
                }
            }
            catch
            {
            }

            Content = null;
            return false;
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
        /// By default content will be retrieved via GET. If POST is needed, set this.
        /// This is expected to be Json.
        /// </summary>
        public string PostMessage { get; set; } = null;

        public string CacheFileName { get; set; }

        /// <summary>
        /// When cache will go stale.
        /// </summary>
        public TimeSpan CacheExpiration { get; set; } = TimeSpan.FromHours( 8 );

        private readonly string mUrl;
        private readonly IFileService mFile;
        private const string scRelativeDirectory = "CachedRemoteFiles";
    }
}
